using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using GLFW;
using Vortice.Mathematics;
using Vortice.Vulkan;
using Color = System.Drawing.Color;
using Exception = System.Exception;
using Vulkan = Vortice.Vulkan.Vulkan;

namespace Proxima.Graphics
{
	public sealed partial class GraphicsDevice : IDisposable
	{
		private struct QueueFamilyIndices
		{
			public uint? graphics;
			public uint? present;

			public bool IsComplete => graphics.HasValue && present.HasValue;
		}

		private ref struct SwapChainSupportDetails
		{
			public VkSurfaceCapabilitiesKHR capabilities;
			public ReadOnlySpan<VkSurfaceFormatKHR> formats;
			public ReadOnlySpan<VkPresentModeKHR> presentModes;
		}

		private static readonly VkStringArray DeviceExtensions = new VkStringArray(new[] { Vulkan.KHRSwapchainExtensionName });

		private NativeWindow window;

		private VkInstance Instance;
		internal VkPhysicalDevice PhysicalDevice;
		internal VkDevice LogicalDevice;

		internal VkQueue GraphicsQueue;
		private VkQueue PresentQueue;

		private VkSurfaceKHR Surface;
		private VkSwapchainKHR Swapchain;
		private VkFormat SwapchainFormat;
		private Size SwapchainExtent;
		private VkImage[] SwapchainImages;
		private VkImageView[] SwapchainImageViews;

		private VkRenderPass RenderPass;
		private VkDescriptorSetLayout DescriptorSetLayout;
		private VkPipelineLayout PipelineLayout;
		private VkPipeline GraphicsPipeline;

		private VkFramebuffer[] SwapchainFramebuffers;
		internal VkCommandPool CommandPool;
		private VkCommandBuffer[] CommandBuffers;

		private const int MaxFramesInFlight = 2;
		private VkSemaphore[] RenderFinishedSemaphores;
		private VkSemaphore[] ImageAvailableSemaphores;
		private VkFence[] InFlightFences;
		private VkFence[] ImagesInFlight;

		private int currentFrame;
		private bool framebufferResized;

		private struct Vertex
		{
			private Vector2 position;
			private Color4 color;
			private Vector2 uv;

			public Vertex(float x, float y, float r, float g, float b, float u, float v)
			{
				position = new Vector2(x, y);
				color = new Color4(r, g, b);
				uv=new Vector2(u,v);
			}

			public static unsafe VkVertexInputBindingDescription GetBindingDescription()
			{
				VkVertexInputBindingDescription description = new VkVertexInputBindingDescription
				{
					binding = 0,
					stride = (uint)sizeof(Vertex),
					inputRate = VkVertexInputRate.Vertex
				};
				return description;
			}

			public static unsafe VkVertexInputAttributeDescription[] GetAttributeDescriptions()
			{
				VkVertexInputAttributeDescription[] descriptions = new VkVertexInputAttributeDescription[3];

				ref VkVertexInputAttributeDescription description = ref descriptions[0];
				description.binding = 0;
				description.location = 0;
				description.format = VkFormat.R32G32SFloat;
				description.offset = 0;

				description = ref descriptions[1];
				description.binding = 0;
				description.location = 1;
				description.format = VkFormat.R32G32B32A32SFloat;
				description.offset = (uint)sizeof(Vector2);
				
				description = ref descriptions[2];
				description.binding = 0;
				description.location = 2;
				description.format = VkFormat.R32G32SFloat;
				description.offset = (uint)(sizeof(Vector2) + sizeof(Color4));

				return descriptions;
			}
		}

		private VertexBuffer<Vertex> VertexBuffer;
		private IndexBuffer<ushort> IndexBuffer;
		private UniformBuffer<UniformBufferObject>[] UniformBuffers;
		private Texture2D Texture;

		private VkDescriptorPool DescriptorPool;
		private VkDescriptorSet[] DescriptorSets;

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

			CreateSwapchain();

			CreateImageViews();

			CreateRenderPass();

			CreateDescriptorSetLayout();

			CreateGraphicsPipeline();

			CreateFramebuffers();

			CreateCommandPool();

