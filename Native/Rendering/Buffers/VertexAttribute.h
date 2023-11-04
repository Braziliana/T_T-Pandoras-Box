#pragma once

#define GLEW_STATIC
#include <GL/glew.h>

struct VertexAttribute {
    GLuint index;
    GLint size;
    GLenum type;
    GLboolean normalized;
    GLsizei stride;
    const void* pointer;
    GLuint divisor;
};
