using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using GLFW;
using Vortice.Mathematics;
using Vortice.Vulkan;
using Exception = System.Exception;
using Vulkan = Vortice.Vulkan.Vulkan;

namespace Proxima.Graphics
{
	public sealed partial class GraphicsDevice : IDisposable
	{
		internal struct QueueFamilyIndices
		{
			public uint? graphics;
			public uint? present;

			public bool IsComplete => graphics.HasValue && present.HasValue;
		}

		internal ref struct SwapChainSupportDetails
		{
			public VkSurfaceCapabilitiesKHR capabilities;
			public ReadOnlySpan<VkSurfaceFormatKHR> formats;
			public ReadOnlySpan<VkPresentModeKHR> presentModes;
		}

		private static readonly VkStringArray DeviceExtensions = new VkStringArray(new[] { Vulkan.KHRSwapchainExtensionName });

		internal NativeWindow window;

		private VkInstance Instance;
		internal VkPhysicalDevice PhysicalDevice;
		internal VkDevice LogicalDevice;

		internal VkQueue GraphicsQueue;
		private VkQueue PresentQueue;

		internal VkSurfaceKHR Surface;
		internal VulkanSwapchain Swapchain;

		private VulkanRenderPass RenderPass;
		private VkDescriptorSetLayout DescriptorSetLayout;
		internal VkPipelineLayout PipelineLayout;
		private VkPipeline GraphicsPipeline;

		private VkFramebuffer[] SwapchainFramebuffers;

		internal VkCommandPool CommandPool;

		private const int MaxFramesInFlight = 2;
		private VkSemaphore[] RenderFinishedSemaphores;
		private VkSemaphore[] ImageAvailableSemaphores;
		private VkFence[] InFlightFences;
		private VkFence[] ImagesInFlight;

		private int currentFrame;
		private bool framebufferResized;

		private UniformBuffer<UniformBufferObject>[] UniformBuffers;
		private Texture2D Texture;

		private VkDescriptorPool DescriptorPool;
		internal VkDescriptorSet[] DescriptorSets;

		private DepthBuffer DepthBuffer;

		public GraphicsDevice(NativeWindow window)
		{
			this.window = window;
		}

		public void Initialize()
		{
			if (Vulkan.Initialize() != VkResult.Success) throw new Exception("Failed to initialize Vulkan");
			if (ValidationEnabled && !IsValidationSupported()) throw new Exception("Validation layers requested, but not available");

			window.SizeChanged += (sender, args) => { framebufferResized = true; };

			CreateInstance();

			SetupDebugging();

			CreateWindowSurface();

			SelectPhysicalDevice();

			CreateLogicalDevice();

			Swapchain = new VulkanSwapchain(this);

			RenderPass = new VulkanRenderPass(this);

			CreateDescriptorSetLayout();

			CreateGraphicsPipeline();

			CreateCommandPool();

			CreateFramebuffers();

			UniformBuffers = new UniformBuffer<UniformBufferObject>[Swapchain.Length];
			for (int i = 0; i < UniformBuffers.Length; i++) UniformBuffers[i] = new UniformBuffer<UniformBufferObject>(this);

			Texture = new Texture2D(this, "Assets/Tom.png");

			CreateDescriptorPool();
			CreateDescriptorSets();

			CreateSyncObjects();
		}

		private unsafe void CreateInstance()
		{
			VkApplicationInfo appInfo = new VkApplicationInfo
			{
				sType = VkStructureType.ApplicationInfo,
				pApplicationName = new VkString(window.Title),
				applicationVersion = new VkVersion(1, 0, 0),
				pEngineName = new VkString("Proxima"),
				engineVersion = new VkVersion(1, 0, 0),
				apiVersion = Vulkan.vkEnumerateInstanceVersion()
			};

			ReadOnlySpan<VkExtensionProperties> extensions = Vulkan.vkEnumerateInstanceExtensionProperties();

			foreach (VkExtensionProperties properties in extensions)
			{
				Log.Debug(properties.GetExtensionName());
			}

			using VkStringArray vkInstanceExtensions = GetRequiredExtensions();

			VkStringArray layers = GetRequiredLayers();
			VkInstanceCreateInfo createInfo = new VkInstanceCreateInfo
			{
				sType = VkStructureType.InstanceCreateInfo,
				pApplicationInfo = &appInfo,
				enabledExtensionCount = vkInstanceExtensions.Length,
				ppEnabledExtensionNames = vkInstanceExtensions,
				enabledLayerCount = layers.Length,
				ppEnabledLayerNames = layers
			};

			if (ValidationEnabled)
			{
				var messengerCreateInfo = CreateDebugMessengerInfo();
				createInfo.pNext = &messengerCreateInfo;
			}

			Vulkan.vkCreateInstance(&createInfo, null, out Instance).CheckResult();
			Vulkan.vkLoadInstance(Instance);
		}

