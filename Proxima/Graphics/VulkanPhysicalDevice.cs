using System;
using Vortice.Vulkan;

namespace Proxima.Graphics
{
	public class VulkanPhysicalDevice : VulkanObject
	{
		private VkPhysicalDevice device;
		
		public override IntPtr Handle => device.Handle;

		public string Name
		{
			get
			{
				Vulkan.vkGetPhysicalDeviceProperties(this, out VkPhysicalDeviceProperties properties);
				return properties.GetDeviceName();
			}
		}
		
		public static implicit operator VkPhysicalDevice(VulkanPhysicalDevice device) => device.device;

		public VulkanPhysicalDevice(GraphicsDevice graphicsDevice, VkPhysicalDevice physicalDevice) : base(graphicsDevice)
		{
			device = physicalDevice;
		}

		public override void Dispose()
		{
			
		}
	}
}