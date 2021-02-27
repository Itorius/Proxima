using System;
using System.Collections.Generic;
using System.Linq;
using Vortice.Vulkan;

namespace Proxima.Graphics
{
	public class Pipeline : VulkanObject
	{
		private VkPipelineLayout pipelineLayout;
		private VkPipeline pipeline;
		private VkDescriptorSetLayout descriptorSetLayout;
		private VkDescriptorPool descriptorPool;
		private VkDescriptorSet[] descriptorSets;

		public UniformBuffer GetBuffer(uint binding)
		{
			foreach (UniformBuffer[] buffers in UniformBuffers)
			{
				var elementType = buffers[0].GetType().GetGenericArguments()[0];
				if (elementType == uniformBufferTypes[binding])
				{
					return buffers[graphicsDevice.CurrentFrameIndex];
				}
			}

			return null;
		}

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

		public Pipeline(GraphicsDevice graphicsDevice, Action<Pipeline> initialization) : base(graphicsDevice)
		{
			initialization.Invoke(this);

			Create();
		}

		private List<UniformBuffer[]> UniformBuffers;
		private List<VertexBuffer> vertexBuffers = new();
		private Dictionary<uint, Type> uniformBufferTypes = new();
		private Dictionary<uint, Texture> textures = new();
		private Shader shader;

		public void SetShader(Shader shader)
		{
			this.shader = shader;
		}

		public void AddVertexBuffer<T>(VertexBuffer<T> buffer) where T : unmanaged
		{
			vertexBuffers.Add(buffer);
		}

		public void AddUniformBuffer<T>(uint binding) where T : unmanaged
		{
			uniformBufferTypes.Add(binding, typeof(T));
		}

		public void AddTexture(uint binding, Texture texture)
		{
			textures.Add(binding, texture);
		}

		public void Create()
		{
			UniformBuffers = new List<UniformBuffer[]>();
			foreach (KeyValuePair<uint, Type> bufferType in uniformBufferTypes)
			{
				var buffers = new UniformBuffer[graphicsDevice.Swapchain.Length];
				for (int i = 0; i < buffers.Length; i++) buffers[i] = (UniformBuffer)Activator.CreateInstance(typeof(UniformBuffer<>).MakeGenericType(bufferType.Value), graphicsDevice);
				UniformBuffers.Add(buffers);
			}

			CreateDescriptorSetLayout();
			CreateDescriptorSets();
			
			CreateGraphicsPipeline();
		}

		private unsafe void CreateDescriptorSetLayout()
		{
			#region Create descriptor set layout
			List<VkDescriptorSetLayoutBinding> bindings = new List<VkDescriptorSetLayoutBinding>();

			foreach (ReflectionData.Ubo ubo in shader.ReflectionData.UBOs)
			{
				bindings.Add(new VkDescriptorSetLayoutBinding
				{
					binding = ubo.Binding,
					descriptorType = VkDescriptorType.UniformBuffer,
					descriptorCount = 1, // > 1 for arrays
					stageFlags = ubo.Stage
				});
			}

			foreach (ReflectionData.Texture texture in shader.ReflectionData.Textures)
			{
				bindings.Add(new VkDescriptorSetLayoutBinding
				{
					binding = texture.Binding,
					descriptorType = VkDescriptorType.CombinedImageSampler,
					descriptorCount = 1, // > 1 for arrays
					stageFlags = texture.Stage
				});
			}

			VkDescriptorSetLayoutCreateInfo layoutCreateInfo = new VkDescriptorSetLayoutCreateInfo
			{
				sType = VkStructureType.DescriptorSetLayoutCreateInfo,
				bindingCount = (uint)bindings.Count
			};
			fixed (VkDescriptorSetLayoutBinding* ptr = bindings.GetInternalArray()) layoutCreateInfo.pBindings = ptr;

			Vulkan.vkCreateDescriptorSetLayout(graphicsDevice.LogicalDevice, &layoutCreateInfo, null, out descriptorSetLayout).CheckResult();
			#endregion

			#region Create descriptor pool
			List<VkDescriptorPoolSize> poolSizes = bindings.Select(binding => new VkDescriptorPoolSize
			{
				type = binding.descriptorType,
				descriptorCount = graphicsDevice.Swapchain.Length
			}).ToList();

			VkDescriptorPoolCreateInfo poolCreateInfo = new VkDescriptorPoolCreateInfo
			{
				sType = VkStructureType.DescriptorPoolCreateInfo,
				poolSizeCount = (uint)poolSizes.Count,
				maxSets = graphicsDevice.Swapchain.Length
			};
			fixed (VkDescriptorPoolSize* ptr = poolSizes.GetInternalArray()) poolCreateInfo.pPoolSizes = ptr;

			Vulkan.vkCreateDescriptorPool(graphicsDevice.LogicalDevice, &poolCreateInfo, null, out descriptorPool).CheckResult();
			#endregion
		}

