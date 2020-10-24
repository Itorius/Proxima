using System;
using System.Collections.Generic;
using System.Linq;
using Vortice.Vulkan;

namespace Proxima.Graphics
{
	public class GraphicsPipeline : GraphicsObject
	{
		public struct Options
		{
			public Type UniformBufferType;
			public Shader Shader;
			public Texture2D Texture;
		}

		public VkPipelineLayout PipelineLayout { get; private set; }
		public VkPipeline Pipeline { get; private set; }
		private VkDescriptorSetLayout DescriptorSetLayout;
		public UniformBuffer[] UniformBuffers { get; private set; }

		private VkDescriptorPool DescriptorPool;
		private VkDescriptorSet[] DescriptorSets;
		private Type uniformBufferType;
		private Shader shader;
		private Texture2D texture;

		public GraphicsPipeline(GraphicsDevice graphicsDevice, Options options, Action<GraphicsPipeline> initialization) : base(graphicsDevice)
		{
			texture = options.Texture;
			shader = options.Shader;
			uniformBufferType = options.UniformBufferType;

			initialization.Invoke(this);

			Create();
		}

		private List<VertexBuffer> vertexBuffers = new List<VertexBuffer>();
		private List<Type> uniformBufferTypes = new List<Type>();

		public void AddVertexBuffer<T>(VertexBuffer<T> buffer) where T : unmanaged
		{
			vertexBuffers.Add(buffer);
		}

		public void AddUniformBuffer<T>() where T : unmanaged
		{
			uniformBufferTypes.Add(typeof(T));
		}

		public void Create()
		{
			UniformBuffers = new UniformBuffer[graphicsDevice.Swapchain.Length];
			for (int i = 0; i < UniformBuffers.Length; i++) UniformBuffers[i] = (UniformBuffer)Activator.CreateInstance(typeof(UniformBuffer<>).MakeGenericType(uniformBufferTypes[0]), graphicsDevice);

			CreateDescriptorSetLayout();
			CreateDescriptorSets();

			CreateGraphicsPipeline();
		}

		private unsafe void CreateDescriptorSetLayout()
		{
			List<VkDescriptorSetLayoutBinding> bindings = new List<VkDescriptorSetLayoutBinding>();

			foreach (var (stage, data) in shader.ReflectionData)
			{
				foreach (ReflectionData.Ubo ubo in data.UBOs)
				{
					bindings.Add(new VkDescriptorSetLayoutBinding
					{
						binding = ubo.Binding,
						descriptorType = VkDescriptorType.UniformBuffer,
						descriptorCount = 1, // > 1 for arrays
						stageFlags = stage
					});
				}

				foreach (ReflectionData.Texture ubo in data.Textures)
				{
					bindings.Add(new VkDescriptorSetLayoutBinding
					{
						binding = ubo.Binding,
						descriptorType = VkDescriptorType.CombinedImageSampler,
						descriptorCount = 1, // > 1 for arrays
						stageFlags = stage
					});
				}
			}

			VkDescriptorSetLayoutCreateInfo layoutCreateInfo = new VkDescriptorSetLayoutCreateInfo
			{
				sType = VkStructureType.DescriptorSetLayoutCreateInfo,
				bindingCount = (uint)bindings.Count
			};
			fixed (VkDescriptorSetLayoutBinding* ptr = bindings.GetInternalArray()) layoutCreateInfo.pBindings = ptr;

			Vulkan.vkCreateDescriptorSetLayout(graphicsDevice.LogicalDevice, &layoutCreateInfo, null, out DescriptorSetLayout).CheckResult();

			List<VkDescriptorPoolSize> poolSizes = new List<VkDescriptorPoolSize>();
			foreach (VkDescriptorType type in bindings.Select(binding => binding.descriptorType))
			{
				poolSizes.Add(new VkDescriptorPoolSize
				{
					type = type,
					descriptorCount = (uint)UniformBuffers.Length
				});
			}

			VkDescriptorPoolCreateInfo poolCreateInfo = new VkDescriptorPoolCreateInfo
			{
				sType = VkStructureType.DescriptorPoolCreateInfo,
				poolSizeCount = (uint)poolSizes.Count,
				maxSets = (uint)UniformBuffers.Length
			};
			fixed (VkDescriptorPoolSize* ptr = poolSizes.GetInternalArray()) poolCreateInfo.pPoolSizes = ptr;

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
				// todo: abstract this out
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
					imageView = texture.View,
					sampler = texture.Sampler
				};

				// todo: abstract this out
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
			List<VkVertexInputBindingDescription> bindingDescriptions = new List<VkVertexInputBindingDescription>();
			List<VkVertexInputAttributeDescription> attributeDescriptions = new List<VkVertexInputAttributeDescription>();

			foreach (VertexBuffer buffer in vertexBuffers)
			{
				bindingDescriptions.Add(buffer.GetVertexInputBindingDescription());
				attributeDescriptions.AddRange(buffer.GetVertexInputAttributeDescriptions());
			}

			VkPipelineVertexInputStateCreateInfo vertexInputInfo = new VkPipelineVertexInputStateCreateInfo
			{
				sType = VkStructureType.PipelineVertexInputStateCreateInfo,
				vertexBindingDescriptionCount = (uint)bindingDescriptions.Count,
				vertexAttributeDescriptionCount = (uint)attributeDescriptions.Count
			};
			fixed (VkVertexInputBindingDescription* ptr = bindingDescriptions.GetInternalArray()) vertexInputInfo.pVertexBindingDescriptions = ptr;
			fixed (VkVertexInputAttributeDescription* ptr = attributeDescriptions.GetInternalArray()) vertexInputInfo.pVertexAttributeDescriptions = ptr;

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

			fixed (VkPipelineShaderStageCreateInfo* ptr = shader.Stages.GetInternalArray()) pipelineCreateInfo.pStages = ptr;

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