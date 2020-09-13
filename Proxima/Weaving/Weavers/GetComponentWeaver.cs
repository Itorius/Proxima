using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using MonoMod.Cil;
using MonoMod.Utils;

namespace Proxima.Weaver
{
	public class GetComponentWeaver : BaseWeaver
	{
		private static readonly Regex MatchMethodRegex = new Regex(@"Proxima\.ECS\.(View|Group)`\d<\S+>::Get<\S+>\(Proxima\.ECS\.Entity\)");

		public override void Execute()
		{
			foreach (MethodDefinition methodDefinition in ModuleDefinition.Types.SelectMany(type => type.Methods).Where(method => !method.IsAbstract)) ProcessMethod(methodDefinition);
		}

		private static void ProcessMethod(MethodDefinition methodDefinition)
		{
			methodDefinition.Body.SimplifyMacros();

			ILContext context = new ILContext(methodDefinition);
			ILCursor cursor = new ILCursor(context);

			while (cursor.TryGotoNext(MoveType.AfterLabel, i => i.MatchCallOrCallvirt(out MethodReference reference) && MatchMethodRegex.IsMatch(reference.GetID())))
			{
				GenericInstanceMethod call = (GenericInstanceMethod)cursor.Next.Operand;

				if (call.GenericArguments.Count > 1) ProcessCallMultiple(methodDefinition.Module, context, cursor);
				else ProcessCall(methodDefinition.Module, cursor);
			}

			methodDefinition.Body.OptimizeMacros();
		}

		private static void ProcessCallMultiple(ModuleDefinition module, ILContext context, ILCursor cursor)
		{
			Instruction i_LoadView = cursor.Previous.Previous;
			VariableDefinition v_View = (VariableDefinition)i_LoadView.Operand;
			VariableDefinition v_Entity = (VariableDefinition)cursor.Previous.Operand;

			Instruction callInstruction = cursor.Next;
			GenericInstanceMethod call = (GenericInstanceMethod)callInstruction.Operand;

			GenericInstanceType viewType = (GenericInstanceType)call.DeclaringType;
			TypeDefinition viewTypeResolved = viewType.Resolve();

			var componentTypes = call.GenericArguments;
			var fields = viewTypeResolved.Fields;

			var map = MapViewFields(viewType);

			// get proper fields from view

			var pools = new List<FieldDefinition>();

			foreach (TypeReference componentType in componentTypes)
			{
				FieldDefinition field = fields.First(field => field.FieldType is GenericInstanceType genericType && map.TryGetValue(genericType.GenericArguments[0].Name, out TypeReference type) && type == componentType);
				pools.Add(field);
			}

			var genericInstanceType = (GenericInstanceType)call.ReturnType;

			List<VariableDefinition> variables = new List<VariableDefinition>();
			for (int i = 0; i < genericInstanceType.GenericArguments.Count; i++)
			{
				int index = 0;
				cursor.GotoNext(MoveType.After, instruction => instruction.MatchStloc(out index));
				VariableDefinition variable = context.Body.Variables[index];
				variables.Add(variable);
			}

			Instruction next = cursor.Next;
			cursor.Goto(i_LoadView);
			while (cursor.Next != next) cursor.Remove();

			for (int i = 0; i < pools.Count; i++)
			{
				TypeDefinition poolType = pools[i].FieldType.Resolve();

				MethodDefinition m_Get = poolType.Methods.First(x => x.Parameters.Count == 1 && x.Name.Contains("Get") && x.Parameters[0].ParameterType.FullName == "Proxima.ECS.Entity");

				GenericInstanceType declaringType = new GenericInstanceType(poolType);
				declaringType.GenericArguments.Add(componentTypes[i]);
				m_Get.SetDeclaringType(declaringType);

				var f_Pool = module.ImportReference(pools[i]);
				f_Pool.DeclaringType = viewType;

				cursor.Emit(OpCodes.Ldloc, v_View);
				cursor.Emit(OpCodes.Ldfld, f_Pool);
				cursor.Emit(OpCodes.Ldloc, v_Entity);
				cursor.Emit(OpCodes.Call, module.ImportReference(m_Get));
				cursor.Emit(OpCodes.Stloc, variables[i]);
			}
		}

		private static void ProcessCall(ModuleDefinition module, ILCursor cursor)
		{
			GenericInstanceMethod call = (GenericInstanceMethod)cursor.Next.Operand;
			GenericInstanceType viewType = (GenericInstanceType)call.DeclaringType;
			TypeDefinition viewTypeResolved = viewType.Resolve();

			TypeReference componentType = call.GenericArguments[0];

			var map = MapViewFields(viewType);

			// get proper field from view
			FieldDefinition? pool = null;
			foreach (FieldDefinition field in viewTypeResolved.Fields)
			{
				if (!(field.FieldType is GenericInstanceType genericType)) continue;

				if (map.TryGetValue(genericType.GenericArguments[0].Name, out TypeReference type) && type == componentType)
				{
					pool = field;
					break;
				}
			}

			if (pool == null) throw new Exception("Failed to get View's pool field");

			// store and remove load entity field because stuff will be injected inbetween
			cursor.Index--;
			Instruction loadEntity = cursor.Next;
			cursor.Remove();

			TypeDefinition poolType = pool.FieldType.Resolve();

			MethodDefinition getMethod = poolType.Methods.First(x => x.Parameters.Count == 1 && x.Name.Contains("Get") && x.Parameters[0].ParameterType.FullName == "Proxima.ECS.Entity");

			GenericInstanceType declaringType = new GenericInstanceType(poolType);
			declaringType.GenericArguments.Add(componentType);
			getMethod.SetDeclaringType(declaringType);

			var importReference = module.ImportReference(pool);
			importReference.DeclaringType = viewType;

			cursor.Emit(OpCodes.Ldfld, importReference);
			cursor.Emit(loadEntity.OpCode, loadEntity.Operand);
			cursor.Emit(OpCodes.Call, module.ImportReference(getMethod));

			cursor.Remove();
		}

		private static Dictionary<string, TypeReference> MapViewFields(GenericInstanceType viewType)
		{
			TypeDefinition viewTypeResolved = viewType.Resolve();

			var map = new Dictionary<string, TypeReference>();

			List<TypeReference> arguments = viewType.GenericArguments.ToList();
			for (int i = 0; i < arguments.Count; i++)
			{
				TypeReference argument = arguments[i];
				GenericParameter parameter = viewTypeResolved.GenericParameters[i];
				map.Add(parameter.Name, argument);
			}

			return map;
		}
	}
}