			VertexBuffer = new VertexBuffer<Vertex>(this, new[]
			{
				new Vertex(-0.5f, -0.5f, 1f, 0f, 0f,0f,0f),
				new Vertex(0.5f, -0.5f, 0f, 1f, 0f, 1f,0f),
				new Vertex(0.5f, 0.5f, 0f, 0f, 1f,1f,1f),
				new Vertex(-0.5f, 0.5f, 1f, 1f, 1f,0f,1f)
			});

			IndexBuffer = new IndexBuffer<ushort>(this, new ushort[] { 0, 1, 2, 2, 3, 0 });

			UniformBuffers = new UniformBuffer<UniformBufferObject>[SwapchainImages.Length];
			for (int i = 0; i < UniformBuffers.Length; i++) UniformBuffers[i] = new UniformBuffer<UniformBufferObject>(this);

			Texture = new Texture2D(this, "Assets/Tom.png");

			CreateDescriptorPool();
			CreateDescriptorSets();

			CreateCommandBuffers();

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

			using VkStringArray vkInstanceExtensions = GetRequiredExtensions();

			VkInstanceCreateInfo createInfo = new VkInstanceCreateInfo
			{
				sType = VkStructureType.InstanceCreateInfo,
				pApplicationInfo = &appInfo,
				enabledExtensionCount = vkInstanceExtensions.Length,
				ppEnabledExtensionNames = vkInstanceExtensions
			};

			if (ValidationEnabled)
			{
				createInfo.enabledLayerCount = ValidationLayers.Length;
				createInfo.ppEnabledLayerNames = ValidationLayers;
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

			VkDeviceCreateInfo deviceCreateInfo = new VkDeviceCreateInfo
			{
				sType = VkStructureType.DeviceCreateInfo,
				pEnabledFeatures = &deviceFeatures,
				ppEnabledExtensionNames = DeviceExtensions,
				enabledExtensionCount = DeviceExtensions.Length
			};

			fixed (VkDeviceQueueCreateInfo* ptr = &queueCreateInfos[0])
			{
				deviceCreateInfo.pQueueCreateInfos = ptr;
				deviceCreateInfo.queueCreateInfoCount = (uint)queueCreateInfos.Length;
			}

			if (ValidationEnabled)
			{
				deviceCreateInfo.enabledLayerCount = ValidationLayers.Length;
				deviceCreateInfo.ppEnabledLayerNames = ValidationLayers;
			}

			Vulkan.vkCreateDevice(PhysicalDevice, &deviceCreateInfo, null, out LogicalDevice).CheckResult();

			Vulkan.vkGetDeviceQueue(LogicalDevice, indices.graphics.Value, 0, out GraphicsQueue);
			Vulkan.vkGetDeviceQueue(LogicalDevice, indices.present.Value, 0, out PresentQueue);
		}

