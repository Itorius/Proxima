using Vortice.Mathematics;
using Vortice.Vulkan;

namespace Proxima.Graphics
{
	public class DepthBuffer : GraphicsObject
	{
		private VkImage image;
		private VkDeviceMemory memory;

		public VkFormat Format { get; }
		public VkImageView View { get; }

		public unsafe DepthBuffer(GraphicsDevice graphicsDevice, Size size) : base(graphicsDevice)
		{
			Format = VulkanUtils.FindDepthFormat(graphicsDevice);

			VkImageCreateInfo imageCreateInfo = new VkImageCreateInfo
			{
				sType = VkStructureType.ImageCreateInfo,
				imageType = VkImageType.Image2D,
				extent = new Size3(size.Width, size.Height, 1),
				mipLevels = 1,
				arrayLayers = 1,
				format = Format,
				tiling = VkImageTiling.Optimal,
				initialLayout = VkImageLayout.Undefined,
				usage = VkImageUsageFlags.DepthStencilAttachment,
				sharingMode = VkSharingMode.Exclusive,
				samples = VkSampleCountFlags.Count1
			};

			Vulkan.vkCreateImage(graphicsDevice.LogicalDevice, &imageCreateInfo, null, out image).CheckResult();

			Vulkan.vkGetImageMemoryRequirements(graphicsDevice.LogicalDevice, image, out VkMemoryRequirements memoryRequirements);

			VkMemoryAllocateInfo allocateInfo = new VkMemoryAllocateInfo
			{
				sType = VkStructureType.MemoryAllocateInfo,
				allocationSize = memoryRequirements.size,
				memoryTypeIndex = VulkanUtils.FindMemoryType(graphicsDevice, memoryRequirements.memoryTypeBits, VkMemoryPropertyFlags.DeviceLocal)
			};

			fixed (VkDeviceMemory* ptr = &memory) Vulkan.vkAllocateMemory(graphicsDevice.LogicalDevice, &allocateInfo, null, ptr).CheckResult();

			Vulkan.vkBindImageMemory(graphicsDevice.LogicalDevice, image, memory, 0);

			VkImageViewCreateInfo imageViewCreateInfo = new VkImageViewCreateInfo
			{
				sType = VkStructureType.ImageViewCreateInfo,
				image = image,
				viewType = VkImageViewType.Image2D,
				format = Format,
				subresourceRange = new VkImageSubresourceRange
				{
					aspectMask = VkImageAspectFlags.Depth,
					baseMipLevel = 0,
					levelCount = 1,
					baseArrayLayer = 0,
					layerCount = 1
				}
			};

			Vulkan.vkCreateImageView(graphicsDevice.LogicalDevice, &imageViewCreateInfo, null, out VkImageView view).CheckResult();
			View = view;

			VulkanUtils.TransitionImageLayout(graphicsDevice, image, Format, VkImageLayout.Undefined, VkImageLayout.DepthAttachmentOptimal);
		}

		public override unsafe void Dispose()
		{
			Vulkan.vkFreeMemory(graphicsDevice.LogicalDevice, memory, null);
			Vulkan.vkDestroyImageView(graphicsDevice.LogicalDevice, View, null);
			Vulkan.vkDestroyImage(graphicsDevice.LogicalDevice, image, null);
		}
	}
}