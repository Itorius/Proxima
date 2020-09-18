using GLFW;
using Vortice.Vulkan;
using Vulkan = Vortice.Vulkan.Vulkan;

namespace Proxima.Graphics
{
	public abstract class Buffer : GraphicsObject
	{
		private static uint FindMemoryType(GraphicsDevice graphicsDevice, uint typeFilter, VkMemoryPropertyFlags properties)
		{
			Vulkan.vkGetPhysicalDeviceMemoryProperties(graphicsDevice.PhysicalDevice, out VkPhysicalDeviceMemoryProperties memoryProperties);

			for (uint i = 0; i < memoryProperties.memoryTypeCount; i++)
			{
				if ((typeFilter & (1 << (int)i)) != 0 && (memoryProperties.GetMemoryType(i).propertyFlags & properties) == properties) return i;
			}

			throw new Exception("Failed to find suitable memory type");
		}

		protected static unsafe (VkBuffer, VkDeviceMemory) CreateBuffer(GraphicsDevice graphicsDevice, ulong size, VkBufferUsageFlags usage, VkMemoryPropertyFlags properties)
		{
			VkDevice logicalDevice = graphicsDevice.LogicalDevice;

			VkBufferCreateInfo bufferCreateInfo = new VkBufferCreateInfo
			{
				sType = VkStructureType.BufferCreateInfo,
				size = size,
				usage = usage,
				sharingMode = VkSharingMode.Exclusive
			};

			Vulkan.vkCreateBuffer(logicalDevice, &bufferCreateInfo, null, out VkBuffer buffer).CheckResult();

			Vulkan.vkGetBufferMemoryRequirements(logicalDevice, buffer, out VkMemoryRequirements memoryRequirements);

			VkMemoryAllocateInfo allocateInfo = new VkMemoryAllocateInfo
			{
				sType = VkStructureType.MemoryAllocateInfo,
				allocationSize = memoryRequirements.size,
				memoryTypeIndex = FindMemoryType(graphicsDevice, memoryRequirements.memoryTypeBits, properties)
			};

			VkDeviceMemory bufferMemory;
			Vulkan.vkAllocateMemory(logicalDevice, &allocateInfo, null, &bufferMemory).CheckResult();

			Vulkan.vkBindBufferMemory(logicalDevice, buffer, bufferMemory, 0).CheckResult();

			return (buffer, bufferMemory);
		}

		protected static unsafe void CopyBuffer(GraphicsDevice graphicsDevice, VkBuffer src, VkBuffer dst, ulong size)
		{
			VkCommandBufferAllocateInfo allocateInfo = new VkCommandBufferAllocateInfo
			{
				sType = VkStructureType.CommandBufferAllocateInfo,
				level = VkCommandBufferLevel.Primary,
				commandPool = graphicsDevice.CommandPool,
				commandBufferCount = 1
			};

			Vulkan.vkAllocateCommandBuffers(graphicsDevice.LogicalDevice, &allocateInfo, out VkCommandBuffer commandBuffer).CheckResult();

			VkCommandBufferBeginInfo beginInfo = new VkCommandBufferBeginInfo
			{
				sType = VkStructureType.CommandBufferBeginInfo,
				flags = VkCommandBufferUsageFlags.OneTimeSubmit
			};

			Vulkan.vkBeginCommandBuffer(commandBuffer, &beginInfo).CheckResult();

			VkBufferCopy copyRegion = new VkBufferCopy
			{
				srcOffset = 0,
				dstOffset = 0,
				size = size
			};

			Vulkan.vkCmdCopyBuffer(commandBuffer, src, dst, 1, &copyRegion);

			Vulkan.vkEndCommandBuffer(commandBuffer);

			VkSubmitInfo submitInfo = new VkSubmitInfo
			{
				sType = VkStructureType.SubmitInfo,
				commandBufferCount = 1,
				pCommandBuffers = &commandBuffer
			};

			Vulkan.vkQueueSubmit(graphicsDevice.GraphicsQueue, submitInfo, VkFence.Null);
			Vulkan.vkQueueWaitIdle(graphicsDevice.GraphicsQueue);

			Vulkan.vkFreeCommandBuffers(graphicsDevice.LogicalDevice, graphicsDevice.CommandPool, commandBuffer);
		}
	}
}