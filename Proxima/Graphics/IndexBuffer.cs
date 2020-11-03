using System;
using Vortice.Vulkan;

namespace Proxima.Graphics
{
	public class IndexBuffer<T> : Buffer where T : unmanaged
	{
		public VkIndexType IndexType { get; private set; }
		public uint Length { get; private set; }

		public unsafe IndexBuffer(GraphicsDevice graphicsDevice, T[] data) : base(graphicsDevice)
		{
			IndexType = GetIndexType();

			Length = (uint)data.Length;

			Size = (ulong)(sizeof(T) * data.Length);
			var (stagingBuffer, stagingBufferMemory) = VulkanUtils.CreateBuffer(graphicsDevice, Size, VkBufferUsageFlags.TransferSrc, VkMemoryPropertyFlags.HostVisible | VkMemoryPropertyFlags.HostCoherent);

			void* dataPtr = null;
			Vulkan.vkMapMemory(graphicsDevice.LogicalDevice, stagingBufferMemory, 0, Size, 0, &dataPtr);
			fixed (T* ptr = data) System.Buffer.MemoryCopy(ptr, dataPtr, Size, Size);
			Vulkan.vkUnmapMemory(graphicsDevice.LogicalDevice, stagingBufferMemory);

			var (vkBuffer, vkDeviceMemory) = VulkanUtils.CreateBuffer(graphicsDevice, Size, VkBufferUsageFlags.TransferDst | VkBufferUsageFlags.IndexBuffer, VkMemoryPropertyFlags.DeviceLocal);
			buffer = vkBuffer;
			memory = vkDeviceMemory;

			VulkanUtils.CopyBuffer(graphicsDevice, stagingBuffer, buffer, Size);

			Vulkan.vkDestroyBuffer(graphicsDevice.LogicalDevice, stagingBuffer, null);
			Vulkan.vkFreeMemory(graphicsDevice.LogicalDevice, stagingBufferMemory, null);
		}

		public unsafe IndexBuffer(GraphicsDevice graphicsDevice, uint elementCount) : base(graphicsDevice)
		{
			Size = (ulong)(elementCount * sizeof(T));
			Length = elementCount;

			IndexType = GetIndexType();

			var (vkBuffer, vkDeviceMemory) = VulkanUtils.CreateBuffer(graphicsDevice, Size, VkBufferUsageFlags.TransferDst | VkBufferUsageFlags.IndexBuffer, VkMemoryPropertyFlags.DeviceLocal);
			buffer = vkBuffer;
			memory = vkDeviceMemory;
		}

		private static VkIndexType GetIndexType()
		{
			if (typeof(T) == typeof(byte)) return VkIndexType.Uint8EXT;
			if (typeof(T) == typeof(ushort)) return VkIndexType.Uint16;
			if (typeof(T) == typeof(uint)) return VkIndexType.Uint32;
			throw new Exception("Unsupported index buffer type " + typeof(T).Name);
		}

		public unsafe void SetData(T[] data)
		{
			Length = (uint)data.Length;
			ulong newSize = (ulong)(sizeof(T) * data.Length);

			if (newSize > Size)
			{
				Size = newSize;

				Vulkan.vkDestroyBuffer(graphicsDevice.LogicalDevice, buffer, null);
				Vulkan.vkFreeMemory(graphicsDevice.LogicalDevice, memory, null);

				var (vkBuffer, vkDeviceMemory) = VulkanUtils.CreateBuffer(graphicsDevice, newSize, VkBufferUsageFlags.TransferDst | VkBufferUsageFlags.IndexBuffer, VkMemoryPropertyFlags.DeviceLocal);
				buffer = vkBuffer;
				memory = vkDeviceMemory;
			}

			var (stagingBuffer, stagingBufferMemory) = VulkanUtils.CreateBuffer(graphicsDevice, newSize, VkBufferUsageFlags.TransferSrc, VkMemoryPropertyFlags.HostVisible | VkMemoryPropertyFlags.HostCoherent);

			void* dataPtr = null;
			Vulkan.vkMapMemory(graphicsDevice.LogicalDevice, stagingBufferMemory, 0, newSize, 0, &dataPtr);
			fixed (T* ptr = data) System.Buffer.MemoryCopy(ptr, dataPtr, newSize, newSize);
			Vulkan.vkUnmapMemory(graphicsDevice.LogicalDevice, stagingBufferMemory);

			VulkanUtils.CopyBuffer(graphicsDevice, stagingBuffer, buffer, newSize);

			Vulkan.vkDestroyBuffer(graphicsDevice.LogicalDevice, stagingBuffer, null);
			Vulkan.vkFreeMemory(graphicsDevice.LogicalDevice, stagingBufferMemory, null);
		}

		public override unsafe void Dispose()
		{
			Vulkan.vkDestroyBuffer(graphicsDevice.LogicalDevice, buffer, null);
			Vulkan.vkFreeMemory(graphicsDevice.LogicalDevice, memory, null);
		}

		public void Bind(VkCommandBuffer cmdBuffer)
		{
			Vulkan.vkCmdBindIndexBuffer(cmdBuffer, buffer, 0, IndexType);
		}

		public unsafe void Resize(uint elementCount)
		{
			ulong newSize = (ulong)(sizeof(T) * elementCount);

			if (newSize > Size)
			{
				Size = newSize;

				Vulkan.vkDestroyBuffer(graphicsDevice.LogicalDevice, buffer, null);
				Vulkan.vkFreeMemory(graphicsDevice.LogicalDevice, memory, null);

				var (vkBuffer, vkDeviceMemory) = VulkanUtils.CreateBuffer(graphicsDevice, newSize, VkBufferUsageFlags.TransferDst | VkBufferUsageFlags.IndexBuffer, VkMemoryPropertyFlags.DeviceLocal);
				buffer = vkBuffer;
				memory = vkDeviceMemory;
			}
		}

		public unsafe T* Map()
		{
			var (stagingBuffer, stagingBufferMemory) = VulkanUtils.CreateBuffer(graphicsDevice, Size, VkBufferUsageFlags.TransferSrc, VkMemoryPropertyFlags.HostVisible | VkMemoryPropertyFlags.HostCoherent);
			sb = stagingBuffer;
			sbm = stagingBufferMemory;

			T* ptr;
			Vulkan.vkMapMemory(graphicsDevice.LogicalDevice, sbm, 0, Size, 0, &ptr);
			return ptr;
		}

		private VkBuffer sb;
		private VkDeviceMemory sbm;

		public unsafe void Unmap()
		{
			if (sb == VkBuffer.Null || sbm == VkDeviceMemory.Null) throw new Exception("Attemped to unmap a buffer that has not been mapped");

			Vulkan.vkUnmapMemory(graphicsDevice.LogicalDevice, sbm);

			VulkanUtils.CopyBuffer(graphicsDevice, sb, buffer, Size);

			Vulkan.vkDestroyBuffer(graphicsDevice.LogicalDevice, sb, null);
			Vulkan.vkFreeMemory(graphicsDevice.LogicalDevice, sbm, null);

			sb = VkBuffer.Null;
			sbm = VkDeviceMemory.Null;
		}
	}
}