#version 450

in vec2 UV;
in float Size;
in vec4 Color;
in vec4 BorderColor;
in float BorderSize;

out vec4 fragColor;

vec4 getFragmentColor()
{
    vec2 pixelCoord = UV * Size;
    vec2 centerPixel = vec2(0.5) * Size;
    float distFromCenter = distance(pixelCoord, centerPixel);
    float end = Size * 0.5;
    float fillEnd = end - BorderSize;
    
    if(distFromCenter > end) 
    {
        return vec4(0, 0, 0, 0);
    }
    else if(distFromCenter < fillEnd)
    {
        return Color;    
    }
    
    return BorderColor;
}

void main() {
    fragColor = getFragmentColor();
}