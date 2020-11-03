using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using GLFW;
using ImGuiNET;
using Proxima.Graphics;
using Vortice.Vulkan;
using Buffer = System.Buffer;
using Vulkan = Vortice.Vulkan.Vulkan;

namespace Proxima
{
	public static class ImGuiController
	{
		private static uint[] ImGuiVertexBytecode =
		{
			0x07230203, 0x00010000, 0x00080001, 0x0000002e, 0x00000000, 0x00020011, 0x00000001, 0x0006000b,
			0x00000001, 0x4c534c47, 0x6474732e, 0x3035342e, 0x00000000, 0x0003000e, 0x00000000, 0x00000001,
			0x000a000f, 0x00000000, 0x00000004, 0x6e69616d, 0x00000000, 0x0000000b, 0x0000000f, 0x00000015,
			0x0000001b, 0x0000001c, 0x00030003, 0x00000002, 0x000001c2, 0x00040005, 0x00000004, 0x6e69616d,
			0x00000000, 0x00030005, 0x00000009, 0x00000000, 0x00050006, 0x00000009, 0x00000000, 0x6f6c6f43,
			0x00000072, 0x00040006, 0x00000009, 0x00000001, 0x00005655, 0x00030005, 0x0000000b, 0x0074754f,
			0x00040005, 0x0000000f, 0x6c6f4361, 0x0000726f, 0x00030005, 0x00000015, 0x00565561, 0x00060005,
			0x00000019, 0x505f6c67, 0x65567265, 0x78657472, 0x00000000, 0x00060006, 0x00000019, 0x00000000,
			0x505f6c67, 0x7469736f, 0x006e6f69, 0x00030005, 0x0000001b, 0x00000000, 0x00040005, 0x0000001c,
			0x736f5061, 0x00000000, 0x00060005, 0x0000001e, 0x73755075, 0x6e6f4368, 0x6e617473, 0x00000074,
			0x00050006, 0x0000001e, 0x00000000, 0x61635375, 0x0000656c, 0x00060006, 0x0000001e, 0x00000001,
			0x61725475, 0x616c736e, 0x00006574, 0x00030005, 0x00000020, 0x00006370, 0x00040047, 0x0000000b,
			0x0000001e, 0x00000000, 0x00040047, 0x0000000f, 0x0000001e, 0x00000002, 0x00040047, 0x00000015,
			0x0000001e, 0x00000001, 0x00050048, 0x00000019, 0x00000000, 0x0000000b, 0x00000000, 0x00030047,
			0x00000019, 0x00000002, 0x00040047, 0x0000001c, 0x0000001e, 0x00000000, 0x00050048, 0x0000001e,
			0x00000000, 0x00000023, 0x00000000, 0x00050048, 0x0000001e, 0x00000001, 0x00000023, 0x00000008,
			0x00030047, 0x0000001e, 0x00000002, 0x00020013, 0x00000002, 0x00030021, 0x00000003, 0x00000002,
			0x00030016, 0x00000006, 0x00000020, 0x00040017, 0x00000007, 0x00000006, 0x00000004, 0x00040017,
			0x00000008, 0x00000006, 0x00000002, 0x0004001e, 0x00000009, 0x00000007, 0x00000008, 0x00040020,
			0x0000000a, 0x00000003, 0x00000009, 0x0004003b, 0x0000000a, 0x0000000b, 0x00000003, 0x00040015,
			0x0000000c, 0x00000020, 0x00000001, 0x0004002b, 0x0000000c, 0x0000000d, 0x00000000, 0x00040020,
			0x0000000e, 0x00000001, 0x00000007, 0x0004003b, 0x0000000e, 0x0000000f, 0x00000001, 0x00040020,
			0x00000011, 0x00000003, 0x00000007, 0x0004002b, 0x0000000c, 0x00000013, 0x00000001, 0x00040020,
			0x00000014, 0x00000001, 0x00000008, 0x0004003b, 0x00000014, 0x00000015, 0x00000001, 0x00040020,
			0x00000017, 0x00000003, 0x00000008, 0x0003001e, 0x00000019, 0x00000007, 0x00040020, 0x0000001a,
			0x00000003, 0x00000019, 0x0004003b, 0x0000001a, 0x0000001b, 0x00000003, 0x0004003b, 0x00000014,
			0x0000001c, 0x00000001, 0x0004001e, 0x0000001e, 0x00000008, 0x00000008, 0x00040020, 0x0000001f,
			0x00000009, 0x0000001e, 0x0004003b, 0x0000001f, 0x00000020, 0x00000009, 0x00040020, 0x00000021,
			0x00000009, 0x00000008, 0x0004002b, 0x00000006, 0x00000028, 0x00000000, 0x0004002b, 0x00000006,
			0x00000029, 0x3f800000, 0x00050036, 0x00000002, 0x00000004, 0x00000000, 0x00000003, 0x000200f8,
			0x00000005, 0x0004003d, 0x00000007, 0x00000010, 0x0000000f, 0x00050041, 0x00000011, 0x00000012,
			0x0000000b, 0x0000000d, 0x0003003e, 0x00000012, 0x00000010, 0x0004003d, 0x00000008, 0x00000016,
			0x00000015, 0x00050041, 0x00000017, 0x00000018, 0x0000000b, 0x00000013, 0x0003003e, 0x00000018,
			0x00000016, 0x0004003d, 0x00000008, 0x0000001d, 0x0000001c, 0x00050041, 0x00000021, 0x00000022,
			0x00000020, 0x0000000d, 0x0004003d, 0x00000008, 0x00000023, 0x00000022, 0x00050085, 0x00000008,
			0x00000024, 0x0000001d, 0x00000023, 0x00050041, 0x00000021, 0x00000025, 0x00000020, 0x00000013,
			0x0004003d, 0x00000008, 0x00000026, 0x00000025, 0x00050081, 0x00000008, 0x00000027, 0x00000024,
			0x00000026, 0x00050051, 0x00000006, 0x0000002a, 0x00000027, 0x00000000, 0x00050051, 0x00000006,
			0x0000002b, 0x00000027, 0x00000001, 0x00070050, 0x00000007, 0x0000002c, 0x0000002a, 0x0000002b,
			0x00000028, 0x00000029, 0x00050041, 0x00000011, 0x0000002d, 0x0000001b, 0x0000000d, 0x0003003e,
			0x0000002d, 0x0000002c, 0x000100fd, 0x00010038
		};

