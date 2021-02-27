using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

// ReSharper disable PossibleNullReferenceException

namespace VulkanGenerator
{
	public static class Program
	{
		private static List<string> PointerTypes = new()
		{
			"ANativeWindow",
			"wl_display",
			"wl_surface",
			"HINSTANCE",
			"HWND",
			"HMONITOR",
			"Display",
			"Window",
			"xcb_connection_t",
			"xcb_window_t",
			"IDirectFB",
			"IDirectFBSurface",
			"zx_handle_t",
			"GgpStreamDescriptor",
			"HANDLE",
			"DWORD",
			"SECURITY_ATTRIBUTES",
			"LPCWSTR",
			"CAMetalLayer"
		};

		public static string ResolveType(string orig)
		{
			if (PointerTypes.Contains(orig)) return "IntPtr";

			return orig switch
			{
				"size_t" => "VkPointerSize",
				"int8_t" => "sbyte",
				"int16_t" => "short",
				"int32_t" => "int",
				"int64_t" => "long",
				"uint8_t" => "byte",
				"uint16_t" => "ushort",
				"uint32_t" => "uint",
				"uint64_t" => "ulong",
				"VkDeviceSize" => "ulong",
				"VkSampleMask" => "uint",
				"VkDeviceAddress" => "IntPtr",
				_ => orig
			};
		}

		public static void Main(string[] args)
		{
			XmlDocument xml = new XmlDocument();
			xml.Load(@"/home/itorius/Development/Proxima/vk.xml");

			GenerateStructs(xml);
			Generator.GenerateEnums(xml);
			GenerateHandles(xml);
		}

		private static void GenerateHandles(XmlDocument xml)
		{
			StringBuilder code = new StringBuilder();

			code.AppendLine("using System;");
			code.AppendLine("using System.Runtime.InteropServices;");
			code.AppendLine("namespace Fireburst {");

			XmlNodeList itemRefList = xml.GetElementsByTagName("type");
			foreach (XmlElement xn in itemRefList)
			{
				if (xn.GetAttributeNode("category") is not { Value: "handle" }) continue;

				var xmlNode = xn.GetElementsByTagName("name")[0];
				if (xmlNode == null) continue;
				string name = xmlNode.InnerText;

				string template = $@"
									public partial struct {name} : IEquatable<{name}>
									{{
										public readonly ulong Handle;
										public {name}(ulong handle) {{ Handle = handle; }}
										public static {name} Null => new(0);
										public static implicit operator {name}(ulong handle) => new(handle);
										public static bool operator ==({name} left, {name} right) => left.Handle == right.Handle;
										public static bool operator !=({name} left, {name} right) => left.Handle != right.Handle;
										public static bool operator ==({name} left, ulong right) => left.Handle == right;
										public static bool operator !=({name} left, ulong right) => left.Handle != right;
										public bool Equals(ref {name} other) => Handle == other.Handle;
										public bool Equals({name} other) => Handle == other.Handle;
										public override bool Equals(object obj) => obj is {name} handle && Equals(ref handle);
										public override int GetHashCode() => Handle.GetHashCode();
										private string DebuggerDisplay => $""{name} [0x{{Handle:X}}]"";
									}}";

				code.AppendLine(template);
			}

			code.Append('}');

			File.WriteAllText("/home/itorius/Development/Proxima/VulkanGenerator/Generated/Vulkan.Handles.cs", code.ToString(), Encoding.UTF8);
		}

		private static void GenerateStructs(XmlDocument xml)
		{
			StringBuilder code = new StringBuilder();

			code.AppendLine("// ReSharper disable FieldCanBeMadeReadOnly.Global");
			code.AppendLine("using System;");
			code.AppendLine("using System.Runtime.InteropServices;");
			code.AppendLine("namespace Fireburst {");

			XmlNodeList itemRefList = xml.GetElementsByTagName("type");
			foreach (XmlElement xn in itemRefList)
			{
				if (xn.GetAttributeNode("category") is not { Value: "struct" }) continue;

				code.AppendLine("[StructLayout(LayoutKind.Sequential)]");
				code.AppendLine($"public struct {xn.GetAttributeNode("name").Value} {{");

				foreach (XmlElement member in xn.GetElementsByTagName("member"))
				{
					bool isPointer = member.InnerText.Contains("*");

					string name = member["name"].InnerText;
					if (name == "object") name = "@object";

					if (isPointer)
						code.AppendLine($"public unsafe {ResolveType(member["type"].InnerText)}* {name};");
					else
						code.AppendLine($"public {ResolveType(member["type"].InnerText)} {name};");
				}

				code.AppendLine("}");
			}

			code.Append('}');

			File.WriteAllText("/home/itorius/Development/Proxima/VulkanGenerator/Generated/Vulkan.Structs.cs", code.ToString(), Encoding.UTF8);
		}
	}
}