		private unsafe void CreateSwapchain()
		{
			SwapChainSupportDetails details = QuerySwapchainSupport(PhysicalDevice, Surface);

			VkSurfaceFormatKHR surfaceFormat = SelectSwapSurfaceFormat(details.formats);
			VkPresentModeKHR presentMode = SelectSwapPresentMode(details.presentModes);
			Size extent = SelectSwapExtent(details.capabilities, window);

			uint imageCount = details.capabilities.minImageCount + 1;
			if (details.capabilities.maxImageCount > 0 && imageCount > details.capabilities.maxImageCount) imageCount = details.capabilities.maxImageCount;

			VkSwapchainCreateInfoKHR createInfo = new VkSwapchainCreateInfoKHR
			{
				sType = VkStructureType.SwapchainCreateInfoKHR,
				surface = Surface,
				minImageCount = imageCount,
				imageFormat = surfaceFormat.format,
				imageColorSpace = surfaceFormat.colorSpace,
				imageExtent = extent,
				imageArrayLayers = 1,
				imageUsage = VkImageUsageFlags.ColorAttachment,
				preTransform = details.capabilities.currentTransform,
				compositeAlpha = VkCompositeAlphaFlagsKHR.Opaque,
				presentMode = presentMode,
				clipped = true,
				oldSwapchain = VkSwapchainKHR.Null
			};

			QueueFamilyIndices indices = FindQueueFamilies(PhysicalDevice, Surface);
			uint[] queueFamilyIndices = { indices.graphics.Value, indices.present.Value };

			if (indices.graphics.Value != indices.present.Value)
			{
				createInfo.imageSharingMode = VkSharingMode.Concurrent;
				createInfo.queueFamilyIndexCount = 2;
				fixed (uint* ptr = &queueFamilyIndices[0]) createInfo.pQueueFamilyIndices = ptr;
			}
			else
			{
				createInfo.imageSharingMode = VkSharingMode.Exclusive;
			}

			Vulkan.vkCreateSwapchainKHR(LogicalDevice, &createInfo, null, out Swapchain).CheckResult();

			SwapchainImages = Vulkan.vkGetSwapchainImagesKHR(LogicalDevice, Swapchain).ToArray();

			SwapchainFormat = surfaceFormat.format;
			SwapchainExtent = extent;

			#region Static

			static VkSurfaceFormatKHR SelectSwapSurfaceFormat(ReadOnlySpan<VkSurfaceFormatKHR> formats)
			{
				foreach (VkSurfaceFormatKHR format in formats)
				{
					if (format.format == VkFormat.B8G8R8A8SRgb && format.colorSpace == VkColorSpaceKHR.SrgbNonLinear) return format;
				}

				return formats[0];
			}

			static VkPresentModeKHR SelectSwapPresentMode(ReadOnlySpan<VkPresentModeKHR> presentModes)
			{
				foreach (VkPresentModeKHR presentMode in presentModes)
				{
					if (presentMode == VkPresentModeKHR.Mailbox) return presentMode;
				}

				return VkPresentModeKHR.Fifo;
			}

			static Size SelectSwapExtent(VkSurfaceCapabilitiesKHR capabilities, NativeWindow window)
			{
				if (capabilities.currentExtent.Width != int.MaxValue) return capabilities.currentExtent;

				Size actualExtent = window.Size;

				actualExtent.Width = Math.Clamp(actualExtent.Width, capabilities.minImageExtent.Width, capabilities.maxImageExtent.Width);
				actualExtent.Height = Math.Clamp(actualExtent.Height, capabilities.minImageExtent.Height, capabilities.maxImageExtent.Height);

				return actualExtent;
			}

			#endregion
		}

		private unsafe void CreateImageViews()
		{
			SwapchainImageViews = new VkImageView[SwapchainImages.Length];

			for (int i = 0; i < SwapchainImages.Length; i++)
			{
				VkImageViewCreateInfo createInfo = new VkImageViewCreateInfo
				{
					sType = VkStructureType.ImageViewCreateInfo,
					image = SwapchainImages[i],
					viewType = VkImageViewType.Image2D,
					format = SwapchainFormat,
					components = VkComponentMapping.Rgba,
					subresourceRange = new VkImageSubresourceRange
					{
						aspectMask = VkImageAspectFlags.Color,
						baseMipLevel = 0,
						levelCount = 1,
						baseArrayLayer = 0,
						layerCount = 1
					}
				};

				Vulkan.vkCreateImageView(LogicalDevice, &createInfo, null, out SwapchainImageViews[i]).CheckResult();
			}
		}

		private unsafe void CreateRenderPass()
		{
			VkAttachmentDescription colorAttachment = new VkAttachmentDescription
			{
				format = SwapchainFormat,
				samples = VkSampleCountFlags.Count1,
				loadOp = VkAttachmentLoadOp.Clear,
				storeOp = VkAttachmentStoreOp.Store,
				stencilLoadOp = VkAttachmentLoadOp.DontCare,
				stencilStoreOp = VkAttachmentStoreOp.DontCare,
				initialLayout = VkImageLayout.Undefined,
				finalLayout = VkImageLayout.PresentSrcKHR
			};

			VkAttachmentReference colorAttachmentRef = new VkAttachmentReference
			{
				attachment = 0,
				layout = VkImageLayout.ColorAttachmentOptimal
			};

			VkSubpassDescription subpass = new VkSubpassDescription
			{
				pipelineBindPoint = VkPipelineBindPoint.Graphics,
				colorAttachmentCount = 1,
				pColorAttachments = &colorAttachmentRef
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

			VkRenderPassCreateInfo renderPassCreateInfo = new VkRenderPassCreateInfo
			{
				sType = VkStructureType.RenderPassCreateInfo,
				attachmentCount = 1,
				pAttachments = &colorAttachment,
				subpassCount = 1,
				pSubpasses = &subpass,
				dependencyCount = 1,
				pDependencies = &dependency
			};

			Vulkan.vkCreateRenderPass(LogicalDevice, &renderPassCreateInfo, null, out RenderPass).CheckResult();
		}

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
			fixed (VkDescriptorSetLayoutBinding* ptr = &bindings[0]) layoutCreateInfo.pBindings = ptr;

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
			fixed (VkDescriptorPoolSize* ptr = &poolSizes[0]) poolCreateInfo.pPoolSizes = ptr;

			Vulkan.vkCreateDescriptorPool(LogicalDevice, &poolCreateInfo, null, out DescriptorPool).CheckResult();
		}

