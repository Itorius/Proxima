using Proxima;
using Proxima.ECS;
using Proxima.Weaver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Vortice.Mathematics;

namespace Sandbox
{
	public class SandboxApp : Application
	{
		private const int TestEntities = 256;

		public static Application CreateApplication()
		{
			SandboxApp app = new SandboxApp(new Options
			{
				Title = "Sandbox",
				Size = new Size(1280, 720),
				VSync = false
			});

			// todo: this will get replaced
			// app.window.Render += Render;
			// app.window.Update += Update;
			// app.window.Load += Load;
			// app.window.Close += Close;
			return app;
		}

		private static void Close()
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			foreach ((string key, List<double> value) in ProfileWeaver.profileData) Console.WriteLine($"Mean execution time of '{key}': {value.Average():F2} ms");
			Console.ResetColor();
		}

		[Profile(false)]
		private static void Update()
		{
			var view = Registry.View<TransformComponent, ColorComponent>();

			float count = 0;

			foreach (Entity entity in view)
			{
				var (transformComponent, colorComponent) = view.Get<TransformComponent, ColorComponent>(entity);
				count += transformComponent.Transform.M11;
				count += colorComponent.color.R;
			}

			Console.WriteLine(count);

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
		private void Load()
		{
			Entity e = Entity.Null;

			for (int i = 0; i < TestEntities; i++)
			{
				e = Registry.CreateEntity($"Entity {i + 1}");

				e.AddComponent<TransformComponent>(Matrix4x4.CreateTranslation(i, 0f, 0f));

				int color = (int)((float)i / TestEntities * 255);
				e.AddComponent<ColorComponent>(new Color(color, color, color));
			}

			Registry.DestroyEntity(e);
			Registry.CreateEntity("Doot");
		}

		[Profile(false)]
		private static void Render()
		{
		}

		public SandboxApp(Options options) : base(options)
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