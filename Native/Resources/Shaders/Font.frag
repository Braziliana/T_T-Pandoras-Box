#version 450

in vec2 TexCoords;
in vec4 Color;

out vec4 fragColor;

uniform sampler2D textureSampler;

void main()
{
    float alpha = texture(textureSampler, TexCoords).r;
    fragColor = vec4(Color.r, Color.g, Color.b, alpha);
}