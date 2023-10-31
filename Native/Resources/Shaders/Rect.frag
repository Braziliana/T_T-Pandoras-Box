#version 450

in vec2 UV;
in vec4 Color;

out vec4 fragColor;

void main() {
    fragColor = Color; // Or use a texture, etc.
}