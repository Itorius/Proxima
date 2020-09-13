using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using GLFW;
using NLog;
using Vortice.Mathematics;
using Vortice.Vulkan;
using Exception = System.Exception;
using Vulkan = Vortice.Vulkan.Vulkan;

namespace Proxima
{
	public abstract class Application
	{
		private struct QueueFamilyIndices
		{
			public uint? graphics;

			public bool IsComplete => graphics.HasValue;
		}

		private Window window;

		private VkInstance vkInstance;

		private VkPhysicalDevice vkPhysicalDevice;

		private VkDevice vkDevice;

		private VkQueue graphicsQueue;

#if DEBUG
		public static readonly bool EnableValidationLayers = true;
#else
		public static readonly bool EnableValidationLayers = false;
#endif

		public Application(Options options)
		{
			if (!Glfw.Init()) throw new Exception("Failed to initialize GLFW");
			if (!GLFW.Vulkan.IsSupported) throw new Exception("GLFW does not support Vulkan");

			InitializeVulkan(options);

			Glfw.WindowHint(Hint.ClientApi, ClientApi.None);
			Glfw.WindowHint(Hint.Resizable, Constants.False);

			window = Glfw.CreateWindow(options.Size.Width, options.Size.Height, options.Title, Monitor.None, Window.None);

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

			SelectPhysicalDevice();

			CreateLogicalDevice();
		}

		private unsafe void CreateLogicalDevice()
		{
			QueueFamilyIndices indices = FindQueueFamilies(vkPhysicalDevice);

			VkDeviceQueueCreateInfo queueCreateInfo = new VkDeviceQueueCreateInfo
			{
				sType = VkStructureType.DeviceQueueCreateInfo,
				queueFamilyIndex = indices.graphics.Value,
				queueCount = 1
			};

			float queuePriority = 1f;
			queueCreateInfo.pQueuePriorities = &queuePriority;

			VkPhysicalDeviceFeatures deviceFeatures = new VkPhysicalDeviceFeatures();

			VkDeviceCreateInfo deviceCreateInfo = new VkDeviceCreateInfo
			{
				sType = VkStructureType.DeviceCreateInfo,
				pQueueCreateInfos = &queueCreateInfo,
				queueCreateInfoCount = 1,
				pEnabledFeatures = &deviceFeatures
			};

			if (EnableValidationLayers)
			{
				deviceCreateInfo.enabledLayerCount = ValidationLayers.Length;
				deviceCreateInfo.ppEnabledLayerNames = ValidationLayers;
			}

			Vulkan.vkCreateDevice(vkPhysicalDevice, &deviceCreateInfo, null, out vkDevice).CheckResult();

			Vulkan.vkGetDeviceQueue(vkDevice, indices.graphics.Value, 0, out graphicsQueue);
		}