		private static uint[] ImGuiFragmentBytecode =
		{
			0x07230203, 0x00010000, 0x00080001, 0x0000001e, 0x00000000, 0x00020011, 0x00000001, 0x0006000b,
			0x00000001, 0x4c534c47, 0x6474732e, 0x3035342e, 0x00000000, 0x0003000e, 0x00000000, 0x00000001,
			0x0007000f, 0x00000004, 0x00000004, 0x6e69616d, 0x00000000, 0x00000009, 0x0000000d, 0x00030010,
			0x00000004, 0x00000007, 0x00030003, 0x00000002, 0x000001c2, 0x00040005, 0x00000004, 0x6e69616d,
			0x00000000, 0x00040005, 0x00000009, 0x6c6f4366, 0x0000726f, 0x00030005, 0x0000000b, 0x00000000,
			0x00050006, 0x0000000b, 0x00000000, 0x6f6c6f43, 0x00000072, 0x00040006, 0x0000000b, 0x00000001,
			0x00005655, 0x00030005, 0x0000000d, 0x00006e49, 0x00050005, 0x00000016, 0x78655473, 0x65727574,
			0x00000000, 0x00040047, 0x00000009, 0x0000001e, 0x00000000, 0x00040047, 0x0000000d, 0x0000001e,
			0x00000000, 0x00040047, 0x00000016, 0x00000022, 0x00000000, 0x00040047, 0x00000016, 0x00000021,
			0x00000000, 0x00020013, 0x00000002, 0x00030021, 0x00000003, 0x00000002, 0x00030016, 0x00000006,
			0x00000020, 0x00040017, 0x00000007, 0x00000006, 0x00000004, 0x00040020, 0x00000008, 0x00000003,
			0x00000007, 0x0004003b, 0x00000008, 0x00000009, 0x00000003, 0x00040017, 0x0000000a, 0x00000006,
			0x00000002, 0x0004001e, 0x0000000b, 0x00000007, 0x0000000a, 0x00040020, 0x0000000c, 0x00000001,
			0x0000000b, 0x0004003b, 0x0000000c, 0x0000000d, 0x00000001, 0x00040015, 0x0000000e, 0x00000020,
			0x00000001, 0x0004002b, 0x0000000e, 0x0000000f, 0x00000000, 0x00040020, 0x00000010, 0x00000001,
			0x00000007, 0x00090019, 0x00000013, 0x00000006, 0x00000001, 0x00000000, 0x00000000, 0x00000000,
			0x00000001, 0x00000000, 0x0003001b, 0x00000014, 0x00000013, 0x00040020, 0x00000015, 0x00000000,
			0x00000014, 0x0004003b, 0x00000015, 0x00000016, 0x00000000, 0x0004002b, 0x0000000e, 0x00000018,
			0x00000001, 0x00040020, 0x00000019, 0x00000001, 0x0000000a, 0x00050036, 0x00000002, 0x00000004,
			0x00000000, 0x00000003, 0x000200f8, 0x00000005, 0x00050041, 0x00000010, 0x00000011, 0x0000000d,
			0x0000000f, 0x0004003d, 0x00000007, 0x00000012, 0x00000011, 0x0004003d, 0x00000014, 0x00000017,
			0x00000016, 0x00050041, 0x00000019, 0x0000001a, 0x0000000d, 0x00000018, 0x0004003d, 0x0000000a,
			0x0000001b, 0x0000001a, 0x00050057, 0x00000007, 0x0000001c, 0x00000017, 0x0000001b, 0x00050085,
			0x00000007, 0x0000001d, 0x00000012, 0x0000001c, 0x0003003e, 0x00000009, 0x0000001d, 0x000100fd,
			0x00010038
		};

		private struct ImGuiVert
		{
			public Vector2 position;
			public Vector2 uv;
			public Vector4 color;
		}

		private static GraphicsDevice gd;
		private static VkDescriptorPool imguiPool;
		private static List<MouseButton> pressedMouseButtons = new List<MouseButton>();
		private static Dictionary<ImGuiMouseCursor, Cursor> g_MouseCursors = new Dictionary<ImGuiMouseCursor, Cursor>();

