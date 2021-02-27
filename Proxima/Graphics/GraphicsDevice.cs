using System;
using System.Collections.Generic;
using System.Numerics;
using GLFW;
using Vortice.Vulkan;
using Exception = System.Exception;
using Vulkan = Vortice.Vulkan.Vulkan;

namespace Proxima.Graphics
{
	public sealed partial class GraphicsDevice : IDisposable
	{
		private static readonly VkStringArray DeviceExtensions = new(new[] { Vulkan.KHRSwapchainExtensionName });

		internal readonly NativeWindow window;

		private VkInstance Instance;
		internal VulkanPhysicalDevice PhysicalDevice;
		internal VkDevice LogicalDevice;

		internal VkQueue GraphicsQueue;
		private VkQueue PresentQueue;
		internal VkCommandPool CommandPool;

		internal VkSurfaceKHR Surface;
		internal Swapchain Swapchain;

		internal RenderPass RenderPass;

		internal FrameBuffer[] Framebuffers;
		private DepthBuffer DepthBuffer;

		private int currentFrame;
		private bool framebufferResized;
		internal bool vsync = true;

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

		public void SetVerticalSync(bool vsync) => this.vsync = vsync;
		
		public uint CurrentFrameIndex { get; private set; }
		private List<VkCommandBuffer> secondaryBuffers = new();

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

		public void SubmitSecondaryBuffer(VkCommandBuffer buffer) => secondaryBuffers.Add(buffer);

		public VkCommandBufferInheritanceInfo GetInheritanceInfo() => new()
		{
			sType = VkStructureType.CommandBufferInheritanceInfo,
			framebuffer = (VkFramebuffer)Framebuffers[CurrentFrameIndex],
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

			VkResult result = Vulkan.vkAcquireNextImageKHR(LogicalDevice, Swapchain.swapchain, uint.MaxValue, ImageAvailableSemaphores[currentFrame], VkFence.Null, out var currentFrameIndex);
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
				framebuffer = (VkFramebuffer)Framebuffers[currentFrameIndex],
				renderArea = new VkRect2D(0, 0, Swapchain.Extent.width, Swapchain.Extent.height),
				clearValueCount = (uint)clearValues.Length
			};
			fixed (VkClearValue* ptr = clearValues) renderPassBeginInfo.pClearValues = ptr;

			Vulkan.vkCmdBeginRenderPass(currentBuffer, &renderPassBeginInfo, VkSubpassContents.SecondaryCommandBuffers);
		}

		private VulkanCommandBuffer currentBuffer;

		internal unsafe void EndFrame()
		{
			fixed (VkCommandBuffer* ptr = secondaryBuffers.GetInternalArray()) Vulkan.vkCmdExecuteCommands(currentBuffer, (uint)secondaryBuffers.Count, ptr);
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
			secondaryBuffers.Clear();

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

			VkSwapchainKHR[] swapchains = { Swapchain.swapchain };
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

			foreach (FrameBuffer framebuffer in Framebuffers) framebuffer.Dispose();

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