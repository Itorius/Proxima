using System;
using System.Collections.Generic;
using System.Linq;
using GLFW;
using Vortice.Vulkan;
using Exception = GLFW.Exception;
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
					baseMipLevel = 0,
					levelCount = 1,
					baseArrayLayer = 0,
					layerCount = 1
				}
			};
			if (newLayout == VkImageLayout.DepthAttachmentOptimal)
			{
				barrier.subresourceRange.aspectMask = VkImageAspectFlags.Depth;
				if (HasStencilComponent(format)) barrier.subresourceRange.aspectMask |= VkImageAspectFlags.Stencil;
			}
			else barrier.subresourceRange.aspectMask = VkImageAspectFlags.Color;

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
			else if (oldLayout == VkImageLayout.Undefined && newLayout == VkImageLayout.DepthAttachmentOptimal)
			{
				barrier.srcAccessMask = VkAccessFlags.None;
				barrier.dstAccessMask = VkAccessFlags.DepthStencilAttachmentRead | VkAccessFlags.DepthStencilAttachmentWrite;

				sourceStage = VkPipelineStageFlags.TopOfPipe;
				destinationStage = VkPipelineStageFlags.EarlyFragmentTests;
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
				imageExtent = new VkExtent3D(width, height, 1)
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

		public static VkFormat FindSupportedFormat(GraphicsDevice graphicsDevice, List<VkFormat> formats, VkImageTiling tiling, VkFormatFeatureFlags featureFlags)
		{
			foreach (VkFormat format in formats)
			{
				Vulkan.vkGetPhysicalDeviceFormatProperties(graphicsDevice.PhysicalDevice, format, out VkFormatProperties formatProperties);

				if (tiling == VkImageTiling.Linear && (formatProperties.linearTilingFeatures & featureFlags) == featureFlags) return format;
				if (tiling == VkImageTiling.Optimal && (formatProperties.optimalTilingFeatures & featureFlags) == featureFlags) return format;
			}

			throw new Exception("Failed to find suitable format");
		}

		public static VkFormat FindDepthFormat(GraphicsDevice graphicsDevice)
		{
			return FindSupportedFormat(graphicsDevice, new List<VkFormat> { VkFormat.D32SFloat, VkFormat.D32SFloatS8UInt, VkFormat.D24UNormS8UInt }, VkImageTiling.Optimal, VkFormatFeatureFlags.DepthStencilAttachment);
		}

		public static bool HasStencilComponent(VkFormat format)
		{
			return format == VkFormat.D32SFloatS8UInt || format == VkFormat.D24UNormS8UInt;
		}

		public static VkSurfaceFormatKHR SelectSwapSurfaceFormat(IReadOnlyList<VkSurfaceFormatKHR> formats)
		{
			foreach (VkSurfaceFormatKHR format in formats)
			{
				if (format.format == VkFormat.B8G8R8A8SRgb && format.colorSpace == VkColorSpaceKHR.SrgbNonLinear) return format;
			}

			return formats[0];
		}

		public static VkPresentModeKHR SelectSwapPresentMode(IReadOnlyList<VkPresentModeKHR> presentModes)
		{
			// return VkPresentModeKHR.Immediate;

			foreach (VkPresentModeKHR presentMode in presentModes)
			{
				if (presentMode == VkPresentModeKHR.Mailbox) return presentMode;
			}

			return VkPresentModeKHR.Fifo;
		}

		public static VkExtent2D SelectSwapExtent(VkSurfaceCapabilitiesKHR capabilities, NativeWindow window)
		{
			if (capabilities.currentExtent.width != int.MaxValue) return capabilities.currentExtent;

			VkExtent2D actualExtent = new VkExtent2D(
				(int)Math.Clamp(window.Size.Width, capabilities.minImageExtent.width, capabilities.maxImageExtent.width),
				(int)Math.Clamp(window.Size.Height, capabilities.minImageExtent.height, capabilities.maxImageExtent.height));

			return actualExtent;
		}

		internal static (VkSurfaceCapabilitiesKHR capabilities, IReadOnlyList<VkSurfaceFormatKHR> formats, IReadOnlyList<VkPresentModeKHR> presentModes) QuerySwapchainSupport(VkPhysicalDevice device, VkSurfaceKHR surface)
		{
			Vulkan.vkGetPhysicalDeviceSurfaceCapabilitiesKHR(device, surface, out var capabilities);

			return (capabilities, Vulkan.vkGetPhysicalDeviceSurfaceFormatsKHR(device, surface).ToArray(), Vulkan.vkGetPhysicalDeviceSurfacePresentModesKHR(device, surface).ToArray());
		}

		internal static bool CheckDeviceExtensionSupport(VkPhysicalDevice device, VkStringArray requestedExtensions)
		{
			var extensions = Vulkan.vkEnumerateDeviceExtensionProperties(device).ToArray().Select(property => property.GetExtensionName()).ToList();

			bool flag = true;
			for (int i = 0; i < requestedExtensions.Length; i++) flag &= extensions.Contains(requestedExtensions[i]);
			return flag;
		}

		internal static VkStringArray GetRequiredExtensions(bool includeValidation = false)
		{
			List<string> extensions = GLFW.Vulkan.GetRequiredInstanceExtensions().ToList();

			if (includeValidation) extensions.Add(Vulkan.EXTDebugUtilsExtensionName);

			return new VkStringArray(extensions);
		}

		internal static QueueFamilyIndices FindQueueFamilies(VkPhysicalDevice device, VkSurfaceKHR surface)
		{
			QueueFamilyIndices indices = new QueueFamilyIndices();

			var properties = Vulkan.vkGetPhysicalDeviceQueueFamilyProperties(device);

			uint i = 0;
			foreach (VkQueueFamilyProperties property in properties)
			{
				if ((property.queueFlags & VkQueueFlags.Graphics) != 0)
				{
					indices.graphics = i;

					Vulkan.vkGetPhysicalDeviceSurfaceSupportKHR(device, i, surface, out VkBool32 presentSupport);
					if (presentSupport) indices.present = i;
				}

				if (indices.IsComplete) break;

				i++;
			}


			return indices;
		}
	}

	internal struct QueueFamilyIndices
	{
		public uint? graphics;
		public uint? present;

		public bool IsComplete => graphics.HasValue && present.HasValue;
	}
}