using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using GLFW;
using ImGuiNET;
using Proxima.Graphics;
using Vortice.Vulkan;
using Vulkan = Vortice.Vulkan.Vulkan;

namespace Proxima
{
	public static class ImGuiController
	{
		private enum GlfwCursorMode
		{
			Normal = 0x00034001,
			Hidden = 0x00034002,
			Disabled = 0x00034003
		}

		private struct FrameRenderBuffers
		{
			internal VertexBuffer<ImDrawVert>? VertexBuffer;
			internal IndexBuffer<ushort>? IndexBuffer;
		}

		private struct WindowRenderBuffers
		{
			internal uint Index;
			internal uint Count;
			internal FrameRenderBuffers[] FrameRenderBuffers;
		}

		private static List<MouseButton> pressedMouseButtons = new List<MouseButton>();
		private static Dictionary<ImGuiMouseCursor, Cursor> mouseCursors = new Dictionary<ImGuiMouseCursor, Cursor>();

		private static GraphicsDevice gd;

		private static Shader imguiShader;
		private static Texture2D fontTexture;

		private static VkDescriptorPool descriptorPool;
		private static VkDescriptorSetLayout descriptorSetLayout;
		private static VkPipelineLayout pipelineLayout;
		private static VkPipeline pipeline;
		private static WindowRenderBuffers windowRenderBuffers;
		private static VulkanCommandBuffer imguiBuffer;

		private unsafe delegate void ImGuiUserCallback(ImDrawList* draw_list, ImDrawCmd* cmd);

		private unsafe delegate string GetClipboardText(void* user_data);

		private unsafe delegate void SetClipboardText(void* user_data, string text);

		internal static unsafe void Initialize(GraphicsDevice graphicsDevice)
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

			Vulkan.vkCreateDescriptorPool(graphicsDevice.LogicalDevice, &poolCreateInfo, null, out descriptorPool).CheckResult();

			ImGuiInitialization();

			CreateDeviceResources();

			CreateFontsTexture();

			imguiBuffer = new VulkanCommandBuffer(gd, gd.GetSecondaryBuffer());
			imguiBuffer.SetName("ImGui");
		}

