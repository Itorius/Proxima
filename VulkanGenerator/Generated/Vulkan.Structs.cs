// ReSharper disable FieldCanBeMadeReadOnly.Global
using System;
using System.Runtime.InteropServices;
namespace Fireburst {
[StructLayout(LayoutKind.Sequential)]
public struct VkBaseOutStructure {
public VkStructureType sType;
public unsafe VkBaseOutStructure* pNext;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkBaseInStructure {
public VkStructureType sType;
public unsafe VkBaseInStructure* pNext;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkOffset2D {
public int x;
public int y;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkOffset3D {
public int x;
public int y;
public int z;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkExtent2D {
public uint width;
public uint height;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkExtent3D {
public uint width;
public uint height;
public uint depth;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkViewport {
public float x;
public float y;
public float width;
public float height;
public float minDepth;
public float maxDepth;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkRect2D {
public VkOffset2D offset;
public VkExtent2D extent;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkClearRect {
public VkRect2D rect;
public uint baseArrayLayer;
public uint layerCount;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkComponentMapping {
public VkComponentSwizzle r;
public VkComponentSwizzle g;
public VkComponentSwizzle b;
public VkComponentSwizzle a;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceProperties {
public uint apiVersion;
public uint driverVersion;
public uint vendorID;
public uint deviceID;
public VkPhysicalDeviceType deviceType;
public char deviceName;
public byte pipelineCacheUUID;
public VkPhysicalDeviceLimits limits;
public VkPhysicalDeviceSparseProperties sparseProperties;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkExtensionProperties {
public char extensionName;
public uint specVersion;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkLayerProperties {
public char layerName;
public uint specVersion;
public uint implementationVersion;
public char description;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkApplicationInfo {
public VkStructureType sType;
public unsafe void* pNext;
public unsafe char* pApplicationName;
public uint applicationVersion;
public unsafe char* pEngineName;
public uint engineVersion;
public uint apiVersion;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkAllocationCallbacks {
public unsafe void* pUserData;
public PFN_vkAllocationFunction pfnAllocation;
public PFN_vkReallocationFunction pfnReallocation;
public PFN_vkFreeFunction pfnFree;
public PFN_vkInternalAllocationNotification pfnInternalAllocation;
public PFN_vkInternalFreeNotification pfnInternalFree;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDeviceQueueCreateInfo {
public VkStructureType sType;
public unsafe void* pNext;
public VkDeviceQueueCreateFlags flags;
public uint queueFamilyIndex;
public uint queueCount;
public unsafe float* pQueuePriorities;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDeviceCreateInfo {
public VkStructureType sType;
public unsafe void* pNext;
public VkDeviceCreateFlags flags;
public uint queueCreateInfoCount;
public unsafe VkDeviceQueueCreateInfo* pQueueCreateInfos;
public uint enabledLayerCount;
public unsafe char* ppEnabledLayerNames;
public uint enabledExtensionCount;
public unsafe char* ppEnabledExtensionNames;
public unsafe VkPhysicalDeviceFeatures* pEnabledFeatures;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkInstanceCreateInfo {
public VkStructureType sType;
public unsafe void* pNext;
public VkInstanceCreateFlags flags;
public unsafe VkApplicationInfo* pApplicationInfo;
public uint enabledLayerCount;
public unsafe char* ppEnabledLayerNames;
public uint enabledExtensionCount;
public unsafe char* ppEnabledExtensionNames;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkQueueFamilyProperties {
public VkQueueFlags queueFlags;
public uint queueCount;
public uint timestampValidBits;
public VkExtent3D minImageTransferGranularity;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceMemoryProperties {
public uint memoryTypeCount;
public VkMemoryType memoryTypes;
public uint memoryHeapCount;
public VkMemoryHeap memoryHeaps;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkMemoryAllocateInfo {
public VkStructureType sType;
public unsafe void* pNext;
public ulong allocationSize;
public uint memoryTypeIndex;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkMemoryRequirements {
public ulong size;
public ulong alignment;
public uint memoryTypeBits;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkSparseImageFormatProperties {
public VkImageAspectFlags aspectMask;
public VkExtent3D imageGranularity;
public VkSparseImageFormatFlags flags;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkSparseImageMemoryRequirements {
public VkSparseImageFormatProperties formatProperties;
public uint imageMipTailFirstLod;
public ulong imageMipTailSize;
public ulong imageMipTailOffset;
public ulong imageMipTailStride;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkMemoryType {
public VkMemoryPropertyFlags propertyFlags;
public uint heapIndex;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkMemoryHeap {
public ulong size;
public VkMemoryHeapFlags flags;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkMappedMemoryRange {
public VkStructureType sType;
public unsafe void* pNext;
public VkDeviceMemory memory;
public ulong offset;
public ulong size;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkFormatProperties {
public VkFormatFeatureFlags linearTilingFeatures;
public VkFormatFeatureFlags optimalTilingFeatures;
public VkFormatFeatureFlags bufferFeatures;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkImageFormatProperties {
public VkExtent3D maxExtent;
public uint maxMipLevels;
public uint maxArrayLayers;
public VkSampleCountFlags sampleCounts;
public ulong maxResourceSize;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDescriptorBufferInfo {
public VkBuffer buffer;
public ulong offset;
public ulong range;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDescriptorImageInfo {
public VkSampler sampler;
public VkImageView imageView;
public VkImageLayout imageLayout;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkWriteDescriptorSet {
public VkStructureType sType;
public unsafe void* pNext;
public VkDescriptorSet dstSet;
public uint dstBinding;
public uint dstArrayElement;
public uint descriptorCount;
public VkDescriptorType descriptorType;
public unsafe VkDescriptorImageInfo* pImageInfo;
public unsafe VkDescriptorBufferInfo* pBufferInfo;
public unsafe VkBufferView* pTexelBufferView;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkCopyDescriptorSet {
public VkStructureType sType;
public unsafe void* pNext;
public VkDescriptorSet srcSet;
public uint srcBinding;
public uint srcArrayElement;
public VkDescriptorSet dstSet;
public uint dstBinding;
public uint dstArrayElement;
public uint descriptorCount;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkBufferCreateInfo {
public VkStructureType sType;
public unsafe void* pNext;
public VkBufferCreateFlags flags;
public ulong size;
public VkBufferUsageFlags usage;
public VkSharingMode sharingMode;
public uint queueFamilyIndexCount;
public unsafe uint* pQueueFamilyIndices;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkBufferViewCreateInfo {
public VkStructureType sType;
public unsafe void* pNext;
public VkBufferViewCreateFlags flags;
public VkBuffer buffer;
public VkFormat format;
public ulong offset;
public ulong range;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkImageSubresource {
public VkImageAspectFlags aspectMask;
public uint mipLevel;
public uint arrayLayer;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkImageSubresourceLayers {
public VkImageAspectFlags aspectMask;
public uint mipLevel;
public uint baseArrayLayer;
public uint layerCount;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkImageSubresourceRange {
public VkImageAspectFlags aspectMask;
public uint baseMipLevel;
public uint levelCount;
public uint baseArrayLayer;
public uint layerCount;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkMemoryBarrier {
public VkStructureType sType;
public unsafe void* pNext;
public VkAccessFlags srcAccessMask;
public VkAccessFlags dstAccessMask;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkBufferMemoryBarrier {
public VkStructureType sType;
public unsafe void* pNext;
public VkAccessFlags srcAccessMask;
public VkAccessFlags dstAccessMask;
public uint srcQueueFamilyIndex;
public uint dstQueueFamilyIndex;
public VkBuffer buffer;
public ulong offset;
public ulong size;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkImageMemoryBarrier {
public VkStructureType sType;
public unsafe void* pNext;
public VkAccessFlags srcAccessMask;
public VkAccessFlags dstAccessMask;
public VkImageLayout oldLayout;
public VkImageLayout newLayout;
public uint srcQueueFamilyIndex;
public uint dstQueueFamilyIndex;
public VkImage image;
public VkImageSubresourceRange subresourceRange;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkImageCreateInfo {
public VkStructureType sType;
public unsafe void* pNext;
public VkImageCreateFlags flags;
public VkImageType imageType;
public VkFormat format;
public VkExtent3D extent;
public uint mipLevels;
public uint arrayLayers;
public VkSampleCountFlagBits samples;
public VkImageTiling tiling;
public VkImageUsageFlags usage;
public VkSharingMode sharingMode;
public uint queueFamilyIndexCount;
public unsafe uint* pQueueFamilyIndices;
public VkImageLayout initialLayout;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkSubresourceLayout {
public ulong offset;
public ulong size;
public ulong rowPitch;
public ulong arrayPitch;
public ulong depthPitch;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkImageViewCreateInfo {
public VkStructureType sType;
public unsafe void* pNext;
public VkImageViewCreateFlags flags;
public VkImage image;
public VkImageViewType viewType;
public VkFormat format;
public VkComponentMapping components;
public VkImageSubresourceRange subresourceRange;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkBufferCopy {
public ulong srcOffset;
public ulong dstOffset;
public ulong size;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkSparseMemoryBind {
public ulong resourceOffset;
public ulong size;
public VkDeviceMemory memory;
public ulong memoryOffset;
public VkSparseMemoryBindFlags flags;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkSparseImageMemoryBind {
public VkImageSubresource subresource;
public VkOffset3D offset;
public VkExtent3D extent;
public VkDeviceMemory memory;
public ulong memoryOffset;
public VkSparseMemoryBindFlags flags;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkSparseBufferMemoryBindInfo {
public VkBuffer buffer;
public uint bindCount;
public unsafe VkSparseMemoryBind* pBinds;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkSparseImageOpaqueMemoryBindInfo {
public VkImage image;
public uint bindCount;
public unsafe VkSparseMemoryBind* pBinds;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkSparseImageMemoryBindInfo {
public VkImage image;
public uint bindCount;
public unsafe VkSparseImageMemoryBind* pBinds;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkBindSparseInfo {
public VkStructureType sType;
public unsafe void* pNext;
public uint waitSemaphoreCount;
public unsafe VkSemaphore* pWaitSemaphores;
public uint bufferBindCount;
public unsafe VkSparseBufferMemoryBindInfo* pBufferBinds;
public uint imageOpaqueBindCount;
public unsafe VkSparseImageOpaqueMemoryBindInfo* pImageOpaqueBinds;
public uint imageBindCount;
public unsafe VkSparseImageMemoryBindInfo* pImageBinds;
public uint signalSemaphoreCount;
public unsafe VkSemaphore* pSignalSemaphores;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkImageCopy {
public VkImageSubresourceLayers srcSubresource;
public VkOffset3D srcOffset;
public VkImageSubresourceLayers dstSubresource;
public VkOffset3D dstOffset;
public VkExtent3D extent;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkImageBlit {
public VkImageSubresourceLayers srcSubresource;
public VkOffset3D srcOffsets;
public VkImageSubresourceLayers dstSubresource;
public VkOffset3D dstOffsets;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkBufferImageCopy {
public ulong bufferOffset;
public uint bufferRowLength;
public uint bufferImageHeight;
public VkImageSubresourceLayers imageSubresource;
public VkOffset3D imageOffset;
public VkExtent3D imageExtent;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkImageResolve {
public VkImageSubresourceLayers srcSubresource;
public VkOffset3D srcOffset;
public VkImageSubresourceLayers dstSubresource;
public VkOffset3D dstOffset;
public VkExtent3D extent;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkShaderModuleCreateInfo {
public VkStructureType sType;
public unsafe void* pNext;
public VkShaderModuleCreateFlags flags;
public VkPointerSize codeSize;
public unsafe uint* pCode;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDescriptorSetLayoutBinding {
public uint binding;
public VkDescriptorType descriptorType;
public uint descriptorCount;
public VkShaderStageFlags stageFlags;
public unsafe VkSampler* pImmutableSamplers;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDescriptorSetLayoutCreateInfo {
public VkStructureType sType;
public unsafe void* pNext;
public VkDescriptorSetLayoutCreateFlags flags;
public uint bindingCount;
public unsafe VkDescriptorSetLayoutBinding* pBindings;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDescriptorPoolSize {
public VkDescriptorType type;
public uint descriptorCount;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDescriptorPoolCreateInfo {
public VkStructureType sType;
public unsafe void* pNext;
public VkDescriptorPoolCreateFlags flags;
public uint maxSets;
public uint poolSizeCount;
public unsafe VkDescriptorPoolSize* pPoolSizes;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDescriptorSetAllocateInfo {
public VkStructureType sType;
public unsafe void* pNext;
public VkDescriptorPool descriptorPool;
public uint descriptorSetCount;
public unsafe VkDescriptorSetLayout* pSetLayouts;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkSpecializationMapEntry {
public uint constantID;
public uint offset;
public VkPointerSize size;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkSpecializationInfo {
public uint mapEntryCount;
public unsafe VkSpecializationMapEntry* pMapEntries;
public VkPointerSize dataSize;
public unsafe void* pData;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPipelineShaderStageCreateInfo {
public VkStructureType sType;
public unsafe void* pNext;
public VkPipelineShaderStageCreateFlags flags;
public VkShaderStageFlagBits stage;
public VkShaderModule module;
public unsafe char* pName;
public unsafe VkSpecializationInfo* pSpecializationInfo;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkComputePipelineCreateInfo {
public VkStructureType sType;
public unsafe void* pNext;
public VkPipelineCreateFlags flags;
public VkPipelineShaderStageCreateInfo stage;
public VkPipelineLayout layout;
public VkPipeline basePipelineHandle;
public int basePipelineIndex;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkVertexInputBindingDescription {
public uint binding;
public uint stride;
public VkVertexInputRate inputRate;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkVertexInputAttributeDescription {
public uint location;
public uint binding;
public VkFormat format;
public uint offset;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPipelineVertexInputStateCreateInfo {
public VkStructureType sType;
public unsafe void* pNext;
public VkPipelineVertexInputStateCreateFlags flags;
public uint vertexBindingDescriptionCount;
public unsafe VkVertexInputBindingDescription* pVertexBindingDescriptions;
public uint vertexAttributeDescriptionCount;
public unsafe VkVertexInputAttributeDescription* pVertexAttributeDescriptions;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPipelineInputAssemblyStateCreateInfo {
public VkStructureType sType;
public unsafe void* pNext;
public VkPipelineInputAssemblyStateCreateFlags flags;
public VkPrimitiveTopology topology;
public VkBool32 primitiveRestartEnable;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPipelineTessellationStateCreateInfo {
public VkStructureType sType;
public unsafe void* pNext;
public VkPipelineTessellationStateCreateFlags flags;
public uint patchControlPoints;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPipelineViewportStateCreateInfo {
public VkStructureType sType;
public unsafe void* pNext;
public VkPipelineViewportStateCreateFlags flags;
public uint viewportCount;
public unsafe VkViewport* pViewports;
public uint scissorCount;
public unsafe VkRect2D* pScissors;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPipelineRasterizationStateCreateInfo {
public VkStructureType sType;
public unsafe void* pNext;
public VkPipelineRasterizationStateCreateFlags flags;
public VkBool32 depthClampEnable;
public VkBool32 rasterizerDiscardEnable;
public VkPolygonMode polygonMode;
public VkCullModeFlags cullMode;
public VkFrontFace frontFace;
public VkBool32 depthBiasEnable;
public float depthBiasConstantFactor;
public float depthBiasClamp;
public float depthBiasSlopeFactor;
public float lineWidth;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPipelineMultisampleStateCreateInfo {
public VkStructureType sType;
public unsafe void* pNext;
public VkPipelineMultisampleStateCreateFlags flags;
public VkSampleCountFlagBits rasterizationSamples;
public VkBool32 sampleShadingEnable;
public float minSampleShading;
public unsafe uint* pSampleMask;
public VkBool32 alphaToCoverageEnable;
public VkBool32 alphaToOneEnable;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPipelineColorBlendAttachmentState {
public VkBool32 blendEnable;
public VkBlendFactor srcColorBlendFactor;
public VkBlendFactor dstColorBlendFactor;
public VkBlendOp colorBlendOp;
public VkBlendFactor srcAlphaBlendFactor;
public VkBlendFactor dstAlphaBlendFactor;
public VkBlendOp alphaBlendOp;
public VkColorComponentFlags colorWriteMask;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPipelineColorBlendStateCreateInfo {
public VkStructureType sType;
public unsafe void* pNext;
public VkPipelineColorBlendStateCreateFlags flags;
public VkBool32 logicOpEnable;
public VkLogicOp logicOp;
public uint attachmentCount;
public unsafe VkPipelineColorBlendAttachmentState* pAttachments;
public float blendConstants;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPipelineDynamicStateCreateInfo {
public VkStructureType sType;
public unsafe void* pNext;
public VkPipelineDynamicStateCreateFlags flags;
public uint dynamicStateCount;
public unsafe VkDynamicState* pDynamicStates;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkStencilOpState {
public VkStencilOp failOp;
public VkStencilOp passOp;
public VkStencilOp depthFailOp;
public VkCompareOp compareOp;
public uint compareMask;
public uint writeMask;
public uint reference;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPipelineDepthStencilStateCreateInfo {
public VkStructureType sType;
public unsafe void* pNext;
public VkPipelineDepthStencilStateCreateFlags flags;
public VkBool32 depthTestEnable;
public VkBool32 depthWriteEnable;
public VkCompareOp depthCompareOp;
public VkBool32 depthBoundsTestEnable;
public VkBool32 stencilTestEnable;
public VkStencilOpState front;
public VkStencilOpState back;
public float minDepthBounds;
public float maxDepthBounds;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkGraphicsPipelineCreateInfo {
public VkStructureType sType;
public unsafe void* pNext;
public VkPipelineCreateFlags flags;
public uint stageCount;
public unsafe VkPipelineShaderStageCreateInfo* pStages;
public unsafe VkPipelineVertexInputStateCreateInfo* pVertexInputState;
public unsafe VkPipelineInputAssemblyStateCreateInfo* pInputAssemblyState;
public unsafe VkPipelineTessellationStateCreateInfo* pTessellationState;
public unsafe VkPipelineViewportStateCreateInfo* pViewportState;
public unsafe VkPipelineRasterizationStateCreateInfo* pRasterizationState;
public unsafe VkPipelineMultisampleStateCreateInfo* pMultisampleState;
public unsafe VkPipelineDepthStencilStateCreateInfo* pDepthStencilState;
public unsafe VkPipelineColorBlendStateCreateInfo* pColorBlendState;
public unsafe VkPipelineDynamicStateCreateInfo* pDynamicState;
public VkPipelineLayout layout;
public VkRenderPass renderPass;
public uint subpass;
public VkPipeline basePipelineHandle;
public int basePipelineIndex;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPipelineCacheCreateInfo {
public VkStructureType sType;
public unsafe void* pNext;
public VkPipelineCacheCreateFlags flags;
public VkPointerSize initialDataSize;
public unsafe void* pInitialData;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPushConstantRange {
public VkShaderStageFlags stageFlags;
public uint offset;
public uint size;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPipelineLayoutCreateInfo {
public VkStructureType sType;
public unsafe void* pNext;
public VkPipelineLayoutCreateFlags flags;
public uint setLayoutCount;
public unsafe VkDescriptorSetLayout* pSetLayouts;
public uint pushConstantRangeCount;
public unsafe VkPushConstantRange* pPushConstantRanges;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkSamplerCreateInfo {
public VkStructureType sType;
public unsafe void* pNext;
public VkSamplerCreateFlags flags;
public VkFilter magFilter;
public VkFilter minFilter;
public VkSamplerMipmapMode mipmapMode;
public VkSamplerAddressMode addressModeU;
public VkSamplerAddressMode addressModeV;
public VkSamplerAddressMode addressModeW;
public float mipLodBias;
public VkBool32 anisotropyEnable;
public float maxAnisotropy;
public VkBool32 compareEnable;
public VkCompareOp compareOp;
public float minLod;
public float maxLod;
public VkBorderColor borderColor;
public VkBool32 unnormalizedCoordinates;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkCommandPoolCreateInfo {
public VkStructureType sType;
public unsafe void* pNext;
public VkCommandPoolCreateFlags flags;
public uint queueFamilyIndex;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkCommandBufferAllocateInfo {
public VkStructureType sType;
public unsafe void* pNext;
public VkCommandPool commandPool;
public VkCommandBufferLevel level;
public uint commandBufferCount;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkCommandBufferInheritanceInfo {
public VkStructureType sType;
public unsafe void* pNext;
public VkRenderPass renderPass;
public uint subpass;
public VkFramebuffer framebuffer;
public VkBool32 occlusionQueryEnable;
public VkQueryControlFlags queryFlags;
public VkQueryPipelineStatisticFlags pipelineStatistics;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkCommandBufferBeginInfo {
public VkStructureType sType;
public unsafe void* pNext;
public VkCommandBufferUsageFlags flags;
public unsafe VkCommandBufferInheritanceInfo* pInheritanceInfo;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkRenderPassBeginInfo {
public VkStructureType sType;
public unsafe void* pNext;
public VkRenderPass renderPass;
public VkFramebuffer framebuffer;
public VkRect2D renderArea;
public uint clearValueCount;
public unsafe VkClearValue* pClearValues;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkClearDepthStencilValue {
public float depth;
public uint stencil;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkClearAttachment {
public VkImageAspectFlags aspectMask;
public uint colorAttachment;
public VkClearValue clearValue;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkAttachmentDescription {
public VkAttachmentDescriptionFlags flags;
public VkFormat format;
public VkSampleCountFlagBits samples;
public VkAttachmentLoadOp loadOp;
public VkAttachmentStoreOp storeOp;
public VkAttachmentLoadOp stencilLoadOp;
public VkAttachmentStoreOp stencilStoreOp;
public VkImageLayout initialLayout;
public VkImageLayout finalLayout;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkAttachmentReference {
public uint attachment;
public VkImageLayout layout;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkSubpassDescription {
public VkSubpassDescriptionFlags flags;
public VkPipelineBindPoint pipelineBindPoint;
public uint inputAttachmentCount;
public unsafe VkAttachmentReference* pInputAttachments;
public uint colorAttachmentCount;
public unsafe VkAttachmentReference* pColorAttachments;
public unsafe VkAttachmentReference* pResolveAttachments;
public unsafe VkAttachmentReference* pDepthStencilAttachment;
public uint preserveAttachmentCount;
public unsafe uint* pPreserveAttachments;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkSubpassDependency {
public uint srcSubpass;
public uint dstSubpass;
public VkPipelineStageFlags srcStageMask;
public VkPipelineStageFlags dstStageMask;
public VkAccessFlags srcAccessMask;
public VkAccessFlags dstAccessMask;
public VkDependencyFlags dependencyFlags;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkRenderPassCreateInfo {
public VkStructureType sType;
public unsafe void* pNext;
public VkRenderPassCreateFlags flags;
public uint attachmentCount;
public unsafe VkAttachmentDescription* pAttachments;
public uint subpassCount;
public unsafe VkSubpassDescription* pSubpasses;
public uint dependencyCount;
public unsafe VkSubpassDependency* pDependencies;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkEventCreateInfo {
public VkStructureType sType;
public unsafe void* pNext;
public VkEventCreateFlags flags;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkFenceCreateInfo {
public VkStructureType sType;
public unsafe void* pNext;
public VkFenceCreateFlags flags;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceFeatures {
public VkBool32 robustBufferAccess;
public VkBool32 fullDrawIndexUint32;
public VkBool32 imageCubeArray;
public VkBool32 independentBlend;
public VkBool32 geometryShader;
public VkBool32 tessellationShader;
public VkBool32 sampleRateShading;
public VkBool32 dualSrcBlend;
public VkBool32 logicOp;
public VkBool32 multiDrawIndirect;
public VkBool32 drawIndirectFirstInstance;
public VkBool32 depthClamp;
public VkBool32 depthBiasClamp;
public VkBool32 fillModeNonSolid;
public VkBool32 depthBounds;
public VkBool32 wideLines;
public VkBool32 largePoints;
public VkBool32 alphaToOne;
public VkBool32 multiViewport;
public VkBool32 samplerAnisotropy;
public VkBool32 textureCompressionETC2;
public VkBool32 textureCompressionASTC_LDR;
public VkBool32 textureCompressionBC;
public VkBool32 occlusionQueryPrecise;
public VkBool32 pipelineStatisticsQuery;
public VkBool32 vertexPipelineStoresAndAtomics;
public VkBool32 fragmentStoresAndAtomics;
public VkBool32 shaderTessellationAndGeometryPointSize;
public VkBool32 shaderImageGatherExtended;
public VkBool32 shaderStorageImageExtendedFormats;
public VkBool32 shaderStorageImageMultisample;
public VkBool32 shaderStorageImageReadWithoutFormat;
public VkBool32 shaderStorageImageWriteWithoutFormat;
public VkBool32 shaderUniformBufferArrayDynamicIndexing;
public VkBool32 shaderSampledImageArrayDynamicIndexing;
public VkBool32 shaderStorageBufferArrayDynamicIndexing;
public VkBool32 shaderStorageImageArrayDynamicIndexing;
public VkBool32 shaderClipDistance;
public VkBool32 shaderCullDistance;
public VkBool32 shaderFloat64;
public VkBool32 shaderInt64;
public VkBool32 shaderInt16;
public VkBool32 shaderResourceResidency;
public VkBool32 shaderResourceMinLod;
public VkBool32 sparseBinding;
public VkBool32 sparseResidencyBuffer;
public VkBool32 sparseResidencyImage2D;
public VkBool32 sparseResidencyImage3D;
public VkBool32 sparseResidency2Samples;
public VkBool32 sparseResidency4Samples;
public VkBool32 sparseResidency8Samples;
public VkBool32 sparseResidency16Samples;
public VkBool32 sparseResidencyAliased;
public VkBool32 variableMultisampleRate;
public VkBool32 inheritedQueries;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceSparseProperties {
public VkBool32 residencyStandard2DBlockShape;
public VkBool32 residencyStandard2DMultisampleBlockShape;
public VkBool32 residencyStandard3DBlockShape;
public VkBool32 residencyAlignedMipSize;
public VkBool32 residencyNonResidentStrict;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceLimits {
public uint maxImageDimension1D;
public uint maxImageDimension2D;
public uint maxImageDimension3D;
public uint maxImageDimensionCube;
public uint maxImageArrayLayers;
public uint maxTexelBufferElements;
public uint maxUniformBufferRange;
public uint maxStorageBufferRange;
public uint maxPushConstantsSize;
public uint maxMemoryAllocationCount;
public uint maxSamplerAllocationCount;
public ulong bufferImageGranularity;
public ulong sparseAddressSpaceSize;
public uint maxBoundDescriptorSets;
public uint maxPerStageDescriptorSamplers;
public uint maxPerStageDescriptorUniformBuffers;
public uint maxPerStageDescriptorStorageBuffers;
public uint maxPerStageDescriptorSampledImages;
public uint maxPerStageDescriptorStorageImages;
public uint maxPerStageDescriptorInputAttachments;
public uint maxPerStageResources;
public uint maxDescriptorSetSamplers;
public uint maxDescriptorSetUniformBuffers;
public uint maxDescriptorSetUniformBuffersDynamic;
public uint maxDescriptorSetStorageBuffers;
public uint maxDescriptorSetStorageBuffersDynamic;
public uint maxDescriptorSetSampledImages;
public uint maxDescriptorSetStorageImages;
public uint maxDescriptorSetInputAttachments;
public uint maxVertexInputAttributes;
public uint maxVertexInputBindings;
public uint maxVertexInputAttributeOffset;
public uint maxVertexInputBindingStride;
public uint maxVertexOutputComponents;
public uint maxTessellationGenerationLevel;
public uint maxTessellationPatchSize;
public uint maxTessellationControlPerVertexInputComponents;
public uint maxTessellationControlPerVertexOutputComponents;
public uint maxTessellationControlPerPatchOutputComponents;
public uint maxTessellationControlTotalOutputComponents;
public uint maxTessellationEvaluationInputComponents;
public uint maxTessellationEvaluationOutputComponents;
public uint maxGeometryShaderInvocations;
public uint maxGeometryInputComponents;
public uint maxGeometryOutputComponents;
public uint maxGeometryOutputVertices;
public uint maxGeometryTotalOutputComponents;
public uint maxFragmentInputComponents;
public uint maxFragmentOutputAttachments;
public uint maxFragmentDualSrcAttachments;
public uint maxFragmentCombinedOutputResources;
public uint maxComputeSharedMemorySize;
public uint maxComputeWorkGroupCount;
public uint maxComputeWorkGroupInvocations;
public uint maxComputeWorkGroupSize;
public uint subPixelPrecisionBits;
public uint subTexelPrecisionBits;
public uint mipmapPrecisionBits;
public uint maxDrawIndexedIndexValue;
public uint maxDrawIndirectCount;
public float maxSamplerLodBias;
public float maxSamplerAnisotropy;
public uint maxViewports;
public uint maxViewportDimensions;
public float viewportBoundsRange;
public uint viewportSubPixelBits;
public VkPointerSize minMemoryMapAlignment;
public ulong minTexelBufferOffsetAlignment;
public ulong minUniformBufferOffsetAlignment;
public ulong minStorageBufferOffsetAlignment;
public int minTexelOffset;
public uint maxTexelOffset;
public int minTexelGatherOffset;
public uint maxTexelGatherOffset;
public float minInterpolationOffset;
public float maxInterpolationOffset;
public uint subPixelInterpolationOffsetBits;
public uint maxFramebufferWidth;
public uint maxFramebufferHeight;
public uint maxFramebufferLayers;
public VkSampleCountFlags framebufferColorSampleCounts;
public VkSampleCountFlags framebufferDepthSampleCounts;
public VkSampleCountFlags framebufferStencilSampleCounts;
public VkSampleCountFlags framebufferNoAttachmentsSampleCounts;
public uint maxColorAttachments;
public VkSampleCountFlags sampledImageColorSampleCounts;
public VkSampleCountFlags sampledImageIntegerSampleCounts;
public VkSampleCountFlags sampledImageDepthSampleCounts;
public VkSampleCountFlags sampledImageStencilSampleCounts;
public VkSampleCountFlags storageImageSampleCounts;
public uint maxSampleMaskWords;
public VkBool32 timestampComputeAndGraphics;
public float timestampPeriod;
public uint maxClipDistances;
public uint maxCullDistances;
public uint maxCombinedClipAndCullDistances;
public uint discreteQueuePriorities;
public float pointSizeRange;
public float lineWidthRange;
public float pointSizeGranularity;
public float lineWidthGranularity;
public VkBool32 strictLines;
public VkBool32 standardSampleLocations;
public ulong optimalBufferCopyOffsetAlignment;
public ulong optimalBufferCopyRowPitchAlignment;
public ulong nonCoherentAtomSize;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkSemaphoreCreateInfo {
public VkStructureType sType;
public unsafe void* pNext;
public VkSemaphoreCreateFlags flags;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkQueryPoolCreateInfo {
public VkStructureType sType;
public unsafe void* pNext;
public VkQueryPoolCreateFlags flags;
public VkQueryType queryType;
public uint queryCount;
public VkQueryPipelineStatisticFlags pipelineStatistics;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkFramebufferCreateInfo {
public VkStructureType sType;
public unsafe void* pNext;
public VkFramebufferCreateFlags flags;
public VkRenderPass renderPass;
public uint attachmentCount;
public unsafe VkImageView* pAttachments;
public uint width;
public uint height;
public uint layers;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDrawIndirectCommand {
public uint vertexCount;
public uint instanceCount;
public uint firstVertex;
public uint firstInstance;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDrawIndexedIndirectCommand {
public uint indexCount;
public uint instanceCount;
public uint firstIndex;
public int vertexOffset;
public uint firstInstance;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDispatchIndirectCommand {
public uint x;
public uint y;
public uint z;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkSubmitInfo {
public VkStructureType sType;
public unsafe void* pNext;
public uint waitSemaphoreCount;
public unsafe VkSemaphore* pWaitSemaphores;
public unsafe VkPipelineStageFlags* pWaitDstStageMask;
public uint commandBufferCount;
public unsafe VkCommandBuffer* pCommandBuffers;
public uint signalSemaphoreCount;
public unsafe VkSemaphore* pSignalSemaphores;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDisplayPropertiesKHR {
public VkDisplayKHR display;
public unsafe char* displayName;
public VkExtent2D physicalDimensions;
public VkExtent2D physicalResolution;
public VkSurfaceTransformFlagsKHR supportedTransforms;
public VkBool32 planeReorderPossible;
public VkBool32 persistentContent;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDisplayPlanePropertiesKHR {
public VkDisplayKHR currentDisplay;
public uint currentStackIndex;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDisplayModeParametersKHR {
public VkExtent2D visibleRegion;
public uint refreshRate;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDisplayModePropertiesKHR {
public VkDisplayModeKHR displayMode;
public VkDisplayModeParametersKHR parameters;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDisplayModeCreateInfoKHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkDisplayModeCreateFlagsKHR flags;
public VkDisplayModeParametersKHR parameters;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDisplayPlaneCapabilitiesKHR {
public VkDisplayPlaneAlphaFlagsKHR supportedAlpha;
public VkOffset2D minSrcPosition;
public VkOffset2D maxSrcPosition;
public VkExtent2D minSrcExtent;
public VkExtent2D maxSrcExtent;
public VkOffset2D minDstPosition;
public VkOffset2D maxDstPosition;
public VkExtent2D minDstExtent;
public VkExtent2D maxDstExtent;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDisplaySurfaceCreateInfoKHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkDisplaySurfaceCreateFlagsKHR flags;
public VkDisplayModeKHR displayMode;
public uint planeIndex;
public uint planeStackIndex;
public VkSurfaceTransformFlagBitsKHR transform;
public float globalAlpha;
public VkDisplayPlaneAlphaFlagBitsKHR alphaMode;
public VkExtent2D imageExtent;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDisplayPresentInfoKHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkRect2D srcRect;
public VkRect2D dstRect;
public VkBool32 persistent;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkSurfaceCapabilitiesKHR {
public uint minImageCount;
public uint maxImageCount;
public VkExtent2D currentExtent;
public VkExtent2D minImageExtent;
public VkExtent2D maxImageExtent;
public uint maxImageArrayLayers;
public VkSurfaceTransformFlagsKHR supportedTransforms;
public VkSurfaceTransformFlagBitsKHR currentTransform;
public VkCompositeAlphaFlagsKHR supportedCompositeAlpha;
public VkImageUsageFlags supportedUsageFlags;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkAndroidSurfaceCreateInfoKHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkAndroidSurfaceCreateFlagsKHR flags;
public unsafe IntPtr* window;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkViSurfaceCreateInfoNN {
public VkStructureType sType;
public unsafe void* pNext;
public VkViSurfaceCreateFlagsNN flags;
public unsafe void* window;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkWaylandSurfaceCreateInfoKHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkWaylandSurfaceCreateFlagsKHR flags;
public unsafe IntPtr* display;
public unsafe IntPtr* surface;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkWin32SurfaceCreateInfoKHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkWin32SurfaceCreateFlagsKHR flags;
public IntPtr hinstance;
public IntPtr hwnd;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkXlibSurfaceCreateInfoKHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkXlibSurfaceCreateFlagsKHR flags;
public unsafe IntPtr* dpy;
public IntPtr window;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkXcbSurfaceCreateInfoKHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkXcbSurfaceCreateFlagsKHR flags;
public unsafe IntPtr* connection;
public IntPtr window;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDirectFBSurfaceCreateInfoEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkDirectFBSurfaceCreateFlagsEXT flags;
public unsafe IntPtr* dfb;
public unsafe IntPtr* surface;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkImagePipeSurfaceCreateInfoFUCHSIA {
public VkStructureType sType;
public unsafe void* pNext;
public VkImagePipeSurfaceCreateFlagsFUCHSIA flags;
public IntPtr imagePipeHandle;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkStreamDescriptorSurfaceCreateInfoGGP {
public VkStructureType sType;
public unsafe void* pNext;
public VkStreamDescriptorSurfaceCreateFlagsGGP flags;
public IntPtr streamDescriptor;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkSurfaceFormatKHR {
public VkFormat format;
public VkColorSpaceKHR colorSpace;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkSwapchainCreateInfoKHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkSwapchainCreateFlagsKHR flags;
public VkSurfaceKHR surface;
public uint minImageCount;
public VkFormat imageFormat;
public VkColorSpaceKHR imageColorSpace;
public VkExtent2D imageExtent;
public uint imageArrayLayers;
public VkImageUsageFlags imageUsage;
public VkSharingMode imageSharingMode;
public uint queueFamilyIndexCount;
public unsafe uint* pQueueFamilyIndices;
public VkSurfaceTransformFlagBitsKHR preTransform;
public VkCompositeAlphaFlagBitsKHR compositeAlpha;
public VkPresentModeKHR presentMode;
public VkBool32 clipped;
public VkSwapchainKHR oldSwapchain;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPresentInfoKHR {
public VkStructureType sType;
public unsafe void* pNext;
public uint waitSemaphoreCount;
public unsafe VkSemaphore* pWaitSemaphores;
public uint swapchainCount;
public unsafe VkSwapchainKHR* pSwapchains;
public unsafe uint* pImageIndices;
public unsafe VkResult* pResults;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDebugReportCallbackCreateInfoEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkDebugReportFlagsEXT flags;
public PFN_vkDebugReportCallbackEXT pfnCallback;
public unsafe void* pUserData;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkValidationFlagsEXT {
public VkStructureType sType;
public unsafe void* pNext;
public uint disabledValidationCheckCount;
public unsafe VkValidationCheckEXT* pDisabledValidationChecks;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkValidationFeaturesEXT {
public VkStructureType sType;
public unsafe void* pNext;
public uint enabledValidationFeatureCount;
public unsafe VkValidationFeatureEnableEXT* pEnabledValidationFeatures;
public uint disabledValidationFeatureCount;
public unsafe VkValidationFeatureDisableEXT* pDisabledValidationFeatures;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPipelineRasterizationStateRasterizationOrderAMD {
public VkStructureType sType;
public unsafe void* pNext;
public VkRasterizationOrderAMD rasterizationOrder;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDebugMarkerObjectNameInfoEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkDebugReportObjectTypeEXT objectType;
public ulong @object;
public unsafe char* pObjectName;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDebugMarkerObjectTagInfoEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkDebugReportObjectTypeEXT objectType;
public ulong @object;
public ulong tagName;
public VkPointerSize tagSize;
public unsafe void* pTag;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDebugMarkerMarkerInfoEXT {
public VkStructureType sType;
public unsafe void* pNext;
public unsafe char* pMarkerName;
public float color;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDedicatedAllocationImageCreateInfoNV {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 dedicatedAllocation;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDedicatedAllocationBufferCreateInfoNV {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 dedicatedAllocation;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDedicatedAllocationMemoryAllocateInfoNV {
public VkStructureType sType;
public unsafe void* pNext;
public VkImage image;
public VkBuffer buffer;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkExternalImageFormatPropertiesNV {
public VkImageFormatProperties imageFormatProperties;
public VkExternalMemoryFeatureFlagsNV externalMemoryFeatures;
public VkExternalMemoryHandleTypeFlagsNV exportFromImportedHandleTypes;
public VkExternalMemoryHandleTypeFlagsNV compatibleHandleTypes;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkExternalMemoryImageCreateInfoNV {
public VkStructureType sType;
public unsafe void* pNext;
public VkExternalMemoryHandleTypeFlagsNV handleTypes;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkExportMemoryAllocateInfoNV {
public VkStructureType sType;
public unsafe void* pNext;
public VkExternalMemoryHandleTypeFlagsNV handleTypes;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkImportMemoryWin32HandleInfoNV {
public VkStructureType sType;
public unsafe void* pNext;
public VkExternalMemoryHandleTypeFlagsNV handleType;
public IntPtr handle;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkExportMemoryWin32HandleInfoNV {
public VkStructureType sType;
public unsafe void* pNext;
public unsafe IntPtr* pAttributes;
public IntPtr dwAccess;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkWin32KeyedMutexAcquireReleaseInfoNV {
public VkStructureType sType;
public unsafe void* pNext;
public uint acquireCount;
public unsafe VkDeviceMemory* pAcquireSyncs;
public unsafe ulong* pAcquireKeys;
public unsafe uint* pAcquireTimeoutMilliseconds;
public uint releaseCount;
public unsafe VkDeviceMemory* pReleaseSyncs;
public unsafe ulong* pReleaseKeys;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceDeviceGeneratedCommandsFeaturesNV {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 deviceGeneratedCommands;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDevicePrivateDataCreateInfoEXT {
public VkStructureType sType;
public unsafe void* pNext;
public uint privateDataSlotRequestCount;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPrivateDataSlotCreateInfoEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkPrivateDataSlotCreateFlagsEXT flags;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDevicePrivateDataFeaturesEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 privateData;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceDeviceGeneratedCommandsPropertiesNV {
public VkStructureType sType;
public unsafe void* pNext;
public uint maxGraphicsShaderGroupCount;
public uint maxIndirectSequenceCount;
public uint maxIndirectCommandsTokenCount;
public uint maxIndirectCommandsStreamCount;
public uint maxIndirectCommandsTokenOffset;
public uint maxIndirectCommandsStreamStride;
public uint minSequencesCountBufferOffsetAlignment;
public uint minSequencesIndexBufferOffsetAlignment;
public uint minIndirectCommandsBufferOffsetAlignment;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkGraphicsShaderGroupCreateInfoNV {
public VkStructureType sType;
public unsafe void* pNext;
public uint stageCount;
public unsafe VkPipelineShaderStageCreateInfo* pStages;
public unsafe VkPipelineVertexInputStateCreateInfo* pVertexInputState;
public unsafe VkPipelineTessellationStateCreateInfo* pTessellationState;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkGraphicsPipelineShaderGroupsCreateInfoNV {
public VkStructureType sType;
public unsafe void* pNext;
public uint groupCount;
public unsafe VkGraphicsShaderGroupCreateInfoNV* pGroups;
public uint pipelineCount;
public unsafe VkPipeline* pPipelines;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkBindShaderGroupIndirectCommandNV {
public uint groupIndex;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkBindIndexBufferIndirectCommandNV {
public IntPtr bufferAddress;
public uint size;
public VkIndexType indexType;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkBindVertexBufferIndirectCommandNV {
public IntPtr bufferAddress;
public uint size;
public uint stride;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkSetStateFlagsIndirectCommandNV {
public uint data;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkIndirectCommandsStreamNV {
public VkBuffer buffer;
public ulong offset;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkIndirectCommandsLayoutTokenNV {
public VkStructureType sType;
public unsafe void* pNext;
public VkIndirectCommandsTokenTypeNV tokenType;
public uint stream;
public uint offset;
public uint vertexBindingUnit;
public VkBool32 vertexDynamicStride;
public VkPipelineLayout pushconstantPipelineLayout;
public VkShaderStageFlags pushconstantShaderStageFlags;
public uint pushconstantOffset;
public uint pushconstantSize;
public VkIndirectStateFlagsNV indirectStateFlags;
public uint indexTypeCount;
public unsafe VkIndexType* pIndexTypes;
public unsafe uint* pIndexTypeValues;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkIndirectCommandsLayoutCreateInfoNV {
public VkStructureType sType;
public unsafe void* pNext;
public VkIndirectCommandsLayoutUsageFlagsNV flags;
public VkPipelineBindPoint pipelineBindPoint;
public uint tokenCount;
public unsafe VkIndirectCommandsLayoutTokenNV* pTokens;
public uint streamCount;
public unsafe uint* pStreamStrides;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkGeneratedCommandsInfoNV {
public VkStructureType sType;
public unsafe void* pNext;
public VkPipelineBindPoint pipelineBindPoint;
public VkPipeline pipeline;
public VkIndirectCommandsLayoutNV indirectCommandsLayout;
public uint streamCount;
public unsafe VkIndirectCommandsStreamNV* pStreams;
public uint sequencesCount;
public VkBuffer preprocessBuffer;
public ulong preprocessOffset;
public ulong preprocessSize;
public VkBuffer sequencesCountBuffer;
public ulong sequencesCountOffset;
public VkBuffer sequencesIndexBuffer;
public ulong sequencesIndexOffset;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkGeneratedCommandsMemoryRequirementsInfoNV {
public VkStructureType sType;
public unsafe void* pNext;
public VkPipelineBindPoint pipelineBindPoint;
public VkPipeline pipeline;
public VkIndirectCommandsLayoutNV indirectCommandsLayout;
public uint maxSequencesCount;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceFeatures2 {
public VkStructureType sType;
public unsafe void* pNext;
public VkPhysicalDeviceFeatures features;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceFeatures2KHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceProperties2 {
public VkStructureType sType;
public unsafe void* pNext;
public VkPhysicalDeviceProperties properties;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceProperties2KHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkFormatProperties2 {
public VkStructureType sType;
public unsafe void* pNext;
public VkFormatProperties formatProperties;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkFormatProperties2KHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkImageFormatProperties2 {
public VkStructureType sType;
public unsafe void* pNext;
public VkImageFormatProperties imageFormatProperties;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkImageFormatProperties2KHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceImageFormatInfo2 {
public VkStructureType sType;
public unsafe void* pNext;
public VkFormat format;
public VkImageType type;
public VkImageTiling tiling;
public VkImageUsageFlags usage;
public VkImageCreateFlags flags;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceImageFormatInfo2KHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkQueueFamilyProperties2 {
public VkStructureType sType;
public unsafe void* pNext;
public VkQueueFamilyProperties queueFamilyProperties;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkQueueFamilyProperties2KHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceMemoryProperties2 {
public VkStructureType sType;
public unsafe void* pNext;
public VkPhysicalDeviceMemoryProperties memoryProperties;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceMemoryProperties2KHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkSparseImageFormatProperties2 {
public VkStructureType sType;
public unsafe void* pNext;
public VkSparseImageFormatProperties properties;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkSparseImageFormatProperties2KHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceSparseImageFormatInfo2 {
public VkStructureType sType;
public unsafe void* pNext;
public VkFormat format;
public VkImageType type;
public VkSampleCountFlagBits samples;
public VkImageUsageFlags usage;
public VkImageTiling tiling;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceSparseImageFormatInfo2KHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDevicePushDescriptorPropertiesKHR {
public VkStructureType sType;
public unsafe void* pNext;
public uint maxPushDescriptors;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkConformanceVersion {
public byte major;
public byte minor;
public byte subminor;
public byte patch;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkConformanceVersionKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceDriverProperties {
public VkStructureType sType;
public unsafe void* pNext;
public VkDriverId driverID;
public char driverName;
public char driverInfo;
public VkConformanceVersion conformanceVersion;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceDriverPropertiesKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPresentRegionsKHR {
public VkStructureType sType;
public unsafe void* pNext;
public uint swapchainCount;
public unsafe VkPresentRegionKHR* pRegions;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPresentRegionKHR {
public uint rectangleCount;
public unsafe VkRectLayerKHR* pRectangles;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkRectLayerKHR {
public VkOffset2D offset;
public VkExtent2D extent;
public uint layer;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceVariablePointersFeatures {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 variablePointersStorageBuffer;
public VkBool32 variablePointers;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceVariablePointersFeaturesKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceVariablePointerFeaturesKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceVariablePointerFeatures {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkExternalMemoryProperties {
public VkExternalMemoryFeatureFlags externalMemoryFeatures;
public VkExternalMemoryHandleTypeFlags exportFromImportedHandleTypes;
public VkExternalMemoryHandleTypeFlags compatibleHandleTypes;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkExternalMemoryPropertiesKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceExternalImageFormatInfo {
public VkStructureType sType;
public unsafe void* pNext;
public VkExternalMemoryHandleTypeFlagBits handleType;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceExternalImageFormatInfoKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkExternalImageFormatProperties {
public VkStructureType sType;
public unsafe void* pNext;
public VkExternalMemoryProperties externalMemoryProperties;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkExternalImageFormatPropertiesKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceExternalBufferInfo {
public VkStructureType sType;
public unsafe void* pNext;
public VkBufferCreateFlags flags;
public VkBufferUsageFlags usage;
public VkExternalMemoryHandleTypeFlagBits handleType;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceExternalBufferInfoKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkExternalBufferProperties {
public VkStructureType sType;
public unsafe void* pNext;
public VkExternalMemoryProperties externalMemoryProperties;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkExternalBufferPropertiesKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceIDProperties {
public VkStructureType sType;
public unsafe void* pNext;
public byte deviceUUID;
public byte driverUUID;
public byte deviceLUID;
public uint deviceNodeMask;
public VkBool32 deviceLUIDValid;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceIDPropertiesKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkExternalMemoryImageCreateInfo {
public VkStructureType sType;
public unsafe void* pNext;
public VkExternalMemoryHandleTypeFlags handleTypes;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkExternalMemoryImageCreateInfoKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkExternalMemoryBufferCreateInfo {
public VkStructureType sType;
public unsafe void* pNext;
public VkExternalMemoryHandleTypeFlags handleTypes;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkExternalMemoryBufferCreateInfoKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkExportMemoryAllocateInfo {
public VkStructureType sType;
public unsafe void* pNext;
public VkExternalMemoryHandleTypeFlags handleTypes;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkExportMemoryAllocateInfoKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkImportMemoryWin32HandleInfoKHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkExternalMemoryHandleTypeFlagBits handleType;
public IntPtr handle;
public IntPtr name;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkExportMemoryWin32HandleInfoKHR {
public VkStructureType sType;
public unsafe void* pNext;
public unsafe IntPtr* pAttributes;
public IntPtr dwAccess;
public IntPtr name;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkMemoryWin32HandlePropertiesKHR {
public VkStructureType sType;
public unsafe void* pNext;
public uint memoryTypeBits;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkMemoryGetWin32HandleInfoKHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkDeviceMemory memory;
public VkExternalMemoryHandleTypeFlagBits handleType;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkImportMemoryFdInfoKHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkExternalMemoryHandleTypeFlagBits handleType;
public int fd;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkMemoryFdPropertiesKHR {
public VkStructureType sType;
public unsafe void* pNext;
public uint memoryTypeBits;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkMemoryGetFdInfoKHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkDeviceMemory memory;
public VkExternalMemoryHandleTypeFlagBits handleType;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkWin32KeyedMutexAcquireReleaseInfoKHR {
public VkStructureType sType;
public unsafe void* pNext;
public uint acquireCount;
public unsafe VkDeviceMemory* pAcquireSyncs;
public unsafe ulong* pAcquireKeys;
public unsafe uint* pAcquireTimeouts;
public uint releaseCount;
public unsafe VkDeviceMemory* pReleaseSyncs;
public unsafe ulong* pReleaseKeys;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceExternalSemaphoreInfo {
public VkStructureType sType;
public unsafe void* pNext;
public VkExternalSemaphoreHandleTypeFlagBits handleType;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceExternalSemaphoreInfoKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkExternalSemaphoreProperties {
public VkStructureType sType;
public unsafe void* pNext;
public VkExternalSemaphoreHandleTypeFlags exportFromImportedHandleTypes;
public VkExternalSemaphoreHandleTypeFlags compatibleHandleTypes;
public VkExternalSemaphoreFeatureFlags externalSemaphoreFeatures;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkExternalSemaphorePropertiesKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkExportSemaphoreCreateInfo {
public VkStructureType sType;
public unsafe void* pNext;
public VkExternalSemaphoreHandleTypeFlags handleTypes;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkExportSemaphoreCreateInfoKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkImportSemaphoreWin32HandleInfoKHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkSemaphore semaphore;
public VkSemaphoreImportFlags flags;
public VkExternalSemaphoreHandleTypeFlagBits handleType;
public IntPtr handle;
public IntPtr name;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkExportSemaphoreWin32HandleInfoKHR {
public VkStructureType sType;
public unsafe void* pNext;
public unsafe IntPtr* pAttributes;
public IntPtr dwAccess;
public IntPtr name;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkD3D12FenceSubmitInfoKHR {
public VkStructureType sType;
public unsafe void* pNext;
public uint waitSemaphoreValuesCount;
public unsafe ulong* pWaitSemaphoreValues;
public uint signalSemaphoreValuesCount;
public unsafe ulong* pSignalSemaphoreValues;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkSemaphoreGetWin32HandleInfoKHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkSemaphore semaphore;
public VkExternalSemaphoreHandleTypeFlagBits handleType;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkImportSemaphoreFdInfoKHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkSemaphore semaphore;
public VkSemaphoreImportFlags flags;
public VkExternalSemaphoreHandleTypeFlagBits handleType;
public int fd;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkSemaphoreGetFdInfoKHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkSemaphore semaphore;
public VkExternalSemaphoreHandleTypeFlagBits handleType;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceExternalFenceInfo {
public VkStructureType sType;
public unsafe void* pNext;
public VkExternalFenceHandleTypeFlagBits handleType;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceExternalFenceInfoKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkExternalFenceProperties {
public VkStructureType sType;
public unsafe void* pNext;
public VkExternalFenceHandleTypeFlags exportFromImportedHandleTypes;
public VkExternalFenceHandleTypeFlags compatibleHandleTypes;
public VkExternalFenceFeatureFlags externalFenceFeatures;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkExternalFencePropertiesKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkExportFenceCreateInfo {
public VkStructureType sType;
public unsafe void* pNext;
public VkExternalFenceHandleTypeFlags handleTypes;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkExportFenceCreateInfoKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkImportFenceWin32HandleInfoKHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkFence fence;
public VkFenceImportFlags flags;
public VkExternalFenceHandleTypeFlagBits handleType;
public IntPtr handle;
public IntPtr name;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkExportFenceWin32HandleInfoKHR {
public VkStructureType sType;
public unsafe void* pNext;
public unsafe IntPtr* pAttributes;
public IntPtr dwAccess;
public IntPtr name;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkFenceGetWin32HandleInfoKHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkFence fence;
public VkExternalFenceHandleTypeFlagBits handleType;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkImportFenceFdInfoKHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkFence fence;
public VkFenceImportFlags flags;
public VkExternalFenceHandleTypeFlagBits handleType;
public int fd;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkFenceGetFdInfoKHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkFence fence;
public VkExternalFenceHandleTypeFlagBits handleType;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceMultiviewFeatures {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 multiview;
public VkBool32 multiviewGeometryShader;
public VkBool32 multiviewTessellationShader;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceMultiviewFeaturesKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceMultiviewProperties {
public VkStructureType sType;
public unsafe void* pNext;
public uint maxMultiviewViewCount;
public uint maxMultiviewInstanceIndex;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceMultiviewPropertiesKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkRenderPassMultiviewCreateInfo {
public VkStructureType sType;
public unsafe void* pNext;
public uint subpassCount;
public unsafe uint* pViewMasks;
public uint dependencyCount;
public unsafe int* pViewOffsets;
public uint correlationMaskCount;
public unsafe uint* pCorrelationMasks;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkRenderPassMultiviewCreateInfoKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkSurfaceCapabilities2EXT {
public VkStructureType sType;
public unsafe void* pNext;
public uint minImageCount;
public uint maxImageCount;
public VkExtent2D currentExtent;
public VkExtent2D minImageExtent;
public VkExtent2D maxImageExtent;
public uint maxImageArrayLayers;
public VkSurfaceTransformFlagsKHR supportedTransforms;
public VkSurfaceTransformFlagBitsKHR currentTransform;
public VkCompositeAlphaFlagsKHR supportedCompositeAlpha;
public VkImageUsageFlags supportedUsageFlags;
public VkSurfaceCounterFlagsEXT supportedSurfaceCounters;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDisplayPowerInfoEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkDisplayPowerStateEXT powerState;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDeviceEventInfoEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkDeviceEventTypeEXT deviceEvent;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDisplayEventInfoEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkDisplayEventTypeEXT displayEvent;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkSwapchainCounterCreateInfoEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkSurfaceCounterFlagsEXT surfaceCounters;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceGroupProperties {
public VkStructureType sType;
public unsafe void* pNext;
public uint physicalDeviceCount;
public VkPhysicalDevice physicalDevices;
public VkBool32 subsetAllocation;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceGroupPropertiesKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkMemoryAllocateFlagsInfo {
public VkStructureType sType;
public unsafe void* pNext;
public VkMemoryAllocateFlags flags;
public uint deviceMask;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkMemoryAllocateFlagsInfoKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkBindBufferMemoryInfo {
public VkStructureType sType;
public unsafe void* pNext;
public VkBuffer buffer;
public VkDeviceMemory memory;
public ulong memoryOffset;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkBindBufferMemoryInfoKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkBindBufferMemoryDeviceGroupInfo {
public VkStructureType sType;
public unsafe void* pNext;
public uint deviceIndexCount;
public unsafe uint* pDeviceIndices;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkBindBufferMemoryDeviceGroupInfoKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkBindImageMemoryInfo {
public VkStructureType sType;
public unsafe void* pNext;
public VkImage image;
public VkDeviceMemory memory;
public ulong memoryOffset;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkBindImageMemoryInfoKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkBindImageMemoryDeviceGroupInfo {
public VkStructureType sType;
public unsafe void* pNext;
public uint deviceIndexCount;
public unsafe uint* pDeviceIndices;
public uint splitInstanceBindRegionCount;
public unsafe VkRect2D* pSplitInstanceBindRegions;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkBindImageMemoryDeviceGroupInfoKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDeviceGroupRenderPassBeginInfo {
public VkStructureType sType;
public unsafe void* pNext;
public uint deviceMask;
public uint deviceRenderAreaCount;
public unsafe VkRect2D* pDeviceRenderAreas;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDeviceGroupRenderPassBeginInfoKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDeviceGroupCommandBufferBeginInfo {
public VkStructureType sType;
public unsafe void* pNext;
public uint deviceMask;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDeviceGroupCommandBufferBeginInfoKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDeviceGroupSubmitInfo {
public VkStructureType sType;
public unsafe void* pNext;
public uint waitSemaphoreCount;
public unsafe uint* pWaitSemaphoreDeviceIndices;
public uint commandBufferCount;
public unsafe uint* pCommandBufferDeviceMasks;
public uint signalSemaphoreCount;
public unsafe uint* pSignalSemaphoreDeviceIndices;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDeviceGroupSubmitInfoKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDeviceGroupBindSparseInfo {
public VkStructureType sType;
public unsafe void* pNext;
public uint resourceDeviceIndex;
public uint memoryDeviceIndex;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDeviceGroupBindSparseInfoKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDeviceGroupPresentCapabilitiesKHR {
public VkStructureType sType;
public unsafe void* pNext;
public uint presentMask;
public VkDeviceGroupPresentModeFlagsKHR modes;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkImageSwapchainCreateInfoKHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkSwapchainKHR swapchain;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkBindImageMemorySwapchainInfoKHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkSwapchainKHR swapchain;
public uint imageIndex;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkAcquireNextImageInfoKHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkSwapchainKHR swapchain;
public ulong timeout;
public VkSemaphore semaphore;
public VkFence fence;
public uint deviceMask;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDeviceGroupPresentInfoKHR {
public VkStructureType sType;
public unsafe void* pNext;
public uint swapchainCount;
public unsafe uint* pDeviceMasks;
public VkDeviceGroupPresentModeFlagBitsKHR mode;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDeviceGroupDeviceCreateInfo {
public VkStructureType sType;
public unsafe void* pNext;
public uint physicalDeviceCount;
public unsafe VkPhysicalDevice* pPhysicalDevices;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDeviceGroupDeviceCreateInfoKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDeviceGroupSwapchainCreateInfoKHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkDeviceGroupPresentModeFlagsKHR modes;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDescriptorUpdateTemplateEntry {
public uint dstBinding;
public uint dstArrayElement;
public uint descriptorCount;
public VkDescriptorType descriptorType;
public VkPointerSize offset;
public VkPointerSize stride;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDescriptorUpdateTemplateEntryKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDescriptorUpdateTemplateCreateInfo {
public VkStructureType sType;
public unsafe void* pNext;
public VkDescriptorUpdateTemplateCreateFlags flags;
public uint descriptorUpdateEntryCount;
public unsafe VkDescriptorUpdateTemplateEntry* pDescriptorUpdateEntries;
public VkDescriptorUpdateTemplateType templateType;
public VkDescriptorSetLayout descriptorSetLayout;
public VkPipelineBindPoint pipelineBindPoint;
public VkPipelineLayout pipelineLayout;
public uint set;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDescriptorUpdateTemplateCreateInfoKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkXYColorEXT {
public float x;
public float y;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkHdrMetadataEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkXYColorEXT displayPrimaryRed;
public VkXYColorEXT displayPrimaryGreen;
public VkXYColorEXT displayPrimaryBlue;
public VkXYColorEXT whitePoint;
public float maxLuminance;
public float minLuminance;
public float maxContentLightLevel;
public float maxFrameAverageLightLevel;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDisplayNativeHdrSurfaceCapabilitiesAMD {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 localDimmingSupport;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkSwapchainDisplayNativeHdrCreateInfoAMD {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 localDimmingEnable;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkRefreshCycleDurationGOOGLE {
public ulong refreshDuration;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPastPresentationTimingGOOGLE {
public uint presentID;
public ulong desiredPresentTime;
public ulong actualPresentTime;
public ulong earliestPresentTime;
public ulong presentMargin;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPresentTimesInfoGOOGLE {
public VkStructureType sType;
public unsafe void* pNext;
public uint swapchainCount;
public unsafe VkPresentTimeGOOGLE* pTimes;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPresentTimeGOOGLE {
public uint presentID;
public ulong desiredPresentTime;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkIOSSurfaceCreateInfoMVK {
public VkStructureType sType;
public unsafe void* pNext;
public VkIOSSurfaceCreateFlagsMVK flags;
public unsafe void* pView;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkMacOSSurfaceCreateInfoMVK {
public VkStructureType sType;
public unsafe void* pNext;
public VkMacOSSurfaceCreateFlagsMVK flags;
public unsafe void* pView;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkMetalSurfaceCreateInfoEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkMetalSurfaceCreateFlagsEXT flags;
public unsafe IntPtr* pLayer;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkViewportWScalingNV {
public float xcoeff;
public float ycoeff;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPipelineViewportWScalingStateCreateInfoNV {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 viewportWScalingEnable;
public uint viewportCount;
public unsafe VkViewportWScalingNV* pViewportWScalings;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkViewportSwizzleNV {
public VkViewportCoordinateSwizzleNV x;
public VkViewportCoordinateSwizzleNV y;
public VkViewportCoordinateSwizzleNV z;
public VkViewportCoordinateSwizzleNV w;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPipelineViewportSwizzleStateCreateInfoNV {
public VkStructureType sType;
public unsafe void* pNext;
public VkPipelineViewportSwizzleStateCreateFlagsNV flags;
public uint viewportCount;
public unsafe VkViewportSwizzleNV* pViewportSwizzles;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceDiscardRectanglePropertiesEXT {
public VkStructureType sType;
public unsafe void* pNext;
public uint maxDiscardRectangles;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPipelineDiscardRectangleStateCreateInfoEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkPipelineDiscardRectangleStateCreateFlagsEXT flags;
public VkDiscardRectangleModeEXT discardRectangleMode;
public uint discardRectangleCount;
public unsafe VkRect2D* pDiscardRectangles;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceMultiviewPerViewAttributesPropertiesNVX {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 perViewPositionAllComponents;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkInputAttachmentAspectReference {
public uint subpass;
public uint inputAttachmentIndex;
public VkImageAspectFlags aspectMask;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkInputAttachmentAspectReferenceKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkRenderPassInputAttachmentAspectCreateInfo {
public VkStructureType sType;
public unsafe void* pNext;
public uint aspectReferenceCount;
public unsafe VkInputAttachmentAspectReference* pAspectReferences;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkRenderPassInputAttachmentAspectCreateInfoKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceSurfaceInfo2KHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkSurfaceKHR surface;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkSurfaceCapabilities2KHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkSurfaceCapabilitiesKHR surfaceCapabilities;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkSurfaceFormat2KHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkSurfaceFormatKHR surfaceFormat;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDisplayProperties2KHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkDisplayPropertiesKHR displayProperties;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDisplayPlaneProperties2KHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkDisplayPlanePropertiesKHR displayPlaneProperties;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDisplayModeProperties2KHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkDisplayModePropertiesKHR displayModeProperties;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDisplayPlaneInfo2KHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkDisplayModeKHR mode;
public uint planeIndex;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDisplayPlaneCapabilities2KHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkDisplayPlaneCapabilitiesKHR capabilities;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkSharedPresentSurfaceCapabilitiesKHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkImageUsageFlags sharedPresentSupportedUsageFlags;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDevice16BitStorageFeatures {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 storageBuffer16BitAccess;
public VkBool32 uniformAndStorageBuffer16BitAccess;
public VkBool32 storagePushConstant16;
public VkBool32 storageInputOutput16;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDevice16BitStorageFeaturesKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceSubgroupProperties {
public VkStructureType sType;
public unsafe void* pNext;
public uint subgroupSize;
public VkShaderStageFlags supportedStages;
public VkSubgroupFeatureFlags supportedOperations;
public VkBool32 quadOperationsInAllStages;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceShaderSubgroupExtendedTypesFeatures {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 shaderSubgroupExtendedTypes;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceShaderSubgroupExtendedTypesFeaturesKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkBufferMemoryRequirementsInfo2 {
public VkStructureType sType;
public unsafe void* pNext;
public VkBuffer buffer;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkBufferMemoryRequirementsInfo2KHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkImageMemoryRequirementsInfo2 {
public VkStructureType sType;
public unsafe void* pNext;
public VkImage image;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkImageMemoryRequirementsInfo2KHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkImageSparseMemoryRequirementsInfo2 {
public VkStructureType sType;
public unsafe void* pNext;
public VkImage image;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkImageSparseMemoryRequirementsInfo2KHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkMemoryRequirements2 {
public VkStructureType sType;
public unsafe void* pNext;
public VkMemoryRequirements memoryRequirements;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkMemoryRequirements2KHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkSparseImageMemoryRequirements2 {
public VkStructureType sType;
public unsafe void* pNext;
public VkSparseImageMemoryRequirements memoryRequirements;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkSparseImageMemoryRequirements2KHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDevicePointClippingProperties {
public VkStructureType sType;
public unsafe void* pNext;
public VkPointClippingBehavior pointClippingBehavior;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDevicePointClippingPropertiesKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkMemoryDedicatedRequirements {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 prefersDedicatedAllocation;
public VkBool32 requiresDedicatedAllocation;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkMemoryDedicatedRequirementsKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkMemoryDedicatedAllocateInfo {
public VkStructureType sType;
public unsafe void* pNext;
public VkImage image;
public VkBuffer buffer;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkMemoryDedicatedAllocateInfoKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkImageViewUsageCreateInfo {
public VkStructureType sType;
public unsafe void* pNext;
public VkImageUsageFlags usage;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkImageViewUsageCreateInfoKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPipelineTessellationDomainOriginStateCreateInfo {
public VkStructureType sType;
public unsafe void* pNext;
public VkTessellationDomainOrigin domainOrigin;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPipelineTessellationDomainOriginStateCreateInfoKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkSamplerYcbcrConversionInfo {
public VkStructureType sType;
public unsafe void* pNext;
public VkSamplerYcbcrConversion conversion;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkSamplerYcbcrConversionInfoKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkSamplerYcbcrConversionCreateInfo {
public VkStructureType sType;
public unsafe void* pNext;
public VkFormat format;
public VkSamplerYcbcrModelConversion ycbcrModel;
public VkSamplerYcbcrRange ycbcrRange;
public VkComponentMapping components;
public VkChromaLocation xChromaOffset;
public VkChromaLocation yChromaOffset;
public VkFilter chromaFilter;
public VkBool32 forceExplicitReconstruction;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkSamplerYcbcrConversionCreateInfoKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkBindImagePlaneMemoryInfo {
public VkStructureType sType;
public unsafe void* pNext;
public VkImageAspectFlagBits planeAspect;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkBindImagePlaneMemoryInfoKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkImagePlaneMemoryRequirementsInfo {
public VkStructureType sType;
public unsafe void* pNext;
public VkImageAspectFlagBits planeAspect;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkImagePlaneMemoryRequirementsInfoKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceSamplerYcbcrConversionFeatures {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 samplerYcbcrConversion;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceSamplerYcbcrConversionFeaturesKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkSamplerYcbcrConversionImageFormatProperties {
public VkStructureType sType;
public unsafe void* pNext;
public uint combinedImageSamplerDescriptorCount;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkSamplerYcbcrConversionImageFormatPropertiesKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkTextureLODGatherFormatPropertiesAMD {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 supportsTextureGatherLODBiasAMD;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkConditionalRenderingBeginInfoEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkBuffer buffer;
public ulong offset;
public VkConditionalRenderingFlagsEXT flags;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkProtectedSubmitInfo {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 protectedSubmit;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceProtectedMemoryFeatures {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 protectedMemory;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceProtectedMemoryProperties {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 protectedNoFault;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDeviceQueueInfo2 {
public VkStructureType sType;
public unsafe void* pNext;
public VkDeviceQueueCreateFlags flags;
public uint queueFamilyIndex;
public uint queueIndex;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPipelineCoverageToColorStateCreateInfoNV {
public VkStructureType sType;
public unsafe void* pNext;
public VkPipelineCoverageToColorStateCreateFlagsNV flags;
public VkBool32 coverageToColorEnable;
public uint coverageToColorLocation;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceSamplerFilterMinmaxProperties {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 filterMinmaxSingleComponentFormats;
public VkBool32 filterMinmaxImageComponentMapping;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceSamplerFilterMinmaxPropertiesEXT {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkSampleLocationEXT {
public float x;
public float y;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkSampleLocationsInfoEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkSampleCountFlagBits sampleLocationsPerPixel;
public VkExtent2D sampleLocationGridSize;
public uint sampleLocationsCount;
public unsafe VkSampleLocationEXT* pSampleLocations;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkAttachmentSampleLocationsEXT {
public uint attachmentIndex;
public VkSampleLocationsInfoEXT sampleLocationsInfo;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkSubpassSampleLocationsEXT {
public uint subpassIndex;
public VkSampleLocationsInfoEXT sampleLocationsInfo;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkRenderPassSampleLocationsBeginInfoEXT {
public VkStructureType sType;
public unsafe void* pNext;
public uint attachmentInitialSampleLocationsCount;
public unsafe VkAttachmentSampleLocationsEXT* pAttachmentInitialSampleLocations;
public uint postSubpassSampleLocationsCount;
public unsafe VkSubpassSampleLocationsEXT* pPostSubpassSampleLocations;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPipelineSampleLocationsStateCreateInfoEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 sampleLocationsEnable;
public VkSampleLocationsInfoEXT sampleLocationsInfo;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceSampleLocationsPropertiesEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkSampleCountFlags sampleLocationSampleCounts;
public VkExtent2D maxSampleLocationGridSize;
public float sampleLocationCoordinateRange;
public uint sampleLocationSubPixelBits;
public VkBool32 variableSampleLocations;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkMultisamplePropertiesEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkExtent2D maxSampleLocationGridSize;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkSamplerReductionModeCreateInfo {
public VkStructureType sType;
public unsafe void* pNext;
public VkSamplerReductionMode reductionMode;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkSamplerReductionModeCreateInfoEXT {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceBlendOperationAdvancedFeaturesEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 advancedBlendCoherentOperations;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceBlendOperationAdvancedPropertiesEXT {
public VkStructureType sType;
public unsafe void* pNext;
public uint advancedBlendMaxColorAttachments;
public VkBool32 advancedBlendIndependentBlend;
public VkBool32 advancedBlendNonPremultipliedSrcColor;
public VkBool32 advancedBlendNonPremultipliedDstColor;
public VkBool32 advancedBlendCorrelatedOverlap;
public VkBool32 advancedBlendAllOperations;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPipelineColorBlendAdvancedStateCreateInfoEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 srcPremultiplied;
public VkBool32 dstPremultiplied;
public VkBlendOverlapEXT blendOverlap;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceInlineUniformBlockFeaturesEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 inlineUniformBlock;
public VkBool32 descriptorBindingInlineUniformBlockUpdateAfterBind;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceInlineUniformBlockPropertiesEXT {
public VkStructureType sType;
public unsafe void* pNext;
public uint maxInlineUniformBlockSize;
public uint maxPerStageDescriptorInlineUniformBlocks;
public uint maxPerStageDescriptorUpdateAfterBindInlineUniformBlocks;
public uint maxDescriptorSetInlineUniformBlocks;
public uint maxDescriptorSetUpdateAfterBindInlineUniformBlocks;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkWriteDescriptorSetInlineUniformBlockEXT {
public VkStructureType sType;
public unsafe void* pNext;
public uint dataSize;
public unsafe void* pData;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDescriptorPoolInlineUniformBlockCreateInfoEXT {
public VkStructureType sType;
public unsafe void* pNext;
public uint maxInlineUniformBlockBindings;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPipelineCoverageModulationStateCreateInfoNV {
public VkStructureType sType;
public unsafe void* pNext;
public VkPipelineCoverageModulationStateCreateFlagsNV flags;
public VkCoverageModulationModeNV coverageModulationMode;
public VkBool32 coverageModulationTableEnable;
public uint coverageModulationTableCount;
public unsafe float* pCoverageModulationTable;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkImageFormatListCreateInfo {
public VkStructureType sType;
public unsafe void* pNext;
public uint viewFormatCount;
public unsafe VkFormat* pViewFormats;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkImageFormatListCreateInfoKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkValidationCacheCreateInfoEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkValidationCacheCreateFlagsEXT flags;
public VkPointerSize initialDataSize;
public unsafe void* pInitialData;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkShaderModuleValidationCacheCreateInfoEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkValidationCacheEXT validationCache;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceMaintenance3Properties {
public VkStructureType sType;
public unsafe void* pNext;
public uint maxPerSetDescriptors;
public ulong maxMemoryAllocationSize;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceMaintenance3PropertiesKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDescriptorSetLayoutSupport {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 supported;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDescriptorSetLayoutSupportKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceShaderDrawParametersFeatures {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 shaderDrawParameters;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceShaderDrawParameterFeatures {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceShaderFloat16Int8Features {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 shaderFloat16;
public VkBool32 shaderInt8;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceShaderFloat16Int8FeaturesKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceFloat16Int8FeaturesKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceFloatControlsProperties {
public VkStructureType sType;
public unsafe void* pNext;
public VkShaderFloatControlsIndependence denormBehaviorIndependence;
public VkShaderFloatControlsIndependence roundingModeIndependence;
public VkBool32 shaderSignedZeroInfNanPreserveFloat16;
public VkBool32 shaderSignedZeroInfNanPreserveFloat32;
public VkBool32 shaderSignedZeroInfNanPreserveFloat64;
public VkBool32 shaderDenormPreserveFloat16;
public VkBool32 shaderDenormPreserveFloat32;
public VkBool32 shaderDenormPreserveFloat64;
public VkBool32 shaderDenormFlushToZeroFloat16;
public VkBool32 shaderDenormFlushToZeroFloat32;
public VkBool32 shaderDenormFlushToZeroFloat64;
public VkBool32 shaderRoundingModeRTEFloat16;
public VkBool32 shaderRoundingModeRTEFloat32;
public VkBool32 shaderRoundingModeRTEFloat64;
public VkBool32 shaderRoundingModeRTZFloat16;
public VkBool32 shaderRoundingModeRTZFloat32;
public VkBool32 shaderRoundingModeRTZFloat64;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceFloatControlsPropertiesKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceHostQueryResetFeatures {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 hostQueryReset;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceHostQueryResetFeaturesEXT {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkNativeBufferUsage2ANDROID {
public ulong consumer;
public ulong producer;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkNativeBufferANDROID {
public VkStructureType sType;
public unsafe void* pNext;
public unsafe void* handle;
public int stride;
public int format;
public int usage;
public VkNativeBufferUsage2ANDROID usage2;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkSwapchainImageCreateInfoANDROID {
public VkStructureType sType;
public unsafe void* pNext;
public VkSwapchainImageUsageFlagsANDROID usage;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDevicePresentationPropertiesANDROID {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 sharedImage;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkShaderResourceUsageAMD {
public uint numUsedVgprs;
public uint numUsedSgprs;
public uint ldsSizePerLocalWorkGroup;
public VkPointerSize ldsUsageSizeInBytes;
public VkPointerSize scratchMemUsageInBytes;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkShaderStatisticsInfoAMD {
public VkShaderStageFlags shaderStageMask;
public VkShaderResourceUsageAMD resourceUsage;
public uint numPhysicalVgprs;
public uint numPhysicalSgprs;
public uint numAvailableVgprs;
public uint numAvailableSgprs;
public uint computeWorkGroupSize;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDeviceQueueGlobalPriorityCreateInfoEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkQueueGlobalPriorityEXT globalPriority;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDebugUtilsObjectNameInfoEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkObjectType objectType;
public ulong objectHandle;
public unsafe char* pObjectName;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDebugUtilsObjectTagInfoEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkObjectType objectType;
public ulong objectHandle;
public ulong tagName;
public VkPointerSize tagSize;
public unsafe void* pTag;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDebugUtilsLabelEXT {
public VkStructureType sType;
public unsafe void* pNext;
public unsafe char* pLabelName;
public float color;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDebugUtilsMessengerCreateInfoEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkDebugUtilsMessengerCreateFlagsEXT flags;
public VkDebugUtilsMessageSeverityFlagsEXT messageSeverity;
public VkDebugUtilsMessageTypeFlagsEXT messageType;
public PFN_vkDebugUtilsMessengerCallbackEXT pfnUserCallback;
public unsafe void* pUserData;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDebugUtilsMessengerCallbackDataEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkDebugUtilsMessengerCallbackDataFlagsEXT flags;
public unsafe char* pMessageIdName;
public int messageIdNumber;
public unsafe char* pMessage;
public uint queueLabelCount;
public unsafe VkDebugUtilsLabelEXT* pQueueLabels;
public uint cmdBufLabelCount;
public unsafe VkDebugUtilsLabelEXT* pCmdBufLabels;
public uint objectCount;
public unsafe VkDebugUtilsObjectNameInfoEXT* pObjects;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceDeviceMemoryReportFeaturesEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 deviceMemoryReport;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDeviceDeviceMemoryReportCreateInfoEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkDeviceMemoryReportFlagsEXT flags;
public PFN_vkDeviceMemoryReportCallbackEXT pfnUserCallback;
public unsafe void* pUserData;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDeviceMemoryReportCallbackDataEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkDeviceMemoryReportFlagsEXT flags;
public VkDeviceMemoryReportEventTypeEXT type;
public ulong memoryObjectId;
public ulong size;
public VkObjectType objectType;
public ulong objectHandle;
public uint heapIndex;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkImportMemoryHostPointerInfoEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkExternalMemoryHandleTypeFlagBits handleType;
public unsafe void* pHostPointer;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkMemoryHostPointerPropertiesEXT {
public VkStructureType sType;
public unsafe void* pNext;
public uint memoryTypeBits;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceExternalMemoryHostPropertiesEXT {
public VkStructureType sType;
public unsafe void* pNext;
public ulong minImportedHostPointerAlignment;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceConservativeRasterizationPropertiesEXT {
public VkStructureType sType;
public unsafe void* pNext;
public float primitiveOverestimationSize;
public float maxExtraPrimitiveOverestimationSize;
public float extraPrimitiveOverestimationSizeGranularity;
public VkBool32 primitiveUnderestimation;
public VkBool32 conservativePointAndLineRasterization;
public VkBool32 degenerateTrianglesRasterized;
public VkBool32 degenerateLinesRasterized;
public VkBool32 fullyCoveredFragmentShaderInputVariable;
public VkBool32 conservativeRasterizationPostDepthCoverage;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkCalibratedTimestampInfoEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkTimeDomainEXT timeDomain;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceShaderCorePropertiesAMD {
public VkStructureType sType;
public unsafe void* pNext;
public uint shaderEngineCount;
public uint shaderArraysPerEngineCount;
public uint computeUnitsPerShaderArray;
public uint simdPerComputeUnit;
public uint wavefrontsPerSimd;
public uint wavefrontSize;
public uint sgprsPerSimd;
public uint minSgprAllocation;
public uint maxSgprAllocation;
public uint sgprAllocationGranularity;
public uint vgprsPerSimd;
public uint minVgprAllocation;
public uint maxVgprAllocation;
public uint vgprAllocationGranularity;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceShaderCoreProperties2AMD {
public VkStructureType sType;
public unsafe void* pNext;
public VkShaderCorePropertiesFlagsAMD shaderCoreFeatures;
public uint activeComputeUnitCount;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPipelineRasterizationConservativeStateCreateInfoEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkPipelineRasterizationConservativeStateCreateFlagsEXT flags;
public VkConservativeRasterizationModeEXT conservativeRasterizationMode;
public float extraPrimitiveOverestimationSize;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceDescriptorIndexingFeatures {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 shaderInputAttachmentArrayDynamicIndexing;
public VkBool32 shaderUniformTexelBufferArrayDynamicIndexing;
public VkBool32 shaderStorageTexelBufferArrayDynamicIndexing;
public VkBool32 shaderUniformBufferArrayNonUniformIndexing;
public VkBool32 shaderSampledImageArrayNonUniformIndexing;
public VkBool32 shaderStorageBufferArrayNonUniformIndexing;
public VkBool32 shaderStorageImageArrayNonUniformIndexing;
public VkBool32 shaderInputAttachmentArrayNonUniformIndexing;
public VkBool32 shaderUniformTexelBufferArrayNonUniformIndexing;
public VkBool32 shaderStorageTexelBufferArrayNonUniformIndexing;
public VkBool32 descriptorBindingUniformBufferUpdateAfterBind;
public VkBool32 descriptorBindingSampledImageUpdateAfterBind;
public VkBool32 descriptorBindingStorageImageUpdateAfterBind;
public VkBool32 descriptorBindingStorageBufferUpdateAfterBind;
public VkBool32 descriptorBindingUniformTexelBufferUpdateAfterBind;
public VkBool32 descriptorBindingStorageTexelBufferUpdateAfterBind;
public VkBool32 descriptorBindingUpdateUnusedWhilePending;
public VkBool32 descriptorBindingPartiallyBound;
public VkBool32 descriptorBindingVariableDescriptorCount;
public VkBool32 runtimeDescriptorArray;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceDescriptorIndexingFeaturesEXT {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceDescriptorIndexingProperties {
public VkStructureType sType;
public unsafe void* pNext;
public uint maxUpdateAfterBindDescriptorsInAllPools;
public VkBool32 shaderUniformBufferArrayNonUniformIndexingNative;
public VkBool32 shaderSampledImageArrayNonUniformIndexingNative;
public VkBool32 shaderStorageBufferArrayNonUniformIndexingNative;
public VkBool32 shaderStorageImageArrayNonUniformIndexingNative;
public VkBool32 shaderInputAttachmentArrayNonUniformIndexingNative;
public VkBool32 robustBufferAccessUpdateAfterBind;
public VkBool32 quadDivergentImplicitLod;
public uint maxPerStageDescriptorUpdateAfterBindSamplers;
public uint maxPerStageDescriptorUpdateAfterBindUniformBuffers;
public uint maxPerStageDescriptorUpdateAfterBindStorageBuffers;
public uint maxPerStageDescriptorUpdateAfterBindSampledImages;
public uint maxPerStageDescriptorUpdateAfterBindStorageImages;
public uint maxPerStageDescriptorUpdateAfterBindInputAttachments;
public uint maxPerStageUpdateAfterBindResources;
public uint maxDescriptorSetUpdateAfterBindSamplers;
public uint maxDescriptorSetUpdateAfterBindUniformBuffers;
public uint maxDescriptorSetUpdateAfterBindUniformBuffersDynamic;
public uint maxDescriptorSetUpdateAfterBindStorageBuffers;
public uint maxDescriptorSetUpdateAfterBindStorageBuffersDynamic;
public uint maxDescriptorSetUpdateAfterBindSampledImages;
public uint maxDescriptorSetUpdateAfterBindStorageImages;
public uint maxDescriptorSetUpdateAfterBindInputAttachments;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceDescriptorIndexingPropertiesEXT {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDescriptorSetLayoutBindingFlagsCreateInfo {
public VkStructureType sType;
public unsafe void* pNext;
public uint bindingCount;
public unsafe VkDescriptorBindingFlags* pBindingFlags;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDescriptorSetLayoutBindingFlagsCreateInfoEXT {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDescriptorSetVariableDescriptorCountAllocateInfo {
public VkStructureType sType;
public unsafe void* pNext;
public uint descriptorSetCount;
public unsafe uint* pDescriptorCounts;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDescriptorSetVariableDescriptorCountAllocateInfoEXT {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDescriptorSetVariableDescriptorCountLayoutSupport {
public VkStructureType sType;
public unsafe void* pNext;
public uint maxVariableDescriptorCount;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDescriptorSetVariableDescriptorCountLayoutSupportEXT {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkAttachmentDescription2 {
public VkStructureType sType;
public unsafe void* pNext;
public VkAttachmentDescriptionFlags flags;
public VkFormat format;
public VkSampleCountFlagBits samples;
public VkAttachmentLoadOp loadOp;
public VkAttachmentStoreOp storeOp;
public VkAttachmentLoadOp stencilLoadOp;
public VkAttachmentStoreOp stencilStoreOp;
public VkImageLayout initialLayout;
public VkImageLayout finalLayout;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkAttachmentDescription2KHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkAttachmentReference2 {
public VkStructureType sType;
public unsafe void* pNext;
public uint attachment;
public VkImageLayout layout;
public VkImageAspectFlags aspectMask;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkAttachmentReference2KHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkSubpassDescription2 {
public VkStructureType sType;
public unsafe void* pNext;
public VkSubpassDescriptionFlags flags;
public VkPipelineBindPoint pipelineBindPoint;
public uint viewMask;
public uint inputAttachmentCount;
public unsafe VkAttachmentReference2* pInputAttachments;
public uint colorAttachmentCount;
public unsafe VkAttachmentReference2* pColorAttachments;
public unsafe VkAttachmentReference2* pResolveAttachments;
public unsafe VkAttachmentReference2* pDepthStencilAttachment;
public uint preserveAttachmentCount;
public unsafe uint* pPreserveAttachments;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkSubpassDescription2KHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkSubpassDependency2 {
public VkStructureType sType;
public unsafe void* pNext;
public uint srcSubpass;
public uint dstSubpass;
public VkPipelineStageFlags srcStageMask;
public VkPipelineStageFlags dstStageMask;
public VkAccessFlags srcAccessMask;
public VkAccessFlags dstAccessMask;
public VkDependencyFlags dependencyFlags;
public int viewOffset;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkSubpassDependency2KHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkRenderPassCreateInfo2 {
public VkStructureType sType;
public unsafe void* pNext;
public VkRenderPassCreateFlags flags;
public uint attachmentCount;
public unsafe VkAttachmentDescription2* pAttachments;
public uint subpassCount;
public unsafe VkSubpassDescription2* pSubpasses;
public uint dependencyCount;
public unsafe VkSubpassDependency2* pDependencies;
public uint correlatedViewMaskCount;
public unsafe uint* pCorrelatedViewMasks;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkRenderPassCreateInfo2KHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkSubpassBeginInfo {
public VkStructureType sType;
public unsafe void* pNext;
public VkSubpassContents contents;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkSubpassBeginInfoKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkSubpassEndInfo {
public VkStructureType sType;
public unsafe void* pNext;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkSubpassEndInfoKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceTimelineSemaphoreFeatures {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 timelineSemaphore;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceTimelineSemaphoreFeaturesKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceTimelineSemaphoreProperties {
public VkStructureType sType;
public unsafe void* pNext;
public ulong maxTimelineSemaphoreValueDifference;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceTimelineSemaphorePropertiesKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkSemaphoreTypeCreateInfo {
public VkStructureType sType;
public unsafe void* pNext;
public VkSemaphoreType semaphoreType;
public ulong initialValue;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkSemaphoreTypeCreateInfoKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkTimelineSemaphoreSubmitInfo {
public VkStructureType sType;
public unsafe void* pNext;
public uint waitSemaphoreValueCount;
public unsafe ulong* pWaitSemaphoreValues;
public uint signalSemaphoreValueCount;
public unsafe ulong* pSignalSemaphoreValues;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkTimelineSemaphoreSubmitInfoKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkSemaphoreWaitInfo {
public VkStructureType sType;
public unsafe void* pNext;
public VkSemaphoreWaitFlags flags;
public uint semaphoreCount;
public unsafe VkSemaphore* pSemaphores;
public unsafe ulong* pValues;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkSemaphoreWaitInfoKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkSemaphoreSignalInfo {
public VkStructureType sType;
public unsafe void* pNext;
public VkSemaphore semaphore;
public ulong value;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkSemaphoreSignalInfoKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkVertexInputBindingDivisorDescriptionEXT {
public uint binding;
public uint divisor;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPipelineVertexInputDivisorStateCreateInfoEXT {
public VkStructureType sType;
public unsafe void* pNext;
public uint vertexBindingDivisorCount;
public unsafe VkVertexInputBindingDivisorDescriptionEXT* pVertexBindingDivisors;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceVertexAttributeDivisorPropertiesEXT {
public VkStructureType sType;
public unsafe void* pNext;
public uint maxVertexAttribDivisor;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDevicePCIBusInfoPropertiesEXT {
public VkStructureType sType;
public unsafe void* pNext;
public uint pciDomain;
public uint pciBus;
public uint pciDevice;
public uint pciFunction;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkImportAndroidHardwareBufferInfoANDROID {
public VkStructureType sType;
public unsafe void* pNext;
public unsafe AHardwareBuffer* buffer;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkAndroidHardwareBufferUsageANDROID {
public VkStructureType sType;
public unsafe void* pNext;
public ulong androidHardwareBufferUsage;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkAndroidHardwareBufferPropertiesANDROID {
public VkStructureType sType;
public unsafe void* pNext;
public ulong allocationSize;
public uint memoryTypeBits;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkMemoryGetAndroidHardwareBufferInfoANDROID {
public VkStructureType sType;
public unsafe void* pNext;
public VkDeviceMemory memory;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkAndroidHardwareBufferFormatPropertiesANDROID {
public VkStructureType sType;
public unsafe void* pNext;
public VkFormat format;
public ulong externalFormat;
public VkFormatFeatureFlags formatFeatures;
public VkComponentMapping samplerYcbcrConversionComponents;
public VkSamplerYcbcrModelConversion suggestedYcbcrModel;
public VkSamplerYcbcrRange suggestedYcbcrRange;
public VkChromaLocation suggestedXChromaOffset;
public VkChromaLocation suggestedYChromaOffset;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkCommandBufferInheritanceConditionalRenderingInfoEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 conditionalRenderingEnable;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkExternalFormatANDROID {
public VkStructureType sType;
public unsafe void* pNext;
public ulong externalFormat;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDevice8BitStorageFeatures {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 storageBuffer8BitAccess;
public VkBool32 uniformAndStorageBuffer8BitAccess;
public VkBool32 storagePushConstant8;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDevice8BitStorageFeaturesKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceConditionalRenderingFeaturesEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 conditionalRendering;
public VkBool32 inheritedConditionalRendering;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceVulkanMemoryModelFeatures {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 vulkanMemoryModel;
public VkBool32 vulkanMemoryModelDeviceScope;
public VkBool32 vulkanMemoryModelAvailabilityVisibilityChains;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceVulkanMemoryModelFeaturesKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceShaderAtomicInt64Features {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 shaderBufferInt64Atomics;
public VkBool32 shaderSharedInt64Atomics;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceShaderAtomicInt64FeaturesKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceShaderAtomicFloatFeaturesEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 shaderBufferFloat32Atomics;
public VkBool32 shaderBufferFloat32AtomicAdd;
public VkBool32 shaderBufferFloat64Atomics;
public VkBool32 shaderBufferFloat64AtomicAdd;
public VkBool32 shaderSharedFloat32Atomics;
public VkBool32 shaderSharedFloat32AtomicAdd;
public VkBool32 shaderSharedFloat64Atomics;
public VkBool32 shaderSharedFloat64AtomicAdd;
public VkBool32 shaderImageFloat32Atomics;
public VkBool32 shaderImageFloat32AtomicAdd;
public VkBool32 sparseImageFloat32Atomics;
public VkBool32 sparseImageFloat32AtomicAdd;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceVertexAttributeDivisorFeaturesEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 vertexAttributeInstanceRateDivisor;
public VkBool32 vertexAttributeInstanceRateZeroDivisor;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkQueueFamilyCheckpointPropertiesNV {
public VkStructureType sType;
public unsafe void* pNext;
public VkPipelineStageFlags checkpointExecutionStageMask;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkCheckpointDataNV {
public VkStructureType sType;
public unsafe void* pNext;
public VkPipelineStageFlagBits stage;
public unsafe void* pCheckpointMarker;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceDepthStencilResolveProperties {
public VkStructureType sType;
public unsafe void* pNext;
public VkResolveModeFlags supportedDepthResolveModes;
public VkResolveModeFlags supportedStencilResolveModes;
public VkBool32 independentResolveNone;
public VkBool32 independentResolve;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceDepthStencilResolvePropertiesKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkSubpassDescriptionDepthStencilResolve {
public VkStructureType sType;
public unsafe void* pNext;
public VkResolveModeFlagBits depthResolveMode;
public VkResolveModeFlagBits stencilResolveMode;
public unsafe VkAttachmentReference2* pDepthStencilResolveAttachment;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkSubpassDescriptionDepthStencilResolveKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkImageViewASTCDecodeModeEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkFormat decodeMode;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceASTCDecodeFeaturesEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 decodeModeSharedExponent;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceTransformFeedbackFeaturesEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 transformFeedback;
public VkBool32 geometryStreams;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceTransformFeedbackPropertiesEXT {
public VkStructureType sType;
public unsafe void* pNext;
public uint maxTransformFeedbackStreams;
public uint maxTransformFeedbackBuffers;
public ulong maxTransformFeedbackBufferSize;
public uint maxTransformFeedbackStreamDataSize;
public uint maxTransformFeedbackBufferDataSize;
public uint maxTransformFeedbackBufferDataStride;
public VkBool32 transformFeedbackQueries;
public VkBool32 transformFeedbackStreamsLinesTriangles;
public VkBool32 transformFeedbackRasterizationStreamSelect;
public VkBool32 transformFeedbackDraw;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPipelineRasterizationStateStreamCreateInfoEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkPipelineRasterizationStateStreamCreateFlagsEXT flags;
public uint rasterizationStream;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceRepresentativeFragmentTestFeaturesNV {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 representativeFragmentTest;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPipelineRepresentativeFragmentTestStateCreateInfoNV {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 representativeFragmentTestEnable;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceExclusiveScissorFeaturesNV {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 exclusiveScissor;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPipelineViewportExclusiveScissorStateCreateInfoNV {
public VkStructureType sType;
public unsafe void* pNext;
public uint exclusiveScissorCount;
public unsafe VkRect2D* pExclusiveScissors;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceCornerSampledImageFeaturesNV {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 cornerSampledImage;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceComputeShaderDerivativesFeaturesNV {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 computeDerivativeGroupQuads;
public VkBool32 computeDerivativeGroupLinear;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceFragmentShaderBarycentricFeaturesNV {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 fragmentShaderBarycentric;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceShaderImageFootprintFeaturesNV {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 imageFootprint;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceDedicatedAllocationImageAliasingFeaturesNV {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 dedicatedAllocationImageAliasing;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkShadingRatePaletteNV {
public uint shadingRatePaletteEntryCount;
public unsafe VkShadingRatePaletteEntryNV* pShadingRatePaletteEntries;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPipelineViewportShadingRateImageStateCreateInfoNV {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 shadingRateImageEnable;
public uint viewportCount;
public unsafe VkShadingRatePaletteNV* pShadingRatePalettes;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceShadingRateImageFeaturesNV {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 shadingRateImage;
public VkBool32 shadingRateCoarseSampleOrder;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceShadingRateImagePropertiesNV {
public VkStructureType sType;
public unsafe void* pNext;
public VkExtent2D shadingRateTexelSize;
public uint shadingRatePaletteSize;
public uint shadingRateMaxCoarseSamples;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkCoarseSampleLocationNV {
public uint pixelX;
public uint pixelY;
public uint sample;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkCoarseSampleOrderCustomNV {
public VkShadingRatePaletteEntryNV shadingRate;
public uint sampleCount;
public uint sampleLocationCount;
public unsafe VkCoarseSampleLocationNV* pSampleLocations;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPipelineViewportCoarseSampleOrderStateCreateInfoNV {
public VkStructureType sType;
public unsafe void* pNext;
public VkCoarseSampleOrderTypeNV sampleOrderType;
public uint customSampleOrderCount;
public unsafe VkCoarseSampleOrderCustomNV* pCustomSampleOrders;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceMeshShaderFeaturesNV {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 taskShader;
public VkBool32 meshShader;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceMeshShaderPropertiesNV {
public VkStructureType sType;
public unsafe void* pNext;
public uint maxDrawMeshTasksCount;
public uint maxTaskWorkGroupInvocations;
public uint maxTaskWorkGroupSize;
public uint maxTaskTotalMemorySize;
public uint maxTaskOutputCount;
public uint maxMeshWorkGroupInvocations;
public uint maxMeshWorkGroupSize;
public uint maxMeshTotalMemorySize;
public uint maxMeshOutputVertices;
public uint maxMeshOutputPrimitives;
public uint maxMeshMultiviewViewCount;
public uint meshOutputPerVertexGranularity;
public uint meshOutputPerPrimitiveGranularity;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDrawMeshTasksIndirectCommandNV {
public uint taskCount;
public uint firstTask;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkRayTracingShaderGroupCreateInfoNV {
public VkStructureType sType;
public unsafe void* pNext;
public VkRayTracingShaderGroupTypeKHR type;
public uint generalShader;
public uint closestHitShader;
public uint anyHitShader;
public uint intersectionShader;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkRayTracingShaderGroupCreateInfoKHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkRayTracingShaderGroupTypeKHR type;
public uint generalShader;
public uint closestHitShader;
public uint anyHitShader;
public uint intersectionShader;
public unsafe void* pShaderGroupCaptureReplayHandle;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkRayTracingPipelineCreateInfoNV {
public VkStructureType sType;
public unsafe void* pNext;
public VkPipelineCreateFlags flags;
public uint stageCount;
public unsafe VkPipelineShaderStageCreateInfo* pStages;
public uint groupCount;
public unsafe VkRayTracingShaderGroupCreateInfoNV* pGroups;
public uint maxRecursionDepth;
public VkPipelineLayout layout;
public VkPipeline basePipelineHandle;
public int basePipelineIndex;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkRayTracingPipelineCreateInfoKHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkPipelineCreateFlags flags;
public uint stageCount;
public unsafe VkPipelineShaderStageCreateInfo* pStages;
public uint groupCount;
public unsafe VkRayTracingShaderGroupCreateInfoKHR* pGroups;
public uint maxPipelineRayRecursionDepth;
public unsafe VkPipelineLibraryCreateInfoKHR* pLibraryInfo;
public unsafe VkRayTracingPipelineInterfaceCreateInfoKHR* pLibraryInterface;
public unsafe VkPipelineDynamicStateCreateInfo* pDynamicState;
public VkPipelineLayout layout;
public VkPipeline basePipelineHandle;
public int basePipelineIndex;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkGeometryTrianglesNV {
public VkStructureType sType;
public unsafe void* pNext;
public VkBuffer vertexData;
public ulong vertexOffset;
public uint vertexCount;
public ulong vertexStride;
public VkFormat vertexFormat;
public VkBuffer indexData;
public ulong indexOffset;
public uint indexCount;
public VkIndexType indexType;
public VkBuffer transformData;
public ulong transformOffset;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkGeometryAABBNV {
public VkStructureType sType;
public unsafe void* pNext;
public VkBuffer aabbData;
public uint numAABBs;
public uint stride;
public ulong offset;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkGeometryDataNV {
public VkGeometryTrianglesNV triangles;
public VkGeometryAABBNV aabbs;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkGeometryNV {
public VkStructureType sType;
public unsafe void* pNext;
public VkGeometryTypeKHR geometryType;
public VkGeometryDataNV geometry;
public VkGeometryFlagsKHR flags;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkAccelerationStructureInfoNV {
public VkStructureType sType;
public unsafe void* pNext;
public VkAccelerationStructureTypeNV type;
public VkBuildAccelerationStructureFlagsNV flags;
public uint instanceCount;
public uint geometryCount;
public unsafe VkGeometryNV* pGeometries;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkAccelerationStructureCreateInfoNV {
public VkStructureType sType;
public unsafe void* pNext;
public ulong compactedSize;
public VkAccelerationStructureInfoNV info;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkBindAccelerationStructureMemoryInfoNV {
public VkStructureType sType;
public unsafe void* pNext;
public VkAccelerationStructureNV accelerationStructure;
public VkDeviceMemory memory;
public ulong memoryOffset;
public uint deviceIndexCount;
public unsafe uint* pDeviceIndices;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkWriteDescriptorSetAccelerationStructureKHR {
public VkStructureType sType;
public unsafe void* pNext;
public uint accelerationStructureCount;
public unsafe VkAccelerationStructureKHR* pAccelerationStructures;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkWriteDescriptorSetAccelerationStructureNV {
public VkStructureType sType;
public unsafe void* pNext;
public uint accelerationStructureCount;
public unsafe VkAccelerationStructureNV* pAccelerationStructures;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkAccelerationStructureMemoryRequirementsInfoNV {
public VkStructureType sType;
public unsafe void* pNext;
public VkAccelerationStructureMemoryRequirementsTypeNV type;
public VkAccelerationStructureNV accelerationStructure;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceAccelerationStructureFeaturesKHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 accelerationStructure;
public VkBool32 accelerationStructureCaptureReplay;
public VkBool32 accelerationStructureIndirectBuild;
public VkBool32 accelerationStructureHostCommands;
public VkBool32 descriptorBindingAccelerationStructureUpdateAfterBind;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceRayTracingPipelineFeaturesKHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 rayTracingPipeline;
public VkBool32 rayTracingPipelineShaderGroupHandleCaptureReplay;
public VkBool32 rayTracingPipelineShaderGroupHandleCaptureReplayMixed;
public VkBool32 rayTracingPipelineTraceRaysIndirect;
public VkBool32 rayTraversalPrimitiveCulling;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceRayQueryFeaturesKHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 rayQuery;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceAccelerationStructurePropertiesKHR {
public VkStructureType sType;
public unsafe void* pNext;
public ulong maxGeometryCount;
public ulong maxInstanceCount;
public ulong maxPrimitiveCount;
public uint maxPerStageDescriptorAccelerationStructures;
public uint maxPerStageDescriptorUpdateAfterBindAccelerationStructures;
public uint maxDescriptorSetAccelerationStructures;
public uint maxDescriptorSetUpdateAfterBindAccelerationStructures;
public uint minAccelerationStructureScratchOffsetAlignment;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceRayTracingPipelinePropertiesKHR {
public VkStructureType sType;
public unsafe void* pNext;
public uint shaderGroupHandleSize;
public uint maxRayRecursionDepth;
public uint maxShaderGroupStride;
public uint shaderGroupBaseAlignment;
public uint shaderGroupHandleCaptureReplaySize;
public uint maxRayDispatchInvocationCount;
public uint shaderGroupHandleAlignment;
public uint maxRayHitAttributeSize;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceRayTracingPropertiesNV {
public VkStructureType sType;
public unsafe void* pNext;
public uint shaderGroupHandleSize;
public uint maxRecursionDepth;
public uint maxShaderGroupStride;
public uint shaderGroupBaseAlignment;
public ulong maxGeometryCount;
public ulong maxInstanceCount;
public ulong maxTriangleCount;
public uint maxDescriptorSetAccelerationStructures;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkStridedDeviceAddressRegionKHR {
public IntPtr deviceAddress;
public ulong stride;
public ulong size;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkTraceRaysIndirectCommandKHR {
public uint width;
public uint height;
public uint depth;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDrmFormatModifierPropertiesListEXT {
public VkStructureType sType;
public unsafe void* pNext;
public uint drmFormatModifierCount;
public unsafe VkDrmFormatModifierPropertiesEXT* pDrmFormatModifierProperties;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDrmFormatModifierPropertiesEXT {
public ulong drmFormatModifier;
public uint drmFormatModifierPlaneCount;
public VkFormatFeatureFlags drmFormatModifierTilingFeatures;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceImageDrmFormatModifierInfoEXT {
public VkStructureType sType;
public unsafe void* pNext;
public ulong drmFormatModifier;
public VkSharingMode sharingMode;
public uint queueFamilyIndexCount;
public unsafe uint* pQueueFamilyIndices;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkImageDrmFormatModifierListCreateInfoEXT {
public VkStructureType sType;
public unsafe void* pNext;
public uint drmFormatModifierCount;
public unsafe ulong* pDrmFormatModifiers;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkImageDrmFormatModifierExplicitCreateInfoEXT {
public VkStructureType sType;
public unsafe void* pNext;
public ulong drmFormatModifier;
public uint drmFormatModifierPlaneCount;
public unsafe VkSubresourceLayout* pPlaneLayouts;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkImageDrmFormatModifierPropertiesEXT {
public VkStructureType sType;
public unsafe void* pNext;
public ulong drmFormatModifier;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkImageStencilUsageCreateInfo {
public VkStructureType sType;
public unsafe void* pNext;
public VkImageUsageFlags stencilUsage;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkImageStencilUsageCreateInfoEXT {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDeviceMemoryOverallocationCreateInfoAMD {
public VkStructureType sType;
public unsafe void* pNext;
public VkMemoryOverallocationBehaviorAMD overallocationBehavior;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceFragmentDensityMapFeaturesEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 fragmentDensityMap;
public VkBool32 fragmentDensityMapDynamic;
public VkBool32 fragmentDensityMapNonSubsampledImages;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceFragmentDensityMap2FeaturesEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 fragmentDensityMapDeferred;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceFragmentDensityMapPropertiesEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkExtent2D minFragmentDensityTexelSize;
public VkExtent2D maxFragmentDensityTexelSize;
public VkBool32 fragmentDensityInvocations;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceFragmentDensityMap2PropertiesEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 subsampledLoads;
public VkBool32 subsampledCoarseReconstructionEarlyAccess;
public uint maxSubsampledArrayLayers;
public uint maxDescriptorSetSubsampledSamplers;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkRenderPassFragmentDensityMapCreateInfoEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkAttachmentReference fragmentDensityMapAttachment;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceScalarBlockLayoutFeatures {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 scalarBlockLayout;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceScalarBlockLayoutFeaturesEXT {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkSurfaceProtectedCapabilitiesKHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 supportsProtected;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceUniformBufferStandardLayoutFeatures {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 uniformBufferStandardLayout;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceUniformBufferStandardLayoutFeaturesKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceDepthClipEnableFeaturesEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 depthClipEnable;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPipelineRasterizationDepthClipStateCreateInfoEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkPipelineRasterizationDepthClipStateCreateFlagsEXT flags;
public VkBool32 depthClipEnable;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceMemoryBudgetPropertiesEXT {
public VkStructureType sType;
public unsafe void* pNext;
public ulong heapBudget;
public ulong heapUsage;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceMemoryPriorityFeaturesEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 memoryPriority;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkMemoryPriorityAllocateInfoEXT {
public VkStructureType sType;
public unsafe void* pNext;
public float priority;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceBufferDeviceAddressFeatures {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 bufferDeviceAddress;
public VkBool32 bufferDeviceAddressCaptureReplay;
public VkBool32 bufferDeviceAddressMultiDevice;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceBufferDeviceAddressFeaturesKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceBufferDeviceAddressFeaturesEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 bufferDeviceAddress;
public VkBool32 bufferDeviceAddressCaptureReplay;
public VkBool32 bufferDeviceAddressMultiDevice;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceBufferAddressFeaturesEXT {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkBufferDeviceAddressInfo {
public VkStructureType sType;
public unsafe void* pNext;
public VkBuffer buffer;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkBufferDeviceAddressInfoKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkBufferDeviceAddressInfoEXT {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkBufferOpaqueCaptureAddressCreateInfo {
public VkStructureType sType;
public unsafe void* pNext;
public ulong opaqueCaptureAddress;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkBufferOpaqueCaptureAddressCreateInfoKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkBufferDeviceAddressCreateInfoEXT {
public VkStructureType sType;
public unsafe void* pNext;
public IntPtr deviceAddress;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceImageViewImageFormatInfoEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkImageViewType imageViewType;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkFilterCubicImageViewImageFormatPropertiesEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 filterCubic;
public VkBool32 filterCubicMinmax;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceImagelessFramebufferFeatures {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 imagelessFramebuffer;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceImagelessFramebufferFeaturesKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkFramebufferAttachmentsCreateInfo {
public VkStructureType sType;
public unsafe void* pNext;
public uint attachmentImageInfoCount;
public unsafe VkFramebufferAttachmentImageInfo* pAttachmentImageInfos;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkFramebufferAttachmentsCreateInfoKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkFramebufferAttachmentImageInfo {
public VkStructureType sType;
public unsafe void* pNext;
public VkImageCreateFlags flags;
public VkImageUsageFlags usage;
public uint width;
public uint height;
public uint layerCount;
public uint viewFormatCount;
public unsafe VkFormat* pViewFormats;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkFramebufferAttachmentImageInfoKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkRenderPassAttachmentBeginInfo {
public VkStructureType sType;
public unsafe void* pNext;
public uint attachmentCount;
public unsafe VkImageView* pAttachments;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkRenderPassAttachmentBeginInfoKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceTextureCompressionASTCHDRFeaturesEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 textureCompressionASTC_HDR;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceCooperativeMatrixFeaturesNV {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 cooperativeMatrix;
public VkBool32 cooperativeMatrixRobustBufferAccess;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceCooperativeMatrixPropertiesNV {
public VkStructureType sType;
public unsafe void* pNext;
public VkShaderStageFlags cooperativeMatrixSupportedStages;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkCooperativeMatrixPropertiesNV {
public VkStructureType sType;
public unsafe void* pNext;
public uint MSize;
public uint NSize;
public uint KSize;
public VkComponentTypeNV AType;
public VkComponentTypeNV BType;
public VkComponentTypeNV CType;
public VkComponentTypeNV DType;
public VkScopeNV scope;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceYcbcrImageArraysFeaturesEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 ycbcrImageArrays;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkImageViewHandleInfoNVX {
public VkStructureType sType;
public unsafe void* pNext;
public VkImageView imageView;
public VkDescriptorType descriptorType;
public VkSampler sampler;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkImageViewAddressPropertiesNVX {
public VkStructureType sType;
public unsafe void* pNext;
public IntPtr deviceAddress;
public ulong size;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPresentFrameTokenGGP {
public VkStructureType sType;
public unsafe void* pNext;
public GgpFrameToken frameToken;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPipelineCreationFeedbackEXT {
public VkPipelineCreationFeedbackFlagsEXT flags;
public ulong duration;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPipelineCreationFeedbackCreateInfoEXT {
public VkStructureType sType;
public unsafe void* pNext;
public unsafe VkPipelineCreationFeedbackEXT* pPipelineCreationFeedback;
public uint pipelineStageCreationFeedbackCount;
public unsafe VkPipelineCreationFeedbackEXT* pPipelineStageCreationFeedbacks;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkSurfaceFullScreenExclusiveInfoEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkFullScreenExclusiveEXT fullScreenExclusive;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkSurfaceFullScreenExclusiveWin32InfoEXT {
public VkStructureType sType;
public unsafe void* pNext;
public IntPtr hmonitor;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkSurfaceCapabilitiesFullScreenExclusiveEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 fullScreenExclusiveSupported;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDevicePerformanceQueryFeaturesKHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 performanceCounterQueryPools;
public VkBool32 performanceCounterMultipleQueryPools;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDevicePerformanceQueryPropertiesKHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 allowCommandBufferQueryCopies;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPerformanceCounterKHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkPerformanceCounterUnitKHR unit;
public VkPerformanceCounterScopeKHR scope;
public VkPerformanceCounterStorageKHR storage;
public byte uuid;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPerformanceCounterDescriptionKHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkPerformanceCounterDescriptionFlagsKHR flags;
public char name;
public char category;
public char description;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkQueryPoolPerformanceCreateInfoKHR {
public VkStructureType sType;
public unsafe void* pNext;
public uint queueFamilyIndex;
public uint counterIndexCount;
public unsafe uint* pCounterIndices;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkAcquireProfilingLockInfoKHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkAcquireProfilingLockFlagsKHR flags;
public ulong timeout;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPerformanceQuerySubmitInfoKHR {
public VkStructureType sType;
public unsafe void* pNext;
public uint counterPassIndex;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkHeadlessSurfaceCreateInfoEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkHeadlessSurfaceCreateFlagsEXT flags;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceCoverageReductionModeFeaturesNV {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 coverageReductionMode;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPipelineCoverageReductionStateCreateInfoNV {
public VkStructureType sType;
public unsafe void* pNext;
public VkPipelineCoverageReductionStateCreateFlagsNV flags;
public VkCoverageReductionModeNV coverageReductionMode;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkFramebufferMixedSamplesCombinationNV {
public VkStructureType sType;
public unsafe void* pNext;
public VkCoverageReductionModeNV coverageReductionMode;
public VkSampleCountFlagBits rasterizationSamples;
public VkSampleCountFlags depthStencilSamples;
public VkSampleCountFlags colorSamples;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceShaderIntegerFunctions2FeaturesINTEL {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 shaderIntegerFunctions2;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPerformanceValueINTEL {
public VkPerformanceValueTypeINTEL type;
public VkPerformanceValueDataINTEL data;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkInitializePerformanceApiInfoINTEL {
public VkStructureType sType;
public unsafe void* pNext;
public unsafe void* pUserData;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkQueryPoolPerformanceQueryCreateInfoINTEL {
public VkStructureType sType;
public unsafe void* pNext;
public VkQueryPoolSamplingModeINTEL performanceCountersSampling;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkQueryPoolCreateInfoINTEL {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPerformanceMarkerInfoINTEL {
public VkStructureType sType;
public unsafe void* pNext;
public ulong marker;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPerformanceStreamMarkerInfoINTEL {
public VkStructureType sType;
public unsafe void* pNext;
public uint marker;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPerformanceOverrideInfoINTEL {
public VkStructureType sType;
public unsafe void* pNext;
public VkPerformanceOverrideTypeINTEL type;
public VkBool32 enable;
public ulong parameter;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPerformanceConfigurationAcquireInfoINTEL {
public VkStructureType sType;
public unsafe void* pNext;
public VkPerformanceConfigurationTypeINTEL type;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceShaderClockFeaturesKHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 shaderSubgroupClock;
public VkBool32 shaderDeviceClock;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceIndexTypeUint8FeaturesEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 indexTypeUint8;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceShaderSMBuiltinsPropertiesNV {
public VkStructureType sType;
public unsafe void* pNext;
public uint shaderSMCount;
public uint shaderWarpsPerSM;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceShaderSMBuiltinsFeaturesNV {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 shaderSMBuiltins;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceFragmentShaderInterlockFeaturesEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 fragmentShaderSampleInterlock;
public VkBool32 fragmentShaderPixelInterlock;
public VkBool32 fragmentShaderShadingRateInterlock;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceSeparateDepthStencilLayoutsFeatures {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 separateDepthStencilLayouts;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceSeparateDepthStencilLayoutsFeaturesKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkAttachmentReferenceStencilLayout {
public VkStructureType sType;
public unsafe void* pNext;
public VkImageLayout stencilLayout;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkAttachmentReferenceStencilLayoutKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkAttachmentDescriptionStencilLayout {
public VkStructureType sType;
public unsafe void* pNext;
public VkImageLayout stencilInitialLayout;
public VkImageLayout stencilFinalLayout;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkAttachmentDescriptionStencilLayoutKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDevicePipelineExecutablePropertiesFeaturesKHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 pipelineExecutableInfo;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPipelineInfoKHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkPipeline pipeline;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPipelineExecutablePropertiesKHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkShaderStageFlags stages;
public char name;
public char description;
public uint subgroupSize;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPipelineExecutableInfoKHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkPipeline pipeline;
public uint executableIndex;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPipelineExecutableStatisticKHR {
public VkStructureType sType;
public unsafe void* pNext;
public char name;
public char description;
public VkPipelineExecutableStatisticFormatKHR format;
public VkPipelineExecutableStatisticValueKHR value;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPipelineExecutableInternalRepresentationKHR {
public VkStructureType sType;
public unsafe void* pNext;
public char name;
public char description;
public VkBool32 isText;
public VkPointerSize dataSize;
public unsafe void* pData;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceShaderDemoteToHelperInvocationFeaturesEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 shaderDemoteToHelperInvocation;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceTexelBufferAlignmentFeaturesEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 texelBufferAlignment;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceTexelBufferAlignmentPropertiesEXT {
public VkStructureType sType;
public unsafe void* pNext;
public ulong storageTexelBufferOffsetAlignmentBytes;
public VkBool32 storageTexelBufferOffsetSingleTexelAlignment;
public ulong uniformTexelBufferOffsetAlignmentBytes;
public VkBool32 uniformTexelBufferOffsetSingleTexelAlignment;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceSubgroupSizeControlFeaturesEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 subgroupSizeControl;
public VkBool32 computeFullSubgroups;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceSubgroupSizeControlPropertiesEXT {
public VkStructureType sType;
public unsafe void* pNext;
public uint minSubgroupSize;
public uint maxSubgroupSize;
public uint maxComputeWorkgroupSubgroups;
public VkShaderStageFlags requiredSubgroupSizeStages;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPipelineShaderStageRequiredSubgroupSizeCreateInfoEXT {
public VkStructureType sType;
public unsafe void* pNext;
public uint requiredSubgroupSize;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkMemoryOpaqueCaptureAddressAllocateInfo {
public VkStructureType sType;
public unsafe void* pNext;
public ulong opaqueCaptureAddress;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkMemoryOpaqueCaptureAddressAllocateInfoKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDeviceMemoryOpaqueCaptureAddressInfo {
public VkStructureType sType;
public unsafe void* pNext;
public VkDeviceMemory memory;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDeviceMemoryOpaqueCaptureAddressInfoKHR {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceLineRasterizationFeaturesEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 rectangularLines;
public VkBool32 bresenhamLines;
public VkBool32 smoothLines;
public VkBool32 stippledRectangularLines;
public VkBool32 stippledBresenhamLines;
public VkBool32 stippledSmoothLines;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceLineRasterizationPropertiesEXT {
public VkStructureType sType;
public unsafe void* pNext;
public uint lineSubPixelPrecisionBits;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPipelineRasterizationLineStateCreateInfoEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkLineRasterizationModeEXT lineRasterizationMode;
public VkBool32 stippledLineEnable;
public uint lineStippleFactor;
public ushort lineStipplePattern;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDevicePipelineCreationCacheControlFeaturesEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 pipelineCreationCacheControl;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceVulkan11Features {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 storageBuffer16BitAccess;
public VkBool32 uniformAndStorageBuffer16BitAccess;
public VkBool32 storagePushConstant16;
public VkBool32 storageInputOutput16;
public VkBool32 multiview;
public VkBool32 multiviewGeometryShader;
public VkBool32 multiviewTessellationShader;
public VkBool32 variablePointersStorageBuffer;
public VkBool32 variablePointers;
public VkBool32 protectedMemory;
public VkBool32 samplerYcbcrConversion;
public VkBool32 shaderDrawParameters;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceVulkan11Properties {
public VkStructureType sType;
public unsafe void* pNext;
public byte deviceUUID;
public byte driverUUID;
public byte deviceLUID;
public uint deviceNodeMask;
public VkBool32 deviceLUIDValid;
public uint subgroupSize;
public VkShaderStageFlags subgroupSupportedStages;
public VkSubgroupFeatureFlags subgroupSupportedOperations;
public VkBool32 subgroupQuadOperationsInAllStages;
public VkPointClippingBehavior pointClippingBehavior;
public uint maxMultiviewViewCount;
public uint maxMultiviewInstanceIndex;
public VkBool32 protectedNoFault;
public uint maxPerSetDescriptors;
public ulong maxMemoryAllocationSize;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceVulkan12Features {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 samplerMirrorClampToEdge;
public VkBool32 drawIndirectCount;
public VkBool32 storageBuffer8BitAccess;
public VkBool32 uniformAndStorageBuffer8BitAccess;
public VkBool32 storagePushConstant8;
public VkBool32 shaderBufferInt64Atomics;
public VkBool32 shaderSharedInt64Atomics;
public VkBool32 shaderFloat16;
public VkBool32 shaderInt8;
public VkBool32 descriptorIndexing;
public VkBool32 shaderInputAttachmentArrayDynamicIndexing;
public VkBool32 shaderUniformTexelBufferArrayDynamicIndexing;
public VkBool32 shaderStorageTexelBufferArrayDynamicIndexing;
public VkBool32 shaderUniformBufferArrayNonUniformIndexing;
public VkBool32 shaderSampledImageArrayNonUniformIndexing;
public VkBool32 shaderStorageBufferArrayNonUniformIndexing;
public VkBool32 shaderStorageImageArrayNonUniformIndexing;
public VkBool32 shaderInputAttachmentArrayNonUniformIndexing;
public VkBool32 shaderUniformTexelBufferArrayNonUniformIndexing;
public VkBool32 shaderStorageTexelBufferArrayNonUniformIndexing;
public VkBool32 descriptorBindingUniformBufferUpdateAfterBind;
public VkBool32 descriptorBindingSampledImageUpdateAfterBind;
public VkBool32 descriptorBindingStorageImageUpdateAfterBind;
public VkBool32 descriptorBindingStorageBufferUpdateAfterBind;
public VkBool32 descriptorBindingUniformTexelBufferUpdateAfterBind;
public VkBool32 descriptorBindingStorageTexelBufferUpdateAfterBind;
public VkBool32 descriptorBindingUpdateUnusedWhilePending;
public VkBool32 descriptorBindingPartiallyBound;
public VkBool32 descriptorBindingVariableDescriptorCount;
public VkBool32 runtimeDescriptorArray;
public VkBool32 samplerFilterMinmax;
public VkBool32 scalarBlockLayout;
public VkBool32 imagelessFramebuffer;
public VkBool32 uniformBufferStandardLayout;
public VkBool32 shaderSubgroupExtendedTypes;
public VkBool32 separateDepthStencilLayouts;
public VkBool32 hostQueryReset;
public VkBool32 timelineSemaphore;
public VkBool32 bufferDeviceAddress;
public VkBool32 bufferDeviceAddressCaptureReplay;
public VkBool32 bufferDeviceAddressMultiDevice;
public VkBool32 vulkanMemoryModel;
public VkBool32 vulkanMemoryModelDeviceScope;
public VkBool32 vulkanMemoryModelAvailabilityVisibilityChains;
public VkBool32 shaderOutputViewportIndex;
public VkBool32 shaderOutputLayer;
public VkBool32 subgroupBroadcastDynamicId;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceVulkan12Properties {
public VkStructureType sType;
public unsafe void* pNext;
public VkDriverId driverID;
public char driverName;
public char driverInfo;
public VkConformanceVersion conformanceVersion;
public VkShaderFloatControlsIndependence denormBehaviorIndependence;
public VkShaderFloatControlsIndependence roundingModeIndependence;
public VkBool32 shaderSignedZeroInfNanPreserveFloat16;
public VkBool32 shaderSignedZeroInfNanPreserveFloat32;
public VkBool32 shaderSignedZeroInfNanPreserveFloat64;
public VkBool32 shaderDenormPreserveFloat16;
public VkBool32 shaderDenormPreserveFloat32;
public VkBool32 shaderDenormPreserveFloat64;
public VkBool32 shaderDenormFlushToZeroFloat16;
public VkBool32 shaderDenormFlushToZeroFloat32;
public VkBool32 shaderDenormFlushToZeroFloat64;
public VkBool32 shaderRoundingModeRTEFloat16;
public VkBool32 shaderRoundingModeRTEFloat32;
public VkBool32 shaderRoundingModeRTEFloat64;
public VkBool32 shaderRoundingModeRTZFloat16;
public VkBool32 shaderRoundingModeRTZFloat32;
public VkBool32 shaderRoundingModeRTZFloat64;
public uint maxUpdateAfterBindDescriptorsInAllPools;
public VkBool32 shaderUniformBufferArrayNonUniformIndexingNative;
public VkBool32 shaderSampledImageArrayNonUniformIndexingNative;
public VkBool32 shaderStorageBufferArrayNonUniformIndexingNative;
public VkBool32 shaderStorageImageArrayNonUniformIndexingNative;
public VkBool32 shaderInputAttachmentArrayNonUniformIndexingNative;
public VkBool32 robustBufferAccessUpdateAfterBind;
public VkBool32 quadDivergentImplicitLod;
public uint maxPerStageDescriptorUpdateAfterBindSamplers;
public uint maxPerStageDescriptorUpdateAfterBindUniformBuffers;
public uint maxPerStageDescriptorUpdateAfterBindStorageBuffers;
public uint maxPerStageDescriptorUpdateAfterBindSampledImages;
public uint maxPerStageDescriptorUpdateAfterBindStorageImages;
public uint maxPerStageDescriptorUpdateAfterBindInputAttachments;
public uint maxPerStageUpdateAfterBindResources;
public uint maxDescriptorSetUpdateAfterBindSamplers;
public uint maxDescriptorSetUpdateAfterBindUniformBuffers;
public uint maxDescriptorSetUpdateAfterBindUniformBuffersDynamic;
public uint maxDescriptorSetUpdateAfterBindStorageBuffers;
public uint maxDescriptorSetUpdateAfterBindStorageBuffersDynamic;
public uint maxDescriptorSetUpdateAfterBindSampledImages;
public uint maxDescriptorSetUpdateAfterBindStorageImages;
public uint maxDescriptorSetUpdateAfterBindInputAttachments;
public VkResolveModeFlags supportedDepthResolveModes;
public VkResolveModeFlags supportedStencilResolveModes;
public VkBool32 independentResolveNone;
public VkBool32 independentResolve;
public VkBool32 filterMinmaxSingleComponentFormats;
public VkBool32 filterMinmaxImageComponentMapping;
public ulong maxTimelineSemaphoreValueDifference;
public VkSampleCountFlags framebufferIntegerColorSampleCounts;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPipelineCompilerControlCreateInfoAMD {
public VkStructureType sType;
public unsafe void* pNext;
public VkPipelineCompilerControlFlagsAMD compilerControlFlags;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceCoherentMemoryFeaturesAMD {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 deviceCoherentMemory;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceToolPropertiesEXT {
public VkStructureType sType;
public unsafe void* pNext;
public char name;
public char version;
public VkToolPurposeFlagsEXT purposes;
public char description;
public char layer;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkSamplerCustomBorderColorCreateInfoEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkClearColorValue customBorderColor;
public VkFormat format;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceCustomBorderColorPropertiesEXT {
public VkStructureType sType;
public unsafe void* pNext;
public uint maxCustomBorderColorSamplers;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceCustomBorderColorFeaturesEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 customBorderColors;
public VkBool32 customBorderColorWithoutFormat;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkAccelerationStructureGeometryTrianglesDataKHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkFormat vertexFormat;
public VkDeviceOrHostAddressConstKHR vertexData;
public ulong vertexStride;
public uint maxVertex;
public VkIndexType indexType;
public VkDeviceOrHostAddressConstKHR indexData;
public VkDeviceOrHostAddressConstKHR transformData;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkAccelerationStructureGeometryAabbsDataKHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkDeviceOrHostAddressConstKHR data;
public ulong stride;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkAccelerationStructureGeometryInstancesDataKHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 arrayOfPointers;
public VkDeviceOrHostAddressConstKHR data;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkAccelerationStructureGeometryKHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkGeometryTypeKHR geometryType;
public VkAccelerationStructureGeometryDataKHR geometry;
public VkGeometryFlagsKHR flags;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkAccelerationStructureBuildGeometryInfoKHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkAccelerationStructureTypeKHR type;
public VkBuildAccelerationStructureFlagsKHR flags;
public VkBuildAccelerationStructureModeKHR mode;
public VkAccelerationStructureKHR srcAccelerationStructure;
public VkAccelerationStructureKHR dstAccelerationStructure;
public uint geometryCount;
public unsafe VkAccelerationStructureGeometryKHR* pGeometries;
public unsafe VkAccelerationStructureGeometryKHR* ppGeometries;
public VkDeviceOrHostAddressKHR scratchData;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkAccelerationStructureBuildRangeInfoKHR {
public uint primitiveCount;
public uint primitiveOffset;
public uint firstVertex;
public uint transformOffset;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkAccelerationStructureCreateInfoKHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkAccelerationStructureCreateFlagsKHR createFlags;
public VkBuffer buffer;
public ulong offset;
public ulong size;
public VkAccelerationStructureTypeKHR type;
public IntPtr deviceAddress;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkAabbPositionsKHR {
public float minX;
public float minY;
public float minZ;
public float maxX;
public float maxY;
public float maxZ;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkAabbPositionsNV {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkTransformMatrixKHR {
public float matrix;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkTransformMatrixNV {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkAccelerationStructureInstanceKHR {
public VkTransformMatrixKHR transform;
public uint instanceCustomIndex;
public uint mask;
public uint instanceShaderBindingTableRecordOffset;
public VkGeometryInstanceFlagsKHR flags;
public ulong accelerationStructureReference;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkAccelerationStructureInstanceNV {
}
[StructLayout(LayoutKind.Sequential)]
public struct VkAccelerationStructureDeviceAddressInfoKHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkAccelerationStructureKHR accelerationStructure;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkAccelerationStructureVersionInfoKHR {
public VkStructureType sType;
public unsafe void* pNext;
public unsafe byte* pVersionData;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkCopyAccelerationStructureInfoKHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkAccelerationStructureKHR src;
public VkAccelerationStructureKHR dst;
public VkCopyAccelerationStructureModeKHR mode;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkCopyAccelerationStructureToMemoryInfoKHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkAccelerationStructureKHR src;
public VkDeviceOrHostAddressKHR dst;
public VkCopyAccelerationStructureModeKHR mode;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkCopyMemoryToAccelerationStructureInfoKHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkDeviceOrHostAddressConstKHR src;
public VkAccelerationStructureKHR dst;
public VkCopyAccelerationStructureModeKHR mode;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkRayTracingPipelineInterfaceCreateInfoKHR {
public VkStructureType sType;
public unsafe void* pNext;
public uint maxPipelineRayPayloadSize;
public uint maxPipelineRayHitAttributeSize;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPipelineLibraryCreateInfoKHR {
public VkStructureType sType;
public unsafe void* pNext;
public uint libraryCount;
public unsafe VkPipeline* pLibraries;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceExtendedDynamicStateFeaturesEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 extendedDynamicState;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkRenderPassTransformBeginInfoQCOM {
public VkStructureType sType;
public unsafe void* pNext;
public VkSurfaceTransformFlagBitsKHR transform;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkCopyCommandTransformInfoQCOM {
public VkStructureType sType;
public unsafe void* pNext;
public VkSurfaceTransformFlagBitsKHR transform;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkCommandBufferInheritanceRenderPassTransformInfoQCOM {
public VkStructureType sType;
public unsafe void* pNext;
public VkSurfaceTransformFlagBitsKHR transform;
public VkRect2D renderArea;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceDiagnosticsConfigFeaturesNV {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 diagnosticsConfig;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDeviceDiagnosticsConfigCreateInfoNV {
public VkStructureType sType;
public unsafe void* pNext;
public VkDeviceDiagnosticsConfigFlagsNV flags;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceZeroInitializeWorkgroupMemoryFeaturesKHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 shaderZeroInitializeWorkgroupMemory;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceRobustness2FeaturesEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 robustBufferAccess2;
public VkBool32 robustImageAccess2;
public VkBool32 nullDescriptor;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceRobustness2PropertiesEXT {
public VkStructureType sType;
public unsafe void* pNext;
public ulong robustStorageBufferAccessSizeAlignment;
public ulong robustUniformBufferAccessSizeAlignment;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceImageRobustnessFeaturesEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 robustImageAccess;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceWorkgroupMemoryExplicitLayoutFeaturesKHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 workgroupMemoryExplicitLayout;
public VkBool32 workgroupMemoryExplicitLayoutScalarBlockLayout;
public VkBool32 workgroupMemoryExplicitLayout8BitAccess;
public VkBool32 workgroupMemoryExplicitLayout16BitAccess;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDevicePortabilitySubsetFeaturesKHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 constantAlphaColorBlendFactors;
public VkBool32 events;
public VkBool32 imageViewFormatReinterpretation;
public VkBool32 imageViewFormatSwizzle;
public VkBool32 imageView2DOn3DImage;
public VkBool32 multisampleArrayImage;
public VkBool32 mutableComparisonSamplers;
public VkBool32 pointPolygons;
public VkBool32 samplerMipLodBias;
public VkBool32 separateStencilMaskRef;
public VkBool32 shaderSampleRateInterpolationFunctions;
public VkBool32 tessellationIsolines;
public VkBool32 tessellationPointMode;
public VkBool32 triangleFans;
public VkBool32 vertexAttributeAccessBeyondStride;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDevicePortabilitySubsetPropertiesKHR {
public VkStructureType sType;
public unsafe void* pNext;
public uint minVertexInputBindingStrideAlignment;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDevice4444FormatsFeaturesEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 formatA4R4G4B4;
public VkBool32 formatA4B4G4R4;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkBufferCopy2KHR {
public VkStructureType sType;
public unsafe void* pNext;
public ulong srcOffset;
public ulong dstOffset;
public ulong size;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkImageCopy2KHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkImageSubresourceLayers srcSubresource;
public VkOffset3D srcOffset;
public VkImageSubresourceLayers dstSubresource;
public VkOffset3D dstOffset;
public VkExtent3D extent;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkImageBlit2KHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkImageSubresourceLayers srcSubresource;
public VkOffset3D srcOffsets;
public VkImageSubresourceLayers dstSubresource;
public VkOffset3D dstOffsets;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkBufferImageCopy2KHR {
public VkStructureType sType;
public unsafe void* pNext;
public ulong bufferOffset;
public uint bufferRowLength;
public uint bufferImageHeight;
public VkImageSubresourceLayers imageSubresource;
public VkOffset3D imageOffset;
public VkExtent3D imageExtent;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkImageResolve2KHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkImageSubresourceLayers srcSubresource;
public VkOffset3D srcOffset;
public VkImageSubresourceLayers dstSubresource;
public VkOffset3D dstOffset;
public VkExtent3D extent;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkCopyBufferInfo2KHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkBuffer srcBuffer;
public VkBuffer dstBuffer;
public uint regionCount;
public unsafe VkBufferCopy2KHR* pRegions;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkCopyImageInfo2KHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkImage srcImage;
public VkImageLayout srcImageLayout;
public VkImage dstImage;
public VkImageLayout dstImageLayout;
public uint regionCount;
public unsafe VkImageCopy2KHR* pRegions;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkBlitImageInfo2KHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkImage srcImage;
public VkImageLayout srcImageLayout;
public VkImage dstImage;
public VkImageLayout dstImageLayout;
public uint regionCount;
public unsafe VkImageBlit2KHR* pRegions;
public VkFilter filter;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkCopyBufferToImageInfo2KHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkBuffer srcBuffer;
public VkImage dstImage;
public VkImageLayout dstImageLayout;
public uint regionCount;
public unsafe VkBufferImageCopy2KHR* pRegions;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkCopyImageToBufferInfo2KHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkImage srcImage;
public VkImageLayout srcImageLayout;
public VkBuffer dstBuffer;
public uint regionCount;
public unsafe VkBufferImageCopy2KHR* pRegions;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkResolveImageInfo2KHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkImage srcImage;
public VkImageLayout srcImageLayout;
public VkImage dstImage;
public VkImageLayout dstImageLayout;
public uint regionCount;
public unsafe VkImageResolve2KHR* pRegions;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceShaderImageAtomicInt64FeaturesEXT {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 shaderImageInt64Atomics;
public VkBool32 sparseImageInt64Atomics;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkFragmentShadingRateAttachmentInfoKHR {
public VkStructureType sType;
public unsafe void* pNext;
public unsafe VkAttachmentReference2* pFragmentShadingRateAttachment;
public VkExtent2D shadingRateAttachmentTexelSize;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPipelineFragmentShadingRateStateCreateInfoKHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkExtent2D fragmentSize;
public VkFragmentShadingRateCombinerOpKHR combinerOps;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceFragmentShadingRateFeaturesKHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 pipelineFragmentShadingRate;
public VkBool32 primitiveFragmentShadingRate;
public VkBool32 attachmentFragmentShadingRate;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceFragmentShadingRatePropertiesKHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkExtent2D minFragmentShadingRateAttachmentTexelSize;
public VkExtent2D maxFragmentShadingRateAttachmentTexelSize;
public uint maxFragmentShadingRateAttachmentTexelSizeAspectRatio;
public VkBool32 primitiveFragmentShadingRateWithMultipleViewports;
public VkBool32 layeredShadingRateAttachments;
public VkBool32 fragmentShadingRateNonTrivialCombinerOps;
public VkExtent2D maxFragmentSize;
public uint maxFragmentSizeAspectRatio;
public uint maxFragmentShadingRateCoverageSamples;
public VkSampleCountFlagBits maxFragmentShadingRateRasterizationSamples;
public VkBool32 fragmentShadingRateWithShaderDepthStencilWrites;
public VkBool32 fragmentShadingRateWithSampleMask;
public VkBool32 fragmentShadingRateWithShaderSampleMask;
public VkBool32 fragmentShadingRateWithConservativeRasterization;
public VkBool32 fragmentShadingRateWithFragmentShaderInterlock;
public VkBool32 fragmentShadingRateWithCustomSampleLocations;
public VkBool32 fragmentShadingRateStrictMultiplyCombiner;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceFragmentShadingRateKHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkSampleCountFlags sampleCounts;
public VkExtent2D fragmentSize;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceShaderTerminateInvocationFeaturesKHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 shaderTerminateInvocation;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceFragmentShadingRateEnumsFeaturesNV {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 fragmentShadingRateEnums;
public VkBool32 supersampleFragmentShadingRates;
public VkBool32 noInvocationFragmentShadingRates;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceFragmentShadingRateEnumsPropertiesNV {
public VkStructureType sType;
public unsafe void* pNext;
public VkSampleCountFlagBits maxFragmentShadingRateInvocationCount;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPipelineFragmentShadingRateEnumStateCreateInfoNV {
public VkStructureType sType;
public unsafe void* pNext;
public VkFragmentShadingRateTypeNV shadingRateType;
public VkFragmentShadingRateNV shadingRate;
public VkFragmentShadingRateCombinerOpKHR combinerOps;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkAccelerationStructureBuildSizesInfoKHR {
public VkStructureType sType;
public unsafe void* pNext;
public ulong accelerationStructureSize;
public ulong updateScratchSize;
public ulong buildScratchSize;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceMutableDescriptorTypeFeaturesVALVE {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 mutableDescriptorType;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkMutableDescriptorTypeListVALVE {
public uint descriptorTypeCount;
public unsafe VkDescriptorType* pDescriptorTypes;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkMutableDescriptorTypeCreateInfoVALVE {
public VkStructureType sType;
public unsafe void* pNext;
public uint mutableDescriptorTypeListCount;
public unsafe VkMutableDescriptorTypeListVALVE* pMutableDescriptorTypeLists;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkMemoryBarrier2KHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkPipelineStageFlags2KHR srcStageMask;
public VkAccessFlags2KHR srcAccessMask;
public VkPipelineStageFlags2KHR dstStageMask;
public VkAccessFlags2KHR dstAccessMask;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkImageMemoryBarrier2KHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkPipelineStageFlags2KHR srcStageMask;
public VkAccessFlags2KHR srcAccessMask;
public VkPipelineStageFlags2KHR dstStageMask;
public VkAccessFlags2KHR dstAccessMask;
public VkImageLayout oldLayout;
public VkImageLayout newLayout;
public uint srcQueueFamilyIndex;
public uint dstQueueFamilyIndex;
public VkImage image;
public VkImageSubresourceRange subresourceRange;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkBufferMemoryBarrier2KHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkPipelineStageFlags2KHR srcStageMask;
public VkAccessFlags2KHR srcAccessMask;
public VkPipelineStageFlags2KHR dstStageMask;
public VkAccessFlags2KHR dstAccessMask;
public uint srcQueueFamilyIndex;
public uint dstQueueFamilyIndex;
public VkBuffer buffer;
public ulong offset;
public ulong size;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkDependencyInfoKHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkDependencyFlags dependencyFlags;
public uint memoryBarrierCount;
public unsafe VkMemoryBarrier2KHR* pMemoryBarriers;
public uint bufferMemoryBarrierCount;
public unsafe VkBufferMemoryBarrier2KHR* pBufferMemoryBarriers;
public uint imageMemoryBarrierCount;
public unsafe VkImageMemoryBarrier2KHR* pImageMemoryBarriers;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkSemaphoreSubmitInfoKHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkSemaphore semaphore;
public ulong value;
public VkPipelineStageFlags2KHR stageMask;
public uint deviceIndex;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkCommandBufferSubmitInfoKHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkCommandBuffer commandBuffer;
public uint deviceMask;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkSubmitInfo2KHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkSubmitFlagsKHR flags;
public uint waitSemaphoreInfoCount;
public unsafe VkSemaphoreSubmitInfoKHR* pWaitSemaphoreInfos;
public uint commandBufferInfoCount;
public unsafe VkCommandBufferSubmitInfoKHR* pCommandBufferInfos;
public uint signalSemaphoreInfoCount;
public unsafe VkSemaphoreSubmitInfoKHR* pSignalSemaphoreInfos;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkQueueFamilyCheckpointProperties2NV {
public VkStructureType sType;
public unsafe void* pNext;
public VkPipelineStageFlags2KHR checkpointExecutionStageMask;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkCheckpointData2NV {
public VkStructureType sType;
public unsafe void* pNext;
public VkPipelineStageFlags2KHR stage;
public unsafe void* pCheckpointMarker;
}
[StructLayout(LayoutKind.Sequential)]
public struct VkPhysicalDeviceSynchronization2FeaturesKHR {
public VkStructureType sType;
public unsafe void* pNext;
public VkBool32 synchronization2;
}
}