		private unsafe void CreateDescriptorSets()
		{
			VkDescriptorSetLayout[] layouts = Enumerable.Repeat(DescriptorSetLayout, SwapchainImages.Length).ToArray();

			VkDescriptorSetAllocateInfo allocateInfo = new VkDescriptorSetAllocateInfo
			{
				sType = VkStructureType.DescriptorSetAllocateInfo,
				descriptorPool = DescriptorPool,
				descriptorSetCount = (uint)SwapchainImages.Length
			};
			fixed (VkDescriptorSetLayout* ptr = &layouts[0]) allocateInfo.pSetLayouts = ptr;

			DescriptorSets = new VkDescriptorSet[SwapchainImages.Length];
			fixed (VkDescriptorSet* ptr = &DescriptorSets[0]) Vulkan.vkAllocateDescriptorSets(LogicalDevice, &allocateInfo, ptr).CheckResult();

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

				VkWriteDescriptorSet[] writeDescriptorSets = new[]
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
			var vertex = File.ReadAllBytes("Assets/vert.spv");
			var fragment = File.ReadAllBytes("Assets/frag.spv");

			VkShaderModule vertexShaderModule = CreateShaderModule(LogicalDevice, vertex);
			VkShaderModule fragmentShaderModule = CreateShaderModule(LogicalDevice, fragment);

			VkPipelineShaderStageCreateInfo vertexCreateInfo = new VkPipelineShaderStageCreateInfo
			{
				sType = VkStructureType.PipelineShaderStageCreateInfo,
				stage = VkShaderStageFlags.Vertex,
				module = vertexShaderModule,
				pName = new VkString("main")
			};

			VkPipelineShaderStageCreateInfo fragmentCreateInfo = new VkPipelineShaderStageCreateInfo
			{
				sType = VkStructureType.PipelineShaderStageCreateInfo,
				stage = VkShaderStageFlags.Fragment,
				module = fragmentShaderModule,
				pName = new VkString("main")
			};

			VkPipelineShaderStageCreateInfo[] shaderStages = { vertexCreateInfo, fragmentCreateInfo };

			var bindingDescription = Vertex.GetBindingDescription();
			var attributeDescriptions = Vertex.GetAttributeDescriptions();

			VkPipelineVertexInputStateCreateInfo vertexInputInfo = new VkPipelineVertexInputStateCreateInfo
			{
				sType = VkStructureType.PipelineVertexInputStateCreateInfo,
				vertexBindingDescriptionCount = 1,
				pVertexBindingDescriptions = &bindingDescription,
				vertexAttributeDescriptionCount = (uint)attributeDescriptions.Length
			};
			fixed (VkVertexInputAttributeDescription* ptr = &attributeDescriptions[0]) vertexInputInfo.pVertexAttributeDescriptions = ptr;

			VkPipelineInputAssemblyStateCreateInfo inputAssembly = new VkPipelineInputAssemblyStateCreateInfo
			{
				sType = VkStructureType.PipelineInputAssemblyStateCreateInfo,
				topology = VkPrimitiveTopology.TriangleList,
				primitiveRestartEnable = false
			};

			Viewport viewport = new Viewport(0, 0, SwapchainExtent.Width, SwapchainExtent.Height, 0f, 1f);
			Rectangle scissor = new Rectangle(0, 0, SwapchainExtent.Width, SwapchainExtent.Height);

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
				frontFace = VkFrontFace.CounterClockwise,
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

			fixed (VkDynamicState* ptr = &dynamicStates[0]) dynamicState.pDynamicStates = ptr;

			VkPipelineLayoutCreateInfo pipelineLayoutCreateInfo = new VkPipelineLayoutCreateInfo
			{
				sType = VkStructureType.PipelineLayoutCreateInfo,
				setLayoutCount = 1,
				pushConstantRangeCount = 0,
				pPushConstantRanges = null
			};
			fixed (VkDescriptorSetLayout* ptr = &DescriptorSetLayout) pipelineLayoutCreateInfo.pSetLayouts = ptr;

			Vulkan.vkCreatePipelineLayout(LogicalDevice, &pipelineLayoutCreateInfo, null, out PipelineLayout).CheckResult();

			VkGraphicsPipelineCreateInfo pipelineCreateInfo = new VkGraphicsPipelineCreateInfo
			{
				sType = VkStructureType.GraphicsPipelineCreateInfo,
				stageCount = 2,
				pVertexInputState = &vertexInputInfo,
				pInputAssemblyState = &inputAssembly,
				pViewportState = &viewportState,
				pRasterizationState = &rasterizer,
				pMultisampleState = &multisampling,
				pDepthStencilState = null,
				pColorBlendState = &colorBlending,
				pDynamicState = null,
				layout = PipelineLayout,
				renderPass = RenderPass,
				subpass = 0,
				basePipelineHandle = VkPipeline.Null,
				basePipelineIndex = -1
			};

			fixed (VkPipelineShaderStageCreateInfo* ptr = &shaderStages[0]) pipelineCreateInfo.pStages = ptr;

			Vulkan.vkCreateGraphicsPipeline(LogicalDevice, VkPipelineCache.Null, pipelineCreateInfo, out GraphicsPipeline).CheckResult();

			Vulkan.vkDestroyShaderModule(LogicalDevice, vertexShaderModule, null);
			Vulkan.vkDestroyShaderModule(LogicalDevice, fragmentShaderModule, null);
		}

