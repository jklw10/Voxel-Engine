#version 450
layout(location = 0) in vec3 inPos;
layout(location = 1) in vec2 inTexCoords;

out vec2 TexCoords;

void main()
{
	gl_Position =  vec4(vec3(inPos),1);
	TexCoords = inTexCoords;
}
