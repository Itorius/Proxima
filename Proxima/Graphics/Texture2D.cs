using System.IO;
using StbImageSharp;
using Vortice.Mathematics;
using Vortice.Vulkan;

namespace Proxima.Graphics
{
	public class Texture2D : GraphicsObject
	{
		private VkImage textureImage;
		private VkDeviceMemory textureMemory;
		public VkImageView View { get; }
		public VkSampler Sampler { get; }

		public unsafe Texture2D(GraphicsDevice graphicsDevice, string path) : base(graphicsDevice)
		{
			using var stream = File.OpenRead(path);
			ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
			VkFormat imageFormat = VkFormat.R8G8B8A8SRgb;

			ulong size = (ulong)(image.Width * image.Height * 4);

			var (stagingBuffer, stagingMemory) = VulkanUtils.CreateBuffer(graphicsDevice, size, VkBufferUsageFlags.TransferSrc, VkMemoryPropertyFlags.HostVisible | VkMemoryPropertyFlags.HostCoherent);

			void* dataPtr = null;
			Vulkan.vkMapMemory(graphicsDevice.LogicalDevice, stagingMemory, 0, size, VkMemoryMapFlags.None, &dataPtr);
			fixed (byte* ptr = image.Data) System.Buffer.MemoryCopy(ptr, dataPtr, size, size);
			Vulkan.vkUnmapMemory(graphicsDevice.LogicalDevice, stagingMemory);

			VkImageCreateInfo imageCreateInfo = new VkImageCreateInfo
			{
				sType = VkStructureType.ImageCreateInfo,
				imageType = VkImageType.Image2D,
				extent = new Size3(image.Width, image.Height, 1),
				mipLevels = 1,
				arrayLayers = 1,
				format = imageFormat,
				tiling = VkImageTiling.Optimal,
				initialLayout = VkImageLayout.Undefined,
				usage = VkImageUsageFlags.TransferDst | VkImageUsageFlags.Sampled,
				sharingMode = VkSharingMode.Exclusive,
				samples = VkSampleCountFlags.Count1
			};

			Vulkan.vkCreateImage(graphicsDevice.LogicalDevice, &imageCreateInfo, null, out textureImage).CheckResult();

			Vulkan.vkGetImageMemoryRequirements(graphicsDevice.LogicalDevice, textureImage, out VkMemoryRequirements memoryRequirements);

			VkMemoryAllocateInfo allocateInfo = new VkMemoryAllocateInfo
			{
				sType = VkStructureType.MemoryAllocateInfo,
				allocationSize = memoryRequirements.size,
				memoryTypeIndex = VulkanUtils.FindMemoryType(graphicsDevice, memoryRequirements.memoryTypeBits, VkMemoryPropertyFlags.DeviceLocal)
			};

			fixed (VkDeviceMemory* ptr = &textureMemory) Vulkan.vkAllocateMemory(graphicsDevice.LogicalDevice, &allocateInfo, null, ptr).CheckResult();

			Vulkan.vkBindImageMemory(graphicsDevice.LogicalDevice, textureImage, textureMemory, 0);

			VulkanUtils.TransitionImageLayout(graphicsDevice, textureImage, imageFormat, VkImageLayout.Undefined, VkImageLayout.TransferDstOptimal);

			VulkanUtils.CopyBufferToImage(graphicsDevice, stagingBuffer, textureImage, image.Width, image.Height);

			VulkanUtils.TransitionImageLayout(graphicsDevice, textureImage, imageFormat, VkImageLayout.TransferDstOptimal, VkImageLayout.ShaderReadOnlyOptimal);

			Vulkan.vkFreeMemory(graphicsDevice.LogicalDevice, stagingMemory, null);
			Vulkan.vkDestroyBuffer(graphicsDevice.LogicalDevice, stagingBuffer, null);

			VkImageViewCreateInfo imageViewCreateInfo = new VkImageViewCreateInfo
			{
				sType = VkStructureType.ImageViewCreateInfo,
				image = textureImage,
				viewType = VkImageViewType.Image2D,
				format = imageFormat,
				subresourceRange = new VkImageSubresourceRange
				{
					aspectMask = VkImageAspectFlags.Color,
					baseMipLevel = 0,
					levelCount = 1,
					baseArrayLayer = 0,
					layerCount = 1
				}
			};

			Vulkan.vkCreateImageView(graphicsDevice.LogicalDevice, &imageViewCreateInfo, null, out var imageView).CheckResult();
			View = imageView;

			VkSamplerCreateInfo samplerCreateInfo = new VkSamplerCreateInfo
			{
				sType = VkStructureType.SamplerCreateInfo,
				magFilter = VkFilter.Linear,
				minFilter = VkFilter.Linear,
				addressModeU = VkSamplerAddressMode.Repeat,
				addressModeV = VkSamplerAddressMode.Repeat,
				addressModeW = VkSamplerAddressMode.Repeat,
				anisotropyEnable = true,
				maxAnisotropy = 16,
				borderColor = VkBorderColor.IntOpaqueBlack,
				unnormalizedCoordinates = false,
				compareEnable = false,
				compareOp = VkCompareOp.Always,
				mipmapMode = VkSamplerMipmapMode.Linear,
				mipLodBias = 0f,
				minLod = 0f,
				maxLod = 0f
			};

			Vulkan.vkCreateSampler(graphicsDevice.LogicalDevice, &samplerCreateInfo, null, out var sampler).CheckResult();
			Sampler = sampler;
		}

		public override unsafe void Dispose()
		{
			Vulkan.vkDestroyImageView(graphicsDevice.LogicalDevice, View, null);
			Vulkan.vkDestroySampler(graphicsDevice.LogicalDevice, Sampler, null);

			Vulkan.vkFreeMemory(graphicsDevice.LogicalDevice, textureMemory, null);
			Vulkan.vkDestroyImage(graphicsDevice.LogicalDevice, textureImage, null);
		}
	}
}