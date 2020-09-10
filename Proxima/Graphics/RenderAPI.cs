using Silk.NET.OpenGL;
using System.Drawing;
using System.Numerics;

namespace Proxima.Graphics
{
	public enum ClearBit
	{
		DepthBufferBit = 256,
		StencilFunction = 1024,
		ColorBufferBit = 16384
	}

	public static class RenderAPI
	{
		private static GL gl;

		internal static void Initialize(GL context) => gl = context;

		public static void ClearColor(Vector4 color) => gl.ClearColor(color.X, color.Y, color.Z, color.Y);
		public static void ClearColor(Color color) => gl.ClearColor(color);

		public static void Clear(ClearBit mask) => gl.Clear((uint)mask);
	}
}