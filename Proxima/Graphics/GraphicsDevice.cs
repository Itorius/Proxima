using System;
using System.Collections.Generic;
using System.Linq;
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
		internal VkCommandPool CommandPool;

		internal VkSurfaceKHR Surface;
		internal VulkanSwapchain Swapchain;

		internal VulkanRenderPass RenderPass;

		private VkFramebuffer[] Framebuffers;
		private DepthBuffer DepthBuffer;

		internal Texture2D Texture;

		private int currentFrame;
		private bool framebufferResized;

		#region Sync Objects
		private const int MaxFramesInFlight = 2;
		private VkSemaphore[] RenderFinishedSemaphores;
		private VkSemaphore[] ImageAvailableSemaphores;
		private VkFence[] InFlightFences;
		private VkFence[] ImagesInFlight;
		#endregion

		internal GraphicsDevice(NativeWindow window)
		{
			this.window = window;
		}

		internal void Initialize()
		{
			if (Vulkan.Initialize() != VkResult.Success) throw new Exception("Failed to initialize Vulkan");
			if (ValidationEnabled && !IsValidationSupported()) throw new Exception("Validation layers requested, but not available");

			window.SizeChanged += (sender, args) => { framebufferResized = true; };

			CreateInstance();

			SetupDebugging();

			CreateWindowSurface();

			SelectPhysicalDevice();

			CreateLogicalDevice();

			CreateCommandPool();

			SwapChainSupportDetails details = QuerySwapchainSupport(PhysicalDevice, Surface);

			VkSurfaceFormatKHR surfaceFormat = VulkanUtils.SelectSwapSurfaceFormat(details.formats);

			RenderPass = new VulkanRenderPass(this, surfaceFormat.format);

			Swapchain = new VulkanSwapchain(this);

			Texture = new Texture2D(this, "Assets/Tom.png");

			CreateFramebuffers();

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
			Surface = new VkSurfaceKHR((ulong)handle.ToInt64());
		}

		private unsafe void CreateFramebuffers()
		{
			DepthBuffer = new DepthBuffer(this, Swapchain.Extent);

			Framebuffers = new VkFramebuffer[Swapchain.Length];

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

				Vulkan.vkCreateFramebuffer(LogicalDevice, &framebufferCreateInfo, null, out Framebuffers[i]).CheckResult();
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

		public event Action OnInvalidate;

		private unsafe void RecreateSwapchain()
		{
			while (window.Size.Width == 0 || window.Size.Height == 0) Glfw.WaitEvents();

			Vulkan.vkDeviceWaitIdle(LogicalDevice);

			Swapchain.Invalidate();

			DepthBuffer.Invalidate(Swapchain.Extent);

			foreach (VkFramebuffer framebuffer in Framebuffers) Vulkan.vkDestroyFramebuffer(LogicalDevice, framebuffer, null);
			CreateFramebuffers();

			RenderPass.Invalidate();

			OnInvalidate.Invoke();
		}

		public unsafe VkCommandBuffer Begin(Color4 color, uint index, GraphicsPipeline graphicsPipeline)
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
				framebuffer = Framebuffers[index],
				renderArea = new VkRect2D(0, 0, Swapchain.Extent.width, Swapchain.Extent.height),
				clearValueCount = (uint)clearValues.Length
			};
			fixed (VkClearValue* ptr = clearValues) renderPassBeginInfo.pClearValues = ptr;

			Vulkan.vkCmdBeginRenderPass(buffer, &renderPassBeginInfo, VkSubpassContents.Inline);

			Vulkan.vkCmdBindPipeline(buffer, VkPipelineBindPoint.Graphics, graphicsPipeline.Pipeline);

			return buffer;
		}

		public void End(VkCommandBuffer buffer)
		{
			Vulkan.vkCmdEndRenderPass(buffer);

			Vulkan.vkEndCommandBuffer(buffer);

			SubmitCommandBuffer(buffer);
		}

		public uint CurrentFrameIndex { get; private set; }

		internal void BeginFrame()
		{
			Vulkan.vkWaitForFences(LogicalDevice, InFlightFences[currentFrame], true, ulong.MaxValue);

			VkResult result = Vulkan.vkAcquireNextImageKHR(LogicalDevice, Swapchain.Swapchain, uint.MaxValue, ImageAvailableSemaphores[currentFrame], VkFence.Null, out var currentFrameIndex);
			CurrentFrameIndex = currentFrameIndex;
			if (result == VkResult.ErrorOutOfDateKHR)
			{
				RecreateSwapchain();
				return;
			}

			if (result != VkResult.Success && result != VkResult.SuboptimalKHR) throw new Exception("Failed to acquire swapchain image");

			if (ImagesInFlight[CurrentFrameIndex] != VkFence.Null) Vulkan.vkWaitForFences(LogicalDevice, ImagesInFlight[CurrentFrameIndex], true, uint.MaxValue);

			ImagesInFlight[CurrentFrameIndex] = InFlightFences[currentFrame];
		}

		private List<VkCommandBuffer> buffers = new List<VkCommandBuffer>();

		public void SubmitCommandBuffer(VkCommandBuffer buffer)
		{
			buffers.Add(buffer);
		}

		public unsafe void EndFrame()
		{
			VkSemaphore[] waitSemaphores = { ImageAvailableSemaphores[currentFrame] };
			VkPipelineStageFlags[] waitStages = { VkPipelineStageFlags.ColorAttachmentOutput };

			VkSubmitInfo submitInfo = new VkSubmitInfo
			{
				sType = VkStructureType.SubmitInfo,
				waitSemaphoreCount = 1,
				commandBufferCount = (uint)buffers.Count,
				signalSemaphoreCount = 1
			};
			fixed (VkSemaphore* ptr = waitSemaphores) submitInfo.pWaitSemaphores = ptr;
			fixed (VkPipelineStageFlags* ptr = waitStages) submitInfo.pWaitDstStageMask = ptr;
			fixed (VkCommandBuffer* ptr = buffers.ToArray()) submitInfo.pCommandBuffers = ptr;

			VkSemaphore[] signalSemaphores = { RenderFinishedSemaphores[currentFrame] };
			fixed (VkSemaphore* ptr = signalSemaphores) submitInfo.pSignalSemaphores = ptr;

			Vulkan.vkResetFences(LogicalDevice, InFlightFences[currentFrame]);

			Vulkan.vkQueueSubmit(GraphicsQueue, 1, &submitInfo, InFlightFences[currentFrame]).CheckResult();
			buffers.Clear();

			VkPresentInfoKHR presentInfo = new VkPresentInfoKHR
			{
				sType = VkStructureType.PresentInfoKHR,
				waitSemaphoreCount = 1,
				swapchainCount = 1,
				pResults = null
			};
			uint currentFrameIndex = CurrentFrameIndex;
			presentInfo.pImageIndices = &currentFrameIndex;
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

		public unsafe void Dispose()
		{
			Vulkan.vkDeviceWaitIdle(LogicalDevice);

			DepthBuffer.Dispose();

			foreach (VkFramebuffer framebuffer in Framebuffers) Vulkan.vkDestroyFramebuffer(LogicalDevice, framebuffer, null);

			RenderPass.Dispose();
			Swapchain.Dispose();

			// GraphicsPipeline.Dispose();

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