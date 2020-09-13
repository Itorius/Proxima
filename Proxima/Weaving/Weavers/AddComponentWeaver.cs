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
	public class AddComponentWeaver : BaseWeaver
	{
		private static readonly Regex MatchMethodRegex = new Regex(@"!!0 Proxima\.ECS\.Entity::AddComponent<\S+>\(System\.Object\[\]\)");

		private TypeDefinition entityType = null!;
		private MethodReference addComponent = null!;
		private TypeDefinition nullable = null!;
		private MethodReference nullableCtor = null!;

		public override void Execute()
		{
			entityType = FindTypeDefinition("Proxima.ECS.Entity");
			addComponent = ModuleDefinition.ImportReference(entityType.FindMethod("T Proxima.ECS.Entity::AddComponent<T>(T)"));

			nullable = FindTypeDefinition("System.Nullable`1");
			nullableCtor = nullable.GetConstructors().First(constructor => constructor.Parameters.Count == 1);

			foreach (MethodDefinition method in ModuleDefinition.Types.SelectMany(type => type.Methods).Where(method => !method.IsAbstract)) ProcessMethod(method);
		}

		private void ProcessMethod(MethodDefinition methodDefinition)
		{
			methodDefinition.Body.SimplifyMacros();

			ILContext context = new ILContext(methodDefinition);
			ILCursor cursor = new ILCursor(context);

			while (cursor.TryGotoNext(MoveType.AfterLabel, i => i.MatchCall(out MethodReference reference) && MatchMethodRegex.IsMatch(reference.GetID()))) ProcessCall(cursor, context);

			methodDefinition.Body.OptimizeMacros();
		}

		private void ProcessCall(ILCursor cursor, ILContext context)
		{
			Instruction callInstruction = cursor.Next;
			GenericInstanceMethod method = (GenericInstanceMethod) callInstruction.Operand;

			cursor.GotoPrev(MoveType.Before, i => i.MatchLdloca(out _), i => i.MatchLdcI4(out _), i => i.MatchNewarr<object>());

			// remove ldc.i4 and newarr instructions
			cursor.Index++;
			cursor.RemoveRange(2);

			// get all instructions that are used to create the component
			List<Instruction> objectCreation = new List<Instruction>();
			while (cursor.Next != callInstruction)
			{
				objectCreation.Add(cursor.Next);
				cursor.Remove();
			}

			// split them into individual parameters
			List<List<Instruction>> split = objectCreation.Split(i => i.OpCode == OpCodes.Dup).Select(x => x.ToList()).ToList();

			List<TypeDefinition> ctorTypes = new List<TypeDefinition>();
			foreach (List<Instruction> instructions in split)
			{
				// remove ldc.i4 and dup instructions
				instructions.RemoveRange(0, 2);

				// remove stemlem.ref
				instructions.RemoveLast();

				// get parameter type and remove box instruction if parameter is ValueType
				Instruction instruction = instructions.Last();
				if (instruction.OpCode == OpCodes.Box)
				{
					TypeReference typeRef = (TypeReference) instruction.Operand;
					instructions.Remove(instruction);

					ctorTypes.Add(typeRef.Resolve());
				}
				else
				{
					TypeReference type = GetTypeRef(instruction);
					ctorTypes.Add(type.Resolve());
				}
			}

			TypeDefinition componentType = method.GenericArguments[0].Resolve();

			MethodDefinition? ctor = componentType.GetConstructors().FirstOrDefault(constructor =>
			{
				if (constructor.Parameters.Count != ctorTypes.Count) return false;

				bool flag = true;
				for (int i = 0; i < constructor.Parameters.Count; i++)
				{
					if (constructor.Parameters[i].ParameterType.Resolve().FullName == nullable.FullName)
					{
						GenericInstanceType t = (GenericInstanceType) constructor.Parameters[i].ParameterType;

						flag &= t.GenericArguments[0].Resolve() == ctorTypes[i];
					}
					else flag &= constructor.Parameters[i].ParameterType.Resolve().IsAssignableFrom(ctorTypes[i]);
				}

				return flag;
			});

			if (ctor == null)
			{
				throw new Exception($"Couldn't find valid constructor for type '{componentType.FullName}' with parameters '{ctorTypes.Select(x => x.FullName).Aggregate((x, y) => x + ", " + y)}'");
			}

			int index = 0;
			foreach (List<Instruction> instructions in split)
			{
				foreach (Instruction instruction in instructions) cursor.Emit(instruction.OpCode, instruction.Operand);

				TypeDefinition type = ctor.Parameters[index].ParameterType.Resolve();
				if (type.FullName == nullable.FullName)
				{
					GenericInstanceType nullableCtorInstance = nullable.MakeGenericInstanceType(ctorTypes[index]);
					cursor.Emit(OpCodes.Newobj, context.Import(nullableCtor.MakeGeneric(nullableCtorInstance).ResolveReflection()));
				}

				index++;
			}

			cursor.Emit(OpCodes.Newobj, ctor);

			GenericInstanceMethod addComponentInstance = new GenericInstanceMethod(addComponent);
			addComponentInstance.GenericArguments.Add(componentType);
			cursor.Emit(OpCodes.Call, addComponentInstance);

			cursor.Remove();
		}

		private TypeReference GetTypeRef(Instruction instruction)
		{
			OpCode opCode = instruction.OpCode;
			object operand = instruction.Operand;
			if (opCode == OpCodes.Call || opCode == OpCodes.Newobj) return ((MethodReference) operand).ReturnType;
			return FindTypeDefinition(operand.GetType().FullName);
		}

		// public override IEnumerable<string> GetAssembliesForScanning()
		// {
		// 	yield return "System";
		// 	yield return "Proxima";
		// }
	}
}