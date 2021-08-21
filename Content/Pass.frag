#version 450
precision highp float;

layout(location = 0) uniform sampler2D screenTex;
layout(location = 1) uniform sampler2D depthTex;

in vec4 gl_FragCoord;
in vec2 TexCoords;
out vec4 color;

uniform vec2 resolution;

uniform vec3 viewPos;

uniform int Mode;

mat4 ProjMatrix;

float near = 0.1f;
float far = 1000f;

vec3 BlinnPhong(vec3 normal, vec3 fragPos, vec3 lightPos, vec3 lightColor)
{
    // diffuse
    vec3 lightDir = normalize(lightPos - fragPos);
    float diff = max(dot(lightDir, normal), 0.0);
    vec3 diffuse = diff * lightColor;
    // specular
    vec3 viewDir = normalize(viewPos - fragPos);
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = 0.0;
    vec3 halfwayDir = normalize(lightDir + viewDir);  
    spec = pow(max(dot(normal, halfwayDir), 0.0), 64.0);
    vec3 specular = spec * lightColor;    
    // simple attenuation
    float max_distance = 1.5;
    float distance = length(lightPos - fragPos);
    float attenuation = 1.0 / (distance * distance);
    
    diffuse *= attenuation;
    specular *= attenuation;
    
    return diffuse + specular;
}

float linearDepth(float depth)
{
	float ndc_depth = depth*2-1;
	return (2*near*far)/(far+near-ndc_depth*(far-near))/(far-near);
}
mat4 invProj = inverse(ProjMatrix);
vec3 CalcCameraSpacePosition(vec4 frag)
{
    vec4 ndcPos;
    ndcPos.xy = ((frag.xy / resolution.xy) * 2.0) - 1.0;
    ndcPos.z = linearDepth(frag.z);
    ndcPos.w = 1.0;
    
    vec4 clipPos = ndcPos / frag.w;
    vec4 clip = invProj * ndcPos;
    vec3 vertex = (clip / clip.w).xyz;

    return vertex;
}



const int sampleSize = 3;
vec3[9] SampleCoordinates(){
	vec3 sampleTex[9];
	for(int y = 0; y < sampleSize; y++){
		for(int x = 0; x < sampleSize; x++){
			vec2 offset = vec2(x-1,y-1); 
			vec4 frag;
			frag.xy = gl_FragCoord.xy + offset;
			frag.z	= texture(depthTex, TexCoords+offset/resolution).r; //div offset by resolution to make it texel
			frag.w	= gl_FragCoord.w;

			sampleTex[x+y*3] = CalcCameraSpacePosition(frag);
			//sampleTex[x+y*3] = vec3(1,0,0);
		}
	}
	return sampleTex;
}

vec3[8] sampleNormals(){
	ivec2 order[6] = ivec2[](
		ivec2(0,0),
		ivec2(0,1),
		ivec2(1,0),
		ivec2(1,0),
		ivec2(0,1),
		ivec2(1,1)
	);
	vec3[9] sampleTex = SampleCoordinates();
	vec3 Normals[8];
	int less = sampleSize-1;
	for(int y = 0; y < less; y++){
		for(int x = 0; x < less; x++){
			for(int z = 0; z < 2; z++){
				ivec2 offa = ivec2(x,y)+order[z+1];
				ivec2 offb = ivec2(x,y)+order[z+2];
				ivec2 offc = ivec2(x,y)+order[z+3];
				vec3 a = sampleTex[offa.x+offa.y*sampleSize];
				vec3 b = sampleTex[offb.x+offb.y*sampleSize];
				vec3 c = sampleTex[offc.x+offc.y*sampleSize];
				Normals[x+y*less+z] = normalize(cross(a-c,b-c));
			}
		}
	}
	return Normals;
}


float gamma = 2.2;
void main()
{
	float dist = linearDepth(texture(depthTex, TexCoords).r);

	color = 1- vec4(pow(texture(screenTex, TexCoords).rgb, vec3(1/gamma)),1) /dist;
	if (Mode == 3){
		color = vec4(vec3(linearDepth(texture(depthTex, TexCoords).r)),0);
	}
	if (Mode == 0){
		vec3 Normals[8] = sampleNormals();
	
		vec3 avg = vec3(0);
		for(int x = 0; x < 8; x++)
		{
			avg += Normals[x];
		}
		avg /= 8;
		color = vec4(avg,0);
	}
	if (Mode == 1){
		vec3 Depths[9] = SampleCoordinates();
		vec3 avg = vec3(0);
		for(int x = 0; x < 9; x++)
		{
			avg += Depths[x];
		}
		avg /= 9;
		color = vec4(avg,0);
	}
	if (Mode == 2){
		color = vec4(pow(texture(screenTex, TexCoords).rgb, vec3(1/gamma)),1);
	}
}
