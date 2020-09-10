using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Proxima.ECS;
using System;
using System.Drawing;
using System.Numerics;

namespace ProximaBenchmarks
{
	public class ViewBenchmarks
	{
		private const int N = 100000;

		public ViewBenchmarks()
		{
			for (int i = 0; i < N; i++)
			{
				Entity e = Registry.CreateEntity($"Entity {i}");
				e.AddComponent<TransformComponent>(Matrix4x4.Identity);
				e.AddComponent<ColorComponent>(Color.CornflowerBlue);
			}
		}

		[Benchmark(Baseline = true)]
		public void IterateContains()
		{
			var view = Registry.View<TransformComponent, ColorComponent>();
			float f = 0f;

			foreach (Entity entity in view)
			{
				TransformComponent transformComponent = view.Get<TransformComponent>(entity);
				f += transformComponent.Transform.M11;
			}

			Console.WriteLine(f);
		}
	}

	internal class Program
	{
		private static void Main(string[] args)
		{
			var summary = BenchmarkRunner.Run<ViewBenchmarks>();
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