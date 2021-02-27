using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using GLFW;
using Proxima;
using Proxima.ECS;
using Proxima.Graphics;
using Proxima.Weaver;

namespace Sandbox
{
	public class SandboxApp : Application
	{
		private const int TestEntities = 256;

		public static Application CreateApplication() => new SandboxApp(new Options
		{
			Title = "Sandbox",
			Size = new Vector2(1280, 720),
			VSync = true
		});

		[Profile]
		protected override void OnLoad()
		{
			// Shader shader = AssetManager.LoadShader("Mandelbrot", "Assets/Renderer2D.vert", "Assets/Mandelbrot_Dist.frag");
			// material = new Material(GraphicsDevice, shader);
			// material.SetTexture("gradientTexture", Renderer2D.gradient);

			Window.MouseScroll += (sender, args) => { scale += (float)(args.Y * 0.1f); };

			Entity e = Entity.Null;

			for (int i = 0; i < TestEntities; i++)
			{
				e = Registry.CreateEntity($"Entity {i + 1}");

				e.AddComponent<TransformComponent>(Matrix4x4.CreateTranslation(i, 0f, 0f));

				float color = (float)i / TestEntities;
				e.AddComponent<ColorComponent>(new Vector4(color, color, color, 1f));
			}

			Registry.DestroyEntity(e);
			Registry.CreateEntity("Doot");
		}

		private Vector2 pos;

		protected override void OnUpdate()
		{
			float movement = Time.DeltaUpdateTime * 100f;

			if (Input.IsKeyDown(Keys.W)) pos.Y += movement;
			if (Input.IsKeyDown(Keys.S)) pos.Y -= movement;
			if (Input.IsKeyDown(Keys.A)) pos.X += movement;
			if (Input.IsKeyDown(Keys.D)) pos.X -= movement;

			var view = Registry.View<TransformComponent, ColorComponent>();

			foreach (Entity entity in view)
			{
				var (transformComponent, colorComponent) = view.Get<TransformComponent, ColorComponent>(entity);
			}
		}

		// private Material material;

		private float angle;
		private float scale = 1f;

		protected override void OnRender()
		{
			Vector4 color = new HslColor(MathF.Sin(Time.TotalUpdateTime) * 0.5f + 0.5f, 1f, 0.5f, 0.1f).ToRGB();
			color = new Vector4(1f, 1f, 1f, 1f);

			Matrix4x4 projection = Matrix4x4.CreateOrthographic(Window.ClientWidth, Window.ClientHeight, -1f, 1f);
			Matrix4x4 view = Matrix4x4.CreateTranslation(pos.X, pos.Y, 0f);

			// // angle += Time.DeltaUpdateTime * 0.1f;
			// // material.SetUniformBufferData("settings", new Renderer2D.Data
			// // {
			// // 	u_Area = new Vector4(pos.X, pos.Y, scale * (window.ClientWidth / (float)window.ClientHeight), scale),
			// // 	u_MaxIterations = 128,
			// // 	u_Angle = angle,
			// // 	u_Time = 0f
			// // });
			//
			Renderer2D.Begin(view * projection /*, material*/);
			// // Renderer2D.DrawQuad(new Vector3(0f, 0f, -0.1f), new Vector2(window.ClientWidth, window.ClientHeight), Vector4.One);
			//
			// const int toms = 25;
			// for (int i = 0; i < toms; i++)
			// {
			// 	float angle = MathF.PI * 2f / toms * i + Time.TotalUpdateTime;
			// 	float x = MathF.Cos(angle) * 250f;
			// 	float y = MathF.Sin(angle) * 250f;
			// 	Renderer2D.DrawQuad(new Vector2(x, y), new Vector2(100f), color);
			// }

			for (int x = 0; x < 10; x++)
			{
				for (int y = 0; y < 10; y++)
				{
					Renderer2D.DrawQuad(new Vector2(x, y) * 110f, new Vector2(100f), color);
				}
			}

			//
			// // Renderer2D.DrawQuad(new Vector3(-200f, 0f, 0.1f), new Vector2(500f), new Vector4(0.9f, 0.1f, 0.1f, 0.3f));
			// // Renderer2D.DrawQuad(new Vector3(200f, 0f, 0.1f), new Vector2(500f), new Vector4(0.9f, 0.1f, 0.1f, 0.3f));
			//
			Renderer2D.End();
		}

		protected override void OnClose()
		{
			// material.Dispose();

			Console.ForegroundColor = ConsoleColor.Yellow;
			foreach ((string key, List<double> value) in ProfileWeaver.profileData) Console.WriteLine($"Mean execution time of '{key}': {value.Average():F2} ms");
			Console.ResetColor();
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
		public Vector4 color;

		public ColorComponent(Vector4 color)
		{
			this.color = color;
		}
	}
}