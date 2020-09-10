using Proxima.Graphics;
using Silk.NET.Input;
using Silk.NET.Input.Common;
using Silk.NET.OpenGL;
using Silk.NET.Windowing.Common;
using System;
using System.Drawing;

namespace Proxima
{
	public sealed class Window
	{
		public struct Options
		{
			public string Title;
			public Size Size;
			public bool VSync;
		}

		private IWindow window = null!;
		private GL gl = null!;

		public Window(Options options)
		{
			if (options.Size.Width <= 0 || options.Size.Height <= 0) throw new ArgumentOutOfRangeException(nameof(options), "Window's size must be greater than [0, 0]");

			var silkOptions = WindowOptions.Default;
			silkOptions.Size = options.Size;
			silkOptions.Title = options.Title;
			silkOptions.VSync = options.VSync ? VSyncMode.On : VSyncMode.Off;
			silkOptions.FramesPerSecond = 10000;
			silkOptions.UpdatesPerSecond = 10000;

			Initialize(silkOptions);
		}

		private void Initialize(WindowOptions options)
		{
			window = Silk.NET.Windowing.Window.Create(options);

			window.Load += OnLoad;
			window.Closing += OnClose;

			window.Update += OnUpdate;
			window.Render += OnRender;
		}

		private void OnClose()
		{
			Close();

			Console.WriteLine($"Disposing {GraphicsObject.refs.Count} GL objects.");

			for (int i = 0; i < GraphicsObject.refs.Count; i++)
			{
				WeakReference<GraphicsObject> reference = GraphicsObject.refs[i];
				if (reference.TryGetTarget(out GraphicsObject glObject)) glObject.Dispose();
			}

			GraphicsObject.refs.Clear();
		}

		public void Run() => window.Run();

		private void OnLoad()
		{
			gl = window.CreateOpenGL();
			RenderAPI.Initialize(gl);

			IInputContext input = window.CreateInput();
			// for (int i = 0; i < input.Keyboards.Count; i++) input.Keyboards[i].KeyDown += KeyDown;

			Load();
		}

		public event Action Load = () => { };
		public event Action Update = () => { };
		public event Action Render = () => { };
		public event Action Close = () => { };

		private void OnRender(double delta)
		{
			Time.DeltaRenderTime = delta;
			Time.TotalRenderTime += delta;

			Render();
		}

		private void OnUpdate(double delta)
		{
			Time.DeltaUpdateTime = delta;
			Time.TotalUpdateTime += delta;

			Update();
		}
	}
}