		public static unsafe void Initialize(GraphicsDevice graphicsDevice)
		{
			gd = graphicsDevice;

			VkDescriptorPoolSize[] poolSizes =
			{
				new VkDescriptorPoolSize
				{
					type = VkDescriptorType.Sampler,
					descriptorCount = 1000
				},
				new VkDescriptorPoolSize
				{
					type = VkDescriptorType.CombinedImageSampler,
					descriptorCount = 1000
				},
				new VkDescriptorPoolSize
				{
					type = VkDescriptorType.SampledImage,
					descriptorCount = 1000
				},
				new VkDescriptorPoolSize
				{
					type = VkDescriptorType.StorageImage,
					descriptorCount = 1000
				},
				new VkDescriptorPoolSize
				{
					type = VkDescriptorType.UniformTexelBuffer,
					descriptorCount = 1000
				},
				new VkDescriptorPoolSize
				{
					type = VkDescriptorType.StorageTexelBuffer,
					descriptorCount = 1000
				},
				new VkDescriptorPoolSize
				{
					type = VkDescriptorType.UniformBuffer,
					descriptorCount = 1000
				},
				new VkDescriptorPoolSize
				{
					type = VkDescriptorType.StorageBuffer,
					descriptorCount = 1000
				},
				new VkDescriptorPoolSize
				{
					type = VkDescriptorType.UniformBufferDynamic,
					descriptorCount = 1000
				},
				new VkDescriptorPoolSize
				{
					type = VkDescriptorType.StorageBufferDynamic,
					descriptorCount = 1000
				},
				new VkDescriptorPoolSize
				{
					type = VkDescriptorType.InputAttachment,
					descriptorCount = 1000
				}
			};

			VkDescriptorPoolCreateInfo poolCreateInfo = new VkDescriptorPoolCreateInfo
			{
				sType = VkStructureType.DescriptorPoolCreateInfo,
				flags = VkDescriptorPoolCreateFlags.FreeDescriptorSet,
				maxSets = 1000,
				poolSizeCount = (uint)poolSizes.Length
			};
			fixed (VkDescriptorPoolSize* ptr = poolSizes) poolCreateInfo.pPoolSizes = ptr;

			Vulkan.vkCreateDescriptorPool(graphicsDevice.LogicalDevice, &poolCreateInfo, null, out imguiPool).CheckResult();

			IntPtr context = ImGui.CreateContext();
			ImGui.SetCurrentContext(context);

			ImGuiIOPtr io = ImGui.GetIO();

			io.BackendFlags |= ImGuiBackendFlags.HasMouseCursors; // We can honor GetMouseCursor() values (optional)
			io.BackendFlags |= ImGuiBackendFlags.HasSetMousePos; // We can honor io.WantSetMousePos requests (optional, rarely used)
			io.BackendFlags |= ImGuiBackendFlags.RendererHasVtxOffset;

			io.KeyMap[(int)ImGuiKey.Tab] = (int)Keys.Tab;
			io.KeyMap[(int)ImGuiKey.LeftArrow] = (int)Keys.Left;
			io.KeyMap[(int)ImGuiKey.RightArrow] = (int)Keys.Right;
			io.KeyMap[(int)ImGuiKey.UpArrow] = (int)Keys.Up;
			io.KeyMap[(int)ImGuiKey.DownArrow] = (int)Keys.Down;
			io.KeyMap[(int)ImGuiKey.PageUp] = (int)Keys.PageUp;
			io.KeyMap[(int)ImGuiKey.PageDown] = (int)Keys.PageDown;
			io.KeyMap[(int)ImGuiKey.Home] = (int)Keys.Home;
			io.KeyMap[(int)ImGuiKey.End] = (int)Keys.End;
			io.KeyMap[(int)ImGuiKey.Insert] = (int)Keys.Insert;
			io.KeyMap[(int)ImGuiKey.Delete] = (int)Keys.Delete;
			io.KeyMap[(int)ImGuiKey.Backspace] = (int)Keys.Backspace;
			io.KeyMap[(int)ImGuiKey.Space] = (int)Keys.Space;
			io.KeyMap[(int)ImGuiKey.Enter] = (int)Keys.Enter;
			io.KeyMap[(int)ImGuiKey.Escape] = (int)Keys.Escape;
			io.KeyMap[(int)ImGuiKey.KeyPadEnter] = (int)Keys.NumpadEnter;
			io.KeyMap[(int)ImGuiKey.A] = (int)Keys.A;
			io.KeyMap[(int)ImGuiKey.C] = (int)Keys.C;
			io.KeyMap[(int)ImGuiKey.V] = (int)Keys.V;
			io.KeyMap[(int)ImGuiKey.X] = (int)Keys.X;
			io.KeyMap[(int)ImGuiKey.Y] = (int)Keys.Y;
			io.KeyMap[(int)ImGuiKey.Z] = (int)Keys.Z;

			gd.window.MouseMoved += (sender, args) => { io.MousePos = new Vector2((float)args.X, (float)args.Y); };

			gd.window.MouseButton += (sender, args) =>
			{
				if (args.Action == InputState.Press) pressedMouseButtons.Add(args.Button);
				else if (args.Action == InputState.Release) pressedMouseButtons.Remove(args.Button);
			};

			gd.window.KeyAction += (sender, args) =>
			{
				ImGuiIOPtr io = ImGui.GetIO();
				if (args.State == InputState.Press) io.KeysDown[(int)args.Key] = true;
				else if (args.State == InputState.Release) io.KeysDown[(int)args.Key] = false;

				io.KeyCtrl = io.KeysDown[(int)Keys.LeftControl] || io.KeysDown[(int)Keys.RightControl];
			};

			gd.window.CharacterInput += (sender, args) =>
			{
				ImGuiIOPtr io = ImGui.GetIO();
				io.AddInputCharacter(args.CodePoint);
			};

			CreateDeviceResources();

			VkCommandBuffer buffer = VulkanUtils.BeginSingleTimeCommands(graphicsDevice);
			CreateFontsTexture(buffer);
			VulkanUtils.EndSingleTimeCommands(graphicsDevice, buffer);

			if (g_UploadBuffer != VkBuffer.Null)
			{
				Vulkan.vkDestroyBuffer(gd.LogicalDevice, g_UploadBuffer, null);
				g_UploadBuffer = VkBuffer.Null;
			}

			if (g_UploadBufferMemory != VkDeviceMemory.Null)
			{
				Vulkan.vkFreeMemory(gd.LogicalDevice, g_UploadBufferMemory, null);
				g_UploadBufferMemory = VkDeviceMemory.Null;
			}

			// io.SetClipboardTextFn = ImGui_ImplGlfw_SetClipboardText;
			// io.GetClipboardTextFn = ImGui_ImplGlfw_GetClipboardText;
			// io.ClipboardUserData = g_Window;

			// Create mouse cursors
			// (By design, on X11 cursors are user configurable and some cursors may be missing. When a cursor doesn't exist,
			// GLFW will emit an error which will often be printed by the app, so we temporarily disable error reporting.
			// Missing cursors will return NULL and our _UpdateMouseCursor() function will use the Arrow cursor instead.)
			// GLFWerrorfun prev_error_callback = glfwSetErrorCallback(NULL);
			g_MouseCursors[ImGuiMouseCursor.Arrow] = Glfw.CreateStandardCursor(CursorType.Arrow);
			g_MouseCursors[ImGuiMouseCursor.TextInput] = Glfw.CreateStandardCursor(CursorType.Beam);
			g_MouseCursors[ImGuiMouseCursor.ResizeNS] = Glfw.CreateStandardCursor(CursorType.ResizeVertical);
			g_MouseCursors[ImGuiMouseCursor.ResizeEW] = Glfw.CreateStandardCursor(CursorType.ResizeHorizontal);
			g_MouseCursors[ImGuiMouseCursor.Hand] = Glfw.CreateStandardCursor(CursorType.Hand);
			// #if GLFW_HAS_NEW_CURSORS
			g_MouseCursors[ImGuiMouseCursor.ResizeAll] = Glfw.CreateStandardCursor(CursorType.Arrow);
			g_MouseCursors[ImGuiMouseCursor.ResizeNESW] = Glfw.CreateStandardCursor(CursorType.Arrow);
			g_MouseCursors[ImGuiMouseCursor.ResizeNWSE] = Glfw.CreateStandardCursor(CursorType.Arrow);
			g_MouseCursors[ImGuiMouseCursor.NotAllowed] = Glfw.CreateStandardCursor(CursorType.Arrow);
			// #else
			// g_MouseCursors[ImGuiMouseCursor_ResizeAll] = glfwCreateStandardCursor(GLFW_ARROW_CURSOR);
			// g_MouseCursors[ImGuiMouseCursor_ResizeNESW] = glfwCreateStandardCursor(GLFW_ARROW_CURSOR);
			// g_MouseCursors[ImGuiMouseCursor_ResizeNWSE] = glfwCreateStandardCursor(GLFW_ARROW_CURSOR);
			// g_MouseCursors[ImGuiMouseCursor_NotAllowed] = glfwCreateStandardCursor(GLFW_ARROW_CURSOR);
			// #endif
			// glfwSetErrorCallback(prev_error_callback);

			// Chain GLFW callbacks: our callbacks will call the user's previously installed callbacks, if any.
			// g_PrevUserCallbackMousebutton = NULL;
			// g_PrevUserCallbackScroll = NULL;
			// g_PrevUserCallbackKey = NULL;
			// g_PrevUserCallbackChar = NULL;
			// if (install_callbacks)
			// {
			// 	g_InstalledCallbacks = true;
			// 	g_PrevUserCallbackMousebutton = glfwSetMouseButtonCallback(window, ImGui_ImplGlfw_MouseButtonCallback);
			// 	g_PrevUserCallbackScroll = glfwSetScrollCallback(window, ImGui_ImplGlfw_ScrollCallback);
			// 	g_PrevUserCallbackKey = glfwSetKeyCallback(window, ImGui_ImplGlfw_KeyCallback);
			// 	g_PrevUserCallbackChar = glfwSetCharCallback(window, ImGui_ImplGlfw_CharCallback);
			// }
			//
			// g_ClientApi = client_api;
		}

