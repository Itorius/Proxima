using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace VulkanGenerator
{
	public static class Generator
	{
		private static readonly Dictionary<string, string> s_knownEnumValueNames = new Dictionary<string, string>
		{
			{ "VK_STENCIL_FRONT_AND_BACK", "FrontAndBack" },
			{ "VK_STRUCTURE_TYPE_MEMORY_ALLOCATE_FLAGS_INFO", "MemoryAllocateFlagsInfo" },

			// VkSampleCountFlagBits
			{ "VK_SAMPLE_COUNT_1_BIT", "Count1" },
			{ "VK_SAMPLE_COUNT_2_BIT", "Count2" },
			{ "VK_SAMPLE_COUNT_4_BIT", "Count4" },
			{ "VK_SAMPLE_COUNT_8_BIT", "Count8" },
			{ "VK_SAMPLE_COUNT_16_BIT", "Count16" },
			{ "VK_SAMPLE_COUNT_32_BIT", "Count32" },
			{ "VK_SAMPLE_COUNT_64_BIT", "Count64" },

			// VkImageType
			{ "VK_IMAGE_TYPE_1D", "Image1D" },
			{ "VK_IMAGE_TYPE_2D", "Image2D" },
			{ "VK_IMAGE_TYPE_3D", "Image3D" },

			// VkImageViewType
			{ "VK_IMAGE_VIEW_TYPE_1D", "Image1D" },
			{ "VK_IMAGE_VIEW_TYPE_2D", "Image2D" },
			{ "VK_IMAGE_VIEW_TYPE_3D", "Image3D" },
			{ "VK_IMAGE_VIEW_TYPE_CUBE", "ImageCube" },
			{ "VK_IMAGE_VIEW_TYPE_1D_ARRAY", "Image1DArray" },
			{ "VK_IMAGE_VIEW_TYPE_2D_ARRAY", "Image2DArray" },
			{ "VK_IMAGE_VIEW_TYPE_CUBE_ARRAY", "ImageCubeArray" },

			// VkColorSpaceKHR
			{ "VK_COLOR_SPACE_SRGB_NONLINEAR_KHR", "SrgbNonLinearKHR" },
			{ "VK_COLOR_SPACE_DISPLAY_P3_NONLINEAR_EXT", "DisplayP3NonLinearEXT" },
			{ "VK_COLOR_SPACE_DCI_P3_NONLINEAR_EXT", "DciP3NonLinearEXT" },
			{ "VK_COLOR_SPACE_BT709_NONLINEAR_EXT", "Bt709NonLinearEXT" },
			{ "VK_COLOR_SPACE_DOLBYVISION_EXT", "DolbyVisionEXT" },
			{ "VK_COLOR_SPACE_ADOBERGB_LINEAR_EXT", "AdobeRgbLinearEXT" },
			{ "VK_COLOR_SPACE_ADOBERGB_NONLINEAR_EXT", "AdobeRgbNonLinearEXT" },
			{ "VK_COLOR_SPACE_EXTENDED_SRGB_NONLINEAR_EXT", "ExtendedSrgbNonLinearEXT" },
			{ "VK_COLORSPACE_SRGB_NONLINEAR_KHR", "SrgbNonLinearKHR" },

			// VkShadingRatePaletteEntryNV
			{ "VK_SHADING_RATE_PALETTE_ENTRY_16_INVOCATIONS_PER_PIXEL_NV", "SixteenInvocationsPerPixel" },
			{ "VK_SHADING_RATE_PALETTE_ENTRY_8_INVOCATIONS_PER_PIXEL_NV", "EightInvocationsPerPixel" },
			{ "VK_SHADING_RATE_PALETTE_ENTRY_4_INVOCATIONS_PER_PIXEL_NV", "FourInvocationsPerPixel" },
			{ "VK_SHADING_RATE_PALETTE_ENTRY_2_INVOCATIONS_PER_PIXEL_NV", "TwoInvocationsPerPixel" },
			{ "VK_SHADING_RATE_PALETTE_ENTRY_1_INVOCATION_PER_PIXEL_NV", "OneInvocationPerPixel" },
			{ "VK_SHADING_RATE_PALETTE_ENTRY_1_INVOCATION_PER_2X1_PIXELS_NV", "OneInvocationPer2x1Pixels" },
			{ "VK_SHADING_RATE_PALETTE_ENTRY_1_INVOCATION_PER_1X2_PIXELS_NV", "OneInvocationPer1x2Pixels" },
			{ "VK_SHADING_RATE_PALETTE_ENTRY_1_INVOCATION_PER_2X2_PIXELS_NV", "OneInvocationPer2x2Pixels" },
			{ "VK_SHADING_RATE_PALETTE_ENTRY_1_INVOCATION_PER_4X2_PIXELS_NV", "OneInvocationPer4x2Pixels" },
			{ "VK_SHADING_RATE_PALETTE_ENTRY_1_INVOCATION_PER_2X4_PIXELS_NV", "OneInvocationPer2x4Pixels" },
			{ "VK_SHADING_RATE_PALETTE_ENTRY_1_INVOCATION_PER_4X4_PIXELS_NV", "OneInvocationPer4x4Pixels" },

			// VkFragmentShadingRateNV
			{ "VK_FRAGMENT_SHADING_RATE_1_INVOCATION_PER_PIXEL_NV", "OneInvocationPerPixel" },
			{ "VK_FRAGMENT_SHADING_RATE_1_INVOCATION_PER_1X2_PIXELS_NV", "OneInvocationPer1x2Pixels" },
			{ "VK_FRAGMENT_SHADING_RATE_1_INVOCATION_PER_2X1_PIXELS_NV", "OneInvocationPer2x1Pixels" },
			{ "VK_FRAGMENT_SHADING_RATE_1_INVOCATION_PER_2X2_PIXELS_NV", "OneInvocationPer2x2Pixels" },
			{ "VK_FRAGMENT_SHADING_RATE_1_INVOCATION_PER_2X4_PIXELS_NV", "OneInvocationPer2x4Pixels" },
			{ "VK_FRAGMENT_SHADING_RATE_1_INVOCATION_PER_4X2_PIXELS_NV", "OneInvocationPer4x2Pixels" },
			{ "VK_FRAGMENT_SHADING_RATE_1_INVOCATION_PER_4X4_PIXELS_NV", "OneInvocationPer4x4Pixels" },
			{ "VK_FRAGMENT_SHADING_RATE_2_INVOCATIONS_PER_PIXEL_NV", "TwoInvocationsPerPixel" },
			{ "VK_FRAGMENT_SHADING_RATE_4_INVOCATIONS_PER_PIXEL_NV", "FourInvocationsPerPixel" },
			{ "VK_FRAGMENT_SHADING_RATE_8_INVOCATIONS_PER_PIXEL_NV", "EightInvocationsPerPixel" },
			{ "VK_FRAGMENT_SHADING_RATE_16_INVOCATIONS_PER_PIXEL_NV", "SixteenInvocationsPerPixel" },
			{ "VK_FRAGMENT_SHADING_RATE_NO_INVOCATIONS_NV", "NoInvocations" },

			// VkDriverId
			{ "VK_DRIVER_ID_GOOGLE_SWIFTSHADER", "GoogleSwiftShader" },
			{ "VK_DRIVER_ID_GOOGLE_SWIFTSHADER_KHR", "GoogleSwiftShaderKHR" },
			{ "VK_DRIVER_ID_MESA_LLVMPIPE", "MesaLLVMPipe" },

			// VkGeometryTypeNV
			{ "VK_GEOMETRY_TYPE_TRIANGLES_NV", "Triangles" },
			{ "VK_GEOMETRY_TYPE_AABBS_NVX", "AABBs" },

			// VkCopyAccelerationStructureModeNV
			{ "VK_COPY_ACCELERATION_STRUCTURE_MODE_CLONE_NV", "Clone" },
			{ "VK_COPY_ACCELERATION_STRUCTURE_MODE_COMPACT_NV", "Compact" },

			// VkAccelerationStructureTypeNV
			{ "VK_ACCELERATION_STRUCTURE_TYPE_TOP_LEVEL_NV", "TopLevel" },
			{ "VK_ACCELERATION_STRUCTURE_TYPE_BOTTOM_LEVEL_NV", "BottomLevel" },

			// VkAccelerationStructureMemoryRequirementsTypeNV
			{ "VK_ACCELERATION_STRUCTURE_MEMORY_REQUIREMENTS_TYPE_OBJECT_NV", "Object" },
			{ "VK_ACCELERATION_STRUCTURE_MEMORY_REQUIREMENTS_TYPE_BUILD_SCRATCH_NV", "BuildScratch" },
			{ "VK_ACCELERATION_STRUCTURE_MEMORY_REQUIREMENTS_TYPE_UPDATE_SCRATCH_NV", "UpdateScratch" },

			// VkRayTracingShaderGroupTypeNV
			{ "VK_RAY_TRACING_SHADER_GROUP_TYPE_GENERAL_NV", "General" },
			{ "VK_RAY_TRACING_SHADER_GROUP_TYPE_TRIANGLES_HIT_GROUP_NV", "TrianglesHitGroup" },
			{ "VK_RAY_TRACING_SHADER_GROUP_TYPE_PROCEDURAL_HIT_GROUP_NV", "ProceduralHitGroup" },

			// VkPerformanceCounterScopeKHR
			{ "VK_QUERY_SCOPE_COMMAND_BUFFER_KHR", "QueryScopeCommandBufferKHR" },
			{ "VK_QUERY_SCOPE_RENDER_PASS_KHR", "QueryScopeRenderPassKHR" },
			{ "VK_QUERY_SCOPE_COMMAND_KHR", "QueryScopeCommandKHR" },

			// VkPerformanceConfigurationTypeINTEL
			{ "VK_PERFORMANCE_CONFIGURATION_TYPE_COMMAND_QUEUE_METRICS_DISCOVERY_ACTIVATED_INTEL", "CommandQueueMetricsDiscoveryActivatedIntel" }
		};

		private record Enum(string name, bool bitmask, List<EnumMember> members);

		private record EnumMember(string name, string value);

		private static List<string> flags = new();

		public static string GetEnumNamePrefix(string typeName)
		{
			// if (s_knownEnumPrefixes.TryGetValue(typeName, out string? knownValue))
			// {
			//     return knownValue;
			// }

			List<string> parts = new List<string>(4);
			int chunkStart = 0;
			for (int i = 0; i < typeName.Length; i++)
			{
				if (char.IsUpper(typeName[i]))
				{
					if (chunkStart != i)
					{
						parts.Add(typeName.Substring(chunkStart, i - chunkStart));
					}

					chunkStart = i;
					if (i == typeName.Length - 1)
					{
						parts.Add(typeName.Substring(i, 1));
					}
				}
				else if (i == typeName.Length - 1)
				{
					parts.Add(typeName.Substring(chunkStart, typeName.Length - chunkStart));
				}
			}

			for (int i = 0; i < parts.Count; i++)
			{
				if (parts[i] == "Flag" ||
				    parts[i] == "Flags" ||
				    parts[i] == "K" && i + 2 < parts.Count && parts[i + 1] == "H" && parts[i + 2] == "R" ||
				    parts[i] == "A" && i + 2 < parts.Count && parts[i + 1] == "M" && parts[i + 2] == "D" ||
				    parts[i] == "E" && i + 2 < parts.Count && parts[i + 1] == "X" && parts[i + 2] == "T" ||
				    parts[i] == "Type" && i + 2 < parts.Count && parts[i + 1] == "N" && parts[i + 2] == "V" ||
				    parts[i] == "Type" && i + 3 < parts.Count && parts[i + 1] == "N" && parts[i + 2] == "V" && parts[i + 3] == "X" ||
				    parts[i] == "Scope" && i + 2 < parts.Count && parts[i + 1] == "N" && parts[i + 2] == "V" ||
				    parts[i] == "Mode" && i + 2 < parts.Count && parts[i + 1] == "N" && parts[i + 2] == "V" ||
				    parts[i] == "Mode" && i + 5 < parts.Count && parts[i + 1] == "I" && parts[i + 2] == "N" && parts[i + 3] == "T" && parts[i + 4] == "E" && parts[i + 5] == "L" ||
				    parts[i] == "Type" && i + 5 < parts.Count && parts[i + 1] == "I" && parts[i + 2] == "N" && parts[i + 3] == "T" && parts[i + 4] == "E" && parts[i + 5] == "L"
				)
				{
					parts = new List<string>(parts.Take(i));
					break;
				}
			}

			return string.Join("_", parts.Select(s => s.ToUpper()));
		}

		private static string GetName(string value, string enumPrefix)
		{
			if (s_knownEnumValueNames.TryGetValue(value, out string knownName))
			{
				return knownName;
			}

			if (value.IndexOf(enumPrefix) != 0)
			{
				return value;
			}

			string[] parts = value[enumPrefix.Length..].Split(new[] { '_' }, StringSplitOptions.RemoveEmptyEntries);

			var sb = new StringBuilder();
			foreach (string part in parts)
			{
				// if (s_ignoredParts.Contains(part))
				// {
				// 	continue;
				// }
				//
				// if (s_preserveCaps.Contains(part))
				// {
				// 	sb.Append(part);
				// }
				// else
				// {
				sb.Append(char.ToUpper(part[0]));
				for (int i = 1; i < part.Length; i++)
				{
					sb.Append(char.ToLower(part[i]));
				}

				// }
			}

			string prettyName = sb.ToString();
			return char.IsNumber(prettyName[0]) ? "_" + prettyName : prettyName;
		}

		internal static void GenerateEnums(XmlDocument xml)
		{
			
			

			StringBuilder code = new StringBuilder();

			code.AppendLine("using System;");
			code.AppendLine("namespace Fireburst {");
			foreach (var (name, bitmask, members) in enums)
			{
				if (bitmask) code.AppendLine("[Flags]");
				code.AppendLine($"public enum {name} {{");

				if (members.Count == 0)
				{
					code.AppendLine("None = 0");
				}
				else
				{
					string enumPrefix = GetEnumNamePrefix(name);

					foreach (var (mName, value) in members)
					{
						code.AppendLine($"{GetName(mName, enumPrefix)} = {value},");
					}
				}

				code.AppendLine("}");
			}


			code.Append('}');

			File.WriteAllText("/home/itorius/Development/Proxima/VulkanGenerator/Generated/Vulkan.Enums.cs", code.ToString(), Encoding.UTF8);
		}
	}
}