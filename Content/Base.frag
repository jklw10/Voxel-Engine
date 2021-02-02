#version 450
precision highp float;

uniform vec2 Resolution;
uniform vec2 ScreenPos;

in vec4 gl_FragCoord;
in vec4 Color;
in vec2 UVtrans;
out vec4 color;

void main()
{
	//if(gl_FragCoord > iResolution){
		color = Color;
		
	   //color -= min(max((ScreenPos-(gl_FragCoord).xy).x,0.0),1.0);
       //color -= min(max((ScreenPos-(gl_FragCoord).xy).y,0.0),1.0);
	   //color -= min(max(((gl_FragCoord).xy-(ScreenPos+Resolution)).x,0.0),1.0);
       //color -= min(max(((gl_FragCoord).xy-(ScreenPos+Resolution)).y,0.0),1.0);
		
		//color = Color;
}