		public static void NewFrame()
		{
			ImGuiIOPtr io = ImGui.GetIO();

			// Debug.Assert(io.Fonts.IsBuilt());

			// IM_ASSERT(io.Fonts->IsBuilt() && "Font atlas not built! It is generally built by the renderer backend. Missing call to renderer _NewFrame() function? e.g. ImGui_ImplOpenGL3_NewFrame().");

			io.DisplaySize = new Vector2(gd.window.Size.Width, gd.window.Size.Height);
			io.DisplayFramebufferScale = new Vector2(1f, 1f);

			io.DeltaTime = Time.DeltaUpdateTime;

			for (int i = 0; i < io.MouseDown.Count; i++) io.MouseDown[i] = pressedMouseButtons.Contains((MouseButton)i);

			if ((io.ConfigFlags & ImGuiConfigFlags.NoMouseCursorChange) == ImGuiConfigFlags.NoMouseCursorChange || Glfw.GetInputMode(gd.window, InputMode.Cursor) == 0x00034003 /*GLFW_CURSOR_DISABLED*/)
				return;

			ImGuiMouseCursor imgui_cursor = ImGui.GetMouseCursor();
			if (imgui_cursor == ImGuiMouseCursor.None || io.MouseDrawCursor)
			{
				// Hide OS mouse cursor if imgui is drawing it or if it wants no cursor
				Glfw.SetInputMode(gd.window, InputMode.Cursor, /*cusror_hidden*/0x00034002);
			}
			else
			{
				// Show OS mouse cursor
				// FIXME-PLATFORM: Unfocused windows seems to fail changing the mouse cursor with GLFW 3.2, but 3.3 works here.
				Glfw.SetCursor(gd.window, g_MouseCursors.ContainsKey(imgui_cursor) ? g_MouseCursors[imgui_cursor] : g_MouseCursors[ImGuiMouseCursor.Arrow]);
				Glfw.SetInputMode(gd.window, InputMode.Cursor, /*GLFW_CURSOR_NORMAL*/0x00034001);
			}
		}

		private static VkSampler fontSampler;
		private static VkDescriptorSetLayout _descriptorSetLayout;
		private static VkDescriptorSet _descriptorSet;
		private static VkPipelineLayout _pipelineLayout;
		private static VkPipeline _pipeline;

		private static unsafe void CreateDeviceResources()
		{
			CreateFontSampler();

			CreateDescriptorSetLayout();

			{
				VkDescriptorSetAllocateInfo alloc_info = new VkDescriptorSetAllocateInfo
				{
					sType = VkStructureType.DescriptorSetAllocateInfo,
					descriptorPool = imguiPool,
					descriptorSetCount = 1
				};
				fixed (VkDescriptorSetLayout* ptr = &_descriptorSetLayout) alloc_info.pSetLayouts = ptr;

				fixed (VkDescriptorSet* ptr = &_descriptorSet) Vulkan.vkAllocateDescriptorSets(gd.LogicalDevice, &alloc_info, ptr).CheckResult();
			}

			CreatePipelineLayout();

			CreatePipeline();
		}

		private static VkShaderModule g_ShaderModuleVert, g_ShaderModuleFrag;

		private static unsafe void CreateShaderModules()
		{
			if (g_ShaderModuleVert == VkShaderModule.Null)
			{
				VkShaderModuleCreateInfo info = new VkShaderModuleCreateInfo
				{
					sType = VkStructureType.ShaderModuleCreateInfo,
					codeSize = new VkPointerSize((uint)(ImGuiVertexBytecode.Length * sizeof(uint)))
				};
				fixed (uint* ptr = ImGuiVertexBytecode) info.pCode = ptr;

				Vulkan.vkCreateShaderModule(gd.LogicalDevice, &info, null, out g_ShaderModuleVert).CheckResult();
			}

			if (g_ShaderModuleFrag == VkShaderModule.Null)
			{
				VkShaderModuleCreateInfo info = new VkShaderModuleCreateInfo
				{
					sType = VkStructureType.ShaderModuleCreateInfo,
					codeSize = new VkPointerSize((uint)(ImGuiFragmentBytecode.Length * sizeof(uint)))
				};
				fixed (uint* ptr = ImGuiFragmentBytecode) info.pCode = ptr;

				Vulkan.vkCreateShaderModule(gd.LogicalDevice, &info, null, out g_ShaderModuleFrag).CheckResult();
			}
		}

		private static unsafe void CreateFontSampler()
		{
			if (fontSampler != VkSampler.Null) return;

			VkSamplerCreateInfo info = new VkSamplerCreateInfo
			{
				sType = VkStructureType.SamplerCreateInfo,
				magFilter = VkFilter.Linear,
				minFilter = VkFilter.Linear,
				mipmapMode = VkSamplerMipmapMode.Linear,
				addressModeU = VkSamplerAddressMode.Repeat,
				addressModeV = VkSamplerAddressMode.Repeat,
				addressModeW = VkSamplerAddressMode.Repeat,
				minLod = -1000,
				maxLod = 1000,
				maxAnisotropy = 1.0f
			};

			Vulkan.vkCreateSampler(gd.LogicalDevice, &info, null, out fontSampler).CheckResult();
		}

		private static unsafe void CreateDescriptorSetLayout()
		{
			if (_descriptorSetLayout != VkDescriptorSetLayout.Null) return;

			VkDescriptorSetLayoutBinding binding = new VkDescriptorSetLayoutBinding
			{
				descriptorType = VkDescriptorType.CombinedImageSampler,
				descriptorCount = 1,
				stageFlags = VkShaderStageFlags.Fragment
			};
			fixed (VkSampler* ptr = &fontSampler) binding.pImmutableSamplers = ptr;

			VkDescriptorSetLayoutCreateInfo info = new VkDescriptorSetLayoutCreateInfo
			{
				sType = VkStructureType.DescriptorSetLayoutCreateInfo,
				bindingCount = 1,
				pBindings = &binding
			};
			Vulkan.vkCreateDescriptorSetLayout(gd.LogicalDevice, &info, null, out _descriptorSetLayout).CheckResult();
		}

