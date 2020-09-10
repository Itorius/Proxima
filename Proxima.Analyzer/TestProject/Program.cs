using System;
using System.Drawing;
using System.Numerics;

namespace TestProject
{
	internal class Program
	{
		public struct TransformComponent
		{
			public readonly Matrix4x4 transform;

			public TransformComponent(Matrix4x4 transform) => this.transform = transform;
		}

		public struct ColorComponent
		{
			public readonly Color color;

			public ColorComponent(Color color) => this.color = color;

			public ColorComponent(byte r, byte g, byte b) => color = Color.FromArgb(r, g, b);
		}

		public struct Entity
		{
			public T AddComponent<T>(params object[] args) where T : struct
			{
				T component = (T)Activator.CreateInstance(typeof(T), args);
				return component;
			}
		}


		private static void Main(string[] args)
		{
			Entity e = new Entity();
			TransformComponent transform = e.AddComponent<TransformComponent>(Matrix4x4.CreateOrthographic(1280, 720, -1f, 1f));

			e.AddComponent<ColorComponent>(255, 170, 0);
			e.AddComponent<ColorComponent>(Color.CornflowerBlue);

			transform = e.AddComponent<TransformComponent>();
		}
	}
}