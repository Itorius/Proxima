using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Proxima;
using Proxima.ECS;
using Proxima.Graphics;
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

		public override void OnUpdate()
		{
			var view = Registry.View<TransformComponent, ColorComponent>();

			foreach (Entity entity in view)
			{
				var (transformComponent, colorComponent) = view.Get<TransformComponent, ColorComponent>(entity);
			}
		}

		public override void OnRender()
		{
			Shader shader = AssetManager.LoadShader("Assets/Mandelbrot_Dist");

			Color4 color = new HslColor(MathF.Sin(Time.TotalUpdateTime) * 0.5f + 0.5f, 1f, 0.5f, 0.7f).ToRGB();

			Matrix4x4 projection = Matrix4x4.CreateOrthographic(window.ClientWidth, window.ClientHeight, -1f, 1f);
			Matrix4x4 view = Matrix4x4.CreateTranslation(MathF.Sin(Time.TotalUpdateTime) * 100f, 0f, 0f);
			view = Matrix4x4.Identity;

			Renderer2D.Begin(view * projection, System.Drawing.Color.Black/*, shader*/);

			float scale = 1.5f;

			// Renderer2D.GraphicsPipelines[shader].GetBuffer<Renderer2D.Data>().SetData(new Renderer2D.Data
			// {
			// 	u_Area = new Vector4(0f, 0f, scale * (window.ClientWidth / (float)window.ClientHeight), scale),
			// 	u_MaxIterations = 16,
			// 	u_Angle = MathF.PI * 0.5f,
			// 	u_Time = 0f
			// });

			Renderer2D.DrawQuad(new Vector3(0f, 0f, -0.1f), new Vector2(window.ClientWidth, window.ClientHeight), new Color4(Vector4.One));

			// const int toms = 25;
			// for (int i = 0; i < toms; i++)
			// {
			// 	float angle = MathF.Tau / toms * i + Time.TotalUpdateTime;
			// 	float x = MathF.Cos(angle) * 250f;
			// 	float y = MathF.Sin(angle) * 250f;
			// 	Renderer2D.DrawQuad(new Vector2(x, y), new Vector2(100f), color);
			// }

			// Renderer2D.DrawQuad(new Vector3(-250f, 0f, -0.1f), new Vector2(500f), new Color4(0.9f, 0.1f, 0.1f, 1f));

			Renderer2D.End();
		}

		public override void OnClose()
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			foreach ((string key, List<double> value) in ProfileWeaver.profileData) Console.WriteLine($"Mean execution time of '{key}': {value.Average():F2} ms");
			Console.ResetColor();
		}

		[Profile]
		public override void OnLoad()
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