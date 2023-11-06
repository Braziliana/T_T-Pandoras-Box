#version 450

in vec2 UV;
in vec4 Color;
in vec4 BorderColor;
in vec2 BorderSize;

out vec4 fragColor;

void main() {

    bool inBorder = (UV.x < BorderSize.x) ||
    (UV.x > 1.0 - BorderSize.x) ||
    (UV.y < BorderSize.y) ||
    (UV.y > 1.0 - BorderSize.y);

    if (inBorder)
    {
        fragColor = BorderColor;
    }
    else
    {
        fragColor = Color;
    }
}