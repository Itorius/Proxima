using Vortice.Vulkan;

namespace Proxima.Graphics
{
	public class RenderPass : VulkanObject
	{
		private VkRenderPass renderPass;

		public static explicit operator VkRenderPass(RenderPass renderPass) => renderPass.renderPass;
		
		public unsafe RenderPass(GraphicsDevice graphicsDevice, VkFormat surfaceFormatFormat) : base(graphicsDevice)
		{
			VkAttachmentDescription colorAttachment = new VkAttachmentDescription
			{
				format = surfaceFormatFormat,
				samples = VkSampleCountFlags.Count1,
				loadOp = VkAttachmentLoadOp.Clear,
				storeOp = VkAttachmentStoreOp.Store,
				stencilLoadOp = VkAttachmentLoadOp.DontCare,
				stencilStoreOp = VkAttachmentStoreOp.DontCare,
				initialLayout = VkImageLayout.Undefined,
				finalLayout = VkImageLayout.PresentSrcKHR
			};

			VkAttachmentDescription depthAttachment = new VkAttachmentDescription
			{
				format = VulkanUtils.FindDepthFormat(graphicsDevice),
				samples = VkSampleCountFlags.Count1,
				loadOp = VkAttachmentLoadOp.Clear,
				storeOp = VkAttachmentStoreOp.Store,
				stencilLoadOp = VkAttachmentLoadOp.Load,
				stencilStoreOp = VkAttachmentStoreOp.Store,
				initialLayout = VkImageLayout.Undefined,
				finalLayout = VkImageLayout.DepthStencilAttachmentOptimal
			};

			VkAttachmentReference colorAttachmentRef = new VkAttachmentReference
			{
				attachment = 0,
				layout = VkImageLayout.ColorAttachmentOptimal
			};

			VkAttachmentReference depthAttachmentRef = new VkAttachmentReference
			{
				attachment = 1,
				layout = VkImageLayout.DepthStencilAttachmentOptimal
			};

			VkSubpassDescription subpass = new VkSubpassDescription
			{
				pipelineBindPoint = VkPipelineBindPoint.Graphics,
				colorAttachmentCount = 1,
				pColorAttachments = &colorAttachmentRef,
				pDepthStencilAttachment = &depthAttachmentRef
			};

			VkSubpassDependency dependency = new VkSubpassDependency
			{
				srcSubpass = Vulkan.SubpassExternal,
				dstSubpass = 0,
				srcStageMask = VkPipelineStageFlags.ColorAttachmentOutput,
				srcAccessMask = 0,
				dstStageMask = VkPipelineStageFlags.ColorAttachmentOutput,
				dstAccessMask = VkAccessFlags.ColorAttachmentWrite
			};

			VkAttachmentDescription[] attachments = { colorAttachment, depthAttachment };

			VkRenderPassCreateInfo renderPassCreateInfo = new VkRenderPassCreateInfo
			{
				sType = VkStructureType.RenderPassCreateInfo,
				attachmentCount = (uint)attachments.Length,
				subpassCount = 1,
				pSubpasses = &subpass,
				dependencyCount = 1,
				pDependencies = &dependency
			};
			fixed (VkAttachmentDescription* ptr = attachments) renderPassCreateInfo.pAttachments = ptr;

			Vulkan.vkCreateRenderPass(graphicsDevice.LogicalDevice, &renderPassCreateInfo, null, out var renderPass).CheckResult();
			this.renderPass = renderPass;
		}

		public unsafe void Invalidate()
		{
			Dispose();
			
			VkAttachmentDescription colorAttachment = new VkAttachmentDescription
			{
				format = graphicsDevice.Swapchain.Format,
				samples = VkSampleCountFlags.Count1,
				loadOp = VkAttachmentLoadOp.Clear,
				storeOp = VkAttachmentStoreOp.Store,
				stencilLoadOp = VkAttachmentLoadOp.DontCare,
				stencilStoreOp = VkAttachmentStoreOp.DontCare,
				initialLayout = VkImageLayout.Undefined,
				finalLayout = VkImageLayout.PresentSrcKHR
			};

			VkAttachmentDescription depthAttachment = new VkAttachmentDescription
			{
				format = VulkanUtils.FindDepthFormat(graphicsDevice),
				samples = VkSampleCountFlags.Count1,
				loadOp = VkAttachmentLoadOp.Clear,
				storeOp = VkAttachmentStoreOp.Store,
				stencilLoadOp = VkAttachmentLoadOp.Load,
				stencilStoreOp = VkAttachmentStoreOp.Store,
				initialLayout = VkImageLayout.Undefined,
				finalLayout = VkImageLayout.DepthStencilAttachmentOptimal
			};

			VkAttachmentReference colorAttachmentRef = new VkAttachmentReference
			{
				attachment = 0,
				layout = VkImageLayout.ColorAttachmentOptimal
			};

			VkAttachmentReference depthAttachmentRef = new VkAttachmentReference
			{
				attachment = 1,
				layout = VkImageLayout.DepthStencilAttachmentOptimal
			};

			VkSubpassDescription subpass = new VkSubpassDescription
			{
				pipelineBindPoint = VkPipelineBindPoint.Graphics,
				colorAttachmentCount = 1,
				pColorAttachments = &colorAttachmentRef,
				pDepthStencilAttachment = &depthAttachmentRef
			};

			VkSubpassDependency dependency = new VkSubpassDependency
			{
				srcSubpass = Vulkan.SubpassExternal,
				dstSubpass = 0,
				srcStageMask = VkPipelineStageFlags.ColorAttachmentOutput,
				srcAccessMask = 0,
				dstStageMask = VkPipelineStageFlags.ColorAttachmentOutput,
				dstAccessMask = VkAccessFlags.ColorAttachmentWrite
			};

			VkAttachmentDescription[] attachments = { colorAttachment, depthAttachment };

			VkRenderPassCreateInfo renderPassCreateInfo = new VkRenderPassCreateInfo
			{
				sType = VkStructureType.RenderPassCreateInfo,
				attachmentCount = (uint)attachments.Length,
				subpassCount = 1,
				pSubpasses = &subpass,
				dependencyCount = 1,
				pDependencies = &dependency
			};
			fixed (VkAttachmentDescription* ptr = attachments) renderPassCreateInfo.pAttachments = ptr;

			Vulkan.vkCreateRenderPass(graphicsDevice.LogicalDevice, &renderPassCreateInfo, null, out var renderPass).CheckResult();
			this.renderPass = renderPass;
		}

		public override unsafe void Dispose()
		{
			Vulkan.vkDestroyRenderPass(graphicsDevice.LogicalDevice, renderPass, null);
		}
	}
}