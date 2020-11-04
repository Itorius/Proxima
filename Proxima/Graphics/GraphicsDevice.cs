using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using GLFW;
using Vortice.Vulkan;
using Exception = System.Exception;
using Vulkan = Vortice.Vulkan.Vulkan;

namespace Proxima.Graphics
{
	public sealed partial class GraphicsDevice : IDisposable
	{
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

		internal VkFramebuffer[] Framebuffers;
		private DepthBuffer DepthBuffer;

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

			var (_, formats, _) = VulkanUtils.QuerySwapchainSupport(PhysicalDevice, Surface);

			VkSurfaceFormatKHR surfaceFormat = VulkanUtils.SelectSwapSurfaceFormat(formats);

			RenderPass = new VulkanRenderPass(this, surfaceFormat.format);

			Swapchain = new VulkanSwapchain(this);

			DepthBuffer = new DepthBuffer(this, Swapchain.Extent);
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

			using VkStringArray vkInstanceExtensions = VulkanUtils.GetRequiredExtensions(ValidationEnabled);

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

		private unsafe void CreateFramebuffers()
		{
			Framebuffers = new VkFramebuffer[Swapchain.Length];

			for (int i = 0; i < Swapchain.Length; i++)
			{
				VkImageView[] attachments = { Swapchain.ImageViews[i], DepthBuffer.View };

				VkFramebufferCreateInfo framebufferCreateInfo = new VkFramebufferCreateInfo
				{
					sType = VkStructureType.FramebufferCreateInfo,
					renderPass = (VkRenderPass)RenderPass,
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

		public event Action OnInvalidate;

		private unsafe void RecreateSwapchain()
		{
			while (window.Size.Width == 0 || window.Size.Height == 0) Glfw.WaitEvents();

			Vulkan.vkDeviceWaitIdle(LogicalDevice);

			Swapchain.Invalidate();
			RenderPass.Invalidate();

			foreach (VkFramebuffer framebuffer in Framebuffers) Vulkan.vkDestroyFramebuffer(LogicalDevice, framebuffer, null);

			DepthBuffer.Invalidate(Swapchain.Extent);

			CreateFramebuffers();

			OnInvalidate.Invoke();
		}

		public uint CurrentFrameIndex { get; private set; }

		private List<VkCommandBuffer> secondary_buffers = new List<VkCommandBuffer>();

		public unsafe VkCommandBuffer GetSecondaryBuffer()
		{
			VkCommandBufferAllocateInfo allocateInfo = new VkCommandBufferAllocateInfo
			{
				sType = VkStructureType.CommandBufferAllocateInfo,
				commandPool = CommandPool,
				level = VkCommandBufferLevel.Secondary,
				commandBufferCount = 1
			};

			Vulkan.vkAllocateCommandBuffers(LogicalDevice, &allocateInfo, out var buffer);

			return buffer;
		}

		public void SubmitSecondaryBuffer(VkCommandBuffer buffer) => secondary_buffers.Add(buffer);

		public VkCommandBufferInheritanceInfo GetInheritanceInfo() => new VkCommandBufferInheritanceInfo
		{
			sType = VkStructureType.CommandBufferInheritanceInfo,
			framebuffer = Framebuffers[CurrentFrameIndex],
			renderPass = (VkRenderPass)RenderPass
		};

		internal unsafe void BeginFrame(Vector4 color)
		{
			// Vulkan.vkWaitForFences(LogicalDevice, InFlightFences[currentFrame], true, ulong.MaxValue).CheckResult();
			// VkResult fenceRes;
			// do
			// {
			// 	Log.Debug(currentFrame);
			// 	fenceRes = Vulkan.vkWaitForFences(LogicalDevice, InFlightFences[currentFrame], true, ulong.MaxValue);
			// } while (fenceRes == VkResult.Timeout);
			// fenceRes.CheckResult();
			//
			// Vulkan.vkResetFences(LogicalDevice, InFlightFences[currentFrame]);

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

			VkCommandBufferBeginInfo beginInfo = new VkCommandBufferBeginInfo
			{
				sType = VkStructureType.CommandBufferBeginInfo,
				flags = VkCommandBufferUsageFlags.SimultaneousUse,
				pInheritanceInfo = null
			};

			Vulkan.vkBeginCommandBuffer(currentBuffer, &beginInfo).CheckResult();

			VkClearValue[] clearValues = new VkClearValue[2];

			clearValues[0].color = new VkClearColorValue(color.X, color.Y, color.Z, color.W);
			clearValues[1].depthStencil = new VkClearDepthStencilValue(1f, 0);

			VkRenderPassBeginInfo renderPassBeginInfo = new VkRenderPassBeginInfo
			{
				sType = VkStructureType.RenderPassBeginInfo,
				renderPass = (VkRenderPass)RenderPass,
				framebuffer = Framebuffers[currentFrameIndex],
				renderArea = new VkRect2D(0, 0, Swapchain.Extent.width, Swapchain.Extent.height),
				clearValueCount = (uint)clearValues.Length
			};
			fixed (VkClearValue* ptr = clearValues) renderPassBeginInfo.pClearValues = ptr;

			Vulkan.vkCmdBeginRenderPass(currentBuffer, &renderPassBeginInfo, VkSubpassContents.SecondaryCommandBuffers);
		}

		private VulkanCommandBuffer currentBuffer;

		internal unsafe void EndFrame()
		{
			fixed (VkCommandBuffer* ptr = secondary_buffers.GetInternalArray()) Vulkan.vkCmdExecuteCommands(currentBuffer, (uint)secondary_buffers.Count, ptr);
			Vulkan.vkResetFences(LogicalDevice, InFlightFences[currentFrame]);

			Vulkan.vkCmdEndRenderPass(currentBuffer);
			Vulkan.vkEndCommandBuffer(currentBuffer).CheckResult();

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

			var tmp = (VkCommandBuffer)currentBuffer;
			submitInfo.pCommandBuffers = &tmp;

			VkSemaphore[] signalSemaphores = { RenderFinishedSemaphores[currentFrame] };
			fixed (VkSemaphore* ptr = signalSemaphores) submitInfo.pSignalSemaphores = ptr;

			// Vulkan.vkResetFences(LogicalDevice, InFlightFences[currentFrame]);

			Vulkan.vkQueueSubmit(GraphicsQueue, 1, &submitInfo, InFlightFences[currentFrame]).CheckResult();

			// Vulkan.vkResetCommandPool(LogicalDevice, CommandPool, VkCommandPoolResetFlags.None).CheckResult();

			// foreach (var buffer in secondary_buffers) Vulkan.vkResetCommandBuffer(buffer, VkCommandBufferResetFlags.None);
			secondary_buffers.Clear();

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

			Vulkan.vkWaitForFences(LogicalDevice, InFlightFences[currentFrame], true, ulong.MaxValue);

			currentFrame = (currentFrame + 1) % MaxFramesInFlight;
		}

		public unsafe void Dispose()
		{
			Vulkan.vkDeviceWaitIdle(LogicalDevice);

			Vulkan.vkResetCommandPool(LogicalDevice, CommandPool, VkCommandPoolResetFlags.ReleaseResources).CheckResult();

			foreach (VkFramebuffer framebuffer in Framebuffers) Vulkan.vkDestroyFramebuffer(LogicalDevice, framebuffer, null);

			RenderPass.Dispose();
			Swapchain.Dispose();
			DepthBuffer.Dispose();

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
	}
}