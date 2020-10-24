using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ShaderCompiler
{
	public static class Program
	{
		public static void Main(string[] args)
		{
			string currentDirectory = Directory.GetCurrentDirectory();
			foreach (string name in Directory.GetFiles(currentDirectory, "*.vert").Select(Path.GetFileNameWithoutExtension))
			{
				Process.Start(new ProcessStartInfo
				{
					FileName = "glslc",
					Arguments = $"{name}.vert -o temp.vert.spv",
					WindowStyle = ProcessWindowStyle.Hidden
				})?.WaitForExit();

				Process process = Process.Start(new ProcessStartInfo
				{
					FileName = "spirv-cross",
					Arguments = "temp.vert.spv --reflect",
					WindowStyle = ProcessWindowStyle.Hidden,
					RedirectStandardOutput = true
				});
				process?.WaitForExit();

				string vertexReflectionData = process?.StandardOutput.ReadToEnd() ?? "{}";

				Process.Start(new ProcessStartInfo
				{
					FileName = "glslc",
					Arguments = $"{name}.frag -o temp.frag.spv",
					WindowStyle = ProcessWindowStyle.Hidden
				})?.WaitForExit();

				process = Process.Start(new ProcessStartInfo
				{
					FileName = "spirv-cross",
					Arguments = "temp.frag.spv --reflect",
					WindowStyle = ProcessWindowStyle.Hidden,
					RedirectStandardOutput = true
				});
				process?.WaitForExit();

				string fragmentReflectionData = process?.StandardOutput.ReadToEnd() ?? "{}";

				using Stream stream = File.OpenWrite($"{name}.pxshader");
				using BinaryWriter writer = new BinaryWriter(stream);

				byte[] data = File.ReadAllBytes("temp.vert.spv");
				writer.Write("vertex");
				writer.Write(vertexReflectionData);
				writer.Write((uint)data.Length);
				writer.Write(data);

				data = File.ReadAllBytes("temp.frag.spv");
				writer.Write("fragment");
				writer.Write(fragmentReflectionData);
				writer.Write((uint)data.Length);
				writer.Write(data);

				// Process.Start(new ProcessStartInfo
				// {
				// 	FileName = "spirv-link",
				// 	Arguments = $"temp.vert.spv temp.frag.spv -o {name}.spv",
				// 	WindowStyle = ProcessWindowStyle.Hidden
				// })?.WaitForExit();

				File.Delete("temp.vert.spv");
				File.Delete("temp.frag.spv");
			}
		}
	}
}