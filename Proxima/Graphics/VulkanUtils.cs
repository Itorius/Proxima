using GLFW;
using Vortice.Mathematics;
using Vortice.Vulkan;
using Vulkan = Vortice.Vulkan.Vulkan;

namespace Proxima.Graphics
{
	public static class VulkanUtils
	{
		public static uint FindMemoryType(GraphicsDevice graphicsDevice, uint typeFilter, VkMemoryPropertyFlags properties)
		{
			Vulkan.vkGetPhysicalDeviceMemoryProperties(graphicsDevice.PhysicalDevice, out VkPhysicalDeviceMemoryProperties memoryProperties);

			for (uint i = 0; i < memoryProperties.memoryTypeCount; i++)
			{
				if ((typeFilter & (1 << (int)i)) != 0 && (memoryProperties.GetMemoryType(i).propertyFlags & properties) == properties) return i;
			}

			throw new Exception("Failed to find suitable memory type");
		}

		public static unsafe (VkBuffer, VkDeviceMemory) CreateBuffer(GraphicsDevice graphicsDevice, ulong size, VkBufferUsageFlags usage, VkMemoryPropertyFlags properties)
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

		private static unsafe VkCommandBuffer BeginSingleTimeCommands(GraphicsDevice graphicsDevice)
		{
			VkCommandBufferAllocateInfo allocateInfo = new VkCommandBufferAllocateInfo
			{
				sType = VkStructureType.CommandBufferAllocateInfo,
				level = VkCommandBufferLevel.Primary,
				commandPool = graphicsDevice.CommandPool,
				commandBufferCount = 1
			};

			Vulkan.vkAllocateCommandBuffers(graphicsDevice.LogicalDevice, &allocateInfo, out VkCommandBuffer commandBuffer);

			VkCommandBufferBeginInfo beginInfo = new VkCommandBufferBeginInfo
			{
				sType = VkStructureType.CommandBufferBeginInfo,
				flags = VkCommandBufferUsageFlags.OneTimeSubmit
			};

			Vulkan.vkBeginCommandBuffer(commandBuffer, &beginInfo);

			return commandBuffer;
		}

		private static unsafe void EndSingleTimeCommands(GraphicsDevice graphicsDevice, VkCommandBuffer commandBuffer)
		{
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

		public static unsafe void TransitionImageLayout(GraphicsDevice graphicsDevice, VkImage image, VkFormat format, VkImageLayout oldLayout, VkImageLayout newLayout)
		{
			VkCommandBuffer commandBuffer = BeginSingleTimeCommands(graphicsDevice);

			VkImageMemoryBarrier barrier = new VkImageMemoryBarrier
			{
				sType = VkStructureType.ImageMemoryBarrier,
				oldLayout = oldLayout,
				newLayout = newLayout,
				srcQueueFamilyIndex = Vulkan.QueueFamilyIgnored,
				dstQueueFamilyIndex = Vulkan.QueueFamilyIgnored,
				image = image,
				subresourceRange = new VkImageSubresourceRange
				{
					aspectMask = VkImageAspectFlags.Color,
					baseMipLevel = 0,
					levelCount = 1,
					baseArrayLayer = 0,
					layerCount = 1
				}
			};

			VkPipelineStageFlags sourceStage = VkPipelineStageFlags.None;
			VkPipelineStageFlags destinationStage = VkPipelineStageFlags.None;

			if (oldLayout == VkImageLayout.Undefined && newLayout == VkImageLayout.TransferDstOptimal)
			{
				barrier.srcAccessMask = 0;
				barrier.dstAccessMask = VkAccessFlags.TransferWrite;

				sourceStage = VkPipelineStageFlags.TopOfPipe;
				destinationStage = VkPipelineStageFlags.Transfer;
			}
			else if (oldLayout == VkImageLayout.TransferDstOptimal && newLayout == VkImageLayout.ShaderReadOnlyOptimal)
			{
				barrier.srcAccessMask = VkAccessFlags.TransferWrite;
				barrier.dstAccessMask = VkAccessFlags.ShaderRead;

				sourceStage = VkPipelineStageFlags.Transfer;
				destinationStage = VkPipelineStageFlags.FragmentShader;
			}
			else throw new Exception("Unsupported layout transition");

			Vulkan.vkCmdPipelineBarrier(commandBuffer, sourceStage, destinationStage, VkDependencyFlags.None, 0, null, 0, null, 1, &barrier);

			EndSingleTimeCommands(graphicsDevice, commandBuffer);
		}

		public static unsafe void CopyBufferToImage(GraphicsDevice graphicsDevice, VkBuffer buffer, VkImage image, int width, int height)
		{
			VkCommandBuffer commandBuffer = BeginSingleTimeCommands(graphicsDevice);

			VkBufferImageCopy region = new VkBufferImageCopy
			{
				bufferOffset = 0,
				bufferRowLength = 0,
				bufferImageHeight = 0,
				imageSubresource = new VkImageSubresourceLayers
				{
					aspectMask = VkImageAspectFlags.Color,
					mipLevel = 0,
					baseArrayLayer = 0,
					layerCount = 1
				},
				imageExtent = new Size3(width, height, 1)
			};

			Vulkan.vkCmdCopyBufferToImage(commandBuffer, buffer, image, VkImageLayout.TransferDstOptimal, 1, &region);

			EndSingleTimeCommands(graphicsDevice, commandBuffer);
		}

		public static unsafe void CopyBuffer(GraphicsDevice graphicsDevice, VkBuffer src, VkBuffer dst, ulong size)
		{
			VkCommandBuffer commandBuffer = BeginSingleTimeCommands(graphicsDevice);

			VkBufferCopy copyRegion = new VkBufferCopy
			{
				srcOffset = 0,
				dstOffset = 0,
				size = size
			};

			Vulkan.vkCmdCopyBuffer(commandBuffer, src, dst, 1, &copyRegion);

			EndSingleTimeCommands(graphicsDevice, commandBuffer);
		}
	}
}