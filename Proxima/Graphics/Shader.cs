using System.IO;
using Vortice.Vulkan;

namespace Proxima.Graphics
{
	public class Shader : GraphicsObject
	{
		private VkShaderModule vertexModule, fragmentModule;

		public VkPipelineShaderStageCreateInfo[] Stages { get; }

		public Shader(GraphicsDevice graphicsDevice, string vertexPath, string fragmentPath) : base(graphicsDevice)
		{
			var vertex = File.ReadAllBytes(vertexPath);
			var fragment = File.ReadAllBytes(fragmentPath);

			vertexModule = CreateShaderModule(graphicsDevice.LogicalDevice, vertex);
			fragmentModule = CreateShaderModule(graphicsDevice.LogicalDevice, fragment);

			VkPipelineShaderStageCreateInfo vertexCreateInfo = new VkPipelineShaderStageCreateInfo
			{
				sType = VkStructureType.PipelineShaderStageCreateInfo,
				stage = VkShaderStageFlags.Vertex,
				module = vertexModule,
				pName = new VkString("main")
			};

			VkPipelineShaderStageCreateInfo fragmentCreateInfo = new VkPipelineShaderStageCreateInfo
			{
				sType = VkStructureType.PipelineShaderStageCreateInfo,
				stage = VkShaderStageFlags.Fragment,
				module = fragmentModule,
				pName = new VkString("main")
			};

			Stages = new[] { vertexCreateInfo, fragmentCreateInfo };
		}

		private static unsafe VkShaderModule CreateShaderModule(VkDevice device, byte[] data)
		{
			VkShaderModuleCreateInfo createInfo = new VkShaderModuleCreateInfo
			{
				sType = VkStructureType.ShaderModuleCreateInfo,
				codeSize = new VkPointerSize((uint)data.Length)
			};

			fixed (byte* ptr = data) createInfo.pCode = (uint*)ptr;

			Vulkan.vkCreateShaderModule(device, &createInfo, null, out VkShaderModule module).CheckResult();
			return module;
		}

		public override unsafe void Dispose()
		{
			Vulkan.vkDestroyShaderModule(graphicsDevice.LogicalDevice, vertexModule, null);
			Vulkan.vkDestroyShaderModule(graphicsDevice.LogicalDevice, fragmentModule, null);
		}
	}
}