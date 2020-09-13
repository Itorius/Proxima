using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using MonoMod.Cil;
using MonoMod.Utils;

namespace Proxima.Weaver
{
	public class LogWeaver : BaseWeaver
	{
		public override void Execute()
		{
			foreach (MethodDefinition methodDefinition in ModuleDefinition.Types.SelectMany(type => type.Methods).Where(method => !method.IsAbstract)) ProcessMethod(methodDefinition);
		}

		private void ProcessMethod(MethodDefinition methodDefinition)
		{
			methodDefinition.Body.SimplifyMacros();

			ILContext context = new ILContext(methodDefinition);
			ILCursor cursor = new ILCursor(context);
			TypeDefinition logType = ModuleDefinition.ImportReference(FindTypeDefinition(typeof(Log).FullName)).Resolve();

			MethodReference reference = default;
			while (cursor.TryGotoNext(MoveType.AfterLabel, i => i.MatchCallOrCallvirt(out reference)))
			{
				if (reference.DeclaringType.Resolve() != logType.Resolve()) continue;

				MethodDefinition def = logType.FindMethod(reference.GetID(simple: true) + "App");

				cursor.Remove();
				cursor.Emit(OpCodes.Call, ModuleDefinition.ImportReference(def));
			}

			methodDefinition.Body.OptimizeMacros();
		}
	}
}