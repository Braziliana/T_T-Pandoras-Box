#version 450

in vec2 UV;
in vec4 Color;

out vec4 fragColor;

void main() {
    float distFromCenter = distance(UV, vec2(0.5));
    if(distFromCenter < 0.48 || distFromCenter >= 0.5){
        fragColor = vec4(0, 0, 0, 0);
        return;
    }
    
    fragColor = Color;
}