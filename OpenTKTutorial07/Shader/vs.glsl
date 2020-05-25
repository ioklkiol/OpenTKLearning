#version 330 core

in vec3 vPosition;
in vec2 aTexCoord;

out vec2 TexCoord;
uniform mat4 modelView;

void main()
{
    gl_Position = modelView * vec4(vPosition, 1.0);
	TexCoord = aTexCoord;
}