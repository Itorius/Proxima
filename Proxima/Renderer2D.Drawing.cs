using System.Numerics;
using Vortice.Mathematics;

namespace Proxima
{
	public static partial class Renderer2D
	{
		public static void DrawQuad(Vector3 position, Vector2 size, Color4 color)
		{
			var (r, g, b, a) = color;

			vertices.Add(new Vertex { Position = new Vector3(position.X - size.X * 0.5f, position.Y - size.Y * 0.5f, position.Z), Color = new Color4(r, g, b, a), UV = new Vector2(0f, 0f) });
			vertices.Add(new Vertex { Position = new Vector3(position.X + size.X * 0.5f, position.Y - size.Y * 0.5f, position.Z), Color = new Color4(r, g, b, a), UV = new Vector2(1f, 0f) });
			vertices.Add(new Vertex { Position = new Vector3(position.X + size.X * 0.5f, position.Y + size.Y * 0.5f, position.Z), Color = new Color4(r, g, b, a), UV = new Vector2(1f, 1f) });
			vertices.Add(new Vertex { Position = new Vector3(position.X - size.X * 0.5f, position.Y + size.Y * 0.5f, position.Z), Color = new Color4(r, g, b, a), UV = new Vector2(0f, 1f) });

			indices.Add(nextQuadID * 4 + 0);
			indices.Add(nextQuadID * 4 + 1);
			indices.Add(nextQuadID * 4 + 2);
			indices.Add(nextQuadID * 4 + 2);
			indices.Add(nextQuadID * 4 + 3);
			indices.Add(nextQuadID * 4 + 0);

			nextQuadID++;
		}
		
		public static void DrawQuad(Vector2 position, Vector2 size, Color4 color)
		{
			var (r, g, b, a) = color;

			vertices.Add(new Vertex { Position = new Vector3(position.X - size.X * 0.5f, position.Y - size.Y * 0.5f, 0f), Color = new Color4(r, g, b, a), UV = new Vector2(0f, 0f) });
			vertices.Add(new Vertex { Position = new Vector3(position.X + size.X * 0.5f, position.Y - size.Y * 0.5f, 0f), Color = new Color4(r, g, b, a), UV = new Vector2(1f, 0f) });
			vertices.Add(new Vertex { Position = new Vector3(position.X + size.X * 0.5f, position.Y + size.Y * 0.5f, 0f), Color = new Color4(r, g, b, a), UV = new Vector2(1f, 1f) });
			vertices.Add(new Vertex { Position = new Vector3(position.X - size.X * 0.5f, position.Y + size.Y * 0.5f, 0f), Color = new Color4(r, g, b, a), UV = new Vector2(0f, 1f) });

			indices.Add(nextQuadID * 4 + 0);
			indices.Add(nextQuadID * 4 + 1);
			indices.Add(nextQuadID * 4 + 2);
			indices.Add(nextQuadID * 4 + 2);
			indices.Add(nextQuadID * 4 + 3);
			indices.Add(nextQuadID * 4 + 0);

			nextQuadID++;
		}

		public static void DrawQuad(Vector2 position, Vector2 size, Color color)
		{
			float r = color.R / 255f;
			float g = color.G / 255f;
			float b = color.B / 255f;
			float a = color.A / 255f;

			vertices.Add(new Vertex { Position = new Vector3(position.X - size.X * 0.5f, position.Y - size.Y * 0.5f, 0f), Color = new Color4(r, g, b, a), UV = new Vector2(0f, 0f) });
			vertices.Add(new Vertex { Position = new Vector3(position.X + size.X * 0.5f, position.Y - size.Y * 0.5f, 0f), Color = new Color4(r, g, b, a), UV = new Vector2(1f, 0f) });
			vertices.Add(new Vertex { Position = new Vector3(position.X + size.X * 0.5f, position.Y + size.Y * 0.5f, 0f), Color = new Color4(r, g, b, a), UV = new Vector2(1f, 1f) });
			vertices.Add(new Vertex { Position = new Vector3(position.X - size.X * 0.5f, position.Y + size.Y * 0.5f, 0f), Color = new Color4(r, g, b, a), UV = new Vector2(0f, 1f) });

			indices.Add(nextQuadID * 4 + 0);
			indices.Add(nextQuadID * 4 + 1);
			indices.Add(nextQuadID * 4 + 2);
			indices.Add(nextQuadID * 4 + 2);
			indices.Add(nextQuadID * 4 + 3);
			indices.Add(nextQuadID * 4 + 0);

			nextQuadID++;
		}
	}
}