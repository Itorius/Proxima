#version 450
#extension GL_ARB_separate_shader_objects : enable

layout(binding = 1) uniform sampler2D texSampler;
layout(binding = 2) uniform sampler2D texSampler1;

layout(binding = 3) uniform k {
	mat4 camera;
} kkkk;

layout(location = 0) in vec4 inColor;
layout(location = 1) in vec2 inUV;

layout(location = 0) out vec4 outColor;

void main() {
	outColor = texture(texSampler, inUV) *texture(texSampler1, inUV) ;
	outColor = kkkk.camera * outColor;
}