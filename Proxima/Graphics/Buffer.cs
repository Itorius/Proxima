using Vortice.Vulkan;

namespace Proxima.Graphics
{
	public abstract class Buffer : GraphicsObject
	{
		public VkDeviceMemory Memory { get; protected set; }
		public VkBuffer VkBuffer { get; protected set; }
		public ulong Size { get; protected set;}
		
		protected Buffer(GraphicsDevice graphicsDevice) : base(graphicsDevice)
		{
		}
		
		public override unsafe void Dispose()
		{
			Vulkan.vkDestroyBuffer(graphicsDevice.LogicalDevice, VkBuffer, null);
			Vulkan.vkFreeMemory(graphicsDevice.LogicalDevice, Memory, null);
		}
	}
}