		private static unsafe void CreatePipelineLayout()
		{
			if (_pipelineLayout != VkPipelineLayout.Null) return;

			VkPushConstantRange push_constants = new VkPushConstantRange
			{
				stageFlags = VkShaderStageFlags.Vertex,
				offset = 0,
				size = sizeof(float) * 4
			};

			VkPipelineLayoutCreateInfo layout_info = new VkPipelineLayoutCreateInfo
			{
				sType = VkStructureType.PipelineLayoutCreateInfo,
				setLayoutCount = 1
			};
			fixed (VkDescriptorSetLayout* ptr = &_descriptorSetLayout) layout_info.pSetLayouts = ptr;
			layout_info.pushConstantRangeCount = 1;
			layout_info.pPushConstantRanges = &push_constants;

			Vulkan.vkCreatePipelineLayout(gd.LogicalDevice, &layout_info, null, out _pipelineLayout).CheckResult();
		}

		private static unsafe void CreatePipeline()
		{
			CreateShaderModules();

			VkPipelineShaderStageCreateInfo[] stage = new VkPipelineShaderStageCreateInfo[2];
			stage[0].sType = VkStructureType.PipelineShaderStageCreateInfo;
			stage[0].stage = VkShaderStageFlags.Vertex;
			stage[0].module = g_ShaderModuleVert;
			stage[0].pName = new VkString("main");
			stage[1].sType = VkStructureType.PipelineShaderStageCreateInfo;
			stage[1].stage = VkShaderStageFlags.Fragment;
			stage[1].module = g_ShaderModuleFrag;
			stage[1].pName = new VkString("main");

			VkVertexInputBindingDescription binding_desc = new VkVertexInputBindingDescription
			{
				stride = (uint)sizeof(ImDrawVert),
				inputRate = VkVertexInputRate.Vertex
			};

			VkVertexInputAttributeDescription[] attribute_desc = new VkVertexInputAttributeDescription[3];
			attribute_desc[0].location = 0;
			attribute_desc[0].binding = binding_desc.binding;
			attribute_desc[0].format = VkFormat.R32G32SFloat;
			attribute_desc[0].offset = 0;
			attribute_desc[1].location = 1;
			attribute_desc[1].binding = binding_desc.binding;
			attribute_desc[1].format = VkFormat.R32G32SFloat;
			attribute_desc[1].offset = (uint)sizeof(Vector2);
			attribute_desc[2].location = 2;
			attribute_desc[2].binding = binding_desc.binding;
			attribute_desc[2].format = VkFormat.R8G8B8A8UNorm;
			attribute_desc[2].offset = (uint)sizeof(Vector2) * 2;

			VkPipelineVertexInputStateCreateInfo vertex_info = new VkPipelineVertexInputStateCreateInfo
			{
				sType = VkStructureType.PipelineVertexInputStateCreateInfo,
				vertexBindingDescriptionCount = 1,
				pVertexBindingDescriptions = &binding_desc,
				vertexAttributeDescriptionCount = 3
			};
			fixed (VkVertexInputAttributeDescription* ptr = attribute_desc) vertex_info.pVertexAttributeDescriptions = ptr;

			VkPipelineInputAssemblyStateCreateInfo ia_info = new VkPipelineInputAssemblyStateCreateInfo
			{
				sType = VkStructureType.PipelineInputAssemblyStateCreateInfo,
				topology = VkPrimitiveTopology.TriangleList
			};

			VkPipelineViewportStateCreateInfo viewport_info = new VkPipelineViewportStateCreateInfo
			{
				sType = VkStructureType.PipelineViewportStateCreateInfo,
				viewportCount = 1,
				scissorCount = 1
			};

			VkPipelineRasterizationStateCreateInfo raster_info = new VkPipelineRasterizationStateCreateInfo
			{
				sType = VkStructureType.PipelineRasterizationStateCreateInfo,
				polygonMode = VkPolygonMode.Fill,
				cullMode = VkCullModeFlags.None,
				frontFace = VkFrontFace.CounterClockwise,
				lineWidth = 1.0f
			};

			VkPipelineMultisampleStateCreateInfo ms_info = new VkPipelineMultisampleStateCreateInfo
			{
				sType = VkStructureType.PipelineMultisampleStateCreateInfo,
				rasterizationSamples = VkSampleCountFlags.Count1
			};

			VkPipelineColorBlendAttachmentState color_attachment = new VkPipelineColorBlendAttachmentState
			{
				blendEnable = true,
				srcColorBlendFactor = VkBlendFactor.SrcAlpha,
				dstColorBlendFactor = VkBlendFactor.OneMinusSrcAlpha,
				colorBlendOp = VkBlendOp.Add,
				srcAlphaBlendFactor = VkBlendFactor.OneMinusSrcAlpha,
				dstAlphaBlendFactor = VkBlendFactor.Zero,
				alphaBlendOp = VkBlendOp.Add,
				colorWriteMask = VkColorComponentFlags.All
			};

			VkPipelineDepthStencilStateCreateInfo depth_info = new VkPipelineDepthStencilStateCreateInfo { sType = VkStructureType.PipelineDepthStencilStateCreateInfo };

			VkPipelineColorBlendStateCreateInfo blend_info = new VkPipelineColorBlendStateCreateInfo
			{
				sType = VkStructureType.PipelineColorBlendStateCreateInfo,
				attachmentCount = 1,
				pAttachments = &color_attachment
			};

			VkDynamicState[] dynamic_states = { VkDynamicState.Viewport, VkDynamicState.Scissor };
			VkPipelineDynamicStateCreateInfo dynamic_state = new VkPipelineDynamicStateCreateInfo
			{
				sType = VkStructureType.PipelineDynamicStateCreateInfo,
				dynamicStateCount = (uint)dynamic_states.Length
			};
			fixed (VkDynamicState* ptr = dynamic_states) dynamic_state.pDynamicStates = ptr;

			CreatePipelineLayout();

			VkGraphicsPipelineCreateInfo info = new VkGraphicsPipelineCreateInfo
			{
				sType = VkStructureType.GraphicsPipelineCreateInfo,
				flags = VkPipelineCreateFlags.None,
				stageCount = 2,
				pVertexInputState = &vertex_info,
				pInputAssemblyState = &ia_info,
				pViewportState = &viewport_info,
				pRasterizationState = &raster_info,
				pMultisampleState = &ms_info,
				pDepthStencilState = &depth_info,
				pColorBlendState = &blend_info,
				pDynamicState = &dynamic_state,
				layout = _pipelineLayout,
				renderPass = (VkRenderPass)gd.RenderPass
			};
			fixed (VkPipelineShaderStageCreateInfo* ptr = stage) info.pStages = ptr;
			Vulkan.vkCreateGraphicsPipeline(gd.LogicalDevice, VkPipelineCache.Null, info, out _pipeline).CheckResult();
		}

		private static VkImage g_FontImage;
		private static VkDeviceMemory g_FontMemory;
		private static VkImageView g_FontView;
		private static VkBuffer g_UploadBuffer;
		private static VkDeviceMemory g_UploadBufferMemory;

