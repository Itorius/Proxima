using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using MonoMod.Utils;

namespace Proxima
{
	internal static class SPIRV
	{
		private const string LibraryName = "libspirv-cross-c-shared";

		static SPIRV()
		{
			NativeLibrary.SetDllImportResolver(typeof(SPIRV).Assembly, ImportResolver);
		}

		private static IntPtr ImportResolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
		{
			IntPtr libHandle = IntPtr.Zero;
			if (libraryName == LibraryName) NativeLibrary.TryLoad("libspirv-cross-c-shared.so", assembly, DllImportSearchPath.System32, out libHandle);
			return libHandle;
		}

		[DllImport(LibraryName, EntryPoint = "spvc_context_create")]
		public static extern SpirvResult CreateContext(out IntPtr context);

		[DllImport(LibraryName, EntryPoint = "spvc_context_set_error_callback")]
		public static extern void SetDebugCallback(IntPtr context, IntPtr func, IntPtr userdata);

		[DllImport(LibraryName, EntryPoint = "spvc_context_destroy")]
		public static extern void DestroyContext(IntPtr context);

		[DllImport(LibraryName, EntryPoint = "spvc_context_parse_spirv")]
		public static extern SpirvResult ParseSPIRV(IntPtr context, IntPtr data, uint wordCount, out IntPtr ir);

		[DllImport(LibraryName, EntryPoint = "spvc_context_create_compiler")]
		public static extern SpirvResult CreateCompiler(IntPtr context, SpirvBackend backend, IntPtr ir, SpirvCaptureMode mode, out IntPtr compiler);

		[DllImport(LibraryName, EntryPoint = "spvc_compiler_create_shader_resources")]
		public static extern SpirvResult CreateShaderResources(IntPtr compiler, out IntPtr resources);

		[DllImport(LibraryName, EntryPoint = "spvc_resources_get_resource_list_for_type")]
		public static extern SpirvResult GetResourceListForType(IntPtr resources, SpirvResourceType resourceType, out IntPtr resourceList, out uint resourceSize);

		public static unsafe ReadOnlySpan<SpirvReflectedResource> GetResourceListForType(IntPtr resources, SpirvResourceType resourceType)
		{
			GetResourceListForType(resources, resourceType, out var list, out var size);

			ReadOnlySpan<SpirvReflectedResource> span = new ReadOnlySpan<SpirvReflectedResource>((void*)list, (int)size);
			return span;
		}

		[DllImport(LibraryName, EntryPoint = "spvc_compiler_get_decoration")]
		public static extern uint GetDecoration(IntPtr compiler, ulong id, SpirvDecoration decoration);

		[DllImport(LibraryName, EntryPoint = "spvc_context_release_allocations")]
		public static extern void ReleaseAllocations(IntPtr context);

		internal static (ShaderStage, Dictionary<ShaderStage, string>) GetStages(string path)
		{
			const int OpEntryPoint = 15;

			ShaderStage shaderStages = ShaderStage.None;
			Dictionary<ShaderStage, string> entryPoints = new Dictionary<ShaderStage, string>();
			
			using BinaryReader stream = new BinaryReader(File.OpenRead(path), Encoding.UTF8);

// Parse SPIR-V data
			Debug.Assert(stream.ReadInt32() == 0x07230203);
			int version = stream.ReadInt32();
			int genmagnum = stream.ReadInt32();
			int bound = stream.ReadInt32();
			int reserved = stream.ReadInt32();

// Instruction stream
			while (stream.BaseStream.Position < stream.BaseStream.Length)
			{
				long pos = stream.BaseStream.Position;
				uint bytes = stream.ReadUInt32();
				int opcode = (int)bytes & 0xffff;
				int wordcount = (int)(bytes >> 16) & 0xffff;
				if (opcode == OpEntryPoint)
				{
					int executionModel = stream.ReadInt32();
					Debug.Assert(executionModel >= 0);
					if (executionModel < 6)
					{
						int entryPointID = stream.ReadInt32(); // entry point

						ShaderStage currentStage = executionModel switch
						{
							0 => ShaderStage.Vertex,
							1 => ShaderStage.TessellationControl,
							2 => ShaderStage.TessellationEvaluation,
							3 => ShaderStage.Geometry,
							4 => ShaderStage.Fragment,
							5 => ShaderStage.GLCompute,
						};
						shaderStages |= currentStage;
						
						entryPoints.Add(currentStage,stream.ReadNullTerminatedString() );
					}
				}

				stream.BaseStream.Seek(pos + wordcount * 4, SeekOrigin.Begin);
			}

			return (shaderStages, entryPoints);
		}
	}
}