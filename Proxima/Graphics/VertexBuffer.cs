using Vortice.Vulkan;

namespace Proxima.Graphics
{
	public class VertexBuffer<T> : Buffer where T : unmanaged
	{
		private readonly VkDeviceMemory memory;

		public VkBuffer Buffer { get; }

		public unsafe VertexBuffer(GraphicsDevice graphicsDevice, T[] data) : base(graphicsDevice)
		{
			this.graphicsDevice = graphicsDevice;

			ulong bufferSize = (ulong)(sizeof(T) * data.Length);
			var (stagingBuffer, stagingBufferMemory) = VulkanUtils.CreateBuffer(graphicsDevice, bufferSize, VkBufferUsageFlags.TransferSrc, VkMemoryPropertyFlags.HostVisible | VkMemoryPropertyFlags.HostCoherent);

			void* dataPtr = null;
			Vulkan.vkMapMemory(graphicsDevice.LogicalDevice, stagingBufferMemory, 0, bufferSize, 0, &dataPtr);
			fixed (T* ptr = &data[0]) System.Buffer.MemoryCopy(ptr, dataPtr, bufferSize, bufferSize);
			Vulkan.vkUnmapMemory(graphicsDevice.LogicalDevice, stagingBufferMemory);

			var (vkBuffer, vkDeviceMemory) = VulkanUtils.CreateBuffer(graphicsDevice, bufferSize, VkBufferUsageFlags.TransferDst | VkBufferUsageFlags.VertexBuffer, VkMemoryPropertyFlags.DeviceLocal);
			Buffer = vkBuffer;
			memory = vkDeviceMemory;

			VulkanUtils.CopyBuffer(graphicsDevice, stagingBuffer, Buffer, bufferSize);

			Vulkan.vkDestroyBuffer(graphicsDevice.LogicalDevice, stagingBuffer, null);
			Vulkan.vkFreeMemory(graphicsDevice.LogicalDevice, stagingBufferMemory, null);
		}

		public override unsafe void Dispose()
		{
			Vulkan.vkDestroyBuffer(graphicsDevice.LogicalDevice, Buffer, null);
			Vulkan.vkFreeMemory(graphicsDevice.LogicalDevice, memory, null);
		}
	}
}