using System.Collections.Generic;
using System.Numerics;
using Proxima.Graphics;
using Vortice.Vulkan;

namespace Proxima
{
	public static partial class Renderer2D
	{
		internal struct Vertex
		{
			public Vector3 Position;
			public Vector4 Color;
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
				description.offset = (uint)(sizeof(Vector3) + sizeof(Vector4));

				return descriptions;
			}
		}

		public struct UniformBufferObject
		{
			public Matrix4x4 Camera;
		}

		private static GraphicsDevice gd;

		// private static VkCommandBuffer buffer;

		public static Dictionary<Shader, Pipeline> GraphicsPipelines;

		private static List<Vertex> vertices = new();
		private static List<uint> indices = new();
		internal static VertexBuffer<Vertex> VertexBuffer;
		private static IndexBuffer<uint> IndexBuffer;
		private static Shader defaultShader;
		private static Texture2D cat;
		public static Texture1D gradient;
		private static Texture2D tom;
		public static Texture2D PlaceholderTexture;
		public static Texture1D PlaceholderTexture1D;

		public const int MaxQuads = 1000;

		internal struct k
		{
			public Matrix4x4 m;
		}

		internal static void Initialize(GraphicsDevice graphicsDevice)
		{
			gd = graphicsDevice;
			gd.OnInvalidate += () => GraphicsPipelines.ForEach(pair => pair.Value.Invalidate());

			defaultShader = AssetManager.LoadShader("Texture", "Assets/Renderer2D.vert", "Assets/Texture.frag");

			VertexBuffer = new VertexBuffer<Vertex>(gd, MaxQuads * 4);
			VertexBuffer.SetVertexInputBindingDescription(Vertex.GetBindingDescription());
			VertexBuffer.SetVertexInputAttributeDescriptions(Vertex.GetAttributeDescriptions());

			IndexBuffer = new IndexBuffer<uint>(gd, MaxQuads * 6);

			cat = AssetManager.LoadTexture2D("Assets/Cat.png");
			gradient = AssetManager.LoadTexture1D("Assets/gradient.png");
			tom = AssetManager.LoadTexture2D("Assets/Tom.png");
			PlaceholderTexture = AssetManager.LoadTexture2D("Assets/Placeholder.png");
			PlaceholderTexture1D = AssetManager.LoadTexture1D("Assets/Placeholder1D.png");

			GraphicsPipelines = new Dictionary<Shader, Pipeline>
			{
				{
					defaultShader, new Pipeline(gd, pipeline =>
					{
						pipeline.SetShader(defaultShader);
						pipeline.AddVertexBuffer(VertexBuffer);
						pipeline.AddUniformBuffer<UniformBufferObject>(0);
						pipeline.AddUniformBuffer<k>(3);
						pipeline.AddTexture(1, tom);
						pipeline.AddTexture(2, cat);
					})
				}
			};

			ActiveShader = defaultShader;
			
			currentBuffer = new VulkanCommandBuffer(graphicsDevice, graphicsDevice.GetSecondaryBuffer());
			currentBuffer.SetName("Renderer2D");
		}

		private static Shader ActiveShader;

		public struct MandelbrotData
		{
			public Vector4 u_Area;
			public int u_MaxIterations;
			public float u_Angle;
			public float u_Time;
		}

		public static void Begin(Matrix4x4 camera)
		{
			UniformBufferObject ubo = new UniformBufferObject
			{
				Camera = camera
			};

			// ubo.Projection = Matrix4x4.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), SwapchainExtent.Width / (float)SwapchainExtent.Height, 0.1f, 100f);
			// ubo.Projection.M22 *= -1;

			GraphicsPipelines[ActiveShader].GetBuffer<UniformBufferObject>().SetData(ubo);
			// GraphicsPipelines[ActiveShader].GetBuffer<k>().SetData(new k
			// {
			// 	m = Matrix4x4.CreateRotationZ(2 * MathF.PI * (MathF.Sin(Time.TotalUpdateTime) * 0.5f + 0.5f))
			// });

			currentBuffer.Begin(VkCommandBufferUsageFlags.RenderPassContinue | VkCommandBufferUsageFlags.SimultaneousUse, gd.GetInheritanceInfo());
			GraphicsPipelines[ActiveShader].Bind(currentBuffer);
		}

		private static VulkanCommandBuffer currentBuffer;

		public static void End()
		{
			VertexBuffer.SetData(vertices.ToArray());
			IndexBuffer.SetData(indices.ToArray());
			vertices.Clear();
			indices.Clear();
			nextQuadID = 0;

			VertexBuffer.Bind(currentBuffer);
			IndexBuffer.Bind(currentBuffer);

			Vulkan.vkCmdDrawIndexed(currentBuffer, IndexBuffer.Length, 1, 0, 0, 0);

			currentBuffer.End();

			gd.SubmitSecondaryBuffer(currentBuffer);
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