		private static unsafe void ImGuiInitialization()
		{
			IntPtr context = ImGui.CreateContext();
			ImGui.SetCurrentContext(context);
			ImGui.StyleColorsDark();

			ImGuiIOPtr io = ImGui.GetIO();

			io.BackendFlags |= ImGuiBackendFlags.HasMouseCursors;
			io.BackendFlags |= ImGuiBackendFlags.HasSetMousePos;
			io.BackendFlags |= ImGuiBackendFlags.RendererHasVtxOffset;

			io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;

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

			gd.window.MouseScroll += (sender, args) =>
			{
				ImGuiIOPtr io = ImGui.GetIO();

				io.MouseWheel += (float)args.Y;
				io.MouseWheelH += (float)args.X;
			};

			gd.window.KeyAction += (sender, args) =>
			{
				ImGuiIOPtr io = ImGui.GetIO();
				if (args.State == InputState.Press) io.KeysDown[(int)args.Key] = true;
				else if (args.State == InputState.Release) io.KeysDown[(int)args.Key] = false;

				io.KeyCtrl = io.KeysDown[(int)Keys.LeftControl] || io.KeysDown[(int)Keys.RightControl];
				io.KeyShift = io.KeysDown[(int)Keys.LeftShift] || io.KeysDown[(int)Keys.RightShift];
				io.KeyAlt = io.KeysDown[(int)Keys.LeftAlt] || io.KeysDown[(int)Keys.RightAlt];
				io.KeySuper = io.KeysDown[(int)Keys.LeftSuper] || io.KeysDown[(int)Keys.RightSuper];
			};

			gd.window.CharacterInput += (sender, args) =>
			{
				ImGuiIOPtr io = ImGui.GetIO();
				io.AddInputCharacter(args.CodePoint);
			};

			io.ClipboardUserData = gd.window;
			GetClipboardText getClipboardText = data => Glfw.GetClipboardString(*(Window*)data);
			io.GetClipboardTextFn = Marshal.GetFunctionPointerForDelegate(getClipboardText);
			SetClipboardText setClipboardText = (data, text) => Glfw.SetClipboardString(*(Window*)data, text);
			io.SetClipboardTextFn = Marshal.GetFunctionPointerForDelegate(setClipboardText);

			mouseCursors[ImGuiMouseCursor.Arrow] = Glfw.CreateStandardCursor(CursorType.Arrow);
			mouseCursors[ImGuiMouseCursor.TextInput] = Glfw.CreateStandardCursor(CursorType.Beam);
			mouseCursors[ImGuiMouseCursor.ResizeNS] = Glfw.CreateStandardCursor(CursorType.ResizeVertical);
			mouseCursors[ImGuiMouseCursor.ResizeEW] = Glfw.CreateStandardCursor(CursorType.ResizeHorizontal);
			mouseCursors[ImGuiMouseCursor.Hand] = Glfw.CreateStandardCursor(CursorType.Hand);

			// note: GLFW 3.4 only
			mouseCursors[ImGuiMouseCursor.ResizeAll] = Glfw.CreateStandardCursor(CursorType.ResizeAll);
			mouseCursors[ImGuiMouseCursor.ResizeNESW] = Glfw.CreateStandardCursor(CursorType.ResizeNESW);
			mouseCursors[ImGuiMouseCursor.ResizeNWSE] = Glfw.CreateStandardCursor(CursorType.ResizeNWSE);
			mouseCursors[ImGuiMouseCursor.NotAllowed] = Glfw.CreateStandardCursor(CursorType.NotAllowed);

			var font = io.Fonts.AddFontFromFileTTF("Assets/Fonts/Open_Sans/OpenSans-Regular.ttf", 17f);
			io.Fonts.AddFontDefault(font.ConfigData);
		}

		internal static unsafe VkDescriptorSet AddTexture(VkSampler sampler, VkImageView image_view, VkImageLayout image_layout)
		{
			VkDescriptorSet descriptor_set;
			VkDescriptorSetAllocateInfo alloc_info = new VkDescriptorSetAllocateInfo
			{
				sType = VkStructureType.DescriptorSetAllocateInfo,
				descriptorPool = descriptorPool,
				descriptorSetCount = 1
			};
			fixed (VkDescriptorSetLayout* ptr = &descriptorSetLayout) alloc_info.pSetLayouts = ptr;
			Vulkan.vkAllocateDescriptorSets(gd.LogicalDevice, &alloc_info, &descriptor_set);

			VkDescriptorImageInfo desc_image = new VkDescriptorImageInfo
			{
				sampler = sampler,
				imageView = image_view,
				imageLayout = image_layout
			};
			VkWriteDescriptorSet write_desc = new VkWriteDescriptorSet
			{
				sType = VkStructureType.WriteDescriptorSet,
				dstSet = descriptor_set,
				descriptorCount = 1,
				descriptorType = VkDescriptorType.CombinedImageSampler,
				pImageInfo = &desc_image
			};
			Vulkan.vkUpdateDescriptorSets(gd.LogicalDevice, write_desc);

			return descriptor_set;
		}

