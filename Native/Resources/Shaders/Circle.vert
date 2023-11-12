#version 450
// Vertex data
layout(location = 0) in vec3 inPosition;
layout(location = 1) in vec2 inUV;
layout(location = 2) in vec4 inColor;

// Instance data
layout(location = 3) in vec3 instancePosition;
layout(location = 4) in vec3 instanceScale;
layout(location = 5) in vec4 instanceColor;
layout(location = 6) in vec4 instanceBorderColor;
layout(location = 7) in float instanceBorderSize;

//GLOBAL
uniform mat4 viewProjectionMatrix;

out vec2 UV;
out float Size;
out vec4 Color;
out vec4 BorderColor;
out float BorderSize;

void main() 
{
    mat4 model = mat4(1.0); // Initialize model matrix to identity
    model[3] = vec4(instancePosition, 1.0); // Translation
    model = model * mat4(instanceScale.x, 0.0, 0.0, 0.0,
                         0.0, instanceScale.y, 0.0, 0.0,
                         0.0, 0.0, instanceScale.z, 0.0,
                         0.0, 0.0, 0.0, 1.0); // Scale

    gl_Position = viewProjectionMatrix * model * vec4(inPosition, 1.0);
    UV = inUV;
    Size = instanceScale.x;
    Color = inColor * instanceColor;

    BorderColor = instanceBorderColor;
    BorderSize = instanceBorderSize;
}