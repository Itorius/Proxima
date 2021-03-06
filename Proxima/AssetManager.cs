using System.Collections.Generic;
using System.IO;
using Proxima.Graphics;
using Vortice.Vulkan;

namespace Proxima
{
	public static class AssetManager
	{
		private static GraphicsDevice gd;
		internal static Dictionary<string, Shader> shaderCache = new Dictionary<string, Shader>();
		internal static Dictionary<string, Texture> textureCache = new Dictionary<string, Texture>();

		internal static void Initialize(GraphicsDevice graphicsDevice)
		{
			gd = graphicsDevice;
		}

		internal static void Dispose()
		{
			Vulkan.vkDeviceWaitIdle(gd.LogicalDevice);

			foreach (KeyValuePair<string, Shader> pair in shaderCache) pair.Value.Dispose();
			foreach (KeyValuePair<string, Texture> pair in textureCache) pair.Value.Dispose();
		}

		public static Shader LoadShader(string path)
		{
			if (shaderCache.TryGetValue(path, out Shader shader)) return shader;

			shader = new Shader(gd, path);
			shaderCache.Add(path, shader);
			return shader;
		}
		
		public static Shader LoadShader(string name, string vertexPath, string fragmentPath)
		{
			if (shaderCache.TryGetValue(name, out Shader shader)) return shader;

			shader = new Shader(gd, vertexPath, fragmentPath);
			shaderCache.Add(name, shader);
			return shader;
		}
		
		public static Texture1D LoadTexture1D(string path)
		{
			if (textureCache.TryGetValue(path, out Texture texture)) return (Texture1D)texture;

			texture = new Texture1D(gd, path);
			textureCache.Add(path, texture);
			return (Texture1D)texture;
		}

		public static Texture2D LoadTexture2D(string path)
		{
			if (textureCache.TryGetValue(path, out Texture texture)) return (Texture2D)texture;

			texture = new Texture2D(gd, path);
			textureCache.Add(path, texture);
			return (Texture2D)texture;
		}
	}
}