		internal static void NewFrame()
		{
			ImGuiIOPtr io = ImGui.GetIO();

			Debug.Assert(io.Fonts.IsBuilt(), "Font atlas not built!");

			io.DisplaySize = new Vector2(gd.window.Size.Width, gd.window.Size.Height);
			io.DisplayFramebufferScale = new Vector2(1f, 1f);

			io.DeltaTime = Time.DeltaUpdateTime;

			for (int i = 0; i < io.MouseDown.Count; i++) io.MouseDown[i] = pressedMouseButtons.Contains((MouseButton)i);

			if ((io.ConfigFlags & ImGuiConfigFlags.NoMouseCursorChange) == ImGuiConfigFlags.NoMouseCursorChange || Glfw.GetInputMode(gd.window, InputMode.Cursor) == (int)GlfwCursorMode.Disabled)
				return;

			ImGuiMouseCursor imgui_cursor = ImGui.GetMouseCursor();
			if (imgui_cursor == ImGuiMouseCursor.None || io.MouseDrawCursor)
			{
				// Hide OS mouse cursor if imgui is drawing it or if it wants no cursor
				Glfw.SetInputMode(gd.window, InputMode.Cursor, (int)GlfwCursorMode.Hidden);
			}
			else
			{
				Glfw.SetCursor(gd.window, mouseCursors.ContainsKey(imgui_cursor) ? mouseCursors[imgui_cursor] : mouseCursors[ImGuiMouseCursor.Arrow]);
				Glfw.SetInputMode(gd.window, InputMode.Cursor, (int)GlfwCursorMode.Normal);
			}

			ImGui.NewFrame();
		}

		private static void CreateDeviceResources()
		{
			CreateDescriptorSetLayout();

			CreatePipelineLayout();

			CreatePipeline();
		}

		private static unsafe void CreateDescriptorSetLayout()
		{
			if (descriptorSetLayout != VkDescriptorSetLayout.Null) return;

			VkDescriptorSetLayoutBinding binding = new VkDescriptorSetLayoutBinding
			{
				descriptorType = VkDescriptorType.CombinedImageSampler,
				descriptorCount = 1,
				stageFlags = VkShaderStageFlags.Fragment
			};

			VkDescriptorSetLayoutCreateInfo info = new VkDescriptorSetLayoutCreateInfo
			{
				sType = VkStructureType.DescriptorSetLayoutCreateInfo,
				bindingCount = 1,
				pBindings = &binding
			};
			Vulkan.vkCreateDescriptorSetLayout(gd.LogicalDevice, &info, null, out descriptorSetLayout).CheckResult();
		}

		private static unsafe void CreatePipelineLayout()
		{
			if (pipelineLayout != VkPipelineLayout.Null) return;

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
			fixed (VkDescriptorSetLayout* ptr = &descriptorSetLayout) layout_info.pSetLayouts = ptr;
			layout_info.pushConstantRangeCount = 1;
			layout_info.pPushConstantRanges = &push_constants;

			Vulkan.vkCreatePipelineLayout(gd.LogicalDevice, &layout_info, null, out pipelineLayout).CheckResult();
		}