		private unsafe void CreateFramebuffers()
		{
			SwapchainFramebuffers = new VkFramebuffer[SwapchainImageViews.Length];

			for (int i = 0; i < SwapchainImageViews.Length; i++)
			{
				VkImageView[] attachments = { SwapchainImageViews[i] };

				VkFramebufferCreateInfo framebufferCreateInfo = new VkFramebufferCreateInfo
				{
					sType = VkStructureType.FramebufferCreateInfo,
					renderPass = RenderPass,
					attachmentCount = 1,
					width = (uint)SwapchainExtent.Width,
					height = (uint)SwapchainExtent.Height,
					layers = 1
				};
				fixed (VkImageView* ptr = &attachments[0]) framebufferCreateInfo.pAttachments = ptr;

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
				flags = 0
			};

			Vulkan.vkCreateCommandPool(LogicalDevice, &commandPoolCreateInfo, null, out CommandPool).CheckResult();
		}


		private unsafe void CreateCommandBuffers()
		{
			CommandBuffers = new VkCommandBuffer[SwapchainFramebuffers.Length];

			VkCommandBufferAllocateInfo allocateInfo = new VkCommandBufferAllocateInfo
			{
				sType = VkStructureType.CommandBufferAllocateInfo,
				commandPool = CommandPool,
				level = VkCommandBufferLevel.Primary,
				commandBufferCount = (uint)CommandBuffers.Length
			};

			Vulkan.vkAllocateCommandBuffers(LogicalDevice, &allocateInfo, out CommandBuffers[0]).CheckResult();

			for (int i = 0; i < CommandBuffers.Length; i++)
			{
				VkCommandBufferBeginInfo beginInfo = new VkCommandBufferBeginInfo
				{
					sType = VkStructureType.CommandBufferBeginInfo,
					flags = 0,
					pInheritanceInfo = null
				};

				Vulkan.vkBeginCommandBuffer(CommandBuffers[i], &beginInfo).CheckResult();

				Color4 clearColor = new Color4(Color.Black);

				VkRenderPassBeginInfo renderPassBeginInfo = new VkRenderPassBeginInfo
				{
					sType = VkStructureType.RenderPassBeginInfo,
					renderPass = RenderPass,
					framebuffer = SwapchainFramebuffers[i],
					renderArea = new Rectangle(0, 0, SwapchainExtent.Width, SwapchainExtent.Height),
					clearValueCount = 1,
					pClearValues = (VkClearValue*)&clearColor
				};

				Vulkan.vkCmdBeginRenderPass(CommandBuffers[i], &renderPassBeginInfo, VkSubpassContents.Inline);

				Vulkan.vkCmdBindPipeline(CommandBuffers[i], VkPipelineBindPoint.Graphics, GraphicsPipeline);

				Vulkan.vkCmdBindVertexBuffers(CommandBuffers[i], 0, VertexBuffer.Buffer);
				Vulkan.vkCmdBindIndexBuffer(CommandBuffers[i], IndexBuffer.Buffer, 0, IndexBuffer.IndexType);

				Vulkan.vkCmdBindDescriptorSets(CommandBuffers[i], VkPipelineBindPoint.Graphics, PipelineLayout, 0, DescriptorSets[i]);

				Vulkan.vkCmdDrawIndexed(CommandBuffers[i], IndexBuffer.Length, 1, 0, 0, 0);
				Vulkan.vkCmdEndRenderPass(CommandBuffers[i]);

				Vulkan.vkEndCommandBuffer(CommandBuffers[i]).CheckResult();
			}
		}

