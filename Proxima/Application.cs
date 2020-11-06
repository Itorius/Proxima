using System;
using System.Collections.Generic;
using System.Numerics;
using GLFW;
using ImGuiNET;
using NLog;
using Proxima.Graphics;
using Vortice.Vulkan;
using Exception = System.Exception;
using Vulkan = GLFW.Vulkan;

namespace Proxima
{
	public abstract class Application
	{
		public struct Options
		{
			public string Title;
			public Vector2 Size;
			public bool VSync;
		}

		protected readonly NativeWindow Window;
		protected readonly GraphicsDevice GraphicsDevice;

		public Application(Options options)
		{
			if (!Glfw.Init()) throw new Exception("Failed to initialize GLFW");
			if (!Vulkan.IsSupported) throw new Exception("GLFW does not support Vulkan");

			Glfw.WindowHint(Hint.ClientApi, ClientApi.None);

			Window = new NativeWindow((int)options.Size.X, (int)options.Size.Y, options.Title, Monitor.None, GLFW.Window.None);
			Window.KeyPress += (sender, args) =>
			{
				if (args.Key == Keys.Escape) Glfw.SetWindowShouldClose(Window, true);
			};

			GraphicsDevice = new GraphicsDevice(Window);
			GraphicsDevice.SetVerticalSync(options.VSync);

			#if ENABLE_VALIDATION
			GraphicsDevice.EnableValidation();
			#endif

			GraphicsDevice.Initialize();

			AssetManager.Initialize(GraphicsDevice);

			ImGuiController.Initialize(GraphicsDevice);

			Renderer2D.Initialize(GraphicsDevice);

			OnLoad();
		}

		private void Cleanup()
		{
			Renderer2D.Dispose();
			ImGuiController.Dispose();
			AssetManager.Dispose();
			GraphicsDevice.Dispose();

			Glfw.DestroyWindow(Window);
			Glfw.Terminate();

			LogManager.Shutdown();
		}

		protected virtual void OnUpdate()
		{
		}

		protected virtual void OnRender()
		{
		}

		protected virtual void OnLoad()
		{
		}

		protected virtual void OnClose()
		{
		}

		private Dictionary<Texture, VkDescriptorSet> imguiImageCache = new Dictionary<Texture, VkDescriptorSet>();

		internal void Run()
		{
			DateTime old = DateTime.Now;

			while (!Glfw.WindowShouldClose(Window))
			{
				DateTime now = DateTime.Now;
				TimeSpan diff = now - old;
				old = now;

				Time.DeltaUpdateTime = (float)diff.TotalSeconds;
				Time.TotalUpdateTime += Time.DeltaUpdateTime;

				Window.Title = $"Sandbox ({1 / Time.DeltaUpdateTime:F1} FPS)";

				Glfw.PollEvents();

				ImGuiController.NewFrame();

				if (ImGui.Begin("Graphics Device Properties"))
				{
					ImGui.Text("Vulkan v" + Vortice.Vulkan.Vulkan.vkEnumerateInstanceVersion());
					ImGui.Text("GLFW v" + Glfw.Version);

					ImGui.Separator();

					ImGui.Text(GraphicsDevice.PhysicalDevice.Name);
					ImGui.End();
				}

				if (ImGui.Begin("Asset Manager"))
				{
					foreach (var (name, texture) in AssetManager.textureCache)
					{
						ImGui.Text(name);

						if (texture is not Texture2D) continue;
						if (!imguiImageCache.ContainsKey(texture)) imguiImageCache.Add(texture, ImGuiController.AddTexture(texture.Sampler, texture.View, VkImageLayout.ShaderReadOnlyOptimal));

						ImGui.Image((IntPtr)imguiImageCache[texture].Handle, new Vector2(100f));
					}

					ImGui.Separator();

					foreach (var (name, shader) in AssetManager.shaderCache)
					{
						ImGui.Text(name);
					}

					ImGui.End();
				}

				GraphicsDevice.BeginFrame(new Vector4(0.03f, 0.03f, 0.03f, 1f));

				OnUpdate();
				OnRender();

				ImGuiController.Draw();

				GraphicsDevice.EndFrame();
			}

			OnClose();

			Cleanup();
		}
	}
}