		private void CreateWindowSurface()
		{
			VkResult result = (VkResult)GLFW.Vulkan.CreateWindowSurface(Instance.Handle, window, IntPtr.Zero, out IntPtr handle);
			if (result != VkResult.Success) throw new Exception("Failed to create window surface");
			Surface = new VkSurfaceKHR((ulong)handle.ToInt64());
		}

		private void SelectPhysicalDevice()
		{
			var physicalDevices = Vulkan.vkEnumeratePhysicalDevices(Instance);

			foreach (VkPhysicalDevice physicalDevice in physicalDevices)
			{
				if (IsDeviceSuitable(physicalDevice, Surface))
				{
					PhysicalDevice = physicalDevice;
					Vulkan.vkGetPhysicalDeviceProperties(physicalDevice, out VkPhysicalDeviceProperties properties);
					Log.Debug("Selected {gpu}", properties.GetDeviceName());

					break;
				}
			}

			static bool IsDeviceSuitable(VkPhysicalDevice device, VkSurfaceKHR surface)
			{
				Vulkan.vkGetPhysicalDeviceProperties(device, out VkPhysicalDeviceProperties properties);
				Vulkan.vkGetPhysicalDeviceFeatures(device, out VkPhysicalDeviceFeatures features);

				QueueFamilyIndices indices = FindQueueFamilies(device, surface);

				bool extensionsSupported = CheckDeviceExtensionSupport(device);
				SwapChainSupportDetails details = QuerySwapchainSupport(device, surface);

				return indices.IsComplete && extensionsSupported && !details.formats.IsEmpty && !details.presentModes.IsEmpty && features.samplerAnisotropy;
			}
		}

		private unsafe void CreateLogicalDevice()
		{
			QueueFamilyIndices indices = FindQueueFamilies(PhysicalDevice, Surface);

			List<uint> uniqueQueueFamilies = new List<uint> { indices.graphics.Value, indices.present.Value };
			VkDeviceQueueCreateInfo[] queueCreateInfos = new VkDeviceQueueCreateInfo[uniqueQueueFamilies.Count];

			float queuePriority = 1f;
			int i = 0;
			foreach (uint queueFamily in uniqueQueueFamilies)
			{
				VkDeviceQueueCreateInfo queueCreateInfo = new VkDeviceQueueCreateInfo
				{
					sType = VkStructureType.DeviceQueueCreateInfo,
					queueFamilyIndex = queueFamily,
					queueCount = 1,
					pQueuePriorities = &queuePriority
				};
				queueCreateInfos[i++] = queueCreateInfo;
			}

			VkPhysicalDeviceFeatures deviceFeatures = new VkPhysicalDeviceFeatures
			{
				samplerAnisotropy = true
			};

			VkStringArray layers = GetRequiredLayers();
			VkDeviceCreateInfo deviceCreateInfo = new VkDeviceCreateInfo
			{
				sType = VkStructureType.DeviceCreateInfo,
				pEnabledFeatures = &deviceFeatures,
				ppEnabledExtensionNames = DeviceExtensions,
				enabledExtensionCount = DeviceExtensions.Length,
				enabledLayerCount = layers.Length,
				ppEnabledLayerNames = layers
			};

			fixed (VkDeviceQueueCreateInfo* ptr = queueCreateInfos)
			{
				deviceCreateInfo.pQueueCreateInfos = ptr;
				deviceCreateInfo.queueCreateInfoCount = (uint)queueCreateInfos.Length;
			}

			Vulkan.vkCreateDevice(PhysicalDevice, &deviceCreateInfo, null, out LogicalDevice).CheckResult();

			Vulkan.vkGetDeviceQueue(LogicalDevice, indices.graphics.Value, 0, out GraphicsQueue);
			Vulkan.vkGetDeviceQueue(LogicalDevice, indices.present.Value, 0, out PresentQueue);
		}

		private VkStringArray GetRequiredLayers()
		{
			List<string> layers = new List<string> { "VK_LAYER_LUNARG_monitor" };

			if (ValidationEnabled) layers.Add("VK_LAYER_KHRONOS_validation");

			return new VkStringArray(layers);
		}

