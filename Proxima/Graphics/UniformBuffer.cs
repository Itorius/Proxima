using System.Numerics;
using Vortice.Vulkan;

namespace Proxima.Graphics
{
	public struct UniformBufferObject
	{
		public Matrix4x4 Model;
		public Matrix4x4 View;
		public Matrix4x4 Projection;
	}

	public class UniformBuffer<T> : Buffer where T : unmanaged
	{
		private readonly VkDeviceMemory Memory;
		public VkBuffer Buffer { get; }
		public ulong Size { get; }

		public unsafe UniformBuffer(GraphicsDevice graphicsDevice) : base(graphicsDevice)
		{
			this.graphicsDevice = graphicsDevice;

			Size = (ulong)sizeof(T);

			var (buffer, memory) = VulkanUtils.CreateBuffer(graphicsDevice, Size, VkBufferUsageFlags.UniformBuffer, VkMemoryPropertyFlags.HostVisible | VkMemoryPropertyFlags.HostCoherent);
			Buffer = buffer;
			Memory = memory;
		}

		public override unsafe void Dispose()
		{
			Vulkan.vkDestroyBuffer(graphicsDevice.LogicalDevice, Buffer, null);
			Vulkan.vkFreeMemory(graphicsDevice.LogicalDevice, Memory, null);
		}

		public unsafe void SetData(T data)
		{
			void* dataPtr = null;
			Vulkan.vkMapMemory(graphicsDevice.LogicalDevice, Memory, 0, Size, 0, &dataPtr);
			System.Buffer.MemoryCopy(&data, dataPtr, Size, Size);
			Vulkan.vkUnmapMemory(graphicsDevice.LogicalDevice, Memory);
		}
	}
}