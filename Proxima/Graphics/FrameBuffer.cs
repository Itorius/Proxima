using System.Collections.Generic;
using Vortice.Vulkan;

namespace Proxima.Graphics
{
	public struct FrameBufferOptions
	{
		public List<VkImageView> Attachments;
		public RenderPass RenderPass;
		public uint Width, Height;
		public uint Layers;
	}

	public class FrameBuffer : VulkanObject
	{
		private VkFramebuffer buffer;
		public FrameBufferOptions options;

		public FrameBuffer(GraphicsDevice graphicsDevice, FrameBufferOptions options) : base(graphicsDevice)
		{
			this.options = options;

			Invalidate();
		}

		public unsafe void Invalidate()
		{
			if (buffer != VkFramebuffer.Null)
			{
				Vulkan.vkDestroyFramebuffer(graphicsDevice.LogicalDevice, buffer, null);
			}

			VkFramebufferCreateInfo info = new VkFramebufferCreateInfo
			{
				sType = VkStructureType.FramebufferCreateInfo,
				renderPass = (VkRenderPass)options.RenderPass,
				attachmentCount = (uint)options.Attachments.Count,
				width = options.Width,
				height = options.Height,
				layers = options.Layers
			};
			fixed (VkImageView* ptr = options.Attachments.ToArray()) info.pAttachments = ptr;

			Vulkan.vkCreateFramebuffer(graphicsDevice.LogicalDevice, &info, null, out buffer).CheckResult();
		}

		public void Resize(uint width, uint height)
		{
			options.Width = width;
			options.Height = height;

			Invalidate();
		}

		public static explicit operator VkFramebuffer(FrameBuffer buffer) => buffer.buffer;

		public override unsafe void Dispose()
		{
			Vulkan.vkDestroyFramebuffer(graphicsDevice.LogicalDevice, buffer, null);
		}
	}
}