using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Proxima
{
	public struct SpirvReflectedResource
	{
		public uint ID;
		public string Name => Marshal.PtrToStringUTF8(name) ?? "Unknown Name";

		public uint base_type_id;
		public uint type_id;
		private IntPtr name;

		public string GetName(IntPtr compiler) => Marshal.PtrToStringAuto(SPIRV.GetName(compiler, ID)) ?? "unknown";

		public uint GetDecoration(IntPtr compiler, SpirvDecoration decoration) => SPIRV.GetDecoration(compiler, ID, decoration);
	}

	public struct SpirvEntryPoint
	{
		public SpirvExecutionModel ExecutionModel;
		public string Name => Marshal.PtrToStringUTF8(namePtr) ?? "Unknown Name";

		public IntPtr namePtr;
	}
}