		private static unsafe void CreateFontsTexture(VkCommandBuffer command_buffer)
		{
			ImGuiIOPtr io = ImGui.GetIO();

			io.Fonts.GetTexDataAsRGBA32(out IntPtr pixels, out int width, out int height);
			uint upload_size = (uint)(width * height * 4);

			VkResult err;

			// Create the Image:
			{
				VkImageCreateInfo info = new VkImageCreateInfo
				{
					sType = VkStructureType.ImageCreateInfo,
					imageType = VkImageType.Image2D,
					format = VkFormat.R8G8B8A8UNorm,
					extent = new VkExtent3D(width, height, 1),
					mipLevels = 1,
					arrayLayers = 1,
					samples = VkSampleCountFlags.Count1,
					tiling = VkImageTiling.Optimal,
					usage = VkImageUsageFlags.Sampled | VkImageUsageFlags.TransferDst,
					sharingMode = VkSharingMode.Exclusive,
					initialLayout = VkImageLayout.Undefined
				};

				Vulkan.vkCreateImage(gd.LogicalDevice, &info, null, out g_FontImage).CheckResult();

				Vulkan.vkGetImageMemoryRequirements(gd.LogicalDevice, g_FontImage, out VkMemoryRequirements req);
				VkMemoryAllocateInfo alloc_info = new VkMemoryAllocateInfo
				{
					sType = VkStructureType.MemoryAllocateInfo,
					allocationSize = req.size,
					memoryTypeIndex = VulkanUtils.FindMemoryType(gd, req.memoryTypeBits, VkMemoryPropertyFlags.DeviceLocal)
				};

				fixed (VkDeviceMemory* ptr = &g_FontMemory) Vulkan.vkAllocateMemory(gd.LogicalDevice, &alloc_info, null, ptr).CheckResult();
				Vulkan.vkBindImageMemory(gd.LogicalDevice, g_FontImage, g_FontMemory, 0).CheckResult();
			}

			// Create the Image View:
			{
				VkImageViewCreateInfo info = new VkImageViewCreateInfo();
				info.sType = VkStructureType.ImageViewCreateInfo;
				info.image = g_FontImage;
				info.viewType = VkImageViewType.Image2D;
				info.format = VkFormat.R8G8B8A8UNorm;
				info.subresourceRange.aspectMask = VkImageAspectFlags.Color;
				info.subresourceRange.levelCount = 1;
				info.subresourceRange.layerCount = 1;
				Vulkan.vkCreateImageView(gd.LogicalDevice, &info, null, out g_FontView).CheckResult();
			}

			// Update the Descriptor Set:
			{
				VkDescriptorImageInfo desc_image = new VkDescriptorImageInfo
				{
					sampler = fontSampler,
					imageView = g_FontView,
					imageLayout = VkImageLayout.ShaderReadOnlyOptimal
				};
				VkWriteDescriptorSet write_desc = new VkWriteDescriptorSet
				{
					sType = VkStructureType.WriteDescriptorSet,
					dstSet = _descriptorSet,
					descriptorCount = 1,
					descriptorType = VkDescriptorType.CombinedImageSampler,
					pImageInfo = &desc_image
				};

				Vulkan.vkUpdateDescriptorSets(gd.LogicalDevice, write_desc);
			}

			// Create the Upload Buffer:
			{
				VkBufferCreateInfo buffer_info = new VkBufferCreateInfo();
				buffer_info.sType = VkStructureType.BufferCreateInfo;
				buffer_info.size = upload_size;
				buffer_info.usage = VkBufferUsageFlags.TransferSrc;
				buffer_info.sharingMode = VkSharingMode.Exclusive;
				Vulkan.vkCreateBuffer(gd.LogicalDevice, &buffer_info, null, out g_UploadBuffer);

				Vulkan.vkGetBufferMemoryRequirements(gd.LogicalDevice, g_UploadBuffer, out VkMemoryRequirements req);
				g_BufferMemoryAlignment = g_BufferMemoryAlignment > req.alignment ? g_BufferMemoryAlignment : req.alignment;
				VkMemoryAllocateInfo alloc_info = new VkMemoryAllocateInfo();
				alloc_info.sType = VkStructureType.MemoryAllocateInfo;
				alloc_info.allocationSize = req.size;
				alloc_info.memoryTypeIndex = VulkanUtils.FindMemoryType(gd, req.memoryTypeBits, VkMemoryPropertyFlags.HostVisible);

				fixed (VkDeviceMemory* ptr = &g_UploadBufferMemory) Vulkan.vkAllocateMemory(gd.LogicalDevice, &alloc_info, null, ptr).CheckResult();

				Vulkan.vkBindBufferMemory(gd.LogicalDevice, g_UploadBuffer, g_UploadBufferMemory, 0);
			}

			// Upload to Buffer:
			{
				void* data;
				Vulkan.vkMapMemory(gd.LogicalDevice, g_UploadBufferMemory, 0, upload_size, 0, &data).CheckResult();
				Buffer.MemoryCopy(pixels.ToPointer(), data, upload_size, upload_size);

				VkMappedMemoryRange range = new VkMappedMemoryRange();
				range.sType = VkStructureType.MappedMemoryRange;
				range.memory = g_UploadBufferMemory;
				range.size = upload_size;

				Vulkan.vkFlushMappedMemoryRanges(gd.LogicalDevice, range);
				Vulkan.vkUnmapMemory(gd.LogicalDevice, g_UploadBufferMemory);
			}

			// Copy to Image:
			{
				VkImageMemoryBarrier copy_barrier = new VkImageMemoryBarrier();
				copy_barrier.sType = VkStructureType.ImageMemoryBarrier;
				copy_barrier.dstAccessMask = VkAccessFlags.TransferWrite;
				copy_barrier.oldLayout = VkImageLayout.Undefined;
				copy_barrier.newLayout = VkImageLayout.TransferDstOptimal;
				copy_barrier.srcQueueFamilyIndex = 0;
				copy_barrier.dstQueueFamilyIndex = 0;
				copy_barrier.image = g_FontImage;
				copy_barrier.subresourceRange.aspectMask = VkImageAspectFlags.Color;
				copy_barrier.subresourceRange.levelCount = 1;
				copy_barrier.subresourceRange.layerCount = 1;
				Vulkan.vkCmdPipelineBarrier(command_buffer, VkPipelineStageFlags.Host, VkPipelineStageFlags.Transfer, 0, 0, null, 0, null, 1, &copy_barrier);

				VkBufferImageCopy region = new VkBufferImageCopy();
				region.imageSubresource.aspectMask = VkImageAspectFlags.Color;
				region.imageSubresource.layerCount = 1;
				region.imageExtent = new VkExtent3D(width, height, 1);
				Vulkan.vkCmdCopyBufferToImage(command_buffer, g_UploadBuffer, g_FontImage, VkImageLayout.TransferDstOptimal, 1, &region);

				VkImageMemoryBarrier use_barrier = new VkImageMemoryBarrier();
				use_barrier.sType = VkStructureType.ImageMemoryBarrier;
				use_barrier.srcAccessMask = VkAccessFlags.TransferWrite;
				use_barrier.dstAccessMask = VkAccessFlags.ShaderRead;
				use_barrier.oldLayout = VkImageLayout.TransferDstOptimal;
				use_barrier.newLayout = VkImageLayout.ShaderReadOnlyOptimal;
				use_barrier.srcQueueFamilyIndex = 0;
				use_barrier.dstQueueFamilyIndex = 0;
				use_barrier.image = g_FontImage;
				use_barrier.subresourceRange.aspectMask = VkImageAspectFlags.Color;
				use_barrier.subresourceRange.levelCount = 1;
				use_barrier.subresourceRange.layerCount = 1;
				Vulkan.vkCmdPipelineBarrier(command_buffer, VkPipelineStageFlags.Transfer, VkPipelineStageFlags.FragmentShader, 0, 0, null, 0, null, 1, &use_barrier);
			}

			// Store our identifier
			io.Fonts.TexID = new IntPtr((long)g_FontImage.Handle);
		}

