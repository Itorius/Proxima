using System;
using GLFW;
using Vortice.Vulkan;
using Vulkan = Vortice.Vulkan.Vulkan;

namespace Proxima.Graphics
{
	public class VulkanSwapchain : GraphicsObject
	{
		public VkSwapchainKHR Swapchain { get; private set; }
		public VkFormat Format { get; private set; }
		public VkExtent2D Extent { get; private set; }
		public VkImage[] Images { get; private set; }
		public VkImageView[] ImageViews { get; private set; }
		public uint Length { get; private set; }

		public VulkanSwapchain(GraphicsDevice graphicsDevice) : base(graphicsDevice)
		{
			Create();
		}

		private unsafe void Create()
		{
			GraphicsDevice.SwapChainSupportDetails details = GraphicsDevice.QuerySwapchainSupport(graphicsDevice.PhysicalDevice, graphicsDevice.Surface);

			VkSurfaceFormatKHR surfaceFormat = SelectSwapSurfaceFormat(details.formats);
			VkPresentModeKHR presentMode = SelectSwapPresentMode(details.presentModes);
			VkExtent2D extent = SelectSwapExtent(details.capabilities, graphicsDevice.window);

			uint imageCount = details.capabilities.minImageCount + 1;
			if (details.capabilities.maxImageCount > 0 && imageCount > details.capabilities.maxImageCount) imageCount = details.capabilities.maxImageCount;

			VkSwapchainCreateInfoKHR createInfo = new VkSwapchainCreateInfoKHR
			{
				sType = VkStructureType.SwapchainCreateInfoKHR,
				surface = graphicsDevice.Surface,
				minImageCount = imageCount,
				imageFormat = surfaceFormat.format,
				imageColorSpace = surfaceFormat.colorSpace,
				imageExtent = extent,
				imageArrayLayers = 1,
				imageUsage = VkImageUsageFlags.ColorAttachment,
				preTransform = details.capabilities.currentTransform,
				compositeAlpha = VkCompositeAlphaFlagsKHR.Opaque,
				presentMode = presentMode,
				clipped = true,
				oldSwapchain = VkSwapchainKHR.Null
			};

			GraphicsDevice.QueueFamilyIndices indices = GraphicsDevice.FindQueueFamilies(graphicsDevice.PhysicalDevice, graphicsDevice.Surface);
			uint[] queueFamilyIndices = { indices.graphics.Value, indices.present.Value };

			if (indices.graphics.Value != indices.present.Value)
			{
				createInfo.imageSharingMode = VkSharingMode.Concurrent;
				createInfo.queueFamilyIndexCount = 2;
				fixed (uint* ptr = queueFamilyIndices) createInfo.pQueueFamilyIndices = ptr;
			}
			else
			{
				createInfo.imageSharingMode = VkSharingMode.Exclusive;
			}

			Vulkan.vkCreateSwapchainKHR(graphicsDevice.LogicalDevice, &createInfo, null, out var swapchain).CheckResult();
			Swapchain = swapchain;

			Images = Vulkan.vkGetSwapchainImagesKHR(graphicsDevice.LogicalDevice, Swapchain).ToArray();
			Length = (uint)Images.Length;

			Format = surfaceFormat.format;
			Extent = extent;

			ImageViews = new VkImageView[Length];

			for (int i = 0; i < Length; i++)
			{
				VkImageViewCreateInfo imageViewCreateInfo = new VkImageViewCreateInfo
				{
					sType = VkStructureType.ImageViewCreateInfo,
					image = Images[i],
					viewType = VkImageViewType.Image2D,
					format = Format,
					components = VkComponentMapping.Rgba,
					subresourceRange = new VkImageSubresourceRange
					{
						aspectMask = VkImageAspectFlags.Color,
						baseMipLevel = 0,
						levelCount = 1,
						baseArrayLayer = 0,
						layerCount = 1
					}
				};

				Vulkan.vkCreateImageView(graphicsDevice.LogicalDevice, &imageViewCreateInfo, null, out ImageViews[i]).CheckResult();
			}

			#region Static

			static VkSurfaceFormatKHR SelectSwapSurfaceFormat(ReadOnlySpan<VkSurfaceFormatKHR> formats)
			{
				foreach (VkSurfaceFormatKHR format in formats)
				{
					if (format.format == VkFormat.B8G8R8A8SRgb && format.colorSpace == VkColorSpaceKHR.SrgbNonLinear) return format;
				}

				return formats[0];
			}

			static VkPresentModeKHR SelectSwapPresentMode(ReadOnlySpan<VkPresentModeKHR> presentModes)
			{
				foreach (VkPresentModeKHR presentMode in presentModes)
				{
					if (presentMode == VkPresentModeKHR.Mailbox) return presentMode;
				}

				return VkPresentModeKHR.Fifo;
			}

			static VkExtent2D SelectSwapExtent(VkSurfaceCapabilitiesKHR capabilities, NativeWindow window)
			{
				if (capabilities.currentExtent.width != int.MaxValue) return capabilities.currentExtent;

				VkExtent2D actualExtent = new VkExtent2D(window.Size.Width, window.Size.Height);

				actualExtent.width = Math.Clamp(actualExtent.width, capabilities.minImageExtent.width, capabilities.maxImageExtent.width);
				actualExtent.height = Math.Clamp(actualExtent.height, capabilities.minImageExtent.height, capabilities.maxImageExtent.height);

				return actualExtent;
			}

			#endregion
		}

		public override unsafe void Dispose()
		{
			foreach (VkImageView imageView in ImageViews) Vulkan.vkDestroyImageView(graphicsDevice.LogicalDevice, imageView, null);

			Vulkan.vkDestroySwapchainKHR(graphicsDevice.LogicalDevice, Swapchain, null);
		}

		public void Invalidate()
		{
			Dispose();

			Create();
		}
	}
}