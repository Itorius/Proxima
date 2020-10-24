using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Vortice.Vulkan;

namespace Proxima.Graphics
{
	public class ReflectionData
	{
		public struct EntryPoint
		{
			[JsonPropertyName("name")] public string Name { get; set; }

			[JsonPropertyName("mode")] public string Mode { get; set; }
		}

		public struct Member
		{
			[JsonPropertyName("name")] public string Name { get; set; }

			[JsonPropertyName("type")] public string Type { get; set; }

			[JsonPropertyName("array")] public List<int>? Array { get; set; }

			[JsonPropertyName("array_size_is_literal")]
			public List<bool>? ArraySizeIsLiteral { get; set; }

			[JsonPropertyName("offset")] public int? Offset { get; set; }

			[JsonPropertyName("matrix_stride")] public int? MatrixStride { get; set; }
		}

		public struct Type
		{
			[JsonPropertyName("name")] public string Name { get; set; }

			[JsonPropertyName("members")] public List<Member> Members { get; set; }
		}

		public struct Input
		{
			[JsonPropertyName("type")] public string Type { get; set; }

			[JsonPropertyName("name")] public string Name { get; set; }

			[JsonPropertyName("location")] public int Location { get; set; }
		}

		public struct Output
		{
			[JsonPropertyName("type")] public string Type { get; set; }

			[JsonPropertyName("name")] public string Name { get; set; }

			[JsonPropertyName("location")] public int Location { get; set; }
		}

		public struct Ubo
		{
			[JsonPropertyName("type")] public string Type { get; set; }

			[JsonPropertyName("name")] public string Name { get; set; }

			[JsonPropertyName("block_size")] public int BlockSize { get; set; }

			[JsonPropertyName("set")] public uint Set { get; set; }

			[JsonPropertyName("binding")] public uint Binding { get; set; }
		}

		public struct Texture
		{
			[JsonPropertyName("type")] public string Type { get; set; }

			[JsonPropertyName("name")] public string Name { get; set; }

			[JsonPropertyName("set")] public uint Set { get; set; }

			[JsonPropertyName("binding")] public uint Binding { get; set; }
		}

		[JsonPropertyName("entryPoints")] public List<EntryPoint> EntryPoints { get; set; } = new List<EntryPoint>();

		[JsonPropertyName("types")] public Dictionary<string, Type> Types { get; set; } = new Dictionary<string, Type>();

		[JsonPropertyName("inputs")] public List<Input> Inputs { get; set; } = new List<Input>();

		[JsonPropertyName("outputs")] public List<Output> Outputs { get; set; } = new List<Output>();

		[JsonPropertyName("ubos")] public List<Ubo> UBOs { get; set; } = new List<Ubo>();

		[JsonPropertyName("textures")] public List<Texture> Textures { get; set; } = new List<Texture>();
	}

	public class Shader : GraphicsObject
	{
		private List<VkShaderModule> shaderModules;

		public List<VkPipelineShaderStageCreateInfo> Stages { get; }

		public Dictionary<VkShaderStageFlags, ReflectionData> ReflectionData { get; } = new Dictionary<VkShaderStageFlags, ReflectionData>();

		internal Shader(GraphicsDevice graphicsDevice, string path) : base(graphicsDevice)
		{
			shaderModules = new List<VkShaderModule>();
			Stages = new List<VkPipelineShaderStageCreateInfo>();

			ProcessShader(path);
		}

		private void ProcessShader(string path)
		{
			using Stream stream = File.OpenRead(path + ".pxshader");
			using BinaryReader reader = new BinaryReader(stream);

			while (stream.Position != stream.Length)
			{
				string stage = reader.ReadString();
				VkShaderStageFlags vkStage = stage == "vertex" ? VkShaderStageFlags.Vertex : VkShaderStageFlags.Fragment;

				string reflectionDataStr = reader.ReadString();
				var reflectionData = JsonSerializer.Deserialize<ReflectionData>(reflectionDataStr);
				ReflectionData.Add(vkStage, reflectionData);

				uint size = reader.ReadUInt32();
				byte[] data = reader.ReadBytes((int)size);

				var shaderModule = CreateShaderModule(graphicsDevice.LogicalDevice, data);
				shaderModules.Add(shaderModule);

				if (reflectionData != null)
				{
					foreach (ReflectionData.EntryPoint entryPoint in reflectionData.EntryPoints)
					{
						VkPipelineShaderStageCreateInfo createInfo = new VkPipelineShaderStageCreateInfo
						{
							sType = VkStructureType.PipelineShaderStageCreateInfo,
							stage = EntryPointModeToStage(entryPoint.Mode),
							module = shaderModule,
							pName = new VkString(entryPoint.Name)
						};
						Stages.Add(createInfo);
					}
				}
			}
		}

		private static VkShaderStageFlags EntryPointModeToStage(string mode) => mode switch
		{
			"vert" => VkShaderStageFlags.Vertex,
			"frag" => VkShaderStageFlags.Fragment,
			_ => throw new Exception("Unsupported execution model " + mode)
		};

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
			foreach (VkShaderModule shaderModule in shaderModules) Vulkan.vkDestroyShaderModule(graphicsDevice.LogicalDevice, shaderModule, null);
		}
	}
}