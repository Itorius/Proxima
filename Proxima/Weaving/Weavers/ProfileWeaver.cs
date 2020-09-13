using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.Utils;

namespace Proxima.Weaver
{
	public class ProfileWeaver : BaseWeaver
	{
		private MethodInfo StopwatchStartNew = null!;

		public static Dictionary<string, List<double>> profileData = new Dictionary<string, List<double>>();

		public override void Execute()
		{
			StopwatchStartNew = typeof(Stopwatch).GetMethod(nameof(Stopwatch.StartNew))!;

			foreach (var method in ModuleDefinition.Types.SelectMany(type => type.Methods.Where(method => !method.IsAbstract))) ProcessMethod(method);
		}

		// todo: actually write to a file
		public static void Log(Stopwatch stopwatch, string method, bool write)
		{
			if (!profileData.ContainsKey(method)) profileData.Add(method, new List<double>());

			stopwatch.Stop();
			profileData[method].Add(stopwatch.Elapsed.TotalMilliseconds);

			if (write) Console.WriteLine($"Method '{method}' executed in {stopwatch.Elapsed.TotalMilliseconds} ms");
		}

		private void ProcessMethod(MethodDefinition methodDefinition)
		{
			var attribute = methodDefinition.GetCustomAttribute(typeof(ProfileAttribute).FullName);

			if (attribute == null) return;

			ILContext context = new ILContext(methodDefinition);
			ILCursor cursor = new ILCursor(context);

			VariableDefinition stopwatch = context.AddVariable<Stopwatch>(this);
			VariableDefinition methodName = context.AddVariable<string>(this);
			VariableDefinition writeToConsole = context.AddVariable<bool>(this);

			cursor.Emit(OpCodes.Call, StopwatchStartNew);
			cursor.Emit(OpCodes.Stloc, stopwatch);

			cursor.Emit(OpCodes.Ldstr, methodDefinition.Name);
			cursor.Emit(OpCodes.Stloc, methodName);

			cursor.Emit(OpCodes.Ldc_I4, (bool)attribute.ConstructorArguments[0].Value ? 1 : 0);
			cursor.Emit(OpCodes.Stloc, writeToConsole);

			while (cursor.TryGotoNext(MoveType.AfterLabel, i => i.MatchRet()))
			{
				cursor.Emit(OpCodes.Ldloc, stopwatch);
				cursor.Emit(OpCodes.Ldloc, methodName);
				cursor.Emit(OpCodes.Ldloc, writeToConsole);
				cursor.Emit<ProfileWeaver>(OpCodes.Call, nameof(Log));
				cursor.Index++;
			}
		}
	}
}