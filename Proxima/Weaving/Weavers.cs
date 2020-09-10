using Mono.Cecil;
using System.Collections.Generic;
using System.IO;

namespace Proxima.Weaver
{
	internal static class Weavers
	{
		private static IEnumerable<BaseWeaver> GetWeavers()
		{
			yield return new ProfileWeaver();
			yield return new AddComponentWeaver();
			yield return new GetComponentWeaver();
		}

		public static string Patch(string assembly)
		{
			AssemblyDefinition definition = AssemblyDefinition.ReadAssembly(assembly, new ReaderParameters
			{
				ReadingMode = ReadingMode.Immediate,
				ReadWrite = true,
				InMemory = true,
				ReadSymbols = true
			});

			foreach (BaseWeaver weaver in GetWeavers())
			{
				weaver.ModuleDefinition = definition.MainModule;
				weaver.Execute();
			}

			string path = Path.Join(Path.GetDirectoryName(assembly), Path.GetFileNameWithoutExtension(assembly) + ".modifed.dll");
			definition.Write(path, new WriterParameters
			{
				WriteSymbols = true
			});
			return path;
		}
	}
}