		private unsafe void CreateSyncObjects()
		{
			ImageAvailableSemaphores = new VkSemaphore[MaxFramesInFlight];
			RenderFinishedSemaphores = new VkSemaphore[MaxFramesInFlight];
			InFlightFences = new VkFence[MaxFramesInFlight];
			ImagesInFlight = Enumerable.Repeat(VkFence.Null, SwapchainImages.Length).ToArray();

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
			foreach (VkFramebuffer framebuffer in SwapchainFramebuffers) Vulkan.vkDestroyFramebuffer(LogicalDevice, framebuffer, null);

			fixed (VkCommandBuffer* ptr = &CommandBuffers[0]) Vulkan.vkFreeCommandBuffers(LogicalDevice, CommandPool, (uint)CommandBuffers.Length, ptr);

			Vulkan.vkDestroyDescriptorPool(LogicalDevice, DescriptorPool, null);

			Vulkan.vkDestroyPipeline(LogicalDevice, GraphicsPipeline, null);
			Vulkan.vkDestroyPipelineLayout(LogicalDevice, PipelineLayout, null);
			Vulkan.vkDestroyRenderPass(LogicalDevice, RenderPass, null);

			foreach (VkImageView imageView in SwapchainImageViews) Vulkan.vkDestroyImageView(LogicalDevice, imageView, null);

			foreach (UniformBuffer<UniformBufferObject> buffer in UniformBuffers) buffer.Dispose();

			Vulkan.vkDestroySwapchainKHR(LogicalDevice, Swapchain, null);
		}

		private void RecreateSwapchain()
		{
			while (window.Size.Width == 0 || window.Size.Height == 0) Glfw.WaitEvents();

			Vulkan.vkDeviceWaitIdle(LogicalDevice);

			CleanupSwapchain();

			CreateSwapchain();
			CreateImageViews();
			CreateRenderPass();
			CreateGraphicsPipeline();
			CreateFramebuffers();

			UniformBuffers = new UniformBuffer<UniformBufferObject>[SwapchainImages.Length];
			for (int i = 0; i < UniformBuffers.Length; i++) UniformBuffers[i] = new UniformBuffer<UniformBufferObject>(this);

			CreateDescriptorPool();
			CreateDescriptorSets();

			CreateCommandBuffers();
		}

