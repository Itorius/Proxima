using System.Collections.Generic;
using System.Numerics;
using Proxima.Graphics;
using Vortice.Mathematics;
using Vortice.Vulkan;

namespace Proxima
{
	public static partial class Renderer2D
	{
		private struct Vertex
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

		public static Dictionary<Shader, GraphicsPipeline> GraphicsPipelines;

		private static List<Vertex> vertices = new List<Vertex>();
		private static List<uint> indices = new List<uint>();
		private static VertexBuffer<Vertex> VertexBuffer;
		private static IndexBuffer<uint> IndexBuffer;
		private static Shader defaultShader;
		private static Texture2D cat;

		public const int MaxQuads = 1000;

		private struct k
		{
			public Matrix4x4 m;
		}

		internal static void Initialize(GraphicsDevice graphicsDevice)
		{
			gd = graphicsDevice;
			gd.OnInvalidate += () => GraphicsPipelines.ForEach(pair => pair.Value.Invalidate());

			defaultShader = AssetManager.LoadShader("Assets/texture");

			VertexBuffer = new VertexBuffer<Vertex>(gd, MaxQuads * 4);
			VertexBuffer.SetVertexInputBindingDescription(Vertex.GetBindingDescription());
			VertexBuffer.SetVertexInputAttributeDescriptions(Vertex.GetAttributeDescriptions());

			IndexBuffer = new IndexBuffer<uint>(gd, MaxQuads * 6);

			cat = new Texture2D(graphicsDevice, "Assets/Cat.png");

			GraphicsPipelines = new Dictionary<Shader, GraphicsPipeline>
			{
				{
					defaultShader, new GraphicsPipeline(gd, pipeline =>
					{
						pipeline.SetShader(defaultShader);
						pipeline.AddVertexBuffer(VertexBuffer);
						pipeline.AddUniformBuffer<UniformBufferObject>(0);
						pipeline.AddUniformBuffer<k>(3);
						pipeline.AddTexture(1, graphicsDevice.Texture);
						pipeline.AddTexture(2, cat);
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

			GraphicsPipelines[ActiveShader].GetBuffer<UniformBufferObject>().SetData(ubo);
			GraphicsPipelines[ActiveShader].GetBuffer<k>().SetData(new k
			{
				m = Matrix4x4.CreateTranslation(50f, 0f, 0f)
			});

			buffer = gd.Begin(color, gd.CurrentFrameIndex);
			GraphicsPipelines[ActiveShader].Bind(buffer);
		}

		public struct Data
		{
			public Vector4 u_Area;
			public int u_MaxIterations;
			public float u_Angle;
			public float u_Time;
		}

		private static void ChangeShader(Shader? shader)
		{
			if (shader != null)
			{
				if (!GraphicsPipelines.ContainsKey(shader))
				{
					GraphicsPipelines.Add(shader, new GraphicsPipeline(gd, pipeline =>
					{
						pipeline.SetShader(shader);
						pipeline.AddVertexBuffer(VertexBuffer);
						pipeline.AddUniformBuffer<UniformBufferObject>(0);
						pipeline.AddUniformBuffer<Data>(1);
						// pipeline.AddTexture(gd.Texture);
					}));
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

			GraphicsPipelines[ActiveShader].GetBuffer<UniformBufferObject>().SetData(ubo);
			GraphicsPipelines[ActiveShader].GetBuffer<k>().SetData(new k
			{
				m = Matrix4x4.Identity
			});

			buffer = gd.Begin(new Color4(color), gd.CurrentFrameIndex);
			GraphicsPipelines[ActiveShader].Bind(buffer);
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
			cat.Dispose();
		}
	}
}