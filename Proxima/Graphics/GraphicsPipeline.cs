using Vortice.Vulkan;

namespace Proxima.Graphics
{
	public class GraphicsPipeline: GraphicsObject
	{
		public struct Options
		{
			public VkViewport Viewport;
			public VkRect2D Scissor;
		}
		
		private VkPipelineLayout PipelineLayout;
		private VkPipeline Pipeline;
		private VkDescriptorSetLayout DescriptorSetLayout;
		
		public unsafe GraphicsPipeline(GraphicsDevice graphicsDevice, Options options, VkRenderPass RenderPass) : base(graphicsDevice)
		{
			VkDescriptorSetLayoutBinding uboLayoutBinding = new VkDescriptorSetLayoutBinding
			{
				binding = 0,
				descriptorType = VkDescriptorType.UniformBuffer,
				descriptorCount = 1,
				stageFlags = VkShaderStageFlags.Vertex
			};

			VkDescriptorSetLayoutBinding samplerLayoutBinding = new VkDescriptorSetLayoutBinding
			{
				binding = 1,
				descriptorType = VkDescriptorType.CombinedImageSampler,
				descriptorCount = 1,
				stageFlags = VkShaderStageFlags.Fragment
			};

			VkDescriptorSetLayoutBinding[] bindings = { uboLayoutBinding, samplerLayoutBinding };

			VkDescriptorSetLayoutCreateInfo layoutCreateInfo = new VkDescriptorSetLayoutCreateInfo
			{
				sType = VkStructureType.DescriptorSetLayoutCreateInfo,
				bindingCount = (uint)bindings.Length
			};
			fixed (VkDescriptorSetLayoutBinding* ptr = bindings) layoutCreateInfo.pBindings = ptr;

			Vulkan.vkCreateDescriptorSetLayout(graphicsDevice.LogicalDevice, &layoutCreateInfo, null, out DescriptorSetLayout).CheckResult();
			
			Shader shader = new Shader(graphicsDevice, "Assets/test.vert.spv", "Assets/test.frag.spv");

			var bindingDescription = Renderer2D.Vertex.GetBindingDescription();
			var attributeDescriptions = Renderer2D.Vertex.GetAttributeDescriptions();

			VkPipelineVertexInputStateCreateInfo vertexInputInfo = new VkPipelineVertexInputStateCreateInfo
			{
				sType = VkStructureType.PipelineVertexInputStateCreateInfo,
				vertexBindingDescriptionCount = 1,
				pVertexBindingDescriptions = &bindingDescription,
				vertexAttributeDescriptionCount = (uint)attributeDescriptions.Length
			};
			fixed (VkVertexInputAttributeDescription* ptr = attributeDescriptions) vertexInputInfo.pVertexAttributeDescriptions = ptr;

			VkPipelineInputAssemblyStateCreateInfo inputAssembly = new VkPipelineInputAssemblyStateCreateInfo
			{
				sType = VkStructureType.PipelineInputAssemblyStateCreateInfo,
				topology = VkPrimitiveTopology.TriangleList,
				primitiveRestartEnable = false
			};

			VkPipelineViewportStateCreateInfo viewportState = new VkPipelineViewportStateCreateInfo
			{
				sType = VkStructureType.PipelineViewportStateCreateInfo,
				viewportCount = 1,
				pViewports = &options.Viewport,
				scissorCount = 1,
				pScissors = &options.Scissor
			};

			VkPipelineRasterizationStateCreateInfo rasterizer = new VkPipelineRasterizationStateCreateInfo
			{
				sType = VkStructureType.PipelineRasterizationStateCreateInfo,
				depthClampEnable = false,
				rasterizerDiscardEnable = false,
				polygonMode = VkPolygonMode.Fill,
				lineWidth = 1f,
				cullMode = VkCullModeFlags.Back,
				frontFace = VkFrontFace.Clockwise,
				depthBiasEnable = false,
				depthBiasConstantFactor = 0f,
				depthBiasClamp = 0f,
				depthBiasSlopeFactor = 0f
			};

			VkPipelineMultisampleStateCreateInfo multisampling = new VkPipelineMultisampleStateCreateInfo
			{
				sType = VkStructureType.PipelineMultisampleStateCreateInfo,
				sampleShadingEnable = false,
				rasterizationSamples = VkSampleCountFlags.Count1,
				minSampleShading = 1f,
				pSampleMask = null,
				alphaToCoverageEnable = false,
				alphaToOneEnable = false
			};

			VkPipelineColorBlendAttachmentState colorBlendAttachment = new VkPipelineColorBlendAttachmentState
			{
				colorWriteMask = VkColorComponentFlags.All,
				blendEnable = true,
				srcColorBlendFactor = VkBlendFactor.SrcAlpha,
				dstColorBlendFactor = VkBlendFactor.OneMinusSrcAlpha,
				colorBlendOp = VkBlendOp.Add,
				srcAlphaBlendFactor = VkBlendFactor.One,
				dstAlphaBlendFactor = VkBlendFactor.Zero,
				alphaBlendOp = VkBlendOp.Add
			};

			VkPipelineColorBlendStateCreateInfo colorBlending = new VkPipelineColorBlendStateCreateInfo
			{
				sType = VkStructureType.PipelineColorBlendStateCreateInfo,
				logicOpEnable = false,
				logicOp = VkLogicOp.Copy,
				attachmentCount = 1,
				pAttachments = &colorBlendAttachment
			};

			VkDynamicState[] dynamicStates = { VkDynamicState.Viewport, VkDynamicState.LineWidth };

			VkPipelineDynamicStateCreateInfo dynamicState = new VkPipelineDynamicStateCreateInfo
			{
				sType = VkStructureType.PipelineDynamicStateCreateInfo,
				dynamicStateCount = 2
			};

			fixed (VkDynamicState* ptr = dynamicStates) dynamicState.pDynamicStates = ptr;

			VkPipelineLayoutCreateInfo pipelineLayoutCreateInfo = new VkPipelineLayoutCreateInfo
			{
				sType = VkStructureType.PipelineLayoutCreateInfo,
				setLayoutCount = 1,
				pushConstantRangeCount = 0,
				pPushConstantRanges = null
			};
			fixed (VkDescriptorSetLayout* ptr = &DescriptorSetLayout) pipelineLayoutCreateInfo.pSetLayouts = ptr;

			Vulkan.vkCreatePipelineLayout(graphicsDevice.LogicalDevice, &pipelineLayoutCreateInfo, null, out PipelineLayout).CheckResult();

			VkPipelineDepthStencilStateCreateInfo depthStencil = new VkPipelineDepthStencilStateCreateInfo
			{
				sType = VkStructureType.PipelineDepthStencilStateCreateInfo,
				depthTestEnable = true,
				depthWriteEnable = true,
				depthCompareOp = VkCompareOp.Less,
				depthBoundsTestEnable = false,
				stencilTestEnable = false
			};

			VkGraphicsPipelineCreateInfo pipelineCreateInfo = new VkGraphicsPipelineCreateInfo
			{
				sType = VkStructureType.GraphicsPipelineCreateInfo,
				pVertexInputState = &vertexInputInfo,
				pInputAssemblyState = &inputAssembly,
				pViewportState = &viewportState,
				pRasterizationState = &rasterizer,
				pMultisampleState = &multisampling,
				pDepthStencilState = &depthStencil,
				pColorBlendState = &colorBlending,
				pDynamicState = null,
				layout = PipelineLayout,
				renderPass = RenderPass,
				subpass = 0,
				basePipelineHandle = VkPipeline.Null,
				basePipelineIndex = -1,
				stageCount = (uint)shader.Stages.Length
			};

			fixed (VkPipelineShaderStageCreateInfo* ptr = shader.Stages) pipelineCreateInfo.pStages = ptr;

			Vulkan.vkCreateGraphicsPipeline(graphicsDevice.LogicalDevice, VkPipelineCache.Null, pipelineCreateInfo, out Pipeline).CheckResult();

			shader.Dispose();
		}

		public override void Dispose()
		{
			throw new System.NotImplementedException();
		}
	}
}