using System;
using System.Collections.Generic;
using System.Linq;
using GLFW;
using Vortice.Vulkan;
using Exception = System.Exception;
using Vulkan = Vortice.Vulkan.Vulkan;

namespace Proxima.Graphics
{
	public partial class GraphicsDevice
	{
		public event Action OnInvalidate;

		internal void Initialize()
		{
			if (Vulkan.Initialize() != VkResult.Success) throw new Exception("Failed to initialize Vulkan");
			if (ValidationEnabled && !IsValidationSupported()) throw new Exception("Validation layers requested, but not available");

			window.SizeChanged += (sender, args) => framebufferResized = true;

			CreateInstance();

			SetupDebugging();

			CreateWindowSurface();

			SelectPhysicalDevice();

			CreateLogicalDevice();

			CreateCommandPool();

			var (_, formats, _) = VulkanUtils.QuerySwapchainSupport(PhysicalDevice, Surface);

			VkSurfaceFormatKHR surfaceFormat = VulkanUtils.SelectSwapSurfaceFormat(formats);

			RenderPass = new RenderPass(this, surfaceFormat.format);

			Swapchain = new Swapchain(this);

			DepthBuffer = new DepthBuffer(this, Swapchain.Extent);
			CreateFramebuffers();

			CreateSyncObjects();
		}

		private unsafe void CreateInstance()
		{
			VkApplicationInfo appInfo = new VkApplicationInfo
			{
				sType = VkStructureType.ApplicationInfo,
				pApplicationName = new VkString(window.Title ?? "Application"),
				applicationVersion = new VkVersion(1, 0, 0),
				pEngineName = new VkString("Proxima"),
				engineVersion = new VkVersion(1, 0, 0),
				apiVersion = Vulkan.vkEnumerateInstanceVersion()
			};

			using VkStringArray extensions = VulkanUtils.GetRequiredExtensions(ValidationEnabled);
			using VkStringArray layers = GetRequiredLayers();

			VkInstanceCreateInfo createInfo = new VkInstanceCreateInfo
			{
				sType = VkStructureType.InstanceCreateInfo,
				pApplicationInfo = &appInfo,
				enabledExtensionCount = extensions.Length,
				ppEnabledExtensionNames = extensions,
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

		private void SelectPhysicalDevice()
		{
			var physicalDevices = Vulkan.vkEnumeratePhysicalDevices(Instance);

			foreach (VkPhysicalDevice physicalDevice in physicalDevices)
			{
				if (IsDeviceSuitable(physicalDevice, Surface))
				{
					PhysicalDevice = new VulkanPhysicalDevice(this, physicalDevice);
					break;
				}
			}

			static bool IsDeviceSuitable(VkPhysicalDevice device, VkSurfaceKHR surface)
			{
				Vulkan.vkGetPhysicalDeviceProperties(device, out VkPhysicalDeviceProperties properties);
				Vulkan.vkGetPhysicalDeviceFeatures(device, out VkPhysicalDeviceFeatures features);

				QueueFamilyIndices indices = VulkanUtils.FindQueueFamilies(device, surface);

				bool extensionsSupported = VulkanUtils.CheckDeviceExtensionSupport(device, DeviceExtensions);
				var (_, formats, presentModes) = VulkanUtils.QuerySwapchainSupport(device, surface);

				return indices.IsComplete && extensionsSupported && formats.Count > 0 && presentModes.Count > 0 && features.samplerAnisotropy;
			}
		}

		private unsafe void CreateLogicalDevice()
		{
			QueueFamilyIndices indices = VulkanUtils.FindQueueFamilies(PhysicalDevice, Surface);

			List<uint> uniqueQueueFamilies = new List<uint> { indices.graphics.Value, indices.present.Value };
			VkDeviceQueueCreateInfo[] queueCreateInfos = new VkDeviceQueueCreateInfo[uniqueQueueFamilies.Count];

			float queuePriority = 1f;
			for (int i = 0; i < uniqueQueueFamilies.Count; i++)
			{
				uint queueFamily = uniqueQueueFamilies[i];
				VkDeviceQueueCreateInfo queueCreateInfo = new VkDeviceQueueCreateInfo
				{
					sType = VkStructureType.DeviceQueueCreateInfo,
					queueFamilyIndex = queueFamily,
					queueCount = 1,
					pQueuePriorities = &queuePriority
				};
				queueCreateInfos[i] = queueCreateInfo;
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

		private void CreateWindowSurface()
		{
			VkResult result = (VkResult)GLFW.Vulkan.CreateWindowSurface(Instance.Handle, window, IntPtr.Zero, out IntPtr handle);
			if (result != VkResult.Success) throw new Exception("Failed to create window surface");
			Surface = new VkSurfaceKHR((ulong)handle);
		}

		private void CreateFramebuffers()
		{
			Framebuffers = new FrameBuffer[Swapchain.Length];

			for (int i = 0; i < Swapchain.Length; i++)
			{
				VkImageView[] attachments = { Swapchain.ImageViews[i], DepthBuffer.View };

				Framebuffers[i] = new FrameBuffer(this, new FrameBufferOptions
				{
					Attachments = attachments.ToList(),
					RenderPass = RenderPass,
					Width = Swapchain.Extent.width,
					Height = Swapchain.Extent.height,
					Layers = 1
				});
			}
		}

		private unsafe void CreateCommandPool()
		{
			QueueFamilyIndices indices = VulkanUtils.FindQueueFamilies(PhysicalDevice, Surface);

			VkCommandPoolCreateInfo commandPoolCreateInfo = new VkCommandPoolCreateInfo
			{
				sType = VkStructureType.CommandPoolCreateInfo,
				queueFamilyIndex = indices.graphics.Value,
				flags = VkCommandPoolCreateFlags.ResetCommandBuffer
			};

			Vulkan.vkCreateCommandPool(LogicalDevice, &commandPoolCreateInfo, null, out CommandPool).CheckResult();

			VkCommandBufferAllocateInfo allocateInfo = new VkCommandBufferAllocateInfo
			{
				sType = VkStructureType.CommandBufferAllocateInfo,
				commandPool = CommandPool,
				level = VkCommandBufferLevel.Primary,
				commandBufferCount = 1
			};

			Vulkan.vkAllocateCommandBuffers(LogicalDevice, &allocateInfo, out var buffer);
			currentBuffer = new VulkanCommandBuffer(this, buffer);
			currentBuffer.SetName("Primary Buffer");
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

		private void RecreateSwapchain()
		{
			while (window.Size.Width == 0 || window.Size.Height == 0) Glfw.WaitEvents();

			Vulkan.vkDeviceWaitIdle(LogicalDevice);

			Swapchain.Invalidate();
			RenderPass.Invalidate();

			DepthBuffer.Invalidate(Swapchain.Extent);

			for (int i = 0; i < Framebuffers.Length; i++)
			{
				FrameBuffer framebuffer = Framebuffers[i];
				framebuffer.options.Attachments = new List<VkImageView> { Swapchain.ImageViews[i], DepthBuffer.View };
				framebuffer.Resize(Swapchain.Extent.width, Swapchain.Extent.height);
			}

			// CreateFramebuffers();

			OnInvalidate.Invoke();
		}
	}
}