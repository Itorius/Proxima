#version 450
#extension GL_ARB_separate_shader_objects : enable

layout(binding = 0) uniform UniformBufferObject {
	mat4 camera;
} ubo;

layout(location = 0) in vec3 inPosition;
layout(location = 1) in vec4 inColor;
layout(location = 2) in vec2 inUV;

layout(location = 0) out vec4 outColor;
layout(location = 1) out vec2 outUV;

void main() {
	gl_Position = ubo.camera * vec4(inPosition, 1.0);
	outColor = inColor;
	outUV = inUV;
}