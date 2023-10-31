#version 450
// Vertex data
layout (location = 0) in vec4 vertex; // <vec2 pos, vec2 tex>
out vec2 TexCoords;

//GLOBAL
uniform mat4 viewProjectionMatrix;

void main()
{
    gl_Position = viewProjectionMatrix * vec4(vertex.xy, 0.0, 1.0);
    TexCoords = vertex.zw;
}  