using System;
using Vortice.Vulkan;

namespace Proxima.Graphics
{
	public class IndexBuffer<T> : Buffer where T : unmanaged
	{
		private readonly VkDeviceMemory memory;

		public VkBuffer Buffer { get; }
		public VkIndexType IndexType { get; }
		public uint Length { get; }

		public unsafe IndexBuffer(GraphicsDevice graphicsDevice, T[] data) : base(graphicsDevice)
		{
			this.graphicsDevice = graphicsDevice;

			IndexType = data[0] switch
			{
				byte => VkIndexType.Uint8EXT,
				ushort => VkIndexType.Uint16,
				uint => VkIndexType.Uint32,
				_ => throw new Exception("Unsupported index buffer type " + typeof(T).Name)
			};

			Length = (uint)data.Length;

			ulong bufferSize = (ulong)(sizeof(T) * data.Length);
			var (stagingBuffer, stagingBufferMemory) = VulkanUtils.CreateBuffer(graphicsDevice, bufferSize, VkBufferUsageFlags.TransferSrc, VkMemoryPropertyFlags.HostVisible | VkMemoryPropertyFlags.HostCoherent);

			void* dataPtr = null;
			Vulkan.vkMapMemory(graphicsDevice.LogicalDevice, stagingBufferMemory, 0, bufferSize, 0, &dataPtr);
			fixed (T* ptr = &data[0]) System.Buffer.MemoryCopy(ptr, dataPtr, bufferSize, bufferSize);
			Vulkan.vkUnmapMemory(graphicsDevice.LogicalDevice, stagingBufferMemory);

			var (vkBuffer, vkDeviceMemory) = VulkanUtils.CreateBuffer(graphicsDevice, bufferSize, VkBufferUsageFlags.TransferDst | VkBufferUsageFlags.IndexBuffer, VkMemoryPropertyFlags.DeviceLocal);
			Buffer = vkBuffer;
			memory = vkDeviceMemory;

			VulkanUtils.CopyBuffer(graphicsDevice, stagingBuffer, Buffer, bufferSize);

			Vulkan.vkDestroyBuffer(graphicsDevice.LogicalDevice, stagingBuffer, null);
			Vulkan.vkFreeMemory(graphicsDevice.LogicalDevice, stagingBufferMemory, null);
		}

		public override unsafe void Dispose()
		{
			Vulkan.vkDestroyBuffer(graphicsDevice.LogicalDevice, Buffer, null);
			Vulkan.vkFreeMemory(graphicsDevice.LogicalDevice, memory, null);
		}
	}
}