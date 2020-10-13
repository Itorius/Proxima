using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using MonoMod.Utils;
using Proxima.Graphics;
using Vortice.Mathematics;
using Vortice.Vulkan;

namespace Proxima
{
	public static class Renderer2D
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

		private static GraphicsDevice gd;

		public const int MaxQuads = 1000;

		private delegate void callback(IntPtr data, IntPtr error);

		internal static unsafe void Initialize(GraphicsDevice graphicsDevice)
		{
			// IntPtr context = IntPtr.Zero;
			// SPIRV.CreateContext(out IntPtr context);
			//
			// callback c = (data, error) => { Log.Debug(Marshal.PtrToStringAuto(error)); };
			// SPIRV.SetDebugCallback(context, Marshal.GetFunctionPointerForDelegate(c), IntPtr.Zero);
			//
			// byte[] data = File.ReadAllBytes("Assets/test.spv");
			//
			// IntPtr ir = IntPtr.Zero;
			// fixed (byte* ptr = data) SPIRV.ParseSPIRV(context, new IntPtr(ptr), (uint)(data.Length / 4), out ir);
			//
			// SPIRV.CreateCompiler(context, SpirvBackend.GLSL, ir, SpirvCaptureMode.TakeOwnership, out IntPtr compiler);
			//
			// SPIRV.CreateShaderResources(compiler, out IntPtr resources);
			//
			// var list = SPIRV.GetResourceListForType(resources, SpirvResourceType.UniformBuffer);
			//
			// foreach (SpirvReflectedResource resource in list)
			// {
			// 	Log.Debug(resource.Name);
			//
			// 	Log.Debug(string.Format(" Set: {0}, Binding: {1}",
			// 		SPIRV.GetDecoration(compiler, resource.ID, SpirvDecoration.DescriptorSet),
			// 		SPIRV.GetDecoration(compiler, resource.ID, SpirvDecoration.Binding)
			// 	));
			// }
			//
			// list = SPIRV.GetResourceListForType(resources, SpirvResourceType.SampledImage);
			//
			// foreach (SpirvReflectedResource resource in list)
			// {
			// 	Log.Debug(resource.Name);
			//
			// 	Log.Debug(string.Format(" Set: {0}, Binding: {1}",
			// 		SPIRV.GetDecoration(compiler, resource.ID, SpirvDecoration.DescriptorSet),
			// 		SPIRV.GetDecoration(compiler, resource.ID, SpirvDecoration.Binding)
			// 	));
			// }
			//
			// SPIRV.DestroyContext(context);

			gd = graphicsDevice;
			gd.OnInvalidate += () => GraphicsPipelines.ForEach(pair => pair.Value.Invalidate());

			ActiveShader = AssetManager.LoadShader("Assets/test");

			GraphicsPipelines = new Dictionary<Shader, GraphicsPipeline>();

			GraphicsPipelines.Add(ActiveShader, new GraphicsPipeline(gd, typeof(UniformBufferObject), ActiveShader));

			VertexBuffer = new VertexBuffer<Vertex>(gd, MaxQuads * 4);
			IndexBuffer = new IndexBuffer<uint>(gd, MaxQuads * 6);
		}

		private static Shader ActiveShader;

		internal static void Dispose()
		{
			Vulkan.vkDeviceWaitIdle(gd.LogicalDevice);

			GraphicsPipelines.ForEach(pair => pair.Value.Dispose());

			VertexBuffer.Dispose();
			IndexBuffer.Dispose();
		}

		public struct UniformBufferObject
		{
			public Matrix4x4 Camera;
		}

		private static VkCommandBuffer buffer;
		// private static GraphicsPipeline GraphicsPipeline;

		private static Dictionary<Shader, GraphicsPipeline> GraphicsPipelines;

		private static List<Vertex> vertices = new List<Vertex>();
		private static List<uint> indices = new List<uint>();
		private static VertexBuffer<Vertex> VertexBuffer;
		private static IndexBuffer<uint> IndexBuffer;

		public static void Begin(Matrix4x4 camera, Color4 color)
		{
			UniformBufferObject ubo = new UniformBufferObject
			{
				Camera = camera
			};

			// ubo.Projection = Matrix4x4.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), SwapchainExtent.Width / (float)SwapchainExtent.Height, 0.1f, 100f);
			// ubo.Projection.M22 *= -1;

			GraphicsPipelines[ActiveShader].UniformBuffers[gd.CurrentFrameIndex].SetData(ubo);

			buffer = gd.Begin(color, gd.CurrentFrameIndex, GraphicsPipelines[ActiveShader]);
		}

		public static void Begin(Matrix4x4 camera, Color color)
		{
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

			Vulkan.vkCmdBindVertexBuffers(buffer, 0, VertexBuffer.Buffer);
			Vulkan.vkCmdBindIndexBuffer(buffer, IndexBuffer.Buffer, 0, IndexBuffer.IndexType);

			Vulkan.vkCmdBindDescriptorSets(buffer, VkPipelineBindPoint.Graphics, GraphicsPipelines[ActiveShader].PipelineLayout, 0, GraphicsPipelines[ActiveShader].DescriptorSets[gd.CurrentFrameIndex]);

			Vulkan.vkCmdDrawIndexed(buffer, IndexBuffer.Length, 1, 0, 0, 0);

			gd.End(buffer);
		}

		private static uint nextQuadID;

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