		// All a part of graphics pipeline
		private unsafe void CreateDescriptorSetLayout()
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

			Vulkan.vkCreateDescriptorSetLayout(LogicalDevice, &layoutCreateInfo, null, out DescriptorSetLayout).CheckResult();
		}

		private unsafe void CreateDescriptorPool()
		{
			VkDescriptorPoolSize[] poolSizes =
			{
				new VkDescriptorPoolSize
				{
					type = VkDescriptorType.UniformBuffer,
					descriptorCount = (uint)UniformBuffers.Length
				},
				new VkDescriptorPoolSize
				{
					type = VkDescriptorType.CombinedImageSampler,
					descriptorCount = (uint)UniformBuffers.Length
				}
			};

			VkDescriptorPoolCreateInfo poolCreateInfo = new VkDescriptorPoolCreateInfo
			{
				sType = VkStructureType.DescriptorPoolCreateInfo,
				poolSizeCount = (uint)poolSizes.Length,
				maxSets = (uint)UniformBuffers.Length
			};
			fixed (VkDescriptorPoolSize* ptr = poolSizes) poolCreateInfo.pPoolSizes = ptr;

			Vulkan.vkCreateDescriptorPool(LogicalDevice, &poolCreateInfo, null, out DescriptorPool).CheckResult();
		}

		private unsafe void CreateDescriptorSets()
		{
			VkDescriptorSetLayout[] layouts = Enumerable.Repeat(DescriptorSetLayout, (int)Swapchain.Length).ToArray();

			VkDescriptorSetAllocateInfo allocateInfo = new VkDescriptorSetAllocateInfo
			{
				sType = VkStructureType.DescriptorSetAllocateInfo,
				descriptorPool = DescriptorPool,
				descriptorSetCount = Swapchain.Length
			};
			fixed (VkDescriptorSetLayout* ptr = layouts) allocateInfo.pSetLayouts = ptr;

			DescriptorSets = new VkDescriptorSet[Swapchain.Length];
			fixed (VkDescriptorSet* ptr = DescriptorSets) Vulkan.vkAllocateDescriptorSets(LogicalDevice, &allocateInfo, ptr).CheckResult();

			for (int i = 0; i < DescriptorSets.Length; i++)
			{
				VkDescriptorBufferInfo bufferInfo = new VkDescriptorBufferInfo
				{
					buffer = UniformBuffers[i].Buffer,
					offset = 0,
					range = UniformBuffers[i].Size
				};

				VkDescriptorImageInfo imageInfo = new VkDescriptorImageInfo
				{
					imageLayout = VkImageLayout.ShaderReadOnlyOptimal,
					imageView = Texture.View,
					sampler = Texture.Sampler
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

				Vulkan.vkUpdateDescriptorSets(LogicalDevice, writeDescriptorSets);
			}
		}

		private unsafe void CreateGraphicsPipeline()
		{
			Shader shader = new Shader(this, "Assets/test.vert.spv", "Assets/test.frag.spv");

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

			VkViewport viewport = new VkViewport(0, 0, Swapchain.Extent.width, Swapchain.Extent.height, 0f, 1f);
			VkRect2D scissor = new VkRect2D(0, 0, Swapchain.Extent.width, Swapchain.Extent.height);

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

			Vulkan.vkCreatePipelineLayout(LogicalDevice, &pipelineLayoutCreateInfo, null, out PipelineLayout).CheckResult();

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
				renderPass = RenderPass.RenderPass,
				subpass = 0,
				basePipelineHandle = VkPipeline.Null,
				basePipelineIndex = -1,
				stageCount = (uint)shader.Stages.Length
			};

			fixed (VkPipelineShaderStageCreateInfo* ptr = shader.Stages) pipelineCreateInfo.pStages = ptr;

			Vulkan.vkCreateGraphicsPipeline(LogicalDevice, VkPipelineCache.Null, pipelineCreateInfo, out GraphicsPipeline).CheckResult();

			shader.Dispose();
		}
		// ---------------