		private unsafe void CreateDescriptorSets()
		{
			#region Allocate sets
			VkDescriptorSetLayout[] layouts = Enumerable.Repeat(descriptorSetLayout, (int)graphicsDevice.Swapchain.Length).ToArray();

			VkDescriptorSetAllocateInfo allocateInfo = new VkDescriptorSetAllocateInfo
			{
				sType = VkStructureType.DescriptorSetAllocateInfo,
				descriptorPool = descriptorPool,
				descriptorSetCount = graphicsDevice.Swapchain.Length
			};
			fixed (VkDescriptorSetLayout* ptr = layouts) allocateInfo.pSetLayouts = ptr;

			descriptorSets = new VkDescriptorSet[graphicsDevice.Swapchain.Length];
			fixed (VkDescriptorSet* ptr = descriptorSets) Vulkan.vkAllocateDescriptorSets(graphicsDevice.LogicalDevice, &allocateInfo, ptr).CheckResult();
			#endregion

			UpdateDescriptorSets();
		}

		public unsafe void UpdateDescriptorSets()
		{
			for (int i = 0; i < descriptorSets.Length; i++)
			{
				int i1 = i;
				ref var descriptorSet = ref descriptorSets[i];

				VkDescriptorBufferInfo[] bufferInfos = new VkDescriptorBufferInfo[UniformBuffers.Count];
				uniformBufferTypes.ForEach((j, pair) => bufferInfos[j] = new VkDescriptorBufferInfo
				{
					buffer = (VkBuffer)UniformBuffers[j][i1],
					offset = 0,
					range = UniformBuffers[j][i1].Size
				});

				VkDescriptorImageInfo[] imageInfos = new VkDescriptorImageInfo[textures.Count];
				textures.ForEach((i, pair) => imageInfos[i] = new VkDescriptorImageInfo
				{
					imageLayout = VkImageLayout.ShaderReadOnlyOptimal,
					imageView = pair.Value.View,
					sampler = pair.Value.Sampler
				});

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
							dstBinding = shader.ReflectionData.UBOs[j].Binding,
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
							dstBinding = shader.ReflectionData.Textures[j].Binding,
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
			fixed (VkDescriptorSetLayout* ptr = &descriptorSetLayout) pipelineLayoutCreateInfo.pSetLayouts = ptr;

			Vulkan.vkCreatePipelineLayout(graphicsDevice.LogicalDevice, &pipelineLayoutCreateInfo, null, out var pipelineLayout).CheckResult();
			this.pipelineLayout = pipelineLayout;

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
				layout = this.pipelineLayout,
				renderPass = (VkRenderPass)graphicsDevice.RenderPass,
				subpass = 0,
				basePipelineHandle = VkPipeline.Null,
				basePipelineIndex = -1,
				stageCount = (uint)shader.Stages.Count
			};

			fixed (VkPipelineShaderStageCreateInfo* ptr = shader.Stages.GetInternalArray()) pipelineCreateInfo.pStages = ptr;

			Vulkan.vkCreateGraphicsPipeline(graphicsDevice.LogicalDevice, VkPipelineCache.Null, pipelineCreateInfo, out var pipeline).CheckResult();
			this.pipeline = pipeline;
		}

		public void Invalidate()
		{
			Dispose();

			Create();
		}

		public override unsafe void Dispose()
		{
			Vulkan.vkDestroyDescriptorPool(graphicsDevice.LogicalDevice, descriptorPool, null);
			Vulkan.vkDestroyDescriptorSetLayout(graphicsDevice.LogicalDevice, descriptorSetLayout, null);
			Vulkan.vkDestroyPipeline(graphicsDevice.LogicalDevice, pipeline, null);
			Vulkan.vkDestroyPipelineLayout(graphicsDevice.LogicalDevice, pipelineLayout, null);

			foreach (UniformBuffer buffer in UniformBuffers.SelectMany(buffers => buffers)) buffer.Dispose();
		}

		public void Bind(VkCommandBuffer buffer)
		{
			Vulkan.vkCmdBindDescriptorSets(buffer, VkPipelineBindPoint.Graphics, pipelineLayout, 0, descriptorSets[graphicsDevice.CurrentFrameIndex]);
			Vulkan.vkCmdBindPipeline(buffer, VkPipelineBindPoint.Graphics, pipeline);
		}
	}
}