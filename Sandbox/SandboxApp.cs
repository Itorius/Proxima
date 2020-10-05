using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Proxima;
using Proxima.ECS;
using Proxima.Weaver;
using Vortice.Mathematics;

namespace Sandbox
{
	public class SandboxApp : Application
	{
		private const int TestEntities = 256;

		public static Application CreateApplication() => new SandboxApp(new Options
		{
			Title = "Sandbox",
			Size = new Size(1280, 720),
			VSync = false
		});

		public override void Update()
		{
			var view = Registry.View<TransformComponent, ColorComponent>();

			foreach (Entity entity in view)
			{
				var (transformComponent, colorComponent) = view.Get<TransformComponent, ColorComponent>(entity);
			}
		}

		public override void Render()
		{
			// Color4 color = new HslColor(MathF.Sin(Time.TotalUpdateTime) * 0.5f + 0.5f, 1f, 0.5f).ToRGB();
			Renderer2D.Begin(System.Drawing.Color.Black);

			for (int i = 0; i < 10; i++)
			{
				float x = MathF.Cos(MathF.Tau / 10 * i) * 250f;
				float y = MathF.Sin(MathF.Tau / 10 * i) * 250f;
				Renderer2D.DrawQuad(new Vector2(x, y), new Vector2(100f), System.Drawing.Color.DeepPink);
			}

			Renderer2D.End();
		}

		public override void Close()
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			foreach ((string key, List<double> value) in ProfileWeaver.profileData) Console.WriteLine($"Mean execution time of '{key}': {value.Average():F2} ms");
			Console.ResetColor();
		}

		[Profile]
		public override void Load()
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