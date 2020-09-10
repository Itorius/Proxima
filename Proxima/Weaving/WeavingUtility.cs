using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Proxima.Weaver
{
	internal static class WeavingUtility
	{
		public static VariableDefinition AddVariable<T>(this ILContext context, BaseWeaver weaver)
		{
			VariableDefinition definition = new VariableDefinition(weaver.ModuleDefinition.ImportReference(weaver.FindTypeDefinition(typeof(T).FullName)));
			context.Body.Variables.Add(definition);
			return definition;
		}

		public static MethodReference MakeGeneric(this MethodReference method, TypeReference declaringType)
		{
			MethodReference reference = new MethodReference(method.Name, method.ReturnType, declaringType)
			{
				HasThis = method.HasThis,
				ExplicitThis = method.ExplicitThis,
				CallingConvention = MethodCallingConvention.Generic
			};
			foreach (ParameterDefinition parameter in method.Parameters) reference.Parameters.Add(new ParameterDefinition(parameter.ParameterType));

			return reference;
		}

		public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
		{
			List<T> enumerable = collection.ToList();
			List<int> indices = enumerable.IndicesOf(predicate).ToList();

			List<T> list = new List<T>();
			for (int i = 0; i < enumerable.Count; i++)
			{
				if (list.Count > 0 && indices.Contains(i))
				{
					yield return list;
					list = new List<T>();
				}

				list.Add(enumerable[i]);
			}

			if (list.Count > 0) yield return list;
		}

		public static void SetDeclaringType(this MethodDefinition method, TypeReference declaringType) => ((MethodReference)method).DeclaringType = declaringType;

		public static bool IsSubclassOf(this TypeDefinition childTypeDef, TypeDefinition parentTypeDef)
		{
			return childTypeDef.MetadataToken != parentTypeDef.MetadataToken && childTypeDef.EnumerateBaseClasses().Any(b => b.MetadataToken == parentTypeDef.MetadataToken);
		}

		public static bool DoesAnySubTypeImplementInterface(this TypeDefinition childType, TypeDefinition parentInterfaceDef)
		{
			return childType.EnumerateBaseClasses().Any(typeDefinition => typeDefinition.DoesSpecificTypeImplementInterface(parentInterfaceDef));
		}

		public static bool DoesSpecificTypeImplementInterface(this TypeDefinition childTypeDef, TypeDefinition parentInterfaceDef)
		{
			return childTypeDef.Interfaces.Any(ifaceDef => DoesSpecificInterfaceImplementInterface(ifaceDef.InterfaceType.Resolve(), parentInterfaceDef));
		}

		public static bool DoesSpecificInterfaceImplementInterface(TypeDefinition iface0, TypeDefinition iface1)
		{
			return iface0.MetadataToken == iface1.MetadataToken || iface0.DoesAnySubTypeImplementInterface(iface1);
		}

		public static bool IsAssignableFrom(this TypeDefinition target, TypeDefinition source)
		{
			return target == source || target.MetadataToken == source.MetadataToken || source.IsSubclassOf(target) || target.IsInterface && source.DoesAnySubTypeImplementInterface(target);
		}

		public static IEnumerable<TypeDefinition> EnumerateBaseClasses(this TypeDefinition klassType)
		{
			for (var typeDefinition = klassType; typeDefinition != null; typeDefinition = typeDefinition.BaseType?.Resolve())
			{
				yield return typeDefinition;
			}
		}
	}
}