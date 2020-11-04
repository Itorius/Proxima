using System;
using System.Diagnostics;
using Vortice.Vulkan;

namespace Proxima.Graphics
{
	public abstract class VulkanObject
	{
		// internal static readonly List<WeakReference<GraphicsObject>> refs = new List<WeakReference<GraphicsObject>>();

		// public GraphicsObject() => refs.Add(new WeakReference<GraphicsObject>(this, false));

		protected GraphicsDevice graphicsDevice;

		public VulkanObject(GraphicsDevice graphicsDevice) => this.graphicsDevice = graphicsDevice;

		public virtual IntPtr Handle { get; }
		
		[Conditional("ENABLE_VALIDATION")]
		public unsafe void SetName(string name)
		{
			var info = new VkDebugUtilsObjectNameInfoEXT
			{
				sType = VkStructureType.DebugUtilsObjectNameInfoEXT,
				objectType = VkObjectType.CommandBuffer,
				objectHandle = (ulong)Handle,
				pObjectName = new VkString(name)
			};
			Vulkan.vkSetDebugUtilsObjectNameEXT(graphicsDevice.LogicalDevice, &info);
		}
		
		public abstract void Dispose();
	}
}