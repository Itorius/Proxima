using System;
using Vortice.Vulkan;

namespace Proxima.Graphics
{
	public abstract class UniformBuffer : Buffer
	{
		protected UniformBuffer(GraphicsDevice graphicsDevice) : base(graphicsDevice)
		{
		}

		public abstract void SetData<T>(T data) where T : unmanaged;
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

		public override unsafe void SetData<K>(K data)
		{
			if (sizeof(K) != sizeof(T)) throw new Exception($"Wrong uniform data type, expected '{typeof(T).FullName}'");

			void* dataPtr = null;
			Vulkan.vkMapMemory(graphicsDevice.LogicalDevice, memory, 0, Size, 0, &dataPtr);
			System.Buffer.MemoryCopy(&data, dataPtr, Size, Size);
			Vulkan.vkUnmapMemory(graphicsDevice.LogicalDevice, memory);
		}
	}
}