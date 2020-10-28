using Vortice.Vulkan;

namespace Proxima.Graphics
{
	public abstract class Texture : GraphicsObject
	{
		protected VkImage textureImage;
		protected VkDeviceMemory textureMemory;
		public VkImageView View { get; protected set; }
		public VkSampler Sampler { get; protected set; }

		protected Texture(GraphicsDevice graphicsDevice) : base(graphicsDevice)
		{
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