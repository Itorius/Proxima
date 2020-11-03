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

		protected NativeWindow window;
		protected GraphicsDevice GraphicsDevice;

		public Application(Options options)
		{
			if (!Glfw.Init()) throw new Exception("Failed to initialize GLFW");
			if (!Vulkan.IsSupported) throw new Exception("GLFW does not support Vulkan");

			Glfw.WindowHint(Hint.ClientApi, ClientApi.None);
			Log.Debug(Glfw.Version);

			window = new NativeWindow((int)options.Size.X, (int)options.Size.Y, options.Title, Monitor.None, Window.None);
			window.KeyPress += (sender, args) =>
			{
				if (args.Key == Keys.Escape) Glfw.SetWindowShouldClose(window, true);
			};

			GraphicsDevice = new GraphicsDevice(window);

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

			Glfw.DestroyWindow(window);
			Glfw.Terminate();

			LogManager.Shutdown();
		}

		public virtual void OnUpdate()
		{
		}

		public virtual void OnRender()
		{
		}

		public virtual void OnLoad()
		{
		}

		public virtual void OnClose()
		{
		}

		private Dictionary<Texture, VkDescriptorSet> imguiImageCache = new Dictionary<Texture, VkDescriptorSet>();

		internal unsafe void Run()
		{
			DateTime old = DateTime.Now;

			while (!Glfw.WindowShouldClose(window))
			{
				DateTime now = DateTime.Now;
				TimeSpan diff = now - old;
				old = now;

				Time.DeltaUpdateTime = (float)diff.TotalSeconds;
				Time.TotalUpdateTime += Time.DeltaUpdateTime;

				window.Title = $"Sandbox ({1 / Time.DeltaUpdateTime:F1} FPS)";

				Glfw.PollEvents();

				ImGuiController.NewFrame();

				ImGui.NewFrame();

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

				// ImGui.ShowDemoWindow();

				GraphicsDevice.BeginFrame(new Vector4(0.1f, 0.1f, 0.1f, 1f));

				OnUpdate();
				OnRender();

				ImGui.Render();
				var imDrawDataPtr = ImGui.GetDrawData();

				VkCommandBufferAllocateInfo allocateInfo = new VkCommandBufferAllocateInfo
				{
					sType = VkStructureType.CommandBufferAllocateInfo,
					commandPool = GraphicsDevice.CommandPool,
					level = VkCommandBufferLevel.Secondary,
					commandBufferCount = 1
				};

				Vortice.Vulkan.Vulkan.vkAllocateCommandBuffers(GraphicsDevice.LogicalDevice, &allocateInfo, out VkCommandBuffer b);

				VkCommandBufferInheritanceInfo info = GraphicsDevice.GetInheritanceInfo();

				VkCommandBufferBeginInfo beginInfo = new VkCommandBufferBeginInfo
				{
					sType = VkStructureType.CommandBufferBeginInfo,
					flags = VkCommandBufferUsageFlags.RenderPassContinue | VkCommandBufferUsageFlags.SimultaneousUse,
					pInheritanceInfo = &info
				};

				Vortice.Vulkan.Vulkan.vkBeginCommandBuffer(b, &beginInfo).CheckResult();

				ImGuiController.RenderDrawData(*imDrawDataPtr.NativePtr, b);

				Vortice.Vulkan.Vulkan.vkEndCommandBuffer(b).CheckResult();

				GraphicsDevice.SubmitSecondaryBuffer(b);

				GraphicsDevice.EndFrame();
			}

			OnClose();

			Cleanup();
		}
	}
}