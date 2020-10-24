using System.Collections.Generic;
using Proxima.Graphics;
using Vortice.Vulkan;

namespace Proxima
{
	public static class AssetManager
	{
		private static GraphicsDevice gd;
		private static Dictionary<string, Shader> shaderCache = new Dictionary<string, Shader>();

		internal static void Initialize(GraphicsDevice graphicsDevice)
		{
			gd = graphicsDevice;
		}

		internal static void Dispose()
		{
			Vulkan.vkDeviceWaitIdle(gd.LogicalDevice);

			foreach (KeyValuePair<string, Shader> pair in shaderCache) pair.Value.Dispose();
		}

		public static Shader LoadShader(string path)
		{
			if (shaderCache.TryGetValue(path, out Shader shader)) return shader;

			shader = new Shader(gd, path);
			shaderCache.Add(path, shader);
			return shader;
		}
	}
}