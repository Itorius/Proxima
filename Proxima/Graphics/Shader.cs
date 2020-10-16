using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Vortice.Vulkan;

namespace Proxima.Graphics
{
	public class Shader : GraphicsObject
	{
		private VkShaderModule shaderModule;

		public List<VkPipelineShaderStageCreateInfo> Stages { get; }
		public List<(VkDescriptorType type, uint set, uint binding)> DescriptorTypes { get; }

		public unsafe Shader(GraphicsDevice graphicsDevice, string path) : base(graphicsDevice)
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

			DescriptorTypes = new List<(VkDescriptorType type, uint set, uint binding)>();

			SPIRV.CreateContext(out IntPtr context);

			SPIRV.SPIRVCallback callback = (data, error) => { Log.Debug(Marshal.PtrToStringAuto(error)); };
			SPIRV.SetDebugCallback(context, Marshal.GetFunctionPointerForDelegate(callback), IntPtr.Zero);

			IntPtr ir = IntPtr.Zero;
			fixed (byte* ptr = data) SPIRV.ParseSPIRV(context, new IntPtr(ptr), (uint)(data.Length / 4), out ir);

			SPIRV.CreateCompiler(context, SpirvBackend.GLSL, ir, SpirvCaptureMode.TakeOwnership, out IntPtr compiler);

			SPIRV.CreateShaderResources(compiler, out IntPtr resources);

			var list = SPIRV.GetResourceListForType(resources, SpirvResourceType.UniformBuffer);

			foreach (SpirvReflectedResource resource in list)
			{
				DescriptorTypes.Add((
					VkDescriptorType.UniformBuffer,
					SPIRV.GetDecoration(compiler, resource.ID, SpirvDecoration.DescriptorSet),
					SPIRV.GetDecoration(compiler, resource.ID, SpirvDecoration.Binding)
				));
			}

			list = SPIRV.GetResourceListForType(resources, SpirvResourceType.SampledImage);

			foreach (SpirvReflectedResource resource in list)
			{
				DescriptorTypes.Add((
					VkDescriptorType.CombinedImageSampler,
					SPIRV.GetDecoration(compiler, resource.ID, SpirvDecoration.DescriptorSet),
					SPIRV.GetDecoration(compiler, resource.ID, SpirvDecoration.Binding)
				));
			}

			SPIRV.DestroyContext(context);
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