		private static ulong g_BufferMemoryAlignment = 256;

		private static unsafe void CreateOrResizeBuffer(ref VkBuffer buffer, ref VkDeviceMemory buffer_memory, ulong p_buffer_size, uint new_size, VkBufferUsageFlags usage)
		{
			if (buffer != VkBuffer.Null) Vulkan.vkDestroyBuffer(gd.LogicalDevice, buffer, null);
			if (buffer_memory != VkDeviceMemory.Null) Vulkan.vkFreeMemory(gd.LogicalDevice, buffer_memory, null);

			ulong vertex_buffer_size_aligned = ((new_size - 1) / g_BufferMemoryAlignment + 1) * g_BufferMemoryAlignment;
			VkBufferCreateInfo buffer_info = new VkBufferCreateInfo
			{
				sType = VkStructureType.BufferCreateInfo,
				size = vertex_buffer_size_aligned,
				usage = usage,
				sharingMode = VkSharingMode.Exclusive
			};
			Vulkan.vkCreateBuffer(gd.LogicalDevice, &buffer_info, null, out buffer).CheckResult();

			Vulkan.vkGetBufferMemoryRequirements(gd.LogicalDevice, buffer, out var req);
			g_BufferMemoryAlignment = g_BufferMemoryAlignment > req.alignment ? g_BufferMemoryAlignment : req.alignment;
			VkMemoryAllocateInfo alloc_info = new VkMemoryAllocateInfo
			{
				sType = VkStructureType.MemoryAllocateInfo,
				allocationSize = req.size,
				memoryTypeIndex = VulkanUtils.FindMemoryType(gd, req.memoryTypeBits, VkMemoryPropertyFlags.HostVisible)
			};
			fixed (VkDeviceMemory* ptr = &buffer_memory) Vulkan.vkAllocateMemory(gd.LogicalDevice, &alloc_info, null, ptr).CheckResult();
			Vulkan.vkBindBufferMemory(gd.LogicalDevice, buffer, buffer_memory, 0).CheckResult();
		}

		private struct ImGui_ImplVulkanH_FrameRenderBuffers
		{
			internal VkDeviceMemory VertexBufferMemory;
			internal VkDeviceMemory IndexBufferMemory;
			internal ulong VertexBufferSize;
			internal ulong IndexBufferSize;
			internal VkBuffer VertexBuffer;
			internal VkBuffer IndexBuffer;
		}

		private struct ImGui_ImplVulkanH_WindowRenderBuffers
		{
			internal uint Index;
			internal uint Count;
			internal ImGui_ImplVulkanH_FrameRenderBuffers[] FrameRenderBuffers;
		}

		private static ImGui_ImplVulkanH_WindowRenderBuffers g_MainWindowRenderBuffers;

		private unsafe delegate void ImGuiUserCallback(ImDrawList* draw_list, ImDrawCmd* cmd);