		private unsafe void CreateFramebuffers()
		{
			DepthBuffer = new DepthBuffer(this, window.Size);

			SwapchainFramebuffers = new VkFramebuffer[Swapchain.Length];

			for (int i = 0; i < Swapchain.Length; i++)
			{
				VkImageView[] attachments = { Swapchain.ImageViews[i], DepthBuffer.View };

				VkFramebufferCreateInfo framebufferCreateInfo = new VkFramebufferCreateInfo
				{
					sType = VkStructureType.FramebufferCreateInfo,
					renderPass = RenderPass.RenderPass,
					attachmentCount = (uint)attachments.Length,
					width = Swapchain.Extent.width,
					height = Swapchain.Extent.height,
					layers = 1
				};
				fixed (VkImageView* ptr = attachments) framebufferCreateInfo.pAttachments = ptr;

				Vulkan.vkCreateFramebuffer(LogicalDevice, &framebufferCreateInfo, null, out SwapchainFramebuffers[i]).CheckResult();
			}
		}

		private unsafe void CreateCommandPool()
		{
			QueueFamilyIndices indices = FindQueueFamilies(PhysicalDevice, Surface);

			VkCommandPoolCreateInfo commandPoolCreateInfo = new VkCommandPoolCreateInfo
			{
				sType = VkStructureType.CommandPoolCreateInfo,
				queueFamilyIndex = indices.graphics.Value,
				flags = VkCommandPoolCreateFlags.ResetCommandBuffer
			};

			Vulkan.vkCreateCommandPool(LogicalDevice, &commandPoolCreateInfo, null, out CommandPool).CheckResult();
		}

		private unsafe void CreateSyncObjects()
		{
			ImageAvailableSemaphores = new VkSemaphore[MaxFramesInFlight];
			RenderFinishedSemaphores = new VkSemaphore[MaxFramesInFlight];
			InFlightFences = new VkFence[MaxFramesInFlight];
			ImagesInFlight = Enumerable.Repeat(VkFence.Null, (int)Swapchain.Length).ToArray();

			VkSemaphoreCreateInfo semaphoreCreateInfo = new VkSemaphoreCreateInfo { sType = VkStructureType.SemaphoreCreateInfo };
			VkFenceCreateInfo fenceCreateInfo = new VkFenceCreateInfo
			{
				sType = VkStructureType.FenceCreateInfo,
				flags = VkFenceCreateFlags.Signaled
			};

			for (int i = 0; i < MaxFramesInFlight; i++)
			{
				Vulkan.vkCreateSemaphore(LogicalDevice, &semaphoreCreateInfo, null, out ImageAvailableSemaphores[i]).CheckResult();
				Vulkan.vkCreateSemaphore(LogicalDevice, &semaphoreCreateInfo, null, out RenderFinishedSemaphores[i]).CheckResult();
				Vulkan.vkCreateFence(LogicalDevice, &fenceCreateInfo, null, out InFlightFences[i]).CheckResult();
			}
		}

		private unsafe void CleanupSwapchain()
		{
			DepthBuffer.Dispose();

			foreach (VkFramebuffer framebuffer in SwapchainFramebuffers) Vulkan.vkDestroyFramebuffer(LogicalDevice, framebuffer, null);

			Vulkan.vkDestroyDescriptorPool(LogicalDevice, DescriptorPool, null);

			Vulkan.vkDestroyPipeline(LogicalDevice, GraphicsPipeline, null);
			Vulkan.vkDestroyPipelineLayout(LogicalDevice, PipelineLayout, null);

			foreach (UniformBuffer<UniformBufferObject> buffer in UniformBuffers) buffer.Dispose();
		}

		private void RecreateSwapchain()
		{
			while (window.Size.Width == 0 || window.Size.Height == 0) Glfw.WaitEvents();

			Vulkan.vkDeviceWaitIdle(LogicalDevice);

			CleanupSwapchain();

			Swapchain.Invalidate();

			RenderPass.Invalidate();

			CreateGraphicsPipeline();
			CreateFramebuffers();

			UniformBuffers = new UniformBuffer<UniformBufferObject>[Swapchain.Length];
			for (int i = 0; i < UniformBuffers.Length; i++) UniformBuffers[i] = new UniformBuffer<UniformBufferObject>(this);

			CreateDescriptorPool();
			CreateDescriptorSets();
		}