		public unsafe void Draw()
		{
			Vulkan.vkWaitForFences(LogicalDevice, InFlightFences[currentFrame], true, ulong.MaxValue);

			VkResult result = Vulkan.vkAcquireNextImageKHR(LogicalDevice, Swapchain, uint.MaxValue, ImageAvailableSemaphores[currentFrame], VkFence.Null, out uint imageIndex);
			if (result == VkResult.ErrorOutOfDateKHR)
			{
				RecreateSwapchain();
				return;
			}

			if (result != VkResult.Success && result != VkResult.SuboptimalKHR) throw new Exception("Failed to acquire swapchain image");

			if (ImagesInFlight[imageIndex] != VkFence.Null) Vulkan.vkWaitForFences(LogicalDevice, ImagesInFlight[imageIndex], true, uint.MaxValue);

			ImagesInFlight[imageIndex] = InFlightFences[currentFrame];

			VkSemaphore[] waitSemaphores = { ImageAvailableSemaphores[currentFrame] };
			VkPipelineStageFlags[] waitStages = { VkPipelineStageFlags.ColorAttachmentOutput };

			UpdateUniformBuffer(imageIndex);

			VkSubmitInfo submitInfo = new VkSubmitInfo
			{
				sType = VkStructureType.SubmitInfo,
				waitSemaphoreCount = 1,
				commandBufferCount = 1,
				signalSemaphoreCount = 1
			};
			fixed (VkSemaphore* ptr = &waitSemaphores[0]) submitInfo.pWaitSemaphores = ptr;
			fixed (VkPipelineStageFlags* ptr = &waitStages[0]) submitInfo.pWaitDstStageMask = ptr;
			fixed (VkCommandBuffer* ptr = &CommandBuffers[imageIndex]) submitInfo.pCommandBuffers = ptr;

			VkSemaphore[] signalSemaphores = { RenderFinishedSemaphores[currentFrame] };
			fixed (VkSemaphore* ptr = &signalSemaphores[0]) submitInfo.pSignalSemaphores = ptr;

			Vulkan.vkResetFences(LogicalDevice, InFlightFences[currentFrame]);

			Vulkan.vkQueueSubmit(GraphicsQueue, 1, &submitInfo, InFlightFences[currentFrame]).CheckResult();

			VkPresentInfoKHR presentInfo = new VkPresentInfoKHR
			{
				sType = VkStructureType.PresentInfoKHR,
				waitSemaphoreCount = 1,
				swapchainCount = 1,
				pImageIndices = &imageIndex,
				pResults = null
			};
			fixed (VkSemaphore* ptr = &signalSemaphores[0]) presentInfo.pWaitSemaphores = ptr;

			VkSwapchainKHR[] swapchains = { Swapchain };
			fixed (VkSwapchainKHR* ptr = &swapchains[0]) presentInfo.pSwapchains = ptr;

			result = Vulkan.vkQueuePresentKHR(PresentQueue, &presentInfo);

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
			UniformBufferObject ubo = new UniformBufferObject();
			ubo.Model = Matrix4x4.CreateRotationZ((float)Time.TotalUpdateTime);
			ubo.View = Matrix4x4.CreateLookAt(new Vector3(0f, 2f, 2f), Vector3.Zero, Vector3.UnitZ);
			ubo.Projection = Matrix4x4.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), SwapchainExtent.Width / (float)SwapchainExtent.Height, 0.1f, 100f);

			ubo.Projection.M22 *= -1;

			UniformBuffers[index].SetData(ubo);
		}

		public unsafe void Dispose()
		{
			Vulkan.vkDeviceWaitIdle(LogicalDevice);

			CleanupSwapchain();

			Vulkan.vkDestroyDescriptorSetLayout(LogicalDevice, DescriptorSetLayout, null);

			VertexBuffer.Dispose();
			IndexBuffer.Dispose();
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

		private static unsafe VkShaderModule CreateShaderModule(VkDevice device, byte[] data)
		{
			VkShaderModuleCreateInfo createInfo = new VkShaderModuleCreateInfo
			{
				sType = VkStructureType.ShaderModuleCreateInfo,
				codeSize = new VkPointerSize((uint)data.Length)
			};

			fixed (byte* ptr = &data[0]) createInfo.pCode = (uint*)ptr;

			Vulkan.vkCreateShaderModule(device, &createInfo, null, out VkShaderModule module).CheckResult();
			return module;
		}

		private static SwapChainSupportDetails QuerySwapchainSupport(VkPhysicalDevice device, VkSurfaceKHR surface)
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

		private static QueueFamilyIndices FindQueueFamilies(VkPhysicalDevice device, VkSurfaceKHR surface)
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