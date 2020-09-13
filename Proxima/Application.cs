using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GLFW;
using NLog;
using Vortice.Mathematics;
using Vortice.Vulkan;
using Exception = System.Exception;
using Monitor = GLFW.Monitor;
using Vulkan = Vortice.Vulkan.Vulkan;

namespace Proxima
{
	public abstract partial class Application
	{
		public struct Options
		{
			public string Title;
			public Size Size;
			public bool VSync;
		}

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

		private static VkInstance vkInstance;

		private VkPhysicalDevice vkPhysicalDevice;

		private VkDevice vkDevice;

		private VkQueue graphicsQueue;
		private VkQueue presentQueue;

		private VkSurfaceKHR vkSurface;
		private Options options;

		private VkSwapchainKHR vkSwapchain;

		public Application(Options options)
		{
			if (!Glfw.Init()) throw new Exception("Failed to initialize GLFW");
			if (!GLFW.Vulkan.IsSupported) throw new Exception("GLFW does not support Vulkan");

			this.options = options;

			Glfw.WindowHint(Hint.ClientApi, ClientApi.None);
			Glfw.WindowHint(Hint.Resizable, Constants.False);
			Log.Debug(Glfw.Version);

			window = new NativeWindow(options.Size.Width, options.Size.Height, options.Title, Monitor.None, Window.None);

			InitializeVulkan(options);

			Glfw.SetKeyCallback(window, (_, key, code, state, mods) =>
			{
				if (key == Keys.Escape && state == InputState.Press)
				{
					Glfw.SetWindowShouldClose(window, true);
				}
			});
		}

		private void InitializeVulkan(Options options)
		{
			if (Vulkan.Initialize() != VkResult.Success) throw new Exception("Failed to initialize Vulkan");
			if (EnableValidationLayers && !CheckValidationLayerSupport()) throw new Exception("Validation layers requested, but not available");

			CreateInstance(options);

			SetupDebugging();

			CreateWindowSurface();

			SelectPhysicalDevice();

			CreateLogicalDevice();

			CreateSwapchain();
		}

		private unsafe void CreateInstance(Options options)
		{
			VkApplicationInfo appInfo = new VkApplicationInfo
			{
				sType = VkStructureType.ApplicationInfo,
				pApplicationName = new VkString(options.Title),
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

			if (EnableValidationLayers)
			{
				createInfo.enabledLayerCount = ValidationLayers.Length;
				createInfo.ppEnabledLayerNames = ValidationLayers;
				var messengerCreateInfo = CreateDebugMessengerInfo();
				createInfo.pNext = &messengerCreateInfo;
			}

			Vulkan.vkCreateInstance(&createInfo, null, out vkInstance).CheckResult();
			Vulkan.vkLoadInstance(vkInstance);
		}

		private void CreateWindowSurface()
		{
			VkResult result = (VkResult)GLFW.Vulkan.CreateWindowSurface(vkInstance.Handle, window, IntPtr.Zero, out IntPtr handle);
			if (result != VkResult.Success) throw new Exception("Failed to create window surface");
			vkSurface = new VkSurfaceKHR((ulong)handle.ToInt64());
		}

		private void SelectPhysicalDevice()
		{
			var physicalDevices = Vulkan.vkEnumeratePhysicalDevices(vkInstance);

			foreach (VkPhysicalDevice physicalDevice in physicalDevices)
			{
				if (IsDeviceSuitable(physicalDevice, vkSurface))
				{
					vkPhysicalDevice = physicalDevice;
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

				return indices.IsComplete && extensionsSupported && !details.formats.IsEmpty && !details.presentModes.IsEmpty;
			}
		}

		private unsafe void CreateLogicalDevice()
		{
			QueueFamilyIndices indices = FindQueueFamilies(vkPhysicalDevice, vkSurface);

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

			VkPhysicalDeviceFeatures deviceFeatures = new VkPhysicalDeviceFeatures();

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

			if (EnableValidationLayers)
			{
				deviceCreateInfo.enabledLayerCount = ValidationLayers.Length;
				deviceCreateInfo.ppEnabledLayerNames = ValidationLayers;
			}

			Vulkan.vkCreateDevice(vkPhysicalDevice, &deviceCreateInfo, null, out vkDevice).CheckResult();

			Vulkan.vkGetDeviceQueue(vkDevice, indices.graphics.Value, 0, out graphicsQueue);
			Vulkan.vkGetDeviceQueue(vkDevice, indices.present.Value, 0, out presentQueue);
		}

		private unsafe void CreateSwapchain()
		{
			SwapChainSupportDetails details = QuerySwapchainSupport(vkPhysicalDevice, vkSurface);

			VkSurfaceFormatKHR surfaceFormat = SelectSwapSurfaceFormat(details.formats);
			VkPresentModeKHR presentMode = SelectSwapPresentMode(details.presentModes);
			Size extent = SelectSwapExtent(details.capabilities, options);

			uint imageCount = details.capabilities.minImageCount + 1;
			if (details.capabilities.maxImageCount > 0 && imageCount > details.capabilities.maxImageCount) imageCount = details.capabilities.maxImageCount;

			VkSwapchainCreateInfoKHR createInfo = new VkSwapchainCreateInfoKHR
			{
				sType = VkStructureType.SwapchainCreateInfoKHR,
				surface = vkSurface,
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

			QueueFamilyIndices indices = FindQueueFamilies(vkPhysicalDevice, vkSurface);
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

			Vulkan.vkCreateSwapchainKHR(vkDevice, &createInfo, null, out vkSwapchain).CheckResult();

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

			static Size SelectSwapExtent(VkSurfaceCapabilitiesKHR capabilities, Options options)
			{
				if (capabilities.currentExtent.Width != uint.MaxValue) return capabilities.currentExtent;

				Size actualExtent = options.Size;

				actualExtent.Width = Math.Clamp(actualExtent.Width, capabilities.minImageExtent.Width, capabilities.maxImageExtent.Width);
				actualExtent.Height = Math.Clamp(actualExtent.Height, capabilities.minImageExtent.Height, capabilities.maxImageExtent.Height);

				return actualExtent;
			}
		}
	
		private unsafe void Cleanup()
		{
			if (EnableValidationLayers) Vulkan.vkDestroyDebugUtilsMessengerEXT(vkInstance, debugMessenger, null);

			Vulkan.vkDestroySwapchainKHR(vkDevice, vkSwapchain, null);
			Vulkan.vkDestroySurfaceKHR(vkInstance, vkSurface, null);
			Vulkan.vkDestroyDevice(vkDevice, null);
			Vulkan.vkDestroyInstance(vkInstance, null);

			Glfw.DestroyWindow(window);
			Glfw.Terminate();

			LogManager.Shutdown();
		}

		internal void Run()
		{
			while (!Glfw.WindowShouldClose(window))
			{
				Glfw.PollEvents();
				Thread.Sleep(20);
			}

			Cleanup();
		}
		
		#region Helper Methods

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

		private static VkStringArray GetRequiredExtensions()
		{
			List<string> extensions = GLFW.Vulkan.GetRequiredInstanceExtensions().ToList();

			if (EnableValidationLayers) extensions.Add(Vulkan.EXTDebugUtilsExtensionName);

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