using System;
using Vortice.Vulkan;

namespace Proxima.Graphics
{
	public class IndexBuffer<T> : Buffer where T : unmanaged
	{
		public VkBuffer Buffer { get; private set; }
		public VkDeviceMemory Memory { get; private set; }

		public VkIndexType IndexType { get; private set; }
		public uint Length { get; private set; }
		private ulong BufferSize;

		public unsafe IndexBuffer(GraphicsDevice graphicsDevice, T[] data) : base(graphicsDevice)
		{
			if (typeof(T) == typeof(byte)) IndexType = VkIndexType.Uint8EXT;
			else if (typeof(T) == typeof(ushort)) IndexType = VkIndexType.Uint16;
			else if (typeof(T) == typeof(uint)) IndexType = VkIndexType.Uint32;
			else throw new Exception("Unsupported index buffer type " + typeof(T).Name);

			Length = (uint)data.Length;

			BufferSize = (ulong)(sizeof(T) * data.Length);
			var (stagingBuffer, stagingBufferMemory) = VulkanUtils.CreateBuffer(graphicsDevice, BufferSize, VkBufferUsageFlags.TransferSrc, VkMemoryPropertyFlags.HostVisible | VkMemoryPropertyFlags.HostCoherent);

			void* dataPtr = null;
			Vulkan.vkMapMemory(graphicsDevice.LogicalDevice, stagingBufferMemory, 0, BufferSize, 0, &dataPtr);
			fixed (T* ptr = data) System.Buffer.MemoryCopy(ptr, dataPtr, BufferSize, BufferSize);
			Vulkan.vkUnmapMemory(graphicsDevice.LogicalDevice, stagingBufferMemory);

			var (vkBuffer, vkDeviceMemory) = VulkanUtils.CreateBuffer(graphicsDevice, BufferSize, VkBufferUsageFlags.TransferDst | VkBufferUsageFlags.IndexBuffer, VkMemoryPropertyFlags.DeviceLocal);
			Buffer = vkBuffer;
			Memory = vkDeviceMemory;

			VulkanUtils.CopyBuffer(graphicsDevice, stagingBuffer, Buffer, BufferSize);

			Vulkan.vkDestroyBuffer(graphicsDevice.LogicalDevice, stagingBuffer, null);
			Vulkan.vkFreeMemory(graphicsDevice.LogicalDevice, stagingBufferMemory, null);
		}

		public unsafe IndexBuffer(GraphicsDevice graphicsDevice, uint elementCount) : base(graphicsDevice)
		{
			BufferSize = (ulong)(elementCount * sizeof(T));
			Length = elementCount;

			if (typeof(T) == typeof(byte)) IndexType = VkIndexType.Uint8EXT;
			else if (typeof(T) == typeof(ushort)) IndexType = VkIndexType.Uint16;
			else if (typeof(T) == typeof(uint)) IndexType = VkIndexType.Uint32;
			else throw new Exception("Unsupported index buffer type " + typeof(T).Name);

			var (vkBuffer, vkDeviceMemory) = VulkanUtils.CreateBuffer(graphicsDevice, BufferSize, VkBufferUsageFlags.TransferDst | VkBufferUsageFlags.IndexBuffer, VkMemoryPropertyFlags.DeviceLocal);
			Buffer = vkBuffer;
			Memory = vkDeviceMemory;
		}

		public unsafe void SetData(T[] data)
		{
			Length = (uint)data.Length;
			ulong newSize = (ulong)(sizeof(T) * data.Length);

			if (newSize > BufferSize)
			{
				BufferSize = newSize;

				Vulkan.vkDestroyBuffer(graphicsDevice.LogicalDevice, Buffer, null);
				Vulkan.vkFreeMemory(graphicsDevice.LogicalDevice, Memory, null);

				var (vkBuffer, vkDeviceMemory) = VulkanUtils.CreateBuffer(graphicsDevice, newSize, VkBufferUsageFlags.TransferDst | VkBufferUsageFlags.IndexBuffer, VkMemoryPropertyFlags.DeviceLocal);
				Buffer = vkBuffer;
				Memory = vkDeviceMemory;
			}

			var (stagingBuffer, stagingBufferMemory) = VulkanUtils.CreateBuffer(graphicsDevice, newSize, VkBufferUsageFlags.TransferSrc, VkMemoryPropertyFlags.HostVisible | VkMemoryPropertyFlags.HostCoherent);

			void* dataPtr = null;
			Vulkan.vkMapMemory(graphicsDevice.LogicalDevice, stagingBufferMemory, 0, newSize, 0, &dataPtr);
			fixed (T* ptr = data) System.Buffer.MemoryCopy(ptr, dataPtr, newSize, newSize);
			Vulkan.vkUnmapMemory(graphicsDevice.LogicalDevice, stagingBufferMemory);

			VulkanUtils.CopyBuffer(graphicsDevice, stagingBuffer, Buffer, newSize);

			Vulkan.vkDestroyBuffer(graphicsDevice.LogicalDevice, stagingBuffer, null);
			Vulkan.vkFreeMemory(graphicsDevice.LogicalDevice, stagingBufferMemory, null);
		}

		public override unsafe void Dispose()
		{
			Vulkan.vkDestroyBuffer(graphicsDevice.LogicalDevice, Buffer, null);
			Vulkan.vkFreeMemory(graphicsDevice.LogicalDevice, Memory, null);
		}
	}
}