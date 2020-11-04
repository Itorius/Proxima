using System;
using Vortice.Vulkan;

namespace Proxima.Graphics
{
	public class VulkanCommandBuffer : VulkanObject
	{
		private VkCommandBuffer buffer;

		public override IntPtr Handle => buffer.Handle;

		public VulkanCommandBuffer(GraphicsDevice graphicsDevice, VkCommandBuffer buffer) : base(graphicsDevice)
		{
			this.buffer = buffer;
		}
		
		public static implicit operator VkCommandBuffer(VulkanCommandBuffer buffer) => buffer.buffer;

		public override void Dispose()
		{
			
		}
	}
}