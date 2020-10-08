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
			VkBuffer = buffer;
			Memory = memory;
		}

		public override unsafe void SetData(object data)
		{
			T p = (T)data;

			void* dataPtr = null;
			Vulkan.vkMapMemory(graphicsDevice.LogicalDevice, Memory, 0, Size, 0, &dataPtr);
			System.Buffer.MemoryCopy(&p, dataPtr, Size, Size);
			Vulkan.vkUnmapMemory(graphicsDevice.LogicalDevice, Memory);
		}
	}
}