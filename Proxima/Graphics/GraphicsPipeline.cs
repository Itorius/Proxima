using System;
using System.Linq;
using System.Reflection;
using Vortice.Vulkan;

namespace Proxima.Graphics
{
	public class GraphicsPipeline : GraphicsObject
	{
		public struct Options
		{
			public Type UniformBufferType;
			public Shader Shader;
		}

		public VkPipelineLayout PipelineLayout { get; private set; }
		public VkPipeline Pipeline { get; private set; }
		private VkDescriptorSetLayout DescriptorSetLayout;
		public UniformBuffer[] UniformBuffers { get; private set; }

		private VkDescriptorPool DescriptorPool;
		private VkDescriptorSet[] DescriptorSets;
		private Type uniformBufferType;
		private Shader shader;

		public GraphicsPipeline(GraphicsDevice graphicsDevice, Options options) : base(graphicsDevice)
		{
			shader = options.Shader;
			uniformBufferType = options.UniformBufferType;
			Create();
		}

		public void Create()
		{
			UniformBuffers = new UniformBuffer[graphicsDevice.Swapchain.Length];
			for (int i = 0; i < UniformBuffers.Length; i++) UniformBuffers[i] = ((UniformBuffer)Activator.CreateInstance(typeof(UniformBuffer<>).MakeGenericType(uniformBufferType), graphicsDevice))!;

			CreateDescriptorSetLayout();
			CreateDescriptorPool();
			CreateDescriptorSets();

			CreateGraphicsPipeline();
		}

		private unsafe void CreateDescriptorSetLayout()
		{
			VkDescriptorSetLayoutBinding[] bindings = new VkDescriptorSetLayoutBinding[shader.DescriptorTypes.Count];

			int i = 0;
			foreach (var (type, set, binding) in shader.DescriptorTypes)
			{
				bindings[i++] = new VkDescriptorSetLayoutBinding
				{
					binding = binding,
					descriptorType = type,
					descriptorCount = 1,
					stageFlags = type == VkDescriptorType.CombinedImageSampler ? VkShaderStageFlags.Fragment : VkShaderStageFlags.Vertex
				};
			}

			VkDescriptorSetLayoutCreateInfo layoutCreateInfo = new VkDescriptorSetLayoutCreateInfo
			{
				sType = VkStructureType.DescriptorSetLayoutCreateInfo,
				bindingCount = (uint)bindings.Length
			};
			fixed (VkDescriptorSetLayoutBinding* ptr = bindings) layoutCreateInfo.pBindings = ptr;

			Vulkan.vkCreateDescriptorSetLayout(graphicsDevice.LogicalDevice, &layoutCreateInfo, null, out DescriptorSetLayout).CheckResult();
		}

		private unsafe void CreateDescriptorPool()
		{
			VkDescriptorPoolSize[] poolSizes = new VkDescriptorPoolSize[shader.DescriptorTypes.Count];
			for (int i = 0; i < poolSizes.Length; i++)
			{
				poolSizes[i] = new VkDescriptorPoolSize
				{
					type = shader.DescriptorTypes[i].type,
					descriptorCount = (uint)UniformBuffers.Length
				};
			}

			VkDescriptorPoolCreateInfo poolCreateInfo = new VkDescriptorPoolCreateInfo
			{
				sType = VkStructureType.DescriptorPoolCreateInfo,
				poolSizeCount = (uint)poolSizes.Length,
				maxSets = (uint)UniformBuffers.Length
			};
			fixed (VkDescriptorPoolSize* ptr = poolSizes) poolCreateInfo.pPoolSizes = ptr;

			Vulkan.vkCreateDescriptorPool(graphicsDevice.LogicalDevice, &poolCreateInfo, null, out DescriptorPool).CheckResult();
		}

