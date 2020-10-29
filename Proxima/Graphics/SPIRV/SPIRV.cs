using System;
using System.Reflection;
using System.Runtime.InteropServices;

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
			if (libraryName == LibraryName) NativeLibrary.TryLoad("lib/libspirv-cross-c-shared.so", assembly, DllImportSearchPath.ApplicationDirectory, out libHandle);
			return libHandle;
		}

		[DllImport(LibraryName, EntryPoint = "spvc_context_create")]
		public static extern SpirvResult CreateContext(out IntPtr context);

		[DllImport(LibraryName, EntryPoint = "spvc_context_set_error_callback")]
		public static extern void SetErrorCallback(IntPtr context, IntPtr func, IntPtr userdata);

		[DllImport(LibraryName, EntryPoint = "spvc_context_destroy")]
		public static extern void DestroyContext(IntPtr context);

		[DllImport(LibraryName, EntryPoint = "spvc_context_parse_spirv")]
		public static extern SpirvResult ParseSPIRV(IntPtr context, IntPtr data, uint wordCount, out IntPtr ir);

		[DllImport(LibraryName, EntryPoint = "spvc_context_create_compiler")]
		public static extern SpirvResult CreateCompiler(IntPtr context, SpirvBackend backend, IntPtr ir, SpirvCaptureMode mode, out IntPtr compiler);

		[DllImport(LibraryName, EntryPoint = "spvc_compiler_create_shader_resources")]
		public static extern SpirvResult CreateShaderResources(IntPtr compiler, out IntPtr resources);

		[DllImport(LibraryName, EntryPoint = "spvc_compiler_get_entry_points")]
		public static extern void GetEntryPoints(IntPtr compiler, out IntPtr entryPoints, out uint entryPointCount);

		public static unsafe ReadOnlySpan<SpirvEntryPoint> GetEntryPoints(IntPtr resources)
		{
			GetEntryPoints(resources, out var list, out var size);

			ReadOnlySpan<SpirvEntryPoint> span = new ReadOnlySpan<SpirvEntryPoint>((void*)list, (int)size);
			return span;
		}

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

		[DllImport(LibraryName, EntryPoint = "spvc_compiler_get_type_handle")]
		public static extern IntPtr GetTypeHandle(IntPtr compiler, ulong id);

		[DllImport(LibraryName, EntryPoint = "spvc_type_get_basetype")]
		public static extern BaseType GetTypeBaseType(IntPtr type);

		[DllImport(LibraryName, EntryPoint = "spvc_type_get_num_member_types")]
		public static extern uint GetMemberTypesCount(IntPtr type);

		[DllImport(LibraryName, EntryPoint = "spvc_type_get_member_type")]
		public static extern uint GetMemberType(IntPtr type, uint index);

		[DllImport(LibraryName, EntryPoint = "spvc_type_get_storage_class")]
		public static extern StorageClass GetTypeStorageClass(IntPtr type);

		[DllImport(LibraryName, EntryPoint = "spvc_compiler_get_name")]
		public static extern IntPtr GetName(IntPtr compiler, ulong id);

		[DllImport(LibraryName, EntryPoint = "spvc_type_get_image_dimension")]
		public static extern SpirvDim GetImageDimension(IntPtr compiler, ulong id);
		
		[DllImport(LibraryName, EntryPoint = "spvc_context_release_allocations")]
		public static extern void ReleaseAllocations(IntPtr context);

		public delegate void SPIRVCallback(IntPtr data, IntPtr error);
	}
}