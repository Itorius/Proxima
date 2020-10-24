using System.Collections.Generic;
using System.Numerics;
using Proxima.Graphics;
using Vortice.Mathematics;
using Vortice.Vulkan;

namespace Proxima
{
	public static partial class Renderer2D
	{
		public struct Vertex
		{
			public Vector3 Position;
			public Color4 Color;
			public Vector2 UV;

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

		public struct UniformBufferObject
		{
			public Matrix4x4 Camera;
		}

		private static GraphicsDevice gd;

		private static VkCommandBuffer buffer;

		private static Dictionary<Shader, GraphicsPipeline> GraphicsPipelines;

		private static List<Vertex> vertices = new List<Vertex>();
		private static List<uint> indices = new List<uint>();
		private static VertexBuffer<Vertex> VertexBuffer;
		private static IndexBuffer<uint> IndexBuffer;
		private static Shader defaultShader;

		public const int MaxQuads = 1000;

		internal static void Initialize(GraphicsDevice graphicsDevice)
		{
			gd = graphicsDevice;
			gd.OnInvalidate += () => GraphicsPipelines.ForEach(pair => pair.Value.Invalidate());

			defaultShader = AssetManager.LoadShader("Assets/test");

			VertexBuffer = new VertexBuffer<Vertex>(gd, MaxQuads * 4);
			VertexBuffer.SetVertexInputBindingDescription(Vertex.GetBindingDescription());
			VertexBuffer.SetVertexInputAttributeDescriptions(Vertex.GetAttributeDescriptions());
			
			IndexBuffer = new IndexBuffer<uint>(gd, MaxQuads * 6);

			GraphicsPipelines = new Dictionary<Shader, GraphicsPipeline>
			{
				{
					defaultShader, new GraphicsPipeline(gd, new GraphicsPipeline.Options
					{
						UniformBufferType = typeof(UniformBufferObject),
						Shader = defaultShader,
						Texture = graphicsDevice.Texture
					}, pipeline =>
					{
						pipeline.AddVertexBuffer(VertexBuffer);
						pipeline.AddUniformBuffer<UniformBufferObject>();
					})
				}
			};
		}

		private static Shader ActiveShader;

		public static void Begin(Matrix4x4 camera, Color4 color, Shader? shader = null)
		{
			ChangeShader(shader);

			UniformBufferObject ubo = new UniformBufferObject
			{
				Camera = camera
			};

			// ubo.Projection = Matrix4x4.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), SwapchainExtent.Width / (float)SwapchainExtent.Height, 0.1f, 100f);
			// ubo.Projection.M22 *= -1;

			GraphicsPipelines[ActiveShader].UniformBuffers[gd.CurrentFrameIndex].SetData(ubo);

			buffer = gd.Begin(color, gd.CurrentFrameIndex, GraphicsPipelines[ActiveShader]);
		}

		private static void ChangeShader(Shader? shader)
		{
			if (shader != null)
			{
				if (!GraphicsPipelines.ContainsKey(shader))
				{
					GraphicsPipelines.Add(shader, new GraphicsPipeline(gd, new GraphicsPipeline.Options
					{
						UniformBufferType = typeof(UniformBufferObject),
						Shader = shader
					}, pipeline => { pipeline.AddVertexBuffer(VertexBuffer); }));
				}

				ActiveShader = shader;
			}
			else ActiveShader = defaultShader;
		}

		public static void Begin(Matrix4x4 camera, Color color, Shader? shader = null)
		{
			ChangeShader(shader);

			UniformBufferObject ubo = new UniformBufferObject
			{
				Camera = camera
			};

			// ubo.Projection = Matrix4x4.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), SwapchainExtent.Width / (float)SwapchainExtent.Height, 0.1f, 100f);
			// ubo.Projection.M22 *= -1;

			GraphicsPipelines[ActiveShader].UniformBuffers[gd.CurrentFrameIndex].SetData(ubo);

			buffer = gd.Begin(new Color4(color), gd.CurrentFrameIndex, GraphicsPipelines[ActiveShader]);
		}

		public static void End()
		{
			VertexBuffer.SetData(vertices.ToArray());
			IndexBuffer.SetData(indices.ToArray());
			vertices.Clear();
			indices.Clear();
			nextQuadID = 0;

			VertexBuffer.Bind(buffer);
			IndexBuffer.Bind(buffer);

			GraphicsPipelines[ActiveShader].Bind(buffer);

			Vulkan.vkCmdDrawIndexed(buffer, IndexBuffer.Length, 1, 0, 0, 0);

			gd.End(buffer);
		}

		private static uint nextQuadID;

		internal static void Dispose()
		{
			Vulkan.vkDeviceWaitIdle(gd.LogicalDevice);

			GraphicsPipelines.ForEach(pair => pair.Value.Dispose());

			VertexBuffer.Dispose();
			IndexBuffer.Dispose();
		}
	}
}