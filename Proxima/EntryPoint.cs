using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Proxima.Weaver;

namespace Proxima
{
	public static class Program
	{
		public static void Main(string[] args)
		{
			if (args.Length != 1) throw new ArgumentException("No dll provided");
			if (!File.Exists(args[0])) throw new FileNotFoundException();

			string path = Weavers.Patch(args[0]);
			Assembly assembly = Assembly.LoadFrom(path);

			var types = assembly.GetTypes();
			MethodInfo? entryPoint = types.Select(type => type.GetMethod("CreateApplication", BindingFlags.Public | BindingFlags.Static)).FirstOrDefault(method => method != null);

			if (entryPoint == null) throw new NullReferenceException($"Application '{assembly.FullName}' failed to define an entry point. (public static BaseApplication CreateApplication() {{}} )");

			Application? app = (Application)entryPoint.Invoke(null, null);

			if (app == null) throw new NullReferenceException("Method 'CreateApplication' returned a null value");

			app.Run();
		}
	}
}