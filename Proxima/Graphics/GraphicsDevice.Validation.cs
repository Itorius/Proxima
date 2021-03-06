using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Vortice.Vulkan;

namespace Proxima.Graphics
{
	public partial class GraphicsDevice
	{
		private static readonly VkStringArray ValidationLayers = new VkStringArray(new[] { "VK_LAYER_KHRONOS_validation" });

		private bool ValidationEnabled;
		private unsafe vkDebugUtilsMessengerCallbackEXT debugMessengerCallbackFunc = DebugMessengerCallback;
		private VkDebugUtilsMessengerEXT debugMessenger = VkDebugUtilsMessengerEXT.Null;

		// todo: allow the user to select validation levels
		public void EnableValidation()
		{
			ValidationEnabled = true;
		}

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
			if (!ValidationEnabled) return;

			VkDebugUtilsMessengerCreateInfoEXT createInfo = CreateDebugMessengerInfo();
			Vulkan.vkCreateDebugUtilsMessengerEXT(Instance, &createInfo, null, out debugMessenger).CheckResult();
		}

		private VkDebugUtilsMessengerCreateInfoEXT CreateDebugMessengerInfo()
		{
			VkDebugUtilsMessengerCreateInfoEXT createInfo = new VkDebugUtilsMessengerCreateInfoEXT
			{
				sType = VkStructureType.DebugUtilsMessengerCreateInfoEXT,
				messageSeverity = VkDebugUtilsMessageSeverityFlagsEXT.Verbose | VkDebugUtilsMessageSeverityFlagsEXT.Warning | VkDebugUtilsMessageSeverityFlagsEXT.Error,
				messageType = VkDebugUtilsMessageTypeFlagsEXT.Validation | VkDebugUtilsMessageTypeFlagsEXT.Performance,
				pfnUserCallback = Marshal.GetFunctionPointerForDelegate(debugMessengerCallbackFunc)
			};
			return createInfo;
		}

		private static bool IsValidationSupported()
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

		private VkStringArray GetRequiredLayers()
		{
			List<string> layers = new List<string> { "VK_LAYER_LUNARG_monitor" };

			if (ValidationEnabled) layers.Add("VK_LAYER_KHRONOS_validation");

			return new VkStringArray(layers);
		}
	}
}