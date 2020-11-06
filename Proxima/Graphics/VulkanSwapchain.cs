using Vortice.Vulkan;

namespace Proxima.Graphics
{
	public class VulkanSwapchain : VulkanObject
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
			var (capabilities, formats, presentModes) = VulkanUtils.QuerySwapchainSupport(graphicsDevice.PhysicalDevice, graphicsDevice.Surface);

			VkSurfaceFormatKHR surfaceFormat = VulkanUtils.SelectSwapSurfaceFormat(formats);
			VkPresentModeKHR presentMode = VulkanUtils.SelectSwapPresentMode(presentModes, graphicsDevice.vsync);
			VkExtent2D extent = VulkanUtils.SelectSwapExtent(capabilities, graphicsDevice.window);

			uint imageCount = capabilities.minImageCount + 1;
			if (capabilities.maxImageCount > 0 && imageCount > capabilities.maxImageCount) imageCount = capabilities.maxImageCount;

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
				preTransform = capabilities.currentTransform,
				compositeAlpha = VkCompositeAlphaFlagsKHR.Opaque,
				presentMode = presentMode,
				clipped = true,
				oldSwapchain = VkSwapchainKHR.Null
			};

			QueueFamilyIndices indices = VulkanUtils.FindQueueFamilies(graphicsDevice.PhysicalDevice, graphicsDevice.Surface);
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