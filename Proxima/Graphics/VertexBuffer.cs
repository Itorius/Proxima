using System.Collections.Generic;
using Vortice.Vulkan;

namespace Proxima.Graphics
{
	public abstract class VertexBuffer : Buffer
	{
		protected VertexBuffer(GraphicsDevice graphicsDevice) : base(graphicsDevice)
		{
		}

		public abstract VkVertexInputBindingDescription GetVertexInputBindingDescription();

		public abstract IEnumerable<VkVertexInputAttributeDescription> GetVertexInputAttributeDescriptions();
	}
	
	public class VertexBuffer<T> : VertexBuffer where T : unmanaged
	{
		internal VkVertexInputBindingDescription VertexInputBindingDescription;
		internal VkVertexInputAttributeDescription[] VertexInputAttributeDescriptions;
		
		public unsafe VertexBuffer(GraphicsDevice graphicsDevice, T[] data) : base(graphicsDevice)
		{
			Size = (ulong)(sizeof(T) * data.Length);
			var (stagingBuffer, stagingBufferMemory) = VulkanUtils.CreateBuffer(graphicsDevice, Size, VkBufferUsageFlags.TransferSrc, VkMemoryPropertyFlags.HostVisible | VkMemoryPropertyFlags.HostCoherent);

			void* dataPtr = null;
			Vulkan.vkMapMemory(graphicsDevice.LogicalDevice, stagingBufferMemory, 0, Size, 0, &dataPtr);
			fixed (T* ptr = data) System.Buffer.MemoryCopy(ptr, dataPtr, Size, Size);
			Vulkan.vkUnmapMemory(graphicsDevice.LogicalDevice, stagingBufferMemory);

			var (vkBuffer, vkDeviceMemory) = VulkanUtils.CreateBuffer(graphicsDevice, Size, VkBufferUsageFlags.TransferDst | VkBufferUsageFlags.VertexBuffer, VkMemoryPropertyFlags.DeviceLocal);
			buffer = vkBuffer;
			memory = vkDeviceMemory;

			VulkanUtils.CopyBuffer(graphicsDevice, stagingBuffer, buffer, Size);

			Vulkan.vkDestroyBuffer(graphicsDevice.LogicalDevice, stagingBuffer, null);
			Vulkan.vkFreeMemory(graphicsDevice.LogicalDevice, stagingBufferMemory, null);
		}

		public void SetVertexInputBindingDescription(VkVertexInputBindingDescription description)
		{
			VertexInputBindingDescription = description;
		}

		public void SetVertexInputAttributeDescriptions(VkVertexInputAttributeDescription[] descriptions)
		{
			VertexInputAttributeDescriptions = descriptions;
		}

		public unsafe VertexBuffer(GraphicsDevice graphicsDevice, uint elementCount) : base(graphicsDevice)
		{
			Size = (ulong)(elementCount * sizeof(T));

			var (vkBuffer, vkDeviceMemory) = VulkanUtils.CreateBuffer(graphicsDevice, Size, VkBufferUsageFlags.TransferDst | VkBufferUsageFlags.VertexBuffer, VkMemoryPropertyFlags.DeviceLocal);
			buffer = vkBuffer;
			memory = vkDeviceMemory;
		}

		public unsafe void SetData(T[] data)
		{
			ulong newSize = (ulong)(sizeof(T) * data.Length);

			if (newSize > Size)
			{
				Size = newSize;

				Vulkan.vkDestroyBuffer(graphicsDevice.LogicalDevice, buffer, null);
				Vulkan.vkFreeMemory(graphicsDevice.LogicalDevice, memory, null);

				var (vkBuffer, vkDeviceMemory) = VulkanUtils.CreateBuffer(graphicsDevice, newSize, VkBufferUsageFlags.TransferDst | VkBufferUsageFlags.VertexBuffer, VkMemoryPropertyFlags.DeviceLocal);
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

		public void Bind(VkCommandBuffer buffer)
		{
			Vulkan.vkCmdBindVertexBuffers(buffer, 0, this.buffer);
		}

		public override VkVertexInputBindingDescription GetVertexInputBindingDescription() => VertexInputBindingDescription;

		public override IEnumerable<VkVertexInputAttributeDescription> GetVertexInputAttributeDescriptions() => VertexInputAttributeDescriptions;
	}
}