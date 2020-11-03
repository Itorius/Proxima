using System;
using System.Collections.Generic;
using System.Diagnostics;
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
		private enum GlfwCursorMode
		{
			Normal = 0x00034001,
			Hidden = 0x00034002,
			Disabled = 0x00034003
		}

		private unsafe delegate string GetClipboardText(void* user_data);

		private unsafe delegate void SetClipboardText(void* user_data, string text);

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

			ImGuiInitialization();

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
		}

		public static unsafe VkDescriptorSet ImGui_ImplVulkan_AddTexture(VkSampler sampler, VkImageView image_view, VkImageLayout image_layout)
		{
			VkDescriptorSet descriptor_set;
			VkDescriptorSetAllocateInfo alloc_info = new VkDescriptorSetAllocateInfo
			{
				sType = VkStructureType.DescriptorSetAllocateInfo,
				descriptorPool = imguiPool,
				descriptorSetCount = 1
			};
			fixed (VkDescriptorSetLayout* ptr = &_descriptorSetLayout) alloc_info.pSetLayouts = ptr;
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

		private static unsafe void ImGuiInitialization()
		{
			IntPtr context = ImGui.CreateContext();
			ImGui.SetCurrentContext(context);

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

			g_MouseCursors[ImGuiMouseCursor.Arrow] = Glfw.CreateStandardCursor(CursorType.Arrow);
			g_MouseCursors[ImGuiMouseCursor.TextInput] = Glfw.CreateStandardCursor(CursorType.Beam);
			g_MouseCursors[ImGuiMouseCursor.ResizeNS] = Glfw.CreateStandardCursor(CursorType.ResizeVertical);
			g_MouseCursors[ImGuiMouseCursor.ResizeEW] = Glfw.CreateStandardCursor(CursorType.ResizeHorizontal);
			g_MouseCursors[ImGuiMouseCursor.Hand] = Glfw.CreateStandardCursor(CursorType.Hand);

			// note: GLFW 3.4 only
			g_MouseCursors[ImGuiMouseCursor.ResizeAll] = Glfw.CreateStandardCursor(CursorType.ResizeAll);
			g_MouseCursors[ImGuiMouseCursor.ResizeNESW] = Glfw.CreateStandardCursor(CursorType.ResizeNESW);
			g_MouseCursors[ImGuiMouseCursor.ResizeNWSE] = Glfw.CreateStandardCursor(CursorType.ResizeNWSE);
			g_MouseCursors[ImGuiMouseCursor.NotAllowed] = Glfw.CreateStandardCursor(CursorType.NotAllowed);

			var font = io.Fonts.AddFontFromFileTTF("Assets/Fonts/Open_Sans/OpenSans-Regular.ttf", 17f);
			io.Fonts.AddFontDefault(font.ConfigData);
		}

		public static void NewFrame()
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
				Glfw.SetCursor(gd.window, g_MouseCursors.ContainsKey(imgui_cursor) ? g_MouseCursors[imgui_cursor] : g_MouseCursors[ImGuiMouseCursor.Arrow]);
				Glfw.SetInputMode(gd.window, InputMode.Cursor, (int)GlfwCursorMode.Normal);
			}
		}

		private static VkSampler g_FontSampler;

		private static VkDescriptorSetLayout _descriptorSetLayout;

		private static VkPipelineLayout _pipelineLayout;
		private static VkPipeline _pipeline;

		private static void CreateDeviceResources()
		{
			CreateFontSampler();

			CreateDescriptorSetLayout();

			CreatePipelineLayout();

			CreatePipeline();
		}

		private static Shader shader;

		private static unsafe void CreateFontSampler()
		{
			if (g_FontSampler != VkSampler.Null) return;

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

			Vulkan.vkCreateSampler(gd.LogicalDevice, &info, null, out g_FontSampler).CheckResult();
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
			shader = AssetManager.LoadShader("ImGui", "Assets/ImGui.vert", "Assets/ImGui.frag");

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
			fixed (VkPipelineShaderStageCreateInfo* ptr = shader.Stages.GetInternalArray()) info.pStages = ptr;
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
				VkImageViewCreateInfo info = new VkImageViewCreateInfo
				{
					sType = VkStructureType.ImageViewCreateInfo,
					image = g_FontImage,
					viewType = VkImageViewType.Image2D,
					format = VkFormat.R8G8B8A8UNorm,
					subresourceRange =
					{
						aspectMask = VkImageAspectFlags.Color,
						levelCount = 1,
						layerCount = 1
					}
				};
				Vulkan.vkCreateImageView(gd.LogicalDevice, &info, null, out g_FontView).CheckResult();
			}

			VkDescriptorSet font_descriptor_set = ImGui_ImplVulkan_AddTexture(g_FontSampler, g_FontView, VkImageLayout.ShaderReadOnlyOptimal);

			// Create the Upload Buffer:
			{
				VkBufferCreateInfo buffer_info = new VkBufferCreateInfo
				{
					sType = VkStructureType.BufferCreateInfo,
					size = upload_size,
					usage = VkBufferUsageFlags.TransferSrc,
					sharingMode = VkSharingMode.Exclusive
				};
				Vulkan.vkCreateBuffer(gd.LogicalDevice, &buffer_info, null, out g_UploadBuffer);

				Vulkan.vkGetBufferMemoryRequirements(gd.LogicalDevice, g_UploadBuffer, out VkMemoryRequirements req);
				VkMemoryAllocateInfo alloc_info = new VkMemoryAllocateInfo
				{
					sType = VkStructureType.MemoryAllocateInfo,
					allocationSize = req.size,
					memoryTypeIndex = VulkanUtils.FindMemoryType(gd, req.memoryTypeBits, VkMemoryPropertyFlags.HostVisible)
				};

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
			io.Fonts.TexID = (IntPtr)font_descriptor_set.Handle;
		}

		private struct ImGui_ImplVulkanH_FrameRenderBuffers
		{
			internal VertexBuffer<ImDrawVert>? VertexBuffer;
			internal IndexBuffer<ushort>? IndexBuffer;
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
			ref ImGui_ImplVulkanH_WindowRenderBuffers wrb = ref g_MainWindowRenderBuffers;
			if (wrb.FrameRenderBuffers == null)
			{
				wrb.Index = 0;
				wrb.Count = 3;
				wrb.FrameRenderBuffers = new ImGui_ImplVulkanH_FrameRenderBuffers[3];
			}

			// IM_ASSERT(wrb->Count == v->ImageCount);
			wrb.Index = (wrb.Index + 1) % wrb.Count;
			ref ImGui_ImplVulkanH_FrameRenderBuffers rb = ref wrb.FrameRenderBuffers[wrb.Index];

			if (draw_data.TotalVtxCount > 0)
			{
				if (rb.VertexBuffer == null) rb.VertexBuffer = new VertexBuffer<ImDrawVert>(gd, (uint)draw_data.TotalVtxCount);
				else rb.VertexBuffer.Resize((uint)draw_data.TotalVtxCount);
				if (rb.IndexBuffer == null) rb.IndexBuffer = new IndexBuffer<ushort>(gd, (uint)draw_data.TotalIdxCount);
				else rb.IndexBuffer.Resize((uint)draw_data.TotalIdxCount);

				// Upload vertex/index data into a single contiguous GPU buffer
				ImDrawVert* vtx_dst = rb.VertexBuffer.Map();
				ushort* idx_dst = rb.IndexBuffer.Map();

				for (int n = 0; n < draw_data.CmdListsCount; n++)
				{
					ImDrawList* cmd_list = draw_data.CmdLists[n];

					Buffer.MemoryCopy(cmd_list->VtxBuffer.Data.ToPointer(), vtx_dst, cmd_list->VtxBuffer.Size * sizeof(ImDrawVert), cmd_list->VtxBuffer.Size * sizeof(ImDrawVert));
					Buffer.MemoryCopy(cmd_list->IdxBuffer.Data.ToPointer(), idx_dst, cmd_list->IdxBuffer.Size * sizeof(ushort), cmd_list->IdxBuffer.Size * sizeof(ushort));
					vtx_dst += cmd_list->VtxBuffer.Size;
					idx_dst += cmd_list->IdxBuffer.Size;
				}

				rb.VertexBuffer.Unmap();
				rb.IndexBuffer.Unmap();
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

							VkDescriptorSet descriptorSet = new VkDescriptorSet((ulong)pcmd.TextureId);
							Vulkan.vkCmdBindDescriptorSets(command_buffer, VkPipelineBindPoint.Graphics, _pipelineLayout, 0, descriptorSet);

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
			Vulkan.vkCmdBindPipeline(command_buffer, VkPipelineBindPoint.Graphics, pipeline);

			// Bind Vertex And Index Buffer:
			if (draw_data.TotalVtxCount > 0)
			{
				rb.VertexBuffer!.Bind(command_buffer);
				rb.IndexBuffer!.Bind(command_buffer);
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

		public static unsafe void Dispose()
		{
			for (ImGuiMouseCursor i = 0; i < ImGuiMouseCursor.COUNT; i++)
			{
				Glfw.DestroyCursor(g_MouseCursors[i]);
				g_MouseCursors[i] = Cursor.None;
			}

			for (int n = 0; n < g_MainWindowRenderBuffers.Count; n++)
			{
				var buffers = g_MainWindowRenderBuffers.FrameRenderBuffers[n];
				buffers.VertexBuffer?.Dispose();
				buffers.IndexBuffer?.Dispose();
			}

			if (g_FontView != VkImageView.Null)
			{
				Vulkan.vkDestroyImageView(gd.LogicalDevice, g_FontView, null);
				g_FontView = VkImageView.Null;
			}

			if (g_FontImage != VkImage.Null)
			{
				Vulkan.vkDestroyImage(gd.LogicalDevice, g_FontImage, null);
				g_FontImage = VkImage.Null;
			}

			if (g_FontMemory != VkDeviceMemory.Null)
			{
				Vulkan.vkFreeMemory(gd.LogicalDevice, g_FontMemory, null);
				g_FontMemory = VkDeviceMemory.Null;
			}

			if (g_FontSampler != VkSampler.Null)
			{
				Vulkan.vkDestroySampler(gd.LogicalDevice, g_FontSampler, null);
				g_FontSampler = VkSampler.Null;
			}

			if (_descriptorSetLayout != VkDescriptorSetLayout.Null)
			{
				Vulkan.vkDestroyDescriptorSetLayout(gd.LogicalDevice, _descriptorSetLayout, null);
				_descriptorSetLayout = VkDescriptorSetLayout.Null;
			}

			if (imguiPool != VkDescriptorPool.Null)
			{
				Vulkan.vkDestroyDescriptorPool(gd.LogicalDevice, imguiPool, null);
				imguiPool = VkDescriptorPool.Null;
			}

			if (_pipelineLayout != VkPipelineLayout.Null)
			{
				Vulkan.vkDestroyPipelineLayout(gd.LogicalDevice, _pipelineLayout, null);
				_pipelineLayout = VkPipelineLayout.Null;
			}

			if (_pipeline != VkPipeline.Null)
			{
				Vulkan.vkDestroyPipeline(gd.LogicalDevice, _pipeline, null);
				_pipeline = VkPipeline.Null;
			}
		}
	}
}