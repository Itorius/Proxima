using System;
using System.Collections.Generic;
using System.Linq;
using Vortice.Vulkan;

namespace Proxima.Graphics
{
	public class GraphicsPipeline : GraphicsObject
	{
		private VkPipelineLayout PipelineLayout;
		private VkPipeline Pipeline;
		private VkDescriptorSetLayout DescriptorSetLayout;
		private List<UniformBuffer[]> UniformBuffers;
		private VkDescriptorPool DescriptorPool;
		private VkDescriptorSet[] DescriptorSets;

		public UniformBuffer<T> GetBuffer<T>() where T : unmanaged
		{
			foreach (UniformBuffer[] buffers in UniformBuffers)
			{
				var elementType = buffers[0].GetType().GetGenericArguments()[0];
				if (elementType == typeof(T))
				{
					return (UniformBuffer<T>)buffers[graphicsDevice.CurrentFrameIndex];
				}
			}

			return null;
		}

		public GraphicsPipeline(GraphicsDevice graphicsDevice, Action<GraphicsPipeline> initialization) : base(graphicsDevice)
		{
			initialization.Invoke(this);

			Create();
		}

		private List<VertexBuffer> vertexBuffers = new List<VertexBuffer>();
		private List<Type> uniformBufferTypes = new List<Type>();
		private List<Texture2D> textures = new List<Texture2D>();
		private Shader shader;

		public void SetShader(Shader shader)
		{
			this.shader = shader;
		}

		public void AddVertexBuffer<T>(VertexBuffer<T> buffer) where T : unmanaged
		{
			vertexBuffers.Add(buffer);
		}

		public void AddUniformBuffer<T>() where T : unmanaged
		{
			uniformBufferTypes.Add(typeof(T));
		}

		public void AddTexture(Texture2D texture)
		{
			textures.Add(texture);
		}

		public void Create()
		{
			UniformBuffers = new List<UniformBuffer[]>();
			foreach (Type bufferType in uniformBufferTypes)
			{
				var buffers = new UniformBuffer[graphicsDevice.Swapchain.Length];
				for (int i = 0; i < buffers.Length; i++) buffers[i] = (UniformBuffer)Activator.CreateInstance(typeof(UniformBuffer<>).MakeGenericType(bufferType), graphicsDevice);
				UniformBuffers.Add(buffers);
			}

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

				foreach (ReflectionData.Texture texture in data.Textures)
				{
					bindings.Add(new VkDescriptorSetLayoutBinding
					{
						binding = texture.Binding,
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
					descriptorCount = graphicsDevice.Swapchain.Length
				});
			}

			VkDescriptorPoolCreateInfo poolCreateInfo = new VkDescriptorPoolCreateInfo
			{
				sType = VkStructureType.DescriptorPoolCreateInfo,
				poolSizeCount = (uint)poolSizes.Count,
				maxSets = graphicsDevice.Swapchain.Length
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
				ref var descriptorSet = ref DescriptorSets[i];

				VkDescriptorBufferInfo[] bufferInfos = new VkDescriptorBufferInfo[UniformBuffers.Count];
				for (int j = 0; j < bufferInfos.Length; j++)
				{
					bufferInfos[j] = new VkDescriptorBufferInfo
					{
						buffer = (VkBuffer)UniformBuffers[j][i],
						offset = 0,
						range = UniformBuffers[j][i].Size
					};
				}

				VkDescriptorImageInfo[] imageInfos = new VkDescriptorImageInfo[textures.Count];
				for (int j = 0; j < imageInfos.Length; j++)
				{
					imageInfos[j] = new VkDescriptorImageInfo
					{
						imageLayout = VkImageLayout.ShaderReadOnlyOptimal,
						imageView = textures[j].View,
						sampler = textures[j].Sampler
					};
				}

				List<VkWriteDescriptorSet> writeDescriptorSets = new List<VkWriteDescriptorSet>();

				if (bufferInfos.Length > 0)
				{
					for (int j = 0; j < bufferInfos.Length; j++)
					{
						ref VkDescriptorBufferInfo bufferInfo = ref bufferInfos[j];
						var set = new VkWriteDescriptorSet
						{
							sType = VkStructureType.WriteDescriptorSet,
							dstSet = descriptorSet,
							// note: this is not nice
							dstBinding = shader.ReflectionData.SelectMany(x => x.Value.UBOs).ToArray()[j].Binding,
							dstArrayElement = 0,
							descriptorType = VkDescriptorType.UniformBuffer,
							descriptorCount = 1
						};
						fixed (VkDescriptorBufferInfo* ptr = &bufferInfo) set.pBufferInfo = ptr;
						writeDescriptorSets.Add(set);
					}
				}

				if (imageInfos.Length > 0)
				{
					for (int j = 0; j < imageInfos.Length; j++)
					{
						ref VkDescriptorImageInfo imageInfo = ref imageInfos[j];
						var set = new VkWriteDescriptorSet
						{
							sType = VkStructureType.WriteDescriptorSet,
							dstSet = descriptorSet,
							// note: this is not nice
							dstBinding = shader.ReflectionData[VkShaderStageFlags.Fragment].Textures[j].Binding,
							dstArrayElement = 0,
							descriptorType = VkDescriptorType.CombinedImageSampler,
							descriptorCount = 1
						};
						fixed (VkDescriptorImageInfo* ptr = &imageInfo) set.pImageInfo = ptr;
						writeDescriptorSets.Add(set);
					}
				}

				fixed (VkWriteDescriptorSet* ptr = writeDescriptorSets.GetInternalArray()) Vulkan.vkUpdateDescriptorSets(graphicsDevice.LogicalDevice, (uint)writeDescriptorSets.Count, ptr, 0, null);
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

			foreach (UniformBuffer buffer in UniformBuffers.SelectMany(buffers => buffers)) buffer.Dispose();
		}

		public void Bind(VkCommandBuffer buffer)
		{
			Vulkan.vkCmdBindDescriptorSets(buffer, VkPipelineBindPoint.Graphics, PipelineLayout, 0, DescriptorSets[graphicsDevice.CurrentFrameIndex]);
			Vulkan.vkCmdBindPipeline(buffer, VkPipelineBindPoint.Graphics, Pipeline);
		}
	}
}