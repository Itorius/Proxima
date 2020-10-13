// ReSharper disable UnusedMember.Global

using System;

namespace Proxima
{
	[Flags]
	internal enum ShaderStage
	{
		None = 0,
		Vertex = 1,
		TessellationControl = 2,
		TessellationEvaluation = 4,
		Geometry = 8,
		Fragment = 16,
		GLCompute = 32
	}

	internal enum SpirvResult
	{
		SUCCESS = 0,

		/* The SPIR-V is invalid. Should have been caught by validation ideally. */
		ERROR_INVALID_SPIRV = -1,

		/* The SPIR-V might be valid or invalid, but SPIRV-Cross currently cannot correctly translate this to your target language. */
		ERROR_UNSUPPORTED_SPIRV = -2,

		/* If for some reason we hit this, new or malloc failed. */
		ERROR_OUT_OF_MEMORY = -3,

		/* Invalid API argument. */
		ERROR_INVALID_ARGUMENT = -4,

		ERROR_INT_MAX = 0x7fffffff
	}

	internal enum SpirvResourceType
	{
		Unknown = 0,
		UniformBuffer = 1,
		StorageBuffer = 2,
		StageInput = 3,
		StageOutput = 4,
		SubpassInput = 5,
		StorageImage = 6,
		SampledImage = 7,
		AtomicCounter = 8,
		PushConstant = 9,
		SeparateImage = 10,
		SeparateSamplers = 11,
		AccelerationStructure = 12,
		RayQuery = 13,
		RESOURCE_TYPE_INT_MAX = 0x7fffffff
	}

	internal enum SpirvCaptureMode
	{
		Copy = 0,

		/*
		 * The payload will now be owned by the compiler.
		 * parsed_ir should now be considered a dead blob and must not be used further.
		 * This is optimal for performance and should be the go-to option.
		 */
		TakeOwnership = 1,

		CAPTURE_MODE_INT_MAX = 0x7fffffff
	}

	internal enum SpirvBackend
	{
		None = 0,
		GLSL = 1, /* spirv_cross::CompilerGLSL */
		HLSL = 2, /* CompilerHLSL */
		MSL = 3, /* CompilerMSL */
		CPP = 4, /* CompilerCPP */
		JSON = 5, /* CompilerReflection w/ JSON backend */
		BACKEND_INT_MAX = 0x7fffffff
	}

	internal enum SpirvDecoration
	{
		RelaxedPrecision = 0,
		SpecId = 1,
		Block = 2,
		BufferBlock = 3,
		RowMajor = 4,
		ColMajor = 5,
		ArrayStride = 6,
		MatrixStride = 7,
		GLSLShared = 8,
		GLSLPacked = 9,
		CPacked = 10,
		BuiltIn = 11,
		NoPerspective = 13,
		Flat = 14,
		Patch = 15,
		Centroid = 16,
		Sample = 17,
		Invariant = 18,
		Restrict = 19,
		Aliased = 20,
		Volatile = 21,
		Constant = 22,
		Coherent = 23,
		NonWritable = 24,
		NonReadable = 25,
		Uniform = 26,
		UniformId = 27,
		SaturatedConversion = 28,
		Stream = 29,
		Location = 30,
		Component = 31,
		Index = 32,
		Binding = 33,
		DescriptorSet = 34,
		Offset = 35,
		XfbBuffer = 36,
		XfbStride = 37,
		FuncParamAttr = 38,
		FPRoundingMode = 39,
		FPFastMathMode = 40,
		LinkageAttributes = 41,
		NoContraction = 42,
		InputAttachmentIndex = 43,
		Alignment = 44,
		MaxByteOffset = 45,
		AlignmentId = 46,
		MaxByteOffsetId = 47,
		NoSignedWrap = 4469,
		NoUnsignedWrap = 4470,
		ExplicitInterpAMD = 4999,
		OverrideCoverageNV = 5248,
		PassthroughNV = 5250,
		ViewportRelativeNV = 5252,
		SecondaryViewportRelativeNV = 5256,
		PerPrimitiveNV = 5271,
		PerViewNV = 5272,
		PerTaskNV = 5273,
		PerVertexNV = 5285,
		NonUniform = 5300,
		NonUniformEXT = 5300,
		RestrictPointer = 5355,
		RestrictPointerEXT = 5355,
		AliasedPointer = 5356,
		AliasedPointerEXT = 5356,
		CounterBuffer = 5634,
		HlslCounterBufferGOOGLE = 5634,
		HlslSemanticGOOGLE = 5635,
		UserSemantic = 5635,
		UserTypeGOOGLE = 5636,
		Max = 0x7fffffff
	}
}