#version 450

in vec2 TexCoords;
in vec4 Color;

out vec4 fragColor;

uniform sampler2D textureSampler;

void main()
{
    vec4 sampled = vec4(1, 1, 1, texture(textureSampler, TexCoords));
    
    fragColor = sampled * Color;
}  