		public static unsafe void ImGui_ImplVulkan_RenderDrawData(ImDrawData draw_data, VkCommandBuffer command_buffer)
		{
			// Avoid rendering when minimized, scale coordinates for retina displays (screen coordinates != framebuffer coordinates)
			int fb_width = (int)(draw_data.DisplaySize.X * draw_data.FramebufferScale.X);
			int fb_height = (int)(draw_data.DisplaySize.Y * draw_data.FramebufferScale.Y);
			if (fb_width <= 0 || fb_height <= 0)
				return;

			// Allocate array to store enough vertex/index buffers
			ImGui_ImplVulkanH_WindowRenderBuffers wrb = g_MainWindowRenderBuffers;
			if (wrb.FrameRenderBuffers == null)
			{
				wrb.Index = 0;
				wrb.Count = 3;
				wrb.FrameRenderBuffers = new ImGui_ImplVulkanH_FrameRenderBuffers[3];
			}

			// IM_ASSERT(wrb->Count == v->ImageCount);
			wrb.Index = (wrb.Index + 1) % wrb.Count;
			ImGui_ImplVulkanH_FrameRenderBuffers rb = wrb.FrameRenderBuffers[wrb.Index];

			if (draw_data.TotalVtxCount > 0)
			{
				// Create or resize the vertex/index buffers
				uint vertex_size = (uint)(draw_data.TotalVtxCount * sizeof(ImDrawVert));
				uint index_size = (uint)(draw_data.TotalIdxCount * sizeof(ushort));
				if (rb.VertexBuffer == VkBuffer.Null || rb.VertexBufferSize < vertex_size)
					CreateOrResizeBuffer(ref rb.VertexBuffer, ref rb.VertexBufferMemory, rb.VertexBufferSize, vertex_size, VkBufferUsageFlags.VertexBuffer);
				if (rb.IndexBuffer == VkBuffer.Null || rb.IndexBufferSize < index_size)
					CreateOrResizeBuffer(ref rb.IndexBuffer, ref rb.IndexBufferMemory, rb.IndexBufferSize, index_size, VkBufferUsageFlags.IndexBuffer);

				// Upload vertex/index data into a single contiguous GPU buffer
				ImDrawVert* vtx_dst;
				ushort* idx_dst;

				Vulkan.vkMapMemory(gd.LogicalDevice, rb.VertexBufferMemory, 0, vertex_size, 0, &vtx_dst);
				Vulkan.vkMapMemory(gd.LogicalDevice, rb.IndexBufferMemory, 0, index_size, 0, &idx_dst);

				for (int n = 0; n < draw_data.CmdListsCount; n++)
				{
					ImDrawList* cmd_list = draw_data.CmdLists[n];

					Buffer.MemoryCopy(cmd_list->VtxBuffer.Data.ToPointer(), vtx_dst, cmd_list->VtxBuffer.Size * sizeof(ImDrawVert), cmd_list->VtxBuffer.Size * sizeof(ImDrawVert));
					Buffer.MemoryCopy(cmd_list->IdxBuffer.Data.ToPointer(), idx_dst, cmd_list->IdxBuffer.Size * sizeof(ushort), cmd_list->IdxBuffer.Size * sizeof(ushort));
					vtx_dst += cmd_list->VtxBuffer.Size;
					idx_dst += cmd_list->IdxBuffer.Size;
				}

				VkMappedMemoryRange[] range = new VkMappedMemoryRange[2];
				range[0].sType = VkStructureType.MappedMemoryRange;
				range[0].memory = rb.VertexBufferMemory;
				range[0].size = 0;
				range[1].sType = VkStructureType.MappedMemoryRange;
				range[1].memory = rb.IndexBufferMemory;
				range[1].size = 0;
				fixed (VkMappedMemoryRange* ptr = range) Vulkan.vkFlushMappedMemoryRanges(gd.LogicalDevice, 2, ptr);
				Vulkan.vkUnmapMemory(gd.LogicalDevice, rb.VertexBufferMemory);
				Vulkan.vkUnmapMemory(gd.LogicalDevice, rb.IndexBufferMemory);
			}

			// Setup desired Vulkan state
			ImGui_ImplVulkan_SetupRenderState(draw_data, _pipeline, command_buffer, rb, fb_width, fb_height);

			// // Will project scissor/clipping rectangles into framebuffer space
			Vector2 clip_off = draw_data.DisplayPos; // (0,0) unless using multi-viewports
			Vector2 clip_scale = draw_data.FramebufferScale; // (1,1) unless using retina display which are often (2,2)

			// Render command lists
			// (Because we merged all buffers into a single one, we maintain our own offset into them)
			int global_vtx_offset = 0;
			int global_idx_offset = 0;
			for (int n = 0; n < draw_data.CmdListsCount; n++)
			{
				ImDrawList* cmd_list = draw_data.CmdLists[n];
				for (int cmd_i = 0; cmd_i < cmd_list->CmdBuffer.Size; cmd_i++)
				{
					ref ImDrawCmd pcmd = ref cmd_list->CmdBuffer.Ref<ImDrawCmd>(cmd_i);
					if (pcmd.UserCallback != IntPtr.Zero)
					{
						if (pcmd.UserCallback == new IntPtr(-1)) ImGui_ImplVulkan_SetupRenderState(draw_data, _pipeline, command_buffer, rb, fb_width, fb_height);
						else
						{
							fixed (ImDrawCmd* ptr = &pcmd) Marshal.GetDelegateForFunctionPointer<ImGuiUserCallback>(pcmd.UserCallback).Invoke(cmd_list, ptr);
						}
					}
					else
					{
						// Project scissor/clipping rectangles into framebuffer space
						Vector4 clip_rect = Vector4.Zero;
						clip_rect.X = (pcmd.ClipRect.X - clip_off.X) * clip_scale.X;
						clip_rect.Y = (pcmd.ClipRect.Y - clip_off.Y) * clip_scale.Y;
						clip_rect.Z = (pcmd.ClipRect.Z - clip_off.X) * clip_scale.X;
						clip_rect.W = (pcmd.ClipRect.W - clip_off.Y) * clip_scale.Y;

						if (clip_rect.X < fb_width && clip_rect.Y < fb_height && clip_rect.Z >= 0.0f && clip_rect.W >= 0.0f)
						{
							// Negative offsets are illegal for vkCmdSetScissor
							if (clip_rect.X < 0.0f) clip_rect.X = 0.0f;
							if (clip_rect.Y < 0.0f) clip_rect.Y = 0.0f;

							// Apply scissor/clipping rectangle
							VkRect2D scissor = new VkRect2D((int)clip_rect.X, (int)clip_rect.Y, (int)(clip_rect.Z - clip_rect.X), (int)(clip_rect.W - clip_rect.Y));
							Vulkan.vkCmdSetScissor(command_buffer, 0, 1, &scissor);

							// Draw
							Vulkan.vkCmdDrawIndexed(command_buffer, pcmd.ElemCount, 1, (uint)(pcmd.IdxOffset + global_idx_offset), (int)(pcmd.VtxOffset + global_vtx_offset), 0);
						}
					}
				}

				global_idx_offset += cmd_list->IdxBuffer.Size;
				global_vtx_offset += cmd_list->VtxBuffer.Size;
			}
		}

		private static unsafe void ImGui_ImplVulkan_SetupRenderState(ImDrawData draw_data, VkPipeline pipeline, VkCommandBuffer command_buffer, ImGui_ImplVulkanH_FrameRenderBuffers rb, int fb_width, int fb_height)
		{
			Vulkan.vkCmdBindDescriptorSets(command_buffer, VkPipelineBindPoint.Graphics, _pipelineLayout, 0, _descriptorSet);
			Vulkan.vkCmdBindPipeline(command_buffer, VkPipelineBindPoint.Graphics, pipeline);

			// Bind Vertex And Index Buffer:
			if (draw_data.TotalVtxCount > 0)
			{
				Vulkan.vkCmdBindVertexBuffers(command_buffer, 0, rb.VertexBuffer);
				Vulkan.vkCmdBindIndexBuffer(command_buffer, rb.IndexBuffer, 0, VkIndexType.Uint16);
			}

			// Setup viewport:
			{
				VkViewport viewport = new VkViewport(0, 0, fb_width, fb_height, 0f, 1f);
				Vulkan.vkCmdSetViewport(command_buffer, 0, 1, &viewport);
			}

			// Setup scale and translation:
			// Our visible imgui space lies from draw_data->DisplayPps (top left) to draw_data->DisplayPos+data_data->DisplaySize (bottom right). DisplayPos is (0,0) for single viewport apps.
			{
				float[] scale = new float[2];
				scale[0] = 2.0f / draw_data.DisplaySize.X;
				scale[1] = 2.0f / draw_data.DisplaySize.Y;
				float[] translate = new float[2];
				translate[0] = -1.0f - draw_data.DisplayPos.X * scale[0];
				translate[1] = -1.0f - draw_data.DisplayPos.Y * scale[1];
				fixed (float* ptr = scale) Vulkan.vkCmdPushConstants(command_buffer, _pipelineLayout, VkShaderStageFlags.Vertex, sizeof(float) * 0, sizeof(float) * 2, ptr);
				fixed (float* ptr = translate) Vulkan.vkCmdPushConstants(command_buffer, _pipelineLayout, VkShaderStageFlags.Vertex, sizeof(float) * 2, sizeof(float) * 2, ptr);
			}
		}

		public static void Dispose()
		{
		}
	}
}