		public unsafe VkCommandBuffer Begin(Color4 color, uint index)
		{
			VkCommandBufferAllocateInfo allocateInfo = new VkCommandBufferAllocateInfo
			{
				sType = VkStructureType.CommandBufferAllocateInfo,
				commandPool = CommandPool,
				level = VkCommandBufferLevel.Primary,
				commandBufferCount = 1
			};

			Vulkan.vkAllocateCommandBuffers(LogicalDevice, &allocateInfo, out VkCommandBuffer buffer);

			VkCommandBufferBeginInfo beginInfo = new VkCommandBufferBeginInfo
			{
				sType = VkStructureType.CommandBufferBeginInfo,
				flags = VkCommandBufferUsageFlags.SimultaneousUse,
				pInheritanceInfo = null
			};

			Vulkan.vkBeginCommandBuffer(buffer, &beginInfo).CheckResult();

			VkClearValue[] clearValues = new VkClearValue[2];

			clearValues[0].color = new VkClearColorValue(color.R, color.G, color.B, color.A);
			clearValues[1].depthStencil = new VkClearDepthStencilValue(1f, 0);

			VkRenderPassBeginInfo renderPassBeginInfo = new VkRenderPassBeginInfo
			{
				sType = VkStructureType.RenderPassBeginInfo,
				renderPass = RenderPass.RenderPass,
				framebuffer = SwapchainFramebuffers[index],
				renderArea = new VkRect2D(0, 0, Swapchain.Extent.width, Swapchain.Extent.height),
				clearValueCount = (uint)clearValues.Length
			};
			fixed (VkClearValue* ptr = clearValues) renderPassBeginInfo.pClearValues = ptr;

			Vulkan.vkCmdBeginRenderPass(buffer, &renderPassBeginInfo, VkSubpassContents.Inline);

			Vulkan.vkCmdBindPipeline(buffer, VkPipelineBindPoint.Graphics, GraphicsPipeline);

			return buffer;
		}

		public void End(VkCommandBuffer buffer)
		{
			Vulkan.vkCmdEndRenderPass(buffer);

			Vulkan.vkEndCommandBuffer(buffer);
		}

		private uint begunFrameIndex;

		internal void BeginFrame()
		{
			Vulkan.vkWaitForFences(LogicalDevice, InFlightFences[currentFrame], true, ulong.MaxValue);

			VkResult result = Vulkan.vkAcquireNextImageKHR(LogicalDevice, Swapchain.Swapchain, uint.MaxValue, ImageAvailableSemaphores[currentFrame], VkFence.Null, out begunFrameIndex);
			if (result == VkResult.ErrorOutOfDateKHR)
			{
				RecreateSwapchain();
				return;
			}

			if (result != VkResult.Success && result != VkResult.SuboptimalKHR) throw new Exception("Failed to acquire swapchain image");

			if (ImagesInFlight[begunFrameIndex] != VkFence.Null) Vulkan.vkWaitForFences(LogicalDevice, ImagesInFlight[begunFrameIndex], true, uint.MaxValue);

			ImagesInFlight[begunFrameIndex] = InFlightFences[currentFrame];

			Renderer2D.imageIndex = begunFrameIndex;
		}

		public unsafe void EndFrame()
		{
			UpdateUniformBuffer(begunFrameIndex);

			VkSemaphore[] waitSemaphores = { ImageAvailableSemaphores[currentFrame] };
			VkPipelineStageFlags[] waitStages = { VkPipelineStageFlags.ColorAttachmentOutput };

			VkSubmitInfo submitInfo = new VkSubmitInfo
			{
				sType = VkStructureType.SubmitInfo,
				waitSemaphoreCount = 1,
				commandBufferCount = 1,
				signalSemaphoreCount = 1
			};
			fixed (VkSemaphore* ptr = waitSemaphores) submitInfo.pWaitSemaphores = ptr;
			fixed (VkPipelineStageFlags* ptr = waitStages) submitInfo.pWaitDstStageMask = ptr;
			fixed (VkCommandBuffer* ptr = &Renderer2D.buffer) submitInfo.pCommandBuffers = ptr;

			VkSemaphore[] signalSemaphores = { RenderFinishedSemaphores[currentFrame] };
			fixed (VkSemaphore* ptr = signalSemaphores) submitInfo.pSignalSemaphores = ptr;

			Vulkan.vkResetFences(LogicalDevice, InFlightFences[currentFrame]);

			Vulkan.vkQueueSubmit(GraphicsQueue, 1, &submitInfo, InFlightFences[currentFrame]).CheckResult();

			VkPresentInfoKHR presentInfo = new VkPresentInfoKHR
			{
				sType = VkStructureType.PresentInfoKHR,
				waitSemaphoreCount = 1,
				swapchainCount = 1,
				pResults = null
			};
			fixed (uint* ptr = &begunFrameIndex) presentInfo.pImageIndices = ptr;
			fixed (VkSemaphore* ptr = signalSemaphores) presentInfo.pWaitSemaphores = ptr;

			VkSwapchainKHR[] swapchains = { Swapchain.Swapchain };
			fixed (VkSwapchainKHR* ptr = swapchains) presentInfo.pSwapchains = ptr;

			VkResult result = Vulkan.vkQueuePresentKHR(PresentQueue, &presentInfo);

			if (result == VkResult.ErrorOutOfDateKHR || result == VkResult.SuboptimalKHR || framebufferResized)
			{
				framebufferResized = false;
				RecreateSwapchain();
			}
			else if (result != VkResult.Success) throw new Exception("Failed to present swapchain image");

			currentFrame = (currentFrame + 1) % MaxFramesInFlight;
		}

