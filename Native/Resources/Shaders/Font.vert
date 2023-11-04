#version 450
// Vertex data
layout(location = 0) in vec3 inPosition;
layout(location = 1) in vec2 inUv;

// Instance data
layout(location = 2) in vec3 instancePosition;
layout(location = 3) in vec3 instanceScale;
layout(location = 4) in vec2 instanceUV;
layout(location = 5) in vec2 instanceUVSize;
layout(location = 6) in vec4 instanceColor;

out vec2 TexCoords;
out vec4 Color;

//GLOBAL
uniform mat4 viewProjectionMatrix;

void main()
{
    mat4 model = mat4(1.0); // Initialize model matrix to identity
    model[3] = vec4(instancePosition, 1.0); // Translation
    model = model * mat4(instanceScale.x, 0.0, 0.0, 0.0,
                         0.0, instanceScale.y, 0.0, 0.0,
                         0.0, 0.0, instanceScale.z, 0.0,
                         0.0, 0.0, 0.0, 1.0); // Scale

    gl_Position = viewProjectionMatrix * model * vec4(inPosition, 1.0);
    TexCoords = inUv * instanceUVSize + instanceUV;
    Color = instanceColor;
}  