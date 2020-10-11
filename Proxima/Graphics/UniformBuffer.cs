using System;
using Vortice.Vulkan;

namespace Proxima.Graphics
{
	public abstract class UniformBuffer : Buffer
	{
		protected UniformBuffer(GraphicsDevice graphicsDevice) : base(graphicsDevice)
		{
		}

		public abstract void SetData(object data);
	}

	public class UniformBuffer<T> : UniformBuffer where T : unmanaged
	{
		public unsafe UniformBuffer(GraphicsDevice graphicsDevice) : base(graphicsDevice)
		{
			this.graphicsDevice = graphicsDevice;

			Size = (ulong)sizeof(T);

			var (buffer, memory) = VulkanUtils.CreateBuffer(graphicsDevice, Size, VkBufferUsageFlags.UniformBuffer, VkMemoryPropertyFlags.HostVisible | VkMemoryPropertyFlags.HostCoherent);
			this.buffer = buffer;
			this.memory = memory;
		}

		public override unsafe void SetData(object data)
		{
			if (!(data is T p)) throw new Exception($"Wrong uniform data type, expected '{typeof(T).FullName}'");

			void* dataPtr = null;
			Vulkan.vkMapMemory(graphicsDevice.LogicalDevice, memory, 0, Size, 0, &dataPtr);
			System.Buffer.MemoryCopy(&p, dataPtr, Size, Size);
			Vulkan.vkUnmapMemory(graphicsDevice.LogicalDevice, memory);
		}
	}
}