		private static unsafe void CreatePipeline()
		{
			imguiShader = AssetManager.LoadShader("ImGui", "Assets/ImGui.vert", "Assets/ImGui.frag");

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
				layout = pipelineLayout,
				renderPass = (VkRenderPass)gd.RenderPass
			};
			fixed (VkPipelineShaderStageCreateInfo* ptr = imguiShader.Stages.GetInternalArray()) info.pStages = ptr;
			Vulkan.vkCreateGraphicsPipeline(gd.LogicalDevice, VkPipelineCache.Null, info, out pipeline).CheckResult();
		}

		private static void CreateFontsTexture()
		{
			ImGuiIOPtr io = ImGui.GetIO();

			io.Fonts.GetTexDataAsRGBA32(out IntPtr pixels, out int width, out int height);

			fontTexture = new Texture2D(gd, pixels, width, height);

			VkDescriptorSet font_descriptor_set = AddTexture(fontTexture.Sampler, fontTexture.View, VkImageLayout.ShaderReadOnlyOptimal);

			io.Fonts.TexID = (IntPtr)font_descriptor_set.Handle;
		}

		private static unsafe void RenderDrawData(ImDrawData draw_data, VkCommandBuffer command_buffer)
		{
			int width = (int)(draw_data.DisplaySize.X * draw_data.FramebufferScale.X);
			int height = (int)(draw_data.DisplaySize.Y * draw_data.FramebufferScale.Y);
			if (width <= 0 || height <= 0) return;

			// Allocate array to store enough vertex/index buffers
			ref WindowRenderBuffers wrb = ref windowRenderBuffers;
			if (wrb.FrameRenderBuffers == null)
			{
				wrb.Index = 0;
				wrb.Count = 3;
				wrb.FrameRenderBuffers = new FrameRenderBuffers[3];
			}

			// IM_ASSERT(wrb->Count == v->ImageCount);
			wrb.Index = (wrb.Index + 1) % wrb.Count;
			ref FrameRenderBuffers rb = ref wrb.FrameRenderBuffers[wrb.Index];

			if (draw_data.TotalVtxCount > 0)
			{
				if (rb.VertexBuffer == null) rb.VertexBuffer = new VertexBuffer<ImDrawVert>(gd, (uint)draw_data.TotalVtxCount);
				else rb.VertexBuffer.Resize((uint)draw_data.TotalVtxCount);
				if (rb.IndexBuffer == null) rb.IndexBuffer = new IndexBuffer<ushort>(gd, (uint)draw_data.TotalIdxCount);
				else rb.IndexBuffer.Resize((uint)draw_data.TotalIdxCount);

				// Upload vertex/index data into a single contiguous GPU buffer
				var vtx_dst = rb.VertexBuffer.Map();
				var idx_dst = rb.IndexBuffer.Map();

				for (int n = 0; n < draw_data.CmdListsCount; n++)
				{
					ImDrawList* cmdList = draw_data.CmdLists[n];

					Utility.MemoryCopy(cmdList->VtxBuffer.Data, vtx_dst, (ulong)(cmdList->VtxBuffer.Size * sizeof(ImDrawVert)));
					Utility.MemoryCopy(cmdList->IdxBuffer.Data, idx_dst, (ulong)(cmdList->IdxBuffer.Size * sizeof(ushort)));
					vtx_dst += cmdList->VtxBuffer.Size;
					idx_dst += cmdList->IdxBuffer.Size;
				}

				rb.VertexBuffer.Unmap();
				rb.IndexBuffer.Unmap();
			}

			// Setup desired Vulkan state
			SetupRenderState(draw_data, pipeline, command_buffer, rb, width, height);

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
						if (pcmd.UserCallback == new IntPtr(-1)) SetupRenderState(draw_data, pipeline, command_buffer, rb, width, height);
						else
						{
							fixed (ImDrawCmd* ptr = &pcmd) Marshal.GetDelegateForFunctionPointer<ImGuiUserCallback>(pcmd.UserCallback).Invoke(cmd_list, ptr);
						}
					}
					else
					{
						// Project scissor/clipping rectangles into framebuffer space
						Vector4 clip = new Vector4(
							(pcmd.ClipRect.X - clip_off.X) * clip_scale.X,
							(pcmd.ClipRect.Y - clip_off.Y) * clip_scale.Y,
							(pcmd.ClipRect.Z - clip_off.X) * clip_scale.X,
							(pcmd.ClipRect.W - clip_off.Y) * clip_scale.Y);

						if (clip.X < width && clip.Y < height && clip.Z >= 0.0f && clip.W >= 0.0f)
						{
							// Negative offsets are illegal for vkCmdSetScissor
							if (clip.X < 0.0f) clip.X = 0.0f;
							if (clip.Y < 0.0f) clip.Y = 0.0f;

							// Apply scissor/clipping rectangle
							VkRect2D scissor = new VkRect2D((int)clip.X, (int)clip.Y, (int)(clip.Z - clip.X), (int)(clip.W - clip.Y));
							Vulkan.vkCmdSetScissor(command_buffer, 0, 1, &scissor);

							VkDescriptorSet descriptorSet = new VkDescriptorSet((ulong)pcmd.TextureId);
							Vulkan.vkCmdBindDescriptorSets(command_buffer, VkPipelineBindPoint.Graphics, pipelineLayout, 0, descriptorSet);

							// Draw
							Vulkan.vkCmdDrawIndexed(command_buffer, pcmd.ElemCount, 1, (uint)(pcmd.IdxOffset + global_idx_offset), (int)(pcmd.VtxOffset + global_vtx_offset), 0);
						}
					}
				}

				global_idx_offset += cmd_list->IdxBuffer.Size;
				global_vtx_offset += cmd_list->VtxBuffer.Size;
			}
		}

		private static unsafe void SetupRenderState(ImDrawData draw_data, VkPipeline pipeline, VkCommandBuffer command_buffer, FrameRenderBuffers rb, int fb_width, int fb_height)
		{
			Vulkan.vkCmdBindPipeline(command_buffer, VkPipelineBindPoint.Graphics, pipeline);

			if (draw_data.TotalVtxCount > 0)
			{
				rb.VertexBuffer!.Bind(command_buffer);
				rb.IndexBuffer!.Bind(command_buffer);
			}

			VkViewport viewport = new VkViewport(0, 0, fb_width, fb_height, 0f, 1f);
			Vulkan.vkCmdSetViewport(command_buffer, 0, 1, &viewport);

			float[] scale = new float[2];
			scale[0] = 2f / draw_data.DisplaySize.X;
			scale[1] = 2f / draw_data.DisplaySize.Y;
			float[] translate = new float[2];
			translate[0] = -1f - draw_data.DisplayPos.X * scale[0];
			translate[1] = -1f - draw_data.DisplayPos.Y * scale[1];
			fixed (float* ptr = scale) Vulkan.vkCmdPushConstants(command_buffer, pipelineLayout, VkShaderStageFlags.Vertex, 0, sizeof(float) * 2, ptr);
			fixed (float* ptr = translate) Vulkan.vkCmdPushConstants(command_buffer, pipelineLayout, VkShaderStageFlags.Vertex, sizeof(float) * 2, sizeof(float) * 2, ptr);
		}

		internal static unsafe void Draw()
		{
			ImGui.Render();
			var imDrawDataPtr = ImGui.GetDrawData();

			imguiBuffer.Begin(VkCommandBufferUsageFlags.RenderPassContinue | VkCommandBufferUsageFlags.SimultaneousUse, gd.GetInheritanceInfo());
			RenderDrawData(*imDrawDataPtr.NativePtr, imguiBuffer);
			imguiBuffer.End();

			gd.SubmitSecondaryBuffer(imguiBuffer);
		}

		internal static unsafe void Dispose()
		{
			for (ImGuiMouseCursor i = 0; i < ImGuiMouseCursor.COUNT; i++)
			{
				Glfw.DestroyCursor(mouseCursors[i]);
				mouseCursors[i] = Cursor.None;
			}

			for (int n = 0; n < windowRenderBuffers.Count; n++)
			{
				var buffers = windowRenderBuffers.FrameRenderBuffers[n];
				buffers.VertexBuffer?.Dispose();
				buffers.IndexBuffer?.Dispose();
			}

			fontTexture.Dispose();

			if (descriptorSetLayout != VkDescriptorSetLayout.Null)
			{
				Vulkan.vkDestroyDescriptorSetLayout(gd.LogicalDevice, descriptorSetLayout, null);
				descriptorSetLayout = VkDescriptorSetLayout.Null;
			}

			if (descriptorPool != VkDescriptorPool.Null)
			{
				Vulkan.vkDestroyDescriptorPool(gd.LogicalDevice, descriptorPool, null);
				descriptorPool = VkDescriptorPool.Null;
			}

			if (pipelineLayout != VkPipelineLayout.Null)
			{
				Vulkan.vkDestroyPipelineLayout(gd.LogicalDevice, pipelineLayout, null);
				pipelineLayout = VkPipelineLayout.Null;
			}

			if (pipeline != VkPipeline.Null)
			{
				Vulkan.vkDestroyPipeline(gd.LogicalDevice, pipeline, null);
				pipeline = VkPipeline.Null;
			}
		}
	}
}