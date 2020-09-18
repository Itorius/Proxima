using GLFW;
using NLog;
using Proxima.Graphics;
using Vortice.Mathematics;
using Exception = System.Exception;

namespace Proxima
{
	public abstract class Application
	{
		public struct Options
		{
			public string Title;
			public Size Size;
			public bool VSync;
		}

		private NativeWindow window;
		private GraphicsDevice GraphicsDevice;

		public Application(Options options)
		{
			if (!Glfw.Init()) throw new Exception("Failed to initialize GLFW");
			if (!Vulkan.IsSupported) throw new Exception("GLFW does not support Vulkan");

			Glfw.WindowHint(Hint.ClientApi, ClientApi.None);
			Log.Debug(Glfw.Version);

			window = new NativeWindow(options.Size.Width, options.Size.Height, options.Title, Monitor.None, Window.None);
			window.KeyPress += (sender, args) =>
			{
				if (args.Key == Keys.Escape) Glfw.SetWindowShouldClose(window, true);
			};

			GraphicsDevice = new GraphicsDevice(window);

			#if ENABLE_VALIDATION
			GraphicsDevice.EnableValidation();
			#endif

			GraphicsDevice.Initialize();
		}

		private void Cleanup()
		{
			GraphicsDevice.Dispose();

			Glfw.DestroyWindow(window);
			Glfw.Terminate();

			LogManager.Shutdown();
		}

		internal void Run()
		{
			while (!Glfw.WindowShouldClose(window))
			{
				Glfw.PollEvents();

				GraphicsDevice.Draw();
			}

			Cleanup();
		}
	}
}