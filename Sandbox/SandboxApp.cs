using Proxima;
using Proxima.ECS;
using Proxima.Graphics;
using Proxima.Weaver;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;

namespace Sandbox
{
	public class SandboxApp : BaseApplication
	{
		private const int TestEntities = 1000;

		public static BaseApplication CreateApplication()
		{
			SandboxApp app = new SandboxApp(new Window.Options
			{
				Size = new Size(1280, 720),
				Title = "Sandbox",
				VSync = false
			});

			// todo: this will get replaced
			app.window.Render += Render;
			app.window.Update += Update;
			app.window.Load += Load;
			app.window.Close += Close;

			return app;
		}

		private static void Close()
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			foreach ((string key, List<double> value) in ProfileWeaver.profileData) Console.WriteLine($"Mean execution time of '{key}': {value.Average():F2} ms");
			Console.ResetColor();
		}

		[Profile]
		private static void Update()
		{
			// var view = Registry.View<TransformComponent, ColorComponent>();
			//
			// float count = 0;
			//
			// foreach (Entity entity in view)
			// {
			// 	// count++;
			//
			// 	var transformComponent = view.Get<TransformComponent>(entity);
			// 	count += transformComponent.Transform.M11;
			// 	// count += colorComponent.color.R;
			// }

			// float count = 0f;
			// var group = Registry.Group<TransformComponent, ColorComponent>();
			//
			// foreach (Entity entity in group)
			// {
			// 	// count++;
			//
			// 	var transformComponent = group.Get<TransformComponent>(entity);
			// 	count += transformComponent.Transform.M11;
			// 	// count += colorComponent.color.R;
			// }
			//
			// Console.WriteLine(count);
		}

		[Profile]
		private static void Load()
		{
			Entity e = default;
			
			for (int i = 0; i < TestEntities; i++)
			{
				e = Registry.CreateEntity($"Entity {i + 1}");

				// e.AddComponent<TransformComponent>(Matrix4x4.CreateTranslation(i, 0f, 0f));
				//
				// if (i % 3 == 0)
				// {
				// 	int color = (int)((float)i / TestEntities * 255);
				// 	e.AddComponent<ColorComponent>(Color.FromArgb(color, color, color));
				// }
			}
			
			Registry.DestroyEntity(e);
			Registry.CreateEntity("");
		}

		[Profile]
		private static void Render()
		{
			RenderAPI.ClearColor(Color.CornflowerBlue);
			RenderAPI.Clear(ClearBit.ColorBufferBit);
		}

		public SandboxApp(Window.Options options) : base(options)
		{
		}
	}

	internal struct TransformComponent
	{
		public Matrix4x4 Transform;

		public TransformComponent(Matrix4x4 transform)
		{
			Transform = transform;
		}
	}

	internal struct ColorComponent
	{
		public Color color;

		public ColorComponent(Color color)
		{
			this.color = color;
		}
	}
}