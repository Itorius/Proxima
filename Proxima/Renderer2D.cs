using System.Collections.Generic;
using System.Numerics;
using Proxima.Graphics;
using Vortice.Mathematics;
using Vortice.Vulkan;

namespace Proxima
{
	public static class Renderer2D
	{
		private static GraphicsDevice gd;

		public struct Vertex
		{
			public Vector3 position;
			private Color4 color;
			private Vector2 uv;

			public Vertex(float x, float y, float z, float r, float g, float b, float u, float v)
			{
				position = new Vector3(x, y, z);
				color = new Color4(r, g, b);
				uv = new Vector2(u, v);
			}

			public static unsafe VkVertexInputBindingDescription GetBindingDescription()
			{
				VkVertexInputBindingDescription description = new VkVertexInputBindingDescription
				{
					binding = 0,
					stride = (uint)sizeof(Vertex),
					inputRate = VkVertexInputRate.Vertex
				};
				return description;
			}

			public static unsafe VkVertexInputAttributeDescription[] GetAttributeDescriptions()
			{
				VkVertexInputAttributeDescription[] descriptions = new VkVertexInputAttributeDescription[3];

				ref VkVertexInputAttributeDescription description = ref descriptions[0];
				description.binding = 0;
				description.location = 0;
				description.format = VkFormat.R32G32B32SFloat;
				description.offset = 0;

				description = ref descriptions[1];
				description.binding = 0;
				description.location = 1;
				description.format = VkFormat.R32G32B32A32SFloat;
				description.offset = (uint)sizeof(Vector3);

				description = ref descriptions[2];
				description.binding = 0;
				description.location = 2;
				description.format = VkFormat.R32G32SFloat;
				description.offset = (uint)(sizeof(Vector3) + sizeof(Color4));

				return descriptions;
			}
		}

		internal static void Initialize(GraphicsDevice graphicsDevice)
		{
			gd = graphicsDevice;

			VertexBuffer = new VertexBuffer<Vertex>(gd, 1000 * 4);
			IndexBuffer = new IndexBuffer<uint>(gd, 1000 * 6);
		}

		internal static void Cleanup()
		{
			Vulkan.vkDeviceWaitIdle(gd.LogicalDevice);
			
			VertexBuffer.Dispose();
			IndexBuffer.Dispose();
		}

		internal static VkCommandBuffer buffer;
		public static uint imageIndex;

		private static List<Vertex> vertices = new List<Vertex>();
		private static List<uint> indices = new List<uint>();
		private static VertexBuffer<Vertex> VertexBuffer;
		private static IndexBuffer<uint> IndexBuffer;

		public static void Begin(Color4 color)
		{
			buffer = gd.Begin(color, imageIndex);
		}

		public static void Begin(Color color)
		{
			
			
			buffer = gd.Begin(new Color4(color), imageIndex);
		}

		public static void End()
		{
			VertexBuffer.SetData(vertices.ToArray());
			IndexBuffer.SetData(indices.ToArray());
			vertices.Clear();
			indices.Clear();
			nextQuadID = 0;

			Vulkan.vkCmdBindVertexBuffers(buffer, 0, VertexBuffer.Buffer);
			Vulkan.vkCmdBindIndexBuffer(buffer, IndexBuffer.Buffer, 0, IndexBuffer.IndexType);

			Vulkan.vkCmdBindDescriptorSets(buffer, VkPipelineBindPoint.Graphics, gd.PipelineLayout, 0, gd.DescriptorSets[imageIndex]);

			Vulkan.vkCmdDrawIndexed(buffer, IndexBuffer.Length, 1, 0, 0, 0);

			gd.End(buffer);
		}

		private static uint nextQuadID;

		public static void DrawQuad(Vector2 position, Vector2 size, Color4 color)
		{
			var (r, g, b, a) = color;
			vertices.Add(new Vertex(position.X - size.X * 0.5f, position.Y - size.Y * 0.5f, 0f, r, g, b, 0f, 0f));
			vertices.Add(new Vertex(position.X + size.X * 0.5f, position.Y - size.Y * 0.5f, 0f, r, g, b, 1f, 0f));
			vertices.Add(new Vertex(position.X + size.X * 0.5f, position.Y + size.Y * 0.5f, 0f, r, g, b, 1f, 1f));
			vertices.Add(new Vertex(position.X - size.X * 0.5f, position.Y + size.Y * 0.5f, 0f, r, g, b, 0f, 1f));

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

			vertices.Add(new Vertex(position.X - size.X * 0.5f, position.Y - size.Y * 0.5f, 0f, r, g, b, 0f, 0f));
			vertices.Add(new Vertex(position.X + size.X * 0.5f, position.Y - size.Y * 0.5f, 0f, r, g, b, 1f, 0f));
			vertices.Add(new Vertex(position.X + size.X * 0.5f, position.Y + size.Y * 0.5f, 0f, r, g, b, 1f, 1f));
			vertices.Add(new Vertex(position.X - size.X * 0.5f, position.Y + size.Y * 0.5f, 0f, r, g, b, 0f, 1f));

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