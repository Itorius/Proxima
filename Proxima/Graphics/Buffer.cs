using Vortice.Vulkan;

namespace Proxima.Graphics
{
	public abstract class Buffer : GraphicsObject
	{
		public ulong Size { get; protected set; }

		protected VkBuffer buffer;
		protected VkDeviceMemory memory;

		protected Buffer(GraphicsDevice graphicsDevice) : base(graphicsDevice)
		{
		}

		public static explicit operator VkBuffer(Buffer buffer) => buffer.buffer;

		public override unsafe void Dispose()
		{
			if (buffer != VkBuffer.Null) Vulkan.vkDestroyBuffer(graphicsDevice.LogicalDevice, buffer, null);
			if (memory != VkDeviceMemory.Null) Vulkan.vkFreeMemory(graphicsDevice.LogicalDevice, memory, null);
		}
	}
}