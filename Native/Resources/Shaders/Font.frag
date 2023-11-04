#version 450

in vec2 TexCoords;
in vec4 Color;

out vec4 fragColor;

uniform sampler2D textureSampler;

void main()
{
    fragColor = vec4(Color.r, Color.g, Color.b, texture(textureSampler, TexCoords).r);
}