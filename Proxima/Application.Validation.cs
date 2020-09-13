using System;
using System.Runtime.InteropServices;
using Vortice.Vulkan;

namespace Proxima
{
	public abstract partial class Application
	{
#if ENABLE_VALIDATION
		public static readonly bool EnableValidationLayers = true;
#else
		public static readonly bool EnableValidationLayers = false;
#endif
		
		private static readonly VkStringArray ValidationLayers = new VkStringArray(new[] { "VK_LAYER_KHRONOS_validation" });
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
				messageSeverity = VkDebugUtilsMessageSeverityFlagsEXT.Verbose | VkDebugUtilsMessageSeverityFlagsEXT.Warning | VkDebugUtilsMessageSeverityFlagsEXT.Error,
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
	}
}