		private void SelectPhysicalDevice()
		{
			var physicalDevices = Vulkan.vkEnumeratePhysicalDevices(vkInstance);

			foreach (VkPhysicalDevice physicalDevice in physicalDevices)
			{
				if (IsDeviceSuitable(physicalDevice))
				{
					vkPhysicalDevice = physicalDevice;
					Vulkan.vkGetPhysicalDeviceProperties(physicalDevice, out VkPhysicalDeviceProperties properties);
					Log.Debug("Selected {gpu}", properties.GetDeviceName());

					break;
				}
			}

			static bool IsDeviceSuitable(VkPhysicalDevice device)
			{
				Vulkan.vkGetPhysicalDeviceProperties(device, out VkPhysicalDeviceProperties properties);
				Vulkan.vkGetPhysicalDeviceFeatures(device, out VkPhysicalDeviceFeatures features);

				QueueFamilyIndices indices = FindQueueFamilies(device);

				return indices.IsComplete;
			}
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

		private static VkStringArray GetRequiredExtensions()
		{
			List<string> extensions = GLFW.Vulkan.GetRequiredInstanceExtensions().ToList();

			if (EnableValidationLayers) extensions.Add(Vulkan.EXTDebugUtilsExtensionName);

			return new VkStringArray(extensions);
		}

		private static QueueFamilyIndices FindQueueFamilies(VkPhysicalDevice device)
		{
			QueueFamilyIndices indices = new QueueFamilyIndices();

			var properties = Vulkan.vkGetPhysicalDeviceQueueFamilyProperties(device);

			int i = 0;
			foreach (VkQueueFamilyProperties property in properties)
			{
				if ((property.queueFlags & VkQueueFlags.Graphics) == VkQueueFlags.Graphics) indices.graphics = (uint?)i;
				if (indices.IsComplete) break;

				i++;
			}

			return indices;
		}

		#region Debugging

		private static readonly VkStringArray ValidationLayers = new VkStringArray(new[] {"VK_LAYER_KHRONOS_validation"});
		private unsafe vkDebugUtilsMessengerCallbackEXT debugMessengerCallbackFunc = DebugMessengerCallback;
		private VkDebugUtilsMessengerEXT debugMessenger = VkDebugUtilsMessengerEXT.Null;

		private static unsafe VkBool32 DebugMessengerCallback(VkDebugUtilsMessageSeverityFlagsEXT messageSeverity, VkDebugUtilsMessageTypeFlagsEXT messageTypes, VkDebugUtilsMessengerCallbackDataEXT* pCallbackData, IntPtr userData)
		{
			string message = Interop.GetString(pCallbackData->pMessage);
			if (messageTypes == VkDebugUtilsMessageTypeFlagsEXT.Validation)
			{
				if (messageSeverity == VkDebugUtilsMessageSeverityFlagsEXT.Error) Log.Error($"[Vulkan]: Validation: {messageSeverity} - {message}");
				else if (messageSeverity == VkDebugUtilsMessageSeverityFlagsEXT.Warning) Log.Warn($"[Vulkan]: Validation: {messageSeverity} - {message}");
				else Log.Info($"[Vulkan]: Validation: {messageSeverity} - {message}");
			}
			else
			{
				if (messageSeverity == VkDebugUtilsMessageSeverityFlagsEXT.Error) Log.Error($"[Vulkan]: {messageSeverity} - {message}");
				else if (messageSeverity == VkDebugUtilsMessageSeverityFlagsEXT.Warning) Log.Warn($"[Vulkan]: {messageSeverity} - {message}");
				else Log.Info($"[Vulkan]: {messageSeverity} - {message}");
			}

			return VkBool32.False;
		}

		private unsafe void SetupDebugging()
		{
			if (!EnableValidationLayers) return;

			VkDebugUtilsMessengerCreateInfoEXT createInfo = CreateDebugMessengerInfo();
			Vulkan.vkCreateDebugUtilsMessengerEXT(vkInstance, &createInfo, null, out debugMessenger).CheckResult();
		}

		private VkDebugUtilsMessengerCreateInfoEXT CreateDebugMessengerInfo()
		{
			VkDebugUtilsMessengerCreateInfoEXT createInfo = new VkDebugUtilsMessengerCreateInfoEXT
			{
				sType = VkStructureType.DebugUtilsMessengerCreateInfoEXT,
				messageSeverity = /*VkDebugUtilsMessageSeverityFlagsEXT.Verbose |*/ VkDebugUtilsMessageSeverityFlagsEXT.Warning | VkDebugUtilsMessageSeverityFlagsEXT.Error,
				messageType = VkDebugUtilsMessageTypeFlagsEXT.General | VkDebugUtilsMessageTypeFlagsEXT.Validation | VkDebugUtilsMessageTypeFlagsEXT.Performance,
				pfnUserCallback = Marshal.GetFunctionPointerForDelegate(debugMessengerCallbackFunc)
			};
			return createInfo;
		}

		private static bool CheckValidationLayerSupport()
		{
			ReadOnlySpan<VkLayerProperties> properties = Vulkan.vkEnumerateInstanceLayerProperties();

			for (int i = 0; i < ValidationLayers.Length; i++)
			{
				bool layerFound = false;

				foreach (VkLayerProperties property in properties)
				{
					if (ValidationLayers[i] == property.GetName())
					{
						layerFound = true;
						break;
					}
				}

				if (!layerFound) return false;
			}

			return true;
		}

		#endregion

		internal void Run()
		{
			while (!Glfw.WindowShouldClose(window))
			{
				Glfw.PollEvents();
			}

			Cleanup();
		}

		private unsafe void Cleanup()
		{
			if (EnableValidationLayers) Vulkan.vkDestroyDebugUtilsMessengerEXT(vkInstance, debugMessenger, null);

			Vulkan.vkDestroyDevice(vkDevice, null);
			Vulkan.vkDestroyInstance(vkInstance, null);

			Glfw.DestroyWindow(window);
			Glfw.Terminate();

			LogManager.Shutdown();
		}

		public struct Options
		{
			public string Title;
			public Size Size;
			public bool VSync;
		}
	}
}