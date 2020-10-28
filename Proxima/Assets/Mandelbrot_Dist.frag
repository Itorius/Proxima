#version 450
#extension GL_ARB_separate_shader_objects : enable

layout(binding = 1) uniform Settings {
	vec4 u_Area;
	int u_MaxIterations;
	float u_Angle;
	float u_Time;
} settings;

layout(binding = 2) uniform sampler1D texSampler;

layout(location = 0) in vec4 inColor;
layout(location = 1) in vec2 inUV;

layout(location = 0) out vec4 outColor;

vec2 rotate(vec2 point, vec2 pivot, float angle)
{
	float s = sin(angle);
	float c = cos(angle);

	point -= pivot;
	point = vec2(point.x * c - point.y * s, point.x * s + point.y * c);
	point += pivot;

	return point;
}

float distanceToMandelbrot( in vec2 c )
{
	float c2 = dot(c, c);
	if( 256.0*c2*c2 - 96.0*c2 + 32.0*c.x - 3.0 < 0.0 ) return 0.0;
	if( 16.0*(c2+2.0*c.x+1.0) - 1.0 < 0.0 ) return 0.0;

	float di =  1.0;
	vec2 z  = vec2(0.0);
	float m2 = 0.0;
	vec2 dz = vec2(0.0);
	for( int i=0; i<settings.u_MaxIterations; i++ )
	{
		if( m2>1024.0 ) { di=0.0; break; }

		dz = 2.0*vec2(z.x*dz.x-z.y*dz.y, z.x*dz.y + z.y*dz.x) + vec2(1.0,0.0);

		z = vec2( z.x*z.x - z.y*z.y, 2.0*z.x*z.y ) + c;

		m2 = dot(z,z);
	}

	float d = 0.5*sqrt(dot(z,z)/dot(dz,dz))*log(dot(z,z));
	if( di>0.5 ) d=0.0;

	return d;
}

void main() {
	vec2 uv = inUV;
	uv = settings.u_Area.xy + (uv - 0.5) * settings.u_Area.zw;
	uv = rotate(uv, settings.u_Area.xy, settings.u_Angle);

	uv.x = abs(uv.x);
	uv.y = abs(uv.y);

	float d = distanceToMandelbrot(uv);

	d = clamp(pow(4.0 * d / settings.u_Area.z, 0.2), 0.0, 1.0);

	outColor = texture(texSampler, d);
}