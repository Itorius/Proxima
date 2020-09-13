using System;
using System.Collections.Generic;
using System.Reflection;
using Mono.Cecil;

namespace Proxima.Weaver
{
	public abstract class BaseWeaver
	{
		public ModuleDefinition ModuleDefinition = null!;

		internal BaseWeaver()
		{
		}

		public abstract void Execute();

		public TypeDefinition FindTypeDefinition(string fullName) => TypeCache.Get(fullName) ?? throw new NullReferenceException();

		public TypeDefinition FindTypeDefinition<T>()
		{
			return TypeCache.Get(typeof(T).FullName) ?? throw new NullReferenceException();
		}
	}

	internal static class TypeCache
	{
		private static Dictionary<string, TypeDefinition> cache = new Dictionary<string, TypeDefinition>();
		private static List<ModuleDefinition> modulesForScanning = new List<ModuleDefinition>();

		static TypeCache()
		{
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				AssemblyDefinition definition = AssemblyDefinition.ReadAssembly(assembly.Location);
				modulesForScanning.AddRange(definition.Modules);
			}
		}

		public static TypeDefinition? Get(string fullName)
		{
			if (cache.TryGetValue(fullName, out TypeDefinition typeDefinition)) return typeDefinition;

			foreach (ModuleDefinition module in modulesForScanning)
			{
				TypeDefinition? type = module.GetType(fullName);
				if (type != null)
				{
					cache[fullName] = type;
					return type;
				}
			}

			return null;
		}
	}
}