using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using shaderc;
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

			public VkShaderStageFlags Stage { get; set; }
		}

		public struct Output
		{
			[JsonPropertyName("type")] public string Type { get; set; }

			[JsonPropertyName("name")] public string Name { get; set; }

			[JsonPropertyName("location")] public int Location { get; set; }

			public VkShaderStageFlags Stage { get; set; }
		}

		public struct Ubo
		{
			[JsonPropertyName("type")] public string Type { get; set; }

			[JsonPropertyName("name")] public string Name { get; set; }

			[JsonPropertyName("block_size")] public int BlockSize { get; set; }

			[JsonPropertyName("set")] public uint Set { get; set; }

			[JsonPropertyName("binding")] public uint Binding { get; set; }

			public VkShaderStageFlags Stage { get; set; }
		}

		public struct Texture
		{
			[JsonPropertyName("type")] public string Type { get; set; }

			[JsonPropertyName("name")] public string Name { get; set; }

			[JsonPropertyName("set")] public uint Set { get; set; }

			[JsonPropertyName("binding")] public uint Binding { get; set; }

			public VkShaderStageFlags Stage { get; set; }
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

		public ReflectionData ReflectionData;

		internal Shader(GraphicsDevice graphicsDevice, string path) : base(graphicsDevice)
		{
			shaderModules = new List<VkShaderModule>();
			Stages = new List<VkPipelineShaderStageCreateInfo>();
			ReflectionData = new ReflectionData();

			ProcessShader(path);
		}

		internal unsafe Shader(GraphicsDevice graphicsDevice, string vertexPath, string fragmentPath) : base(graphicsDevice)
		{
			shaderModules = new List<VkShaderModule>();
			Stages = new List<VkPipelineShaderStageCreateInfo>();
			ReflectionData = new ReflectionData();

			using Compiler comp = new Compiler(new Options { SourceLanguage = SourceLanguage.Glsl });
			using (Result res = comp.Compile(vertexPath, ShaderKind.VertexShader))
			{
				VkShaderModuleCreateInfo createInfo = new VkShaderModuleCreateInfo
				{
					sType = VkStructureType.ShaderModuleCreateInfo,
					codeSize = new VkPointerSize(res.CodeLength),
					pCode = (uint*)res.CodePointer.ToPointer()
				};

				Vulkan.vkCreateShaderModule(graphicsDevice.LogicalDevice, &createInfo, null, out VkShaderModule module).CheckResult();
				shaderModules.Add(module);
			}

			using (Result res = comp.Compile(fragmentPath, ShaderKind.FragmentShader))
			{
				VkShaderModuleCreateInfo createInfo = new VkShaderModuleCreateInfo
				{
					sType = VkStructureType.ShaderModuleCreateInfo,
					codeSize = new VkPointerSize(res.CodeLength),
					pCode = (uint*)res.CodePointer.ToPointer()
				};

				Vulkan.vkCreateShaderModule(graphicsDevice.LogicalDevice, &createInfo, null, out VkShaderModule module).CheckResult();
				shaderModules.Add(module);
			}

			using Stream stream = File.OpenRead($"{Path.GetDirectoryName(vertexPath)}/{Path.GetFileNameWithoutExtension(vertexPath)}.pxshader");
			using BinaryReader reader = new BinaryReader(stream);

			int i = 0;
			while (stream.Position != stream.Length)
			{
				string stage = reader.ReadString();
				VkShaderStageFlags vkStage = stage switch
				{
					"vertex" => VkShaderStageFlags.Vertex,
					"fragment" => VkShaderStageFlags.Fragment,
					_ => throw new Exception("Unsupported stage " + stage)
				};

				string reflectionDataStr = reader.ReadString();
				var reflectionData = JsonSerializer.Deserialize<ReflectionData>(reflectionDataStr);

				uint size = reader.ReadUInt32();
				byte[] data = reader.ReadBytes((int)size);

				// var shaderModule = CreateShaderModule(graphicsDevice.LogicalDevice, data);
				// shaderModules.Add(shaderModule);

				if (reflectionData != null)
				{
					ReflectionData.UBOs.AddRange(reflectionData.UBOs.Select(ubo =>
					{
						ubo.Stage = vkStage;
						return ubo;
					}));

					ReflectionData.Textures.AddRange(reflectionData.Textures.Select(texture =>
					{
						texture.Stage = vkStage;
						return texture;
					}));

					ReflectionData.Inputs.AddRange(reflectionData.Inputs.Select(input =>
					{
						input.Stage = vkStage;
						return input;
					}));

					ReflectionData.Outputs.AddRange(reflectionData.Outputs.Select(output =>
					{
						output.Stage = vkStage;
						return output;
					}));

					foreach (ReflectionData.EntryPoint entryPoint in reflectionData.EntryPoints)
					{
						VkPipelineShaderStageCreateInfo createInfo = new VkPipelineShaderStageCreateInfo
						{
							sType = VkStructureType.PipelineShaderStageCreateInfo,
							stage = EntryPointModeToStage(entryPoint.Mode),
							module = shaderModules[i++],
							pName = new VkString(entryPoint.Name)
						};
						Stages.Add(createInfo);
					}
				}
			}
		}

		private void ProcessShader(string path)
		{
			using Stream stream = File.OpenRead(path + ".pxshader");
			using BinaryReader reader = new BinaryReader(stream);

			while (stream.Position != stream.Length)
			{
				string stage = reader.ReadString();
				VkShaderStageFlags vkStage = stage switch
				{
					"vertex" => VkShaderStageFlags.Vertex,
					"fragment" => VkShaderStageFlags.Fragment,
					_ => throw new Exception("Unsupported stage " + stage)
				};

				string reflectionDataStr = reader.ReadString();
				var reflectionData = JsonSerializer.Deserialize<ReflectionData>(reflectionDataStr);

				uint size = reader.ReadUInt32();
				byte[] data = reader.ReadBytes((int)size);

				var shaderModule = CreateShaderModule(graphicsDevice.LogicalDevice, data);
				shaderModules.Add(shaderModule);

				if (reflectionData != null)
				{
					ReflectionData.UBOs.AddRange(reflectionData.UBOs.Select(ubo =>
					{
						ubo.Stage = vkStage;
						return ubo;
					}));

					ReflectionData.Textures.AddRange(reflectionData.Textures.Select(texture =>
					{
						texture.Stage = vkStage;
						return texture;
					}));

					ReflectionData.Inputs.AddRange(reflectionData.Inputs.Select(input =>
					{
						input.Stage = vkStage;
						return input;
					}));

					ReflectionData.Outputs.AddRange(reflectionData.Outputs.Select(output =>
					{
						output.Stage = vkStage;
						return output;
					}));

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