		private unsafe void CreateDescriptorSets()
		{
			VkDescriptorSetLayout[] layouts = Enumerable.Repeat(DescriptorSetLayout, (int)graphicsDevice.Swapchain.Length).ToArray();

			VkDescriptorSetAllocateInfo allocateInfo = new VkDescriptorSetAllocateInfo
			{
				sType = VkStructureType.DescriptorSetAllocateInfo,
				descriptorPool = DescriptorPool,
				descriptorSetCount = graphicsDevice.Swapchain.Length
			};
			fixed (VkDescriptorSetLayout* ptr = layouts) allocateInfo.pSetLayouts = ptr;

			DescriptorSets = new VkDescriptorSet[graphicsDevice.Swapchain.Length];
			fixed (VkDescriptorSet* ptr = DescriptorSets) Vulkan.vkAllocateDescriptorSets(graphicsDevice.LogicalDevice, &allocateInfo, ptr).CheckResult();

			for (int i = 0; i < DescriptorSets.Length; i++)
			{
				VkDescriptorBufferInfo bufferInfo = new VkDescriptorBufferInfo
				{
					buffer = (VkBuffer)UniformBuffers[i],
					offset = 0,
					range = UniformBuffers[i].Size
				};

				// todo: abstract this out
				VkDescriptorImageInfo imageInfo = new VkDescriptorImageInfo
				{
					imageLayout = VkImageLayout.ShaderReadOnlyOptimal,
					imageView = graphicsDevice.Texture.View,
					sampler = graphicsDevice.Texture.Sampler
				};

				VkWriteDescriptorSet[] writeDescriptorSets =
				{
					new VkWriteDescriptorSet
					{
						sType = VkStructureType.WriteDescriptorSet,
						dstSet = DescriptorSets[i],
						dstBinding = 0,
						dstArrayElement = 0,
						descriptorType = VkDescriptorType.UniformBuffer,
						descriptorCount = 1,
						pBufferInfo = &bufferInfo
					},
					new VkWriteDescriptorSet
					{
						sType = VkStructureType.WriteDescriptorSet,
						dstSet = DescriptorSets[i],
						dstBinding = 1,
						dstArrayElement = 0,
						descriptorType = VkDescriptorType.CombinedImageSampler,
						descriptorCount = 1,
						pImageInfo = &imageInfo
					}
				};

				Vulkan.vkUpdateDescriptorSets(graphicsDevice.LogicalDevice, writeDescriptorSets);
			}
		}

		private unsafe void CreateGraphicsPipeline()
		{
			// todo: abstract this out
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

			VkViewport viewport = new VkViewport(0, 0, graphicsDevice.Swapchain.Extent.width, graphicsDevice.Swapchain.Extent.height, 0f, 1f);
			VkRect2D scissor = new VkRect2D(0, 0, graphicsDevice.Swapchain.Extent.width, graphicsDevice.Swapchain.Extent.height);

			VkPipelineViewportStateCreateInfo viewportState = new VkPipelineViewportStateCreateInfo
			{
				sType = VkStructureType.PipelineViewportStateCreateInfo,
				viewportCount = 1,
				pViewports = &viewport,
				scissorCount = 1,
				pScissors = &scissor
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
				srcAlphaBlendFactor = VkBlendFactor.SrcAlpha,
				dstAlphaBlendFactor = VkBlendFactor.OneMinusSrcAlpha,
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
				dynamicStateCount = (uint)dynamicStates.Length
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

			Vulkan.vkCreatePipelineLayout(graphicsDevice.LogicalDevice, &pipelineLayoutCreateInfo, null, out var pipelineLayout).CheckResult();
			PipelineLayout = pipelineLayout;

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
				renderPass = (VkRenderPass)graphicsDevice.RenderPass,
				subpass = 0,
				basePipelineHandle = VkPipeline.Null,
				basePipelineIndex = -1,
				stageCount = (uint)shader.Stages.Count
			};

			FieldInfo fieldInfo = shader.Stages.GetType().GetField("_items", BindingFlags.NonPublic | BindingFlags.Instance);
			VkPipelineShaderStageCreateInfo[] p = (VkPipelineShaderStageCreateInfo[])fieldInfo.GetValue(shader.Stages);

			fixed (VkPipelineShaderStageCreateInfo* ptr = p) pipelineCreateInfo.pStages = ptr;

			Vulkan.vkCreateGraphicsPipeline(graphicsDevice.LogicalDevice, VkPipelineCache.Null, pipelineCreateInfo, out var pipeline).CheckResult();
			Pipeline = pipeline;
		}

		public void Invalidate()
		{
			Dispose();

			Create();
		}

		public override unsafe void Dispose()
		{
			Vulkan.vkDestroyDescriptorPool(graphicsDevice.LogicalDevice, DescriptorPool, null);
			Vulkan.vkDestroyDescriptorSetLayout(graphicsDevice.LogicalDevice, DescriptorSetLayout, null);
			Vulkan.vkDestroyPipeline(graphicsDevice.LogicalDevice, Pipeline, null);
			Vulkan.vkDestroyPipelineLayout(graphicsDevice.LogicalDevice, PipelineLayout, null);

			foreach (var buffer in UniformBuffers) buffer.Dispose();
		}

		public void Bind(VkCommandBuffer buffer)
		{
			Vulkan.vkCmdBindDescriptorSets(buffer, VkPipelineBindPoint.Graphics, PipelineLayout, 0, DescriptorSets[graphicsDevice.CurrentFrameIndex]);
		}
	}
}