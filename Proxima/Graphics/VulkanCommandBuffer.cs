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

		public unsafe void Begin(VkCommandBufferUsageFlags flags, VkCommandBufferInheritanceInfo inheritance)
		{
			VkCommandBufferBeginInfo info = new VkCommandBufferBeginInfo
			{
				sType = VkStructureType.CommandBufferBeginInfo,
				flags = flags,
				pInheritanceInfo = &inheritance
			};

			Vulkan.vkBeginCommandBuffer(this, &info);
		}

		public unsafe void Begin(VkCommandBufferUsageFlags flags)
		{
			VkCommandBufferBeginInfo info = new VkCommandBufferBeginInfo
			{
				sType = VkStructureType.CommandBufferBeginInfo,
				flags = flags,
			};

			Vulkan.vkBeginCommandBuffer(this, &info);
		}
		
		public void End()
		{
			Vulkan.vkEndCommandBuffer(this).CheckResult();
		}

		public override void Dispose()
		{
		}
	}
}