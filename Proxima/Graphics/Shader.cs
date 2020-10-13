using System.Collections.Generic;
using System.IO;
using Vortice.Vulkan;

namespace Proxima.Graphics
{
	public class Shader : GraphicsObject
	{
		private VkShaderModule shaderModule;

		public List<VkPipelineShaderStageCreateInfo> Stages { get; }

		public Shader(GraphicsDevice graphicsDevice, string path) : base(graphicsDevice)
		{
			byte[] data = File.ReadAllBytes(path);
			shaderModule = CreateShaderModule(graphicsDevice.LogicalDevice, data);
			Stages = new List<VkPipelineShaderStageCreateInfo>();

			var (shaderStages, entryPoints) = SPIRV.GetStages(path);
			if ((shaderStages & ShaderStage.Vertex) == ShaderStage.Vertex)
			{
				VkPipelineShaderStageCreateInfo createInfo = new VkPipelineShaderStageCreateInfo
				{
					sType = VkStructureType.PipelineShaderStageCreateInfo,
					stage = VkShaderStageFlags.Vertex,
					module = shaderModule,
					pName = new VkString(entryPoints[ShaderStage.Vertex])
				};
				Stages.Add(createInfo);
			}

			if ((shaderStages & ShaderStage.Fragment) == ShaderStage.Fragment)
			{
				VkPipelineShaderStageCreateInfo createInfo = new VkPipelineShaderStageCreateInfo
				{
					sType = VkStructureType.PipelineShaderStageCreateInfo,
					stage = VkShaderStageFlags.Fragment,
					module = shaderModule,
					pName = new VkString(entryPoints[ShaderStage.Fragment])
				};
				Stages.Add(createInfo);
			}
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
			Vulkan.vkDestroyShaderModule(graphicsDevice.LogicalDevice, shaderModule, null);
		}
	}
}