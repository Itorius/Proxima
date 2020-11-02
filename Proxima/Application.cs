using System;
using System.Numerics;
using GLFW;
using ImGuiNET;
using NLog;
using Proxima.Graphics;
using Exception = System.Exception;

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

				ImGuiIOPtr io = ImGui.GetIO();
				// IM_ASSERT(io.Fonts->IsBuilt() && "Font atlas not built! It is generally built by the renderer backend. Missing call to renderer _NewFrame() function? e.g. ImGui_ImplOpenGL3_NewFrame().");

				io.DisplaySize = new Vector2(window.Size.Width, window.Size.Height);
				io.DisplayFramebufferScale = new Vector2(1f, 1f);

				io.DeltaTime = Time.DeltaUpdateTime;

				ImGui.NewFrame();
				ImGui.ShowDemoWindow();

				GraphicsDevice.BeginFrame();

				var buffer = GraphicsDevice.Begin(new Vector4(0f, 0f, 0f, 0f), GraphicsDevice.CurrentFrameIndex);

				OnUpdate();
				OnRender();

				ImGui.Render();
				var imDrawDataPtr = ImGui.GetDrawData();

				ImGuiController.ImGui_ImplVulkan_RenderDrawData(*imDrawDataPtr.NativePtr, buffer);

				GraphicsDevice.End(buffer);

				GraphicsDevice.EndFrame();
			}

			OnClose();

			Cleanup();
		}
	}
}