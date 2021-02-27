using System;
namespace Fireburst {
public enum VkImageLayout {
Undefined = 0,
General = 1,
ColorAttachmentOptimal = 2,
DepthStencilAttachmentOptimal = 3,
DepthStencilReadOnlyOptimal = 4,
ShaderReadOnlyOptimal = 5,
TransferSrcOptimal = 6,
TransferDstOptimal = 7,
Preinitialized = 8,
}
public enum VkAttachmentLoadOp {
Load = 0,
Clear = 1,
DontCare = 2,
}
public enum VkAttachmentStoreOp {
Store = 0,
DontCare = 1,
}
public enum VkImageType {
Image1D = 0,
Image2D = 1,
Image3D = 2,
}
public enum VkImageTiling {
Optimal = 0,
Linear = 1,
}
public enum VkImageViewType {
Image1D = 0,
Image2D = 1,
Image3D = 2,
ImageCube = 3,
Image1DArray = 4,
Image2DArray = 5,
ImageCubeArray = 6,
}
public enum VkCommandBufferLevel {
Primary = 0,
Secondary = 1,
}
public enum VkComponentSwizzle {
Identity = 0,
Zero = 1,
One = 2,
R = 3,
G = 4,
B = 5,
A = 6,
}
public enum VkDescriptorType {
Sampler = 0,
CombinedImageSampler = 1,
SampledImage = 2,
StorageImage = 3,
UniformTexelBuffer = 4,
StorageTexelBuffer = 5,
UniformBuffer = 6,
StorageBuffer = 7,
UniformBufferDynamic = 8,
StorageBufferDynamic = 9,
InputAttachment = 10,
}
public enum VkQueryType {
Occlusion = 0,
PipelineStatistics = 1,
Timestamp = 2,
}
public enum VkBorderColor {
FloatTransparentBlack = 0,
IntTransparentBlack = 1,
FloatOpaqueBlack = 2,
IntOpaqueBlack = 3,
FloatOpaqueWhite = 4,
IntOpaqueWhite = 5,
}
public enum VkPipelineBindPoint {
Graphics = 0,
Compute = 1,
}
public enum VkPipelineCacheHeaderVersion {
One = 1,
}
[Flags]
public enum VkPipelineCacheCreateFlags {
None = 0
}
public enum VkPrimitiveTopology {
PointList = 0,
LineList = 1,
LineStrip = 2,
TriangleList = 3,
TriangleStrip = 4,
TriangleFan = 5,
LineListWithAdjacency = 6,
LineStripWithAdjacency = 7,
TriangleListWithAdjacency = 8,
TriangleStripWithAdjacency = 9,
PatchList = 10,
}
public enum VkSharingMode {
Exclusive = 0,
Concurrent = 1,
}
public enum VkIndexType {
Uint16 = 0,
Uint32 = 1,
}
public enum VkFilter {
Nearest = 0,
Linear = 1,
}
public enum VkSamplerMipmapMode {
Nearest = 0,
Linear = 1,
}
public enum VkSamplerAddressMode {
Repeat = 0,
MirroredRepeat = 1,
ClampToEdge = 2,
ClampToBorder = 3,
}
public enum VkCompareOp {
Never = 0,
Less = 1,
Equal = 2,
LessOrEqual = 3,
Greater = 4,
NotEqual = 5,
GreaterOrEqual = 6,
Always = 7,
}
public enum VkPolygonMode {
Fill = 0,
Line = 1,
Point = 2,
}
public enum VkFrontFace {
CounterClockwise = 0,
Clockwise = 1,
}
public enum VkBlendFactor {
Zero = 0,
One = 1,
SrcColor = 2,
OneMinusSrcColor = 3,
DstColor = 4,
OneMinusDstColor = 5,
SrcAlpha = 6,
OneMinusSrcAlpha = 7,
DstAlpha = 8,
OneMinusDstAlpha = 9,
ConstantColor = 10,
OneMinusConstantColor = 11,
ConstantAlpha = 12,
OneMinusConstantAlpha = 13,
SrcAlphaSaturate = 14,
Src1Color = 15,
OneMinusSrc1Color = 16,
Src1Alpha = 17,
OneMinusSrc1Alpha = 18,
}
public enum VkBlendOp {
Add = 0,
Subtract = 1,
ReverseSubtract = 2,
Min = 3,
Max = 4,
}
public enum VkStencilOp {
Keep = 0,
Zero = 1,
Replace = 2,
IncrementAndClamp = 3,
DecrementAndClamp = 4,
Invert = 5,
IncrementAndWrap = 6,
DecrementAndWrap = 7,
}
public enum VkLogicOp {
Clear = 0,
And = 1,
AndReverse = 2,
Copy = 3,
AndInverted = 4,
NoOp = 5,
Xor = 6,
Or = 7,
Nor = 8,
Equivalent = 9,
Invert = 10,
OrReverse = 11,
CopyInverted = 12,
OrInverted = 13,
Nand = 14,
Set = 15,
}
public enum VkInternalAllocationType {
Executable = 0,
}
public enum VkSystemAllocationScope {
Command = 0,
Object = 1,
Cache = 2,
Device = 3,
Instance = 4,
}
public enum VkPhysicalDeviceType {
Other = 0,
IntegratedGpu = 1,
DiscreteGpu = 2,
VirtualGpu = 3,
Cpu = 4,
}
public enum VkVertexInputRate {
Vertex = 0,
Instance = 1,
}
public enum VkFormat {
Undefined = 0,
R4g4UnormPack8 = 1,
R4g4b4a4UnormPack16 = 2,
B4g4r4a4UnormPack16 = 3,
R5g6b5UnormPack16 = 4,
B5g6r5UnormPack16 = 5,
R5g5b5a1UnormPack16 = 6,
B5g5r5a1UnormPack16 = 7,
A1r5g5b5UnormPack16 = 8,
R8Unorm = 9,
R8Snorm = 10,
R8Uscaled = 11,
R8Sscaled = 12,
R8Uint = 13,
R8Sint = 14,
R8Srgb = 15,
R8g8Unorm = 16,
R8g8Snorm = 17,
R8g8Uscaled = 18,
R8g8Sscaled = 19,
R8g8Uint = 20,
R8g8Sint = 21,
R8g8Srgb = 22,
R8g8b8Unorm = 23,
R8g8b8Snorm = 24,
R8g8b8Uscaled = 25,
R8g8b8Sscaled = 26,
R8g8b8Uint = 27,
R8g8b8Sint = 28,
R8g8b8Srgb = 29,
B8g8r8Unorm = 30,
B8g8r8Snorm = 31,
B8g8r8Uscaled = 32,
B8g8r8Sscaled = 33,
B8g8r8Uint = 34,
B8g8r8Sint = 35,
B8g8r8Srgb = 36,
R8g8b8a8Unorm = 37,
R8g8b8a8Snorm = 38,
R8g8b8a8Uscaled = 39,
R8g8b8a8Sscaled = 40,
R8g8b8a8Uint = 41,
R8g8b8a8Sint = 42,
R8g8b8a8Srgb = 43,
B8g8r8a8Unorm = 44,
B8g8r8a8Snorm = 45,
B8g8r8a8Uscaled = 46,
B8g8r8a8Sscaled = 47,
B8g8r8a8Uint = 48,
B8g8r8a8Sint = 49,
B8g8r8a8Srgb = 50,
A8b8g8r8UnormPack32 = 51,
A8b8g8r8SnormPack32 = 52,
A8b8g8r8UscaledPack32 = 53,
A8b8g8r8SscaledPack32 = 54,
A8b8g8r8UintPack32 = 55,
A8b8g8r8SintPack32 = 56,
A8b8g8r8SrgbPack32 = 57,
A2r10g10b10UnormPack32 = 58,
A2r10g10b10SnormPack32 = 59,
A2r10g10b10UscaledPack32 = 60,
A2r10g10b10SscaledPack32 = 61,
A2r10g10b10UintPack32 = 62,
A2r10g10b10SintPack32 = 63,
A2b10g10r10UnormPack32 = 64,
A2b10g10r10SnormPack32 = 65,
A2b10g10r10UscaledPack32 = 66,
A2b10g10r10SscaledPack32 = 67,
A2b10g10r10UintPack32 = 68,
A2b10g10r10SintPack32 = 69,
R16Unorm = 70,
R16Snorm = 71,
R16Uscaled = 72,
R16Sscaled = 73,
R16Uint = 74,
R16Sint = 75,
R16Sfloat = 76,
R16g16Unorm = 77,
R16g16Snorm = 78,
R16g16Uscaled = 79,
R16g16Sscaled = 80,
R16g16Uint = 81,
R16g16Sint = 82,
R16g16Sfloat = 83,
R16g16b16Unorm = 84,
R16g16b16Snorm = 85,
R16g16b16Uscaled = 86,
R16g16b16Sscaled = 87,
R16g16b16Uint = 88,
R16g16b16Sint = 89,
R16g16b16Sfloat = 90,
R16g16b16a16Unorm = 91,
R16g16b16a16Snorm = 92,
R16g16b16a16Uscaled = 93,
R16g16b16a16Sscaled = 94,
R16g16b16a16Uint = 95,
R16g16b16a16Sint = 96,
R16g16b16a16Sfloat = 97,
R32Uint = 98,
R32Sint = 99,
R32Sfloat = 100,
R32g32Uint = 101,
R32g32Sint = 102,
R32g32Sfloat = 103,
R32g32b32Uint = 104,
R32g32b32Sint = 105,
R32g32b32Sfloat = 106,
R32g32b32a32Uint = 107,
R32g32b32a32Sint = 108,
R32g32b32a32Sfloat = 109,
R64Uint = 110,
R64Sint = 111,
R64Sfloat = 112,
R64g64Uint = 113,
R64g64Sint = 114,
R64g64Sfloat = 115,
R64g64b64Uint = 116,
R64g64b64Sint = 117,
R64g64b64Sfloat = 118,
R64g64b64a64Uint = 119,
R64g64b64a64Sint = 120,
R64g64b64a64Sfloat = 121,
B10g11r11UfloatPack32 = 122,
E5b9g9r9UfloatPack32 = 123,
D16Unorm = 124,
X8D24UnormPack32 = 125,
D32Sfloat = 126,
S8Uint = 127,
D16UnormS8Uint = 128,
D24UnormS8Uint = 129,
D32SfloatS8Uint = 130,
Bc1RgbUnormBlock = 131,
Bc1RgbSrgbBlock = 132,
Bc1RgbaUnormBlock = 133,
Bc1RgbaSrgbBlock = 134,
Bc2UnormBlock = 135,
Bc2SrgbBlock = 136,
Bc3UnormBlock = 137,
Bc3SrgbBlock = 138,
Bc4UnormBlock = 139,
Bc4SnormBlock = 140,
Bc5UnormBlock = 141,
Bc5SnormBlock = 142,
Bc6hUfloatBlock = 143,
Bc6hSfloatBlock = 144,
Bc7UnormBlock = 145,
Bc7SrgbBlock = 146,
Etc2R8g8b8UnormBlock = 147,
Etc2R8g8b8SrgbBlock = 148,
Etc2R8g8b8a1UnormBlock = 149,
Etc2R8g8b8a1SrgbBlock = 150,
Etc2R8g8b8a8UnormBlock = 151,
Etc2R8g8b8a8SrgbBlock = 152,
EacR11UnormBlock = 153,
EacR11SnormBlock = 154,
EacR11g11UnormBlock = 155,
EacR11g11SnormBlock = 156,
Astc4x4UnormBlock = 157,
Astc4x4SrgbBlock = 158,
Astc5x4UnormBlock = 159,
Astc5x4SrgbBlock = 160,
Astc5x5UnormBlock = 161,
Astc5x5SrgbBlock = 162,
Astc6x5UnormBlock = 163,
Astc6x5SrgbBlock = 164,
Astc6x6UnormBlock = 165,
Astc6x6SrgbBlock = 166,
Astc8x5UnormBlock = 167,
Astc8x5SrgbBlock = 168,
Astc8x6UnormBlock = 169,
Astc8x6SrgbBlock = 170,
Astc8x8UnormBlock = 171,
Astc8x8SrgbBlock = 172,
Astc10x5UnormBlock = 173,
Astc10x5SrgbBlock = 174,
Astc10x6UnormBlock = 175,
Astc10x6SrgbBlock = 176,
Astc10x8UnormBlock = 177,
Astc10x8SrgbBlock = 178,
Astc10x10UnormBlock = 179,
Astc10x10SrgbBlock = 180,
Astc12x10UnormBlock = 181,
Astc12x10SrgbBlock = 182,
Astc12x12UnormBlock = 183,
Astc12x12SrgbBlock = 184,
}
public enum VkStructureType {
ApplicationInfo = 0,
InstanceCreateInfo = 1,
DeviceQueueCreateInfo = 2,
DeviceCreateInfo = 3,
SubmitInfo = 4,
MemoryAllocateInfo = 5,
MappedMemoryRange = 6,
BindSparseInfo = 7,
FenceCreateInfo = 8,
SemaphoreCreateInfo = 9,
EventCreateInfo = 10,
QueryPoolCreateInfo = 11,
BufferCreateInfo = 12,
BufferViewCreateInfo = 13,
ImageCreateInfo = 14,
ImageViewCreateInfo = 15,
ShaderModuleCreateInfo = 16,
PipelineCacheCreateInfo = 17,
PipelineShaderStageCreateInfo = 18,
PipelineVertexInputStateCreateInfo = 19,
PipelineInputAssemblyStateCreateInfo = 20,
PipelineTessellationStateCreateInfo = 21,
PipelineViewportStateCreateInfo = 22,
PipelineRasterizationStateCreateInfo = 23,
PipelineMultisampleStateCreateInfo = 24,
PipelineDepthStencilStateCreateInfo = 25,
PipelineColorBlendStateCreateInfo = 26,
PipelineDynamicStateCreateInfo = 27,
GraphicsPipelineCreateInfo = 28,
ComputePipelineCreateInfo = 29,
PipelineLayoutCreateInfo = 30,
SamplerCreateInfo = 31,
DescriptorSetLayoutCreateInfo = 32,
DescriptorPoolCreateInfo = 33,
DescriptorSetAllocateInfo = 34,
WriteDescriptorSet = 35,
CopyDescriptorSet = 36,
FramebufferCreateInfo = 37,
RenderPassCreateInfo = 38,
CommandPoolCreateInfo = 39,
CommandBufferAllocateInfo = 40,
CommandBufferInheritanceInfo = 41,
CommandBufferBeginInfo = 42,
RenderPassBeginInfo = 43,
BufferMemoryBarrier = 44,
ImageMemoryBarrier = 45,
MemoryBarrier = 46,
LoaderInstanceCreateInfo = 47,
LoaderDeviceCreateInfo = 48,
}
public enum VkSubpassContents {
Inline = 0,
SecondaryCommandBuffers = 1,
}
public enum VkResult {
VK_SUCCESS = 0,
VK_NOT_READY = 1,
VK_TIMEOUT = 2,
VK_EVENT_SET = 3,
VK_EVENT_RESET = 4,
VK_INCOMPLETE = 5,
VK_ERROR_OUT_OF_HOST_MEMORY = -1,
VK_ERROR_OUT_OF_DEVICE_MEMORY = -2,
VK_ERROR_INITIALIZATION_FAILED = -3,
VK_ERROR_DEVICE_LOST = -4,
VK_ERROR_MEMORY_MAP_FAILED = -5,
VK_ERROR_LAYER_NOT_PRESENT = -6,
VK_ERROR_EXTENSION_NOT_PRESENT = -7,
VK_ERROR_FEATURE_NOT_PRESENT = -8,
VK_ERROR_INCOMPATIBLE_DRIVER = -9,
VK_ERROR_TOO_MANY_OBJECTS = -10,
VK_ERROR_FORMAT_NOT_SUPPORTED = -11,
VK_ERROR_FRAGMENTED_POOL = -12,
VK_ERROR_UNKNOWN = -13,
}
public enum VkDynamicState {
Viewport = 0,
Scissor = 1,
LineWidth = 2,
DepthBias = 3,
BlendConstants = 4,
DepthBounds = 5,
StencilCompareMask = 6,
StencilWriteMask = 7,
StencilReference = 8,
}
public enum VkDescriptorUpdateTemplateType {
DescriptorSet = 0,
}
public enum VkObjectType {
Unknown = 0,
Instance = 1,
PhysicalDevice = 2,
Device = 3,
Queue = 4,
Semaphore = 5,
CommandBuffer = 6,
Fence = 7,
DeviceMemory = 8,
Buffer = 9,
Image = 10,
Event = 11,
QueryPool = 12,
BufferView = 13,
ImageView = 14,
ShaderModule = 15,
PipelineCache = 16,
PipelineLayout = 17,
RenderPass = 18,
Pipeline = 19,
DescriptorSetLayout = 20,
Sampler = 21,
DescriptorPool = 22,
DescriptorSet = 23,
Framebuffer = 24,
CommandPool = 25,
}
[Flags]
public enum VkQueueFlags {
GraphicsBit = 1,
ComputeBit = 2,
TransferBit = 4,
SparseBindingBit = 8,
}
[Flags]
public enum VkCullModeFlags {
None = 0,
FrontBit = 1,
BackBit = 2,
FrontAndBack = 0x00000003,
}
[Flags]
public enum VkRenderPassCreateFlags {
None = 0
}
[Flags]
public enum VkDeviceQueueCreateFlags {
None = 0
}
[Flags]
public enum VkMemoryPropertyFlags {
DeviceLocalBit = 1,
HostVisibleBit = 2,
HostCoherentBit = 4,
HostCachedBit = 8,
LazilyAllocatedBit = 16,
}
[Flags]
public enum VkMemoryHeapFlags {
DeviceLocalBit = 1,
}
[Flags]
public enum VkAccessFlags {
IndirectCommandReadBit = 1,
IndexReadBit = 2,
VertexAttributeReadBit = 4,
UniformReadBit = 8,
InputAttachmentReadBit = 16,
ShaderReadBit = 32,
ShaderWriteBit = 64,
ColorAttachmentReadBit = 128,
ColorAttachmentWriteBit = 256,
DepthStencilAttachmentReadBit = 512,
DepthStencilAttachmentWriteBit = 1024,
TransferReadBit = 2048,
TransferWriteBit = 4096,
HostReadBit = 8192,
HostWriteBit = 16384,
MemoryReadBit = 32768,
MemoryWriteBit = 65536,
}
[Flags]
public enum VkBufferUsageFlags {
TransferSrcBit = 1,
TransferDstBit = 2,
UniformTexelBufferBit = 4,
StorageTexelBufferBit = 8,
UniformBufferBit = 16,
StorageBufferBit = 32,
IndexBufferBit = 64,
VertexBufferBit = 128,
IndirectBufferBit = 256,
}
[Flags]
public enum VkBufferCreateFlags {
SparseBindingBit = 1,
SparseResidencyBit = 2,
SparseAliasedBit = 4,
}
[Flags]
public enum VkShaderStageFlags {
VertexBit = 1,
TessellationControlBit = 2,
TessellationEvaluationBit = 4,
GeometryBit = 8,
FragmentBit = 16,
ComputeBit = 32,
AllGraphics = 0x0000001F,
All = 0x7FFFFFFF,
}
[Flags]
public enum VkImageUsageFlags {
TransferSrcBit = 1,
TransferDstBit = 2,
SampledBit = 4,
StorageBit = 8,
ColorAttachmentBit = 16,
DepthStencilAttachmentBit = 32,
TransientAttachmentBit = 64,
InputAttachmentBit = 128,
}
[Flags]
public enum VkImageCreateFlags {
SparseBindingBit = 1,
SparseResidencyBit = 2,
SparseAliasedBit = 4,
MutableFormatBit = 8,
CubeCompatibleBit = 16,
}
[Flags]
public enum VkImageViewCreateFlags {
None = 0
}
[Flags]
public enum VkSamplerCreateFlags {
None = 0
}
[Flags]
public enum VkPipelineCreateFlags {
DisableOptimizationBit = 1,
AllowDerivativesBit = 2,
DerivativeBit = 4,
}
[Flags]
public enum VkPipelineShaderStageCreateFlags {
None = 0
}
[Flags]
public enum VkColorComponentFlags {
RBit = 1,
GBit = 2,
BBit = 4,
ABit = 8,
}
[Flags]
public enum VkFenceCreateFlags {
SignaledBit = 1,
}
[Flags]
public enum VkFormatFeatureFlags {
SampledImageBit = 1,
StorageImageBit = 2,
StorageImageAtomicBit = 4,
UniformTexelBufferBit = 8,
StorageTexelBufferBit = 16,
StorageTexelBufferAtomicBit = 32,
VertexBufferBit = 64,
ColorAttachmentBit = 128,
ColorAttachmentBlendBit = 256,
DepthStencilAttachmentBit = 512,
BlitSrcBit = 1024,
BlitDstBit = 2048,
SampledImageFilterLinearBit = 4096,
}
[Flags]
public enum VkQueryControlFlags {
PreciseBit = 1,
}
[Flags]
public enum VkQueryResultFlags {
_64Bit = 1,
WaitBit = 2,
WithAvailabilityBit = 4,
PartialBit = 8,
}
[Flags]
public enum VkCommandBufferUsageFlags {
OneTimeSubmitBit = 1,
RenderPassContinueBit = 2,
SimultaneousUseBit = 4,
}
[Flags]
public enum VkQueryPipelineStatisticFlags {
InputAssemblyVerticesBit = 1,
InputAssemblyPrimitivesBit = 2,
VertexShaderInvocationsBit = 4,
GeometryShaderInvocationsBit = 8,
GeometryShaderPrimitivesBit = 16,
ClippingInvocationsBit = 32,
ClippingPrimitivesBit = 64,
FragmentShaderInvocationsBit = 128,
TessellationControlShaderPatchesBit = 256,
TessellationEvaluationShaderInvocationsBit = 512,
ComputeShaderInvocationsBit = 1024,
}
[Flags]
public enum VkImageAspectFlags {
ColorBit = 1,
DepthBit = 2,
StencilBit = 4,
MetadataBit = 8,
}
[Flags]
public enum VkSparseImageFormatFlags {
SingleMiptailBit = 1,
AlignedMipSizeBit = 2,
NonstandardBlockSizeBit = 4,
}
[Flags]
public enum VkSparseMemoryBindFlags {
MetadataBit = 1,
}
[Flags]
public enum VkPipelineStageFlags {
TopOfPipeBit = 1,
DrawIndirectBit = 2,
VertexInputBit = 4,
VertexShaderBit = 8,
TessellationControlShaderBit = 16,
TessellationEvaluationShaderBit = 32,
GeometryShaderBit = 64,
FragmentShaderBit = 128,
EarlyFragmentTestsBit = 256,
LateFragmentTestsBit = 512,
ColorAttachmentOutputBit = 1024,
ComputeShaderBit = 2048,
TransferBit = 4096,
BottomOfPipeBit = 8192,
HostBit = 16384,
AllGraphicsBit = 32768,
AllCommandsBit = 65536,
}
[Flags]
public enum VkCommandPoolCreateFlags {
TransientBit = 1,
ResetCommandBufferBit = 2,
}
[Flags]
public enum VkCommandPoolResetFlags {
ReleaseResourcesBit = 1,
}
[Flags]
public enum VkCommandBufferResetFlags {
ReleaseResourcesBit = 1,
}
[Flags]
public enum VkSampleCountFlags {
Count1 = 1,
Count2 = 2,
Count4 = 4,
Count8 = 8,
Count16 = 16,
Count32 = 32,
Count64 = 64,
}
[Flags]
public enum VkAttachmentDescriptionFlags {
MayAliasBit = 1,
}
[Flags]
public enum VkStencilFaceFlags {
FrontBit = 1,
BackBit = 2,
FrontAndBack = 0x00000003,
}
[Flags]
public enum VkDescriptorPoolCreateFlags {
FreeDescriptorSetBit = 1,
}
[Flags]
public enum VkDependencyFlags {
ByRegionBit = 1,
}
public enum VkSemaphoreType {
Binary = 0,
Timeline = 1,
}
[Flags]
public enum VkSemaphoreWaitFlags {
AnyBit = 1,
}
public enum VkPresentModeKHR {
ImmediateKhr = 0,
MailboxKhr = 1,
FifoKhr = 2,
FifoRelaxedKhr = 3,
}
public enum VkColorSpaceKHR {
SrgbNonLinearKHR = 0,
}
[Flags]
public enum VkDisplayPlaneAlphaFlagsKHR {
OpaqueBitKhr = 1,
GlobalBitKhr = 2,
PerPixelBitKhr = 4,
PerPixelPremultipliedBitKhr = 8,
}
[Flags]
public enum VkCompositeAlphaFlagsKHR {
OpaqueBitKhr = 1,
PreMultipliedBitKhr = 2,
PostMultipliedBitKhr = 4,
InheritBitKhr = 8,
}
[Flags]
public enum VkSurfaceTransformFlagsKHR {
IdentityBitKhr = 1,
Rotate90BitKhr = 2,
Rotate180BitKhr = 4,
Rotate270BitKhr = 8,
HorizontalMirrorBitKhr = 16,
HorizontalMirrorRotate90BitKhr = 32,
HorizontalMirrorRotate180BitKhr = 64,
HorizontalMirrorRotate270BitKhr = 128,
InheritBitKhr = 256,
}
[Flags]
public enum VkSwapchainImageUsageFlagsANDROID {
SharedBitAndroid = 1,
}
public enum VkTimeDomainEXT {
DeviceExt = 0,
ClockMonotonicExt = 1,
ClockMonotonicRawExt = 2,
QueryPerformanceCounterExt = 3,
}
[Flags]
public enum VkDebugReportFlagsEXT {
InformationBitExt = 1,
WarningBitExt = 2,
PerformanceWarningBitExt = 4,
ErrorBitExt = 8,
DebugBitExt = 16,
}
public enum VkDebugReportObjectTypeEXT {
UnknownExt = 0,
InstanceExt = 1,
PhysicalDeviceExt = 2,
DeviceExt = 3,
QueueExt = 4,
SemaphoreExt = 5,
CommandBufferExt = 6,
FenceExt = 7,
DeviceMemoryExt = 8,
BufferExt = 9,
ImageExt = 10,
EventExt = 11,
QueryPoolExt = 12,
BufferViewExt = 13,
ImageViewExt = 14,
ShaderModuleExt = 15,
PipelineCacheExt = 16,
PipelineLayoutExt = 17,
RenderPassExt = 18,
PipelineExt = 19,
DescriptorSetLayoutExt = 20,
SamplerExt = 21,
DescriptorPoolExt = 22,
DescriptorSetExt = 23,
FramebufferExt = 24,
CommandPoolExt = 25,
SurfaceKhrExt = 26,
SwapchainKhrExt = 27,
DebugReportCallbackExtExt = 28,
DisplayKhrExt = 29,
DisplayModeKhrExt = 30,
ValidationCacheExtExt = 33,
}
public enum VkDeviceMemoryReportEventTypeEXT {
AllocateExt = 0,
FreeExt = 1,
ImportExt = 2,
UnimportExt = 3,
AllocationFailedExt = 4,
}
public enum VkRasterizationOrderAMD {
StrictAmd = 0,
RelaxedAmd = 1,
}
[Flags]
public enum VkExternalMemoryHandleTypeFlagsNV {
OpaqueWin32BitNv = 1,
OpaqueWin32KmtBitNv = 2,
D3d11ImageBitNv = 4,
D3d11ImageKmtBitNv = 8,
}
[Flags]
public enum VkExternalMemoryFeatureFlagsNV {
DedicatedOnlyBitNv = 1,
ExportableBitNv = 2,
ImportableBitNv = 4,
}
public enum VkValidationCheckEXT {
AllExt = 0,
ShadersExt = 1,
}
public enum VkValidationFeatureEnableEXT {
GpuAssistedExt = 0,
GpuAssistedReserveBindingSlotExt = 1,
BestPracticesExt = 2,
DebugPrintfExt = 3,
SynchronizationValidationExt = 4,
}
public enum VkValidationFeatureDisableEXT {
AllExt = 0,
ShadersExt = 1,
ThreadSafetyExt = 2,
ApiParametersExt = 3,
ObjectLifetimesExt = 4,
CoreChecksExt = 5,
UniqueHandlesExt = 6,
}
[Flags]
public enum VkSubgroupFeatureFlags {
BasicBit = 1,
VoteBit = 2,
ArithmeticBit = 4,
BallotBit = 8,
ShuffleBit = 16,
ShuffleRelativeBit = 32,
ClusteredBit = 64,
QuadBit = 128,
}
[Flags]
public enum VkIndirectCommandsLayoutUsageFlagsNV {
ExplicitPreprocessBitNv = 1,
IndexedSequencesBitNv = 2,
UnorderedSequencesBitNv = 4,
}
[Flags]
public enum VkIndirectStateFlagsNV {
FlagFrontfaceBitNv = 1,
}
public enum VkIndirectCommandsTokenTypeNV {
TypeShaderGroupNv = 0,
TypeStateFlagsNv = 1,
TypeIndexBufferNv = 2,
TypeVertexBufferNv = 3,
TypePushConstantNv = 4,
TypeDrawIndexedNv = 5,
TypeDrawNv = 6,
TypeDrawTasksNv = 7,
}
[Flags]
public enum VkPrivateDataSlotCreateFlagsEXT {
None = 0
}
[Flags]
public enum VkDescriptorSetLayoutCreateFlags {
None = 0
}
[Flags]
public enum VkExternalMemoryHandleTypeFlags {
OpaqueFdBit = 1,
OpaqueWin32Bit = 2,
OpaqueWin32KmtBit = 4,
D3d11TextureBit = 8,
D3d11TextureKmtBit = 16,
D3d12HeapBit = 32,
D3d12ResourceBit = 64,
}
[Flags]
public enum VkExternalMemoryFeatureFlags {
DedicatedOnlyBit = 1,
ExportableBit = 2,
ImportableBit = 4,
}
[Flags]
public enum VkExternalSemaphoreHandleTypeFlags {
OpaqueFdBit = 1,
OpaqueWin32Bit = 2,
OpaqueWin32KmtBit = 4,
D3d12FenceBit = 8,
SyncFdBit = 16,
}
[Flags]
public enum VkExternalSemaphoreFeatureFlags {
ExportableBit = 1,
ImportableBit = 2,
}
[Flags]
public enum VkSemaphoreImportFlags {
TemporaryBit = 1,
}
[Flags]
public enum VkExternalFenceHandleTypeFlags {
OpaqueFdBit = 1,
OpaqueWin32Bit = 2,
OpaqueWin32KmtBit = 4,
SyncFdBit = 8,
}
[Flags]
public enum VkExternalFenceFeatureFlags {
ExportableBit = 1,
ImportableBit = 2,
}
[Flags]
public enum VkFenceImportFlags {
TemporaryBit = 1,
}
[Flags]
public enum VkSurfaceCounterFlagsEXT {
VblankBitExt = 1,
}
public enum VkDisplayPowerStateEXT {
OffExt = 0,
SuspendExt = 1,
OnExt = 2,
}
public enum VkDeviceEventTypeEXT {
DisplayHotplugExt = 0,
}
public enum VkDisplayEventTypeEXT {
FirstPixelOutExt = 0,
}
[Flags]
public enum VkPeerMemoryFeatureFlags {
CopySrcBit = 1,
CopyDstBit = 2,
GenericSrcBit = 4,
GenericDstBit = 8,
}
[Flags]
public enum VkMemoryAllocateFlags {
DeviceMaskBit = 1,
}
[Flags]
public enum VkDeviceGroupPresentModeFlagsKHR {
LocalBitKhr = 1,
RemoteBitKhr = 2,
SumBitKhr = 4,
LocalMultiDeviceBitKhr = 8,
}
[Flags]
public enum VkSwapchainCreateFlagsKHR {
None = 0
}
public enum VkViewportCoordinateSwizzleNV {
VK_VIEWPORT_COORDINATE_SWIZZLE_POSITIVE_X_NV = 0,
VK_VIEWPORT_COORDINATE_SWIZZLE_NEGATIVE_X_NV = 1,
VK_VIEWPORT_COORDINATE_SWIZZLE_POSITIVE_Y_NV = 2,
VK_VIEWPORT_COORDINATE_SWIZZLE_NEGATIVE_Y_NV = 3,
VK_VIEWPORT_COORDINATE_SWIZZLE_POSITIVE_Z_NV = 4,
VK_VIEWPORT_COORDINATE_SWIZZLE_NEGATIVE_Z_NV = 5,
VK_VIEWPORT_COORDINATE_SWIZZLE_POSITIVE_W_NV = 6,
VK_VIEWPORT_COORDINATE_SWIZZLE_NEGATIVE_W_NV = 7,
}
public enum VkDiscardRectangleModeEXT {
InclusiveExt = 0,
ExclusiveExt = 1,
}
[Flags]
public enum VkSubpassDescriptionFlags {
None = 0
}
public enum VkPointClippingBehavior {
AllClipPlanes = 0,
UserClipPlanesOnly = 1,
}
public enum VkSamplerReductionMode {
WeightedAverage = 0,
Min = 1,
Max = 2,
}
public enum VkTessellationDomainOrigin {
UpperLeft = 0,
LowerLeft = 1,
}
public enum VkSamplerYcbcrModelConversion {
RgbIdentity = 0,
YcbcrIdentity = 1,
Ycbcr709 = 2,
Ycbcr601 = 3,
Ycbcr2020 = 4,
}
public enum VkSamplerYcbcrRange {
ItuFull = 0,
ItuNarrow = 1,
}
public enum VkChromaLocation {
CositedEven = 0,
Midpoint = 1,
}
public enum VkBlendOverlapEXT {
UncorrelatedExt = 0,
DisjointExt = 1,
ConjointExt = 2,
}
public enum VkCoverageModulationModeNV {
ModeNoneNv = 0,
ModeRgbNv = 1,
ModeAlphaNv = 2,
ModeRgbaNv = 3,
}
public enum VkCoverageReductionModeNV {
ModeMergeNv = 0,
ModeTruncateNv = 1,
}
public enum VkValidationCacheHeaderVersionEXT {
OneExt = 1,
}
public enum VkShaderInfoTypeAMD {
StatisticsAmd = 0,
BinaryAmd = 1,
DisassemblyAmd = 2,
}
public enum VkQueueGlobalPriorityEXT {
LowExt = 128,
MediumExt = 256,
HighExt = 512,
RealtimeExt = 1024,
}
[Flags]
public enum VkDebugUtilsMessageSeverityFlagsEXT {
VerboseBitExt = 1,
InfoBitExt = 16,
WarningBitExt = 256,
ErrorBitExt = 4096,
}
[Flags]
public enum VkDebugUtilsMessageTypeFlagsEXT {
GeneralBitExt = 1,
ValidationBitExt = 2,
PerformanceBitExt = 4,
}
public enum VkConservativeRasterizationModeEXT {
DisabledExt = 0,
OverestimateExt = 1,
UnderestimateExt = 2,
}
[Flags]
public enum VkDescriptorBindingFlags {
UpdateAfterBindBit = 1,
UpdateUnusedWhilePendingBit = 2,
PartiallyBoundBit = 4,
VariableDescriptorCountBit = 8,
}
public enum VkVendorId {
Viv = 0x10001,
Vsi = 0x10002,
Kazan = 0x10003,
Codeplay = 0x10004,
Mesa = 0x10005,
Pocl = 0x10006,
}
public enum VkDriverId {
AmdProprietary = 1,
AmdOpenSource = 2,
MesaRadv = 3,
NvidiaProprietary = 4,
IntelProprietaryWindows = 5,
IntelOpenSourceMesa = 6,
ImaginationProprietary = 7,
QualcommProprietary = 8,
ArmProprietary = 9,
GoogleSwiftShader = 10,
GgpProprietary = 11,
BroadcomProprietary = 12,
MesaLLVMPipe = 13,
Moltenvk = 14,
}
[Flags]
public enum VkConditionalRenderingFlagsEXT {
InvertedBitExt = 1,
}
[Flags]
public enum VkResolveModeFlags {
None = 0,
SampleZeroBit = 1,
AverageBit = 2,
MinBit = 4,
MaxBit = 8,
}
public enum VkShadingRatePaletteEntryNV {
VK_SHADING_RATE_PALETTE_ENTRY_NO_INVOCATIONS_NV = 0,
SixteenInvocationsPerPixel = 1,
EightInvocationsPerPixel = 2,
FourInvocationsPerPixel = 3,
TwoInvocationsPerPixel = 4,
OneInvocationPerPixel = 5,
OneInvocationPer2x1Pixels = 6,
OneInvocationPer1x2Pixels = 7,
OneInvocationPer2x2Pixels = 8,
OneInvocationPer4x2Pixels = 9,
OneInvocationPer2x4Pixels = 10,
OneInvocationPer4x4Pixels = 11,
}
public enum VkCoarseSampleOrderTypeNV {
TypeDefaultNv = 0,
TypeCustomNv = 1,
TypePixelMajorNv = 2,
TypeSampleMajorNv = 3,
}
[Flags]
public enum VkGeometryInstanceFlagsKHR {
TriangleFacingCullDisableBitKhr = 1,
TriangleFrontCounterclockwiseBitKhr = 2,
ForceOpaqueBitKhr = 4,
ForceNoOpaqueBitKhr = 8,
}
[Flags]
public enum VkGeometryFlagsKHR {
OpaqueBitKhr = 1,
NoDuplicateAnyHitInvocationBitKhr = 2,
}
[Flags]
public enum VkBuildAccelerationStructureFlagsKHR {
AllowUpdateBitKhr = 1,
AllowCompactionBitKhr = 2,
PreferFastTraceBitKhr = 4,
PreferFastBuildBitKhr = 8,
LowMemoryBitKhr = 16,
}
[Flags]
public enum VkAccelerationStructureCreateFlagsKHR {
DeviceAddressCaptureReplayBitKhr = 1,
}
public enum VkCopyAccelerationStructureModeKHR {
CloneKhr = 0,
CompactKhr = 1,
SerializeKhr = 2,
DeserializeKhr = 3,
}
public enum VkBuildAccelerationStructureModeKHR {
BuildKhr = 0,
UpdateKhr = 1,
}
public enum VkAccelerationStructureTypeKHR {
TopLevelKhr = 0,
BottomLevelKhr = 1,
GenericKhr = 2,
}
public enum VkGeometryTypeKHR {
TrianglesKhr = 0,
AabbsKhr = 1,
InstancesKhr = 2,
}
public enum VkAccelerationStructureMemoryRequirementsTypeNV {
Object = 0,
BuildScratch = 1,
UpdateScratch = 2,
}
public enum VkAccelerationStructureBuildTypeKHR {
HostKhr = 0,
DeviceKhr = 1,
HostOrDeviceKhr = 2,
}
public enum VkRayTracingShaderGroupTypeKHR {
GeneralKhr = 0,
TrianglesHitGroupKhr = 1,
ProceduralHitGroupKhr = 2,
}
public enum VkAccelerationStructureCompatibilityKHR {
CompatibleKhr = 0,
IncompatibleKhr = 1,
}
public enum VkShaderGroupShaderKHR {
GeneralKhr = 0,
ClosestHitKhr = 1,
AnyHitKhr = 2,
IntersectionKhr = 3,
}
public enum VkMemoryOverallocationBehaviorAMD {
DefaultAmd = 0,
AllowedAmd = 1,
DisallowedAmd = 2,
}
[Flags]
public enum VkFramebufferCreateFlags {
None = 0
}
public enum VkScopeNV {
ScopeDeviceNv = 1,
ScopeWorkgroupNv = 2,
ScopeSubgroupNv = 3,
ScopeQueueFamilyNv = 5,
}
public enum VkComponentTypeNV {
TypeFloat16Nv = 0,
TypeFloat32Nv = 1,
TypeFloat64Nv = 2,
TypeSint8Nv = 3,
TypeSint16Nv = 4,
TypeSint32Nv = 5,
TypeSint64Nv = 6,
TypeUint8Nv = 7,
TypeUint16Nv = 8,
TypeUint32Nv = 9,
TypeUint64Nv = 10,
}
[Flags]
public enum VkDeviceDiagnosticsConfigFlagsNV {
EnableShaderDebugInfoBitNv = 1,
EnableResourceTrackingBitNv = 2,
EnableAutomaticCheckpointsBitNv = 4,
}
[Flags]
public enum VkPipelineCreationFeedbackFlagsEXT {
ValidBitExt = 1,
ApplicationPipelineCacheHitBitExt = 2,
BasePipelineAccelerationBitExt = 4,
}
public enum VkFullScreenExclusiveEXT {
DefaultExt = 0,
AllowedExt = 1,
DisallowedExt = 2,
ApplicationControlledExt = 3,
}
public enum VkPerformanceCounterScopeKHR {
CommandBufferKhr = 0,
RenderPassKhr = 1,
CommandKhr = 2,
}
public enum VkPerformanceCounterUnitKHR {
GenericKhr = 0,
PercentageKhr = 1,
NanosecondsKhr = 2,
BytesKhr = 3,
BytesPerSecondKhr = 4,
KelvinKhr = 5,
WattsKhr = 6,
VoltsKhr = 7,
AmpsKhr = 8,
HertzKhr = 9,
CyclesKhr = 10,
}
public enum VkPerformanceCounterStorageKHR {
Int32Khr = 0,
Int64Khr = 1,
Uint32Khr = 2,
Uint64Khr = 3,
Float32Khr = 4,
Float64Khr = 5,
}
[Flags]
public enum VkPerformanceCounterDescriptionFlagsKHR {
PerformanceImpactingBitKhr = 1,
ConcurrentlyImpactedBitKhr = 2,
}
[Flags]
public enum VkAcquireProfilingLockFlagsKHR {
None = 0
}
[Flags]
public enum VkShaderCorePropertiesFlagsAMD {
None = 0
}
public enum VkPerformanceConfigurationTypeINTEL {
CommandQueueMetricsDiscoveryActivatedIntel = 0,
}
public enum VkQueryPoolSamplingModeINTEL {
ModeManualIntel = 0,
}
public enum VkPerformanceOverrideTypeINTEL {
TypeNullHardwareIntel = 0,
TypeFlushGpuCachesIntel = 1,
}
public enum VkPerformanceParameterTypeINTEL {
TypeHwCountersSupportedIntel = 0,
TypeStreamMarkerValidBitsIntel = 1,
}
public enum VkPerformanceValueTypeINTEL {
TypeUint32Intel = 0,
TypeUint64Intel = 1,
TypeFloatIntel = 2,
TypeBoolIntel = 3,
TypeStringIntel = 4,
}
public enum VkShaderFloatControlsIndependence {
_32BitOnly = 0,
All = 1,
None = 2,
}
public enum VkPipelineExecutableStatisticFormatKHR {
Bool32Khr = 0,
Int64Khr = 1,
Uint64Khr = 2,
Float64Khr = 3,
}
public enum VkLineRasterizationModeEXT {
DefaultExt = 0,
RectangularExt = 1,
BresenhamExt = 2,
RectangularSmoothExt = 3,
}
[Flags]
public enum VkShaderModuleCreateFlags {
None = 0
}
[Flags]
public enum VkPipelineCompilerControlFlagsAMD {
None = 0
}
[Flags]
public enum VkToolPurposeFlagsEXT {
ValidationBitExt = 1,
ProfilingBitExt = 2,
TracingBitExt = 4,
AdditionalFeaturesBitExt = 8,
ModifyingFeaturesBitExt = 16,
}
public enum VkFragmentShadingRateCombinerOpKHR {
KeepKhr = 0,
ReplaceKhr = 1,
MinKhr = 2,
MaxKhr = 3,
MulKhr = 4,
}
public enum VkFragmentShadingRateNV {
OneInvocationPerPixel = 0,
OneInvocationPer1x2Pixels = 1,
OneInvocationPer2x1Pixels = 4,
OneInvocationPer2x2Pixels = 5,
OneInvocationPer2x4Pixels = 6,
OneInvocationPer4x2Pixels = 9,
OneInvocationPer4x4Pixels = 10,
TwoInvocationsPerPixel = 11,
FourInvocationsPerPixel = 12,
EightInvocationsPerPixel = 13,
SixteenInvocationsPerPixel = 14,
NoInvocations = 15,
}
public enum VkFragmentShadingRateTypeNV {
TypeFragmentSizeNv = 0,
TypeEnumsNv = 1,
}
[Flags]
public enum VkAccessFlags2KHR {
VK_ACCESS_2_NONE_KHR = 0,
VK_ACCESS_2_INDIRECT_COMMAND_READ_BIT_KHR = 1,
VK_ACCESS_2_INDEX_READ_BIT_KHR = 2,
VK_ACCESS_2_VERTEX_ATTRIBUTE_READ_BIT_KHR = 4,
VK_ACCESS_2_UNIFORM_READ_BIT_KHR = 8,
VK_ACCESS_2_INPUT_ATTACHMENT_READ_BIT_KHR = 16,
VK_ACCESS_2_SHADER_READ_BIT_KHR = 32,
VK_ACCESS_2_SHADER_WRITE_BIT_KHR = 64,
VK_ACCESS_2_COLOR_ATTACHMENT_READ_BIT_KHR = 128,
VK_ACCESS_2_COLOR_ATTACHMENT_WRITE_BIT_KHR = 256,
VK_ACCESS_2_DEPTH_STENCIL_ATTACHMENT_READ_BIT_KHR = 512,
VK_ACCESS_2_DEPTH_STENCIL_ATTACHMENT_WRITE_BIT_KHR = 1024,
VK_ACCESS_2_TRANSFER_READ_BIT_KHR = 2048,
VK_ACCESS_2_TRANSFER_WRITE_BIT_KHR = 4096,
VK_ACCESS_2_HOST_READ_BIT_KHR = 8192,
VK_ACCESS_2_HOST_WRITE_BIT_KHR = 16384,
VK_ACCESS_2_MEMORY_READ_BIT_KHR = 32768,
VK_ACCESS_2_MEMORY_WRITE_BIT_KHR = 65536,
VK_ACCESS_2_SHADER_SAMPLED_READ_BIT_KHR = 1,
VK_ACCESS_2_SHADER_STORAGE_READ_BIT_KHR = 2,
VK_ACCESS_2_SHADER_STORAGE_WRITE_BIT_KHR = 4,
}
[Flags]
public enum VkPipelineStageFlags2KHR {
VK_PIPELINE_STAGE_2_NONE_KHR = 0,
VK_PIPELINE_STAGE_2_TOP_OF_PIPE_BIT_KHR = 1,
VK_PIPELINE_STAGE_2_DRAW_INDIRECT_BIT_KHR = 2,
VK_PIPELINE_STAGE_2_VERTEX_INPUT_BIT_KHR = 4,
VK_PIPELINE_STAGE_2_VERTEX_SHADER_BIT_KHR = 8,
VK_PIPELINE_STAGE_2_TESSELLATION_CONTROL_SHADER_BIT_KHR = 16,
VK_PIPELINE_STAGE_2_TESSELLATION_EVALUATION_SHADER_BIT_KHR = 32,
VK_PIPELINE_STAGE_2_GEOMETRY_SHADER_BIT_KHR = 64,
VK_PIPELINE_STAGE_2_FRAGMENT_SHADER_BIT_KHR = 128,
VK_PIPELINE_STAGE_2_EARLY_FRAGMENT_TESTS_BIT_KHR = 256,
VK_PIPELINE_STAGE_2_LATE_FRAGMENT_TESTS_BIT_KHR = 512,
VK_PIPELINE_STAGE_2_COLOR_ATTACHMENT_OUTPUT_BIT_KHR = 1024,
VK_PIPELINE_STAGE_2_COMPUTE_SHADER_BIT_KHR = 2048,
VK_PIPELINE_STAGE_2_ALL_TRANSFER_BIT_KHR = 4096,
VK_PIPELINE_STAGE_2_BOTTOM_OF_PIPE_BIT_KHR = 8192,
VK_PIPELINE_STAGE_2_HOST_BIT_KHR = 16384,
VK_PIPELINE_STAGE_2_ALL_GRAPHICS_BIT_KHR = 32768,
VK_PIPELINE_STAGE_2_ALL_COMMANDS_BIT_KHR = 65536,
VK_PIPELINE_STAGE_2_COPY_BIT_KHR = 1,
VK_PIPELINE_STAGE_2_RESOLVE_BIT_KHR = 2,
VK_PIPELINE_STAGE_2_BLIT_BIT_KHR = 4,
VK_PIPELINE_STAGE_2_CLEAR_BIT_KHR = 8,
VK_PIPELINE_STAGE_2_INDEX_INPUT_BIT_KHR = 16,
VK_PIPELINE_STAGE_2_VERTEX_ATTRIBUTE_INPUT_BIT_KHR = 32,
VK_PIPELINE_STAGE_2_PRE_RASTERIZATION_SHADERS_BIT_KHR = 64,
}
[Flags]
public enum VkSubmitFlagsKHR {
ProtectedBitKhr = 1,
}
[Flags]
public enum VkEventCreateFlags {
None = 0
}
[Flags]
public enum VkQueryPoolCreateFlags {
None = 0,
}
[Flags]
public enum VkPipelineLayoutCreateFlags {
None = 0,
}
[Flags]
public enum VkPipelineDepthStencilStateCreateFlags {
None = 0,
}
[Flags]
public enum VkPipelineDynamicStateCreateFlags {
None = 0,
}
[Flags]
public enum VkPipelineColorBlendStateCreateFlags {
None = 0,
}
[Flags]
public enum VkPipelineMultisampleStateCreateFlags {
None = 0,
}
[Flags]
public enum VkPipelineRasterizationStateCreateFlags {
None = 0,
}
[Flags]
public enum VkPipelineViewportStateCreateFlags {
None = 0,
}
[Flags]
public enum VkPipelineTessellationStateCreateFlags {
None = 0,
}
[Flags]
public enum VkPipelineInputAssemblyStateCreateFlags {
None = 0,
}
[Flags]
public enum VkPipelineVertexInputStateCreateFlags {
None = 0,
}
[Flags]
public enum VkBufferViewCreateFlags {
None = 0,
}
[Flags]
public enum VkInstanceCreateFlags {
None = 0,
}
[Flags]
public enum VkDeviceCreateFlags {
None = 0,
}
[Flags]
public enum VkSemaphoreCreateFlags {
None = 0,
}
[Flags]
public enum VkMemoryMapFlags {
None = 0,
}
[Flags]
public enum VkDescriptorPoolResetFlags {
None = 0,
}
[Flags]
public enum VkDescriptorUpdateTemplateCreateFlags {
None = 0,
}
[Flags]
public enum VkDisplayModeCreateFlagsKHR {
None = 0,
}
[Flags]
public enum VkDisplaySurfaceCreateFlagsKHR {
None = 0,
}
[Flags]
public enum VkAndroidSurfaceCreateFlagsKHR {
None = 0,
}
[Flags]
public enum VkViSurfaceCreateFlagsNN {
None = 0,
}
[Flags]
public enum VkWaylandSurfaceCreateFlagsKHR {
None = 0,
}
[Flags]
public enum VkWin32SurfaceCreateFlagsKHR {
None = 0,
}
[Flags]
public enum VkXlibSurfaceCreateFlagsKHR {
None = 0,
}
[Flags]
public enum VkXcbSurfaceCreateFlagsKHR {
None = 0,
}
[Flags]
public enum VkDirectFBSurfaceCreateFlagsEXT {
None = 0,
}
[Flags]
public enum VkIOSSurfaceCreateFlagsMVK {
None = 0,
}
[Flags]
public enum VkMacOSSurfaceCreateFlagsMVK {
None = 0,
}
[Flags]
public enum VkMetalSurfaceCreateFlagsEXT {
None = 0,
}
[Flags]
public enum VkImagePipeSurfaceCreateFlagsFUCHSIA {
None = 0,
}
[Flags]
public enum VkStreamDescriptorSurfaceCreateFlagsGGP {
None = 0,
}
[Flags]
public enum VkHeadlessSurfaceCreateFlagsEXT {
None = 0,
}
[Flags]
public enum VkCommandPoolTrimFlags {
None = 0,
}
[Flags]
public enum VkPipelineViewportSwizzleStateCreateFlagsNV {
None = 0,
}
[Flags]
public enum VkPipelineDiscardRectangleStateCreateFlagsEXT {
None = 0,
}
[Flags]
public enum VkPipelineCoverageToColorStateCreateFlagsNV {
None = 0,
}
[Flags]
public enum VkPipelineCoverageModulationStateCreateFlagsNV {
None = 0,
}
[Flags]
public enum VkPipelineCoverageReductionStateCreateFlagsNV {
None = 0,
}
[Flags]
public enum VkValidationCacheCreateFlagsEXT {
None = 0,
}
[Flags]
public enum VkDebugUtilsMessengerCreateFlagsEXT {
None = 0,
}
[Flags]
public enum VkDebugUtilsMessengerCallbackDataFlagsEXT {
None = 0,
}
[Flags]
public enum VkDeviceMemoryReportFlagsEXT {
None = 0,
}
[Flags]
public enum VkPipelineRasterizationConservativeStateCreateFlagsEXT {
None = 0,
}
[Flags]
public enum VkPipelineRasterizationStateStreamCreateFlagsEXT {
None = 0,
}
[Flags]
public enum VkPipelineRasterizationDepthClipStateCreateFlagsEXT {
None = 0,
}
}