		private void UpdateUniformBuffer(uint index)
		{
			UniformBufferObject ubo = new UniformBufferObject
			{
				Model = Matrix4x4.Identity,
				View = Matrix4x4.Identity,
				Projection = Matrix4x4.CreateOrthographic(Swapchain.Extent.width, Swapchain.Extent.height, -1f, 1f)
			};

			// ubo.Projection = Matrix4x4.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), SwapchainExtent.Width / (float)SwapchainExtent.Height, 0.1f, 100f);
			// ubo.Projection.M22 *= -1;

			UniformBuffers[index].SetData(ubo);
		}

		public unsafe void Dispose()
		{
			Vulkan.vkDeviceWaitIdle(LogicalDevice);

			CleanupSwapchain();

			RenderPass.Dispose();
			Swapchain.Dispose();

			Vulkan.vkDestroyDescriptorSetLayout(LogicalDevice, DescriptorSetLayout, null);

			Texture.Dispose();

			for (int i = 0; i < MaxFramesInFlight; i++)
			{
				Vulkan.vkDestroySemaphore(LogicalDevice, RenderFinishedSemaphores[i], null);
				Vulkan.vkDestroySemaphore(LogicalDevice, ImageAvailableSemaphores[i], null);
				Vulkan.vkDestroyFence(LogicalDevice, InFlightFences[i], null);
			}

			Vulkan.vkDestroyCommandPool(LogicalDevice, CommandPool, null);

			Vulkan.vkDestroySurfaceKHR(Instance, Surface, null);

			if (ValidationEnabled) Vulkan.vkDestroyDebugUtilsMessengerEXT(Instance, debugMessenger, null);

			Vulkan.vkDestroyDevice(LogicalDevice, null);
			Vulkan.vkDestroyInstance(Instance, null);
		}

		#region Helper Methods

		internal static SwapChainSupportDetails QuerySwapchainSupport(VkPhysicalDevice device, VkSurfaceKHR surface)
		{
			SwapChainSupportDetails details = new SwapChainSupportDetails();

			Vulkan.vkGetPhysicalDeviceSurfaceCapabilitiesKHR(device, surface, out details.capabilities);

			details.formats = Vulkan.vkGetPhysicalDeviceSurfaceFormatsKHR(device, surface);
			details.presentModes = Vulkan.vkGetPhysicalDeviceSurfacePresentModesKHR(device, surface);

			return details;
		}

		private static bool CheckDeviceExtensionSupport(VkPhysicalDevice device)
		{
			var extensions = Vulkan.vkEnumerateDeviceExtensionProperties(device).ToArray().Select(property => property.GetExtensionName()).ToList();

			bool containsAll = true;
			for (int i = 0; i < DeviceExtensions.Length; i++) containsAll &= extensions.Contains(DeviceExtensions[i]);
			return containsAll;
		}

		private VkStringArray GetRequiredExtensions()
		{
			List<string> extensions = GLFW.Vulkan.GetRequiredInstanceExtensions().ToList();

			if (ValidationEnabled) extensions.Add(Vulkan.EXTDebugUtilsExtensionName);

			return new VkStringArray(extensions);
		}

		internal static QueueFamilyIndices FindQueueFamilies(VkPhysicalDevice device, VkSurfaceKHR surface)
		{
			QueueFamilyIndices indices = new QueueFamilyIndices();

			var properties = Vulkan.vkGetPhysicalDeviceQueueFamilyProperties(device);

			uint i = 0;
			foreach (VkQueueFamilyProperties property in properties)
			{
				if ((property.queueFlags & VkQueueFlags.Graphics) != 0)
				{
					indices.graphics = i;

					Vulkan.vkGetPhysicalDeviceSurfaceSupportKHR(device, i, surface, out VkBool32 presentSupport);
					if (presentSupport) indices.present = i;
				}

				if (indices.IsComplete) break;

				i++;
			}


			return indices;
		}

		#endregion
	}
}