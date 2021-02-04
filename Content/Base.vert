#version 450
layout(location = 0) in vec3 Vertex;

layout(location = 1) in vec4 ColorIn;
layout(location = 2) in vec4 Rotation;
layout(location = 3) in vec3 Translation;

uniform float Scale;


uniform mat4 ViewMatrix;
uniform mat4 ProjMatrix;

out vec4 Color;

vec3 Rotate(vec3 v, vec4 q)
{ 
    vec3 vPrime;
    // Extract the vector part of the quaternion
    vec3 u = q.xyz;

    // Extract the scalar part of the quaternion
    float s = q.w;

    // Do the math
    vPrime = 2.0f * dot(u, v) * u
          + (s*s - dot(u, u)) * v
          + 2.0f * s * cross(u, v);

    return vPrime;
}

void main()
{
	gl_Position =  vec4(Translation+Rotate(Vertex * Scale, Rotation),1.0) *ViewMatrix *ProjMatrix;
	Color = ColorIn;
}

