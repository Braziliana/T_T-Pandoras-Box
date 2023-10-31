#pragma once
#include <vector>

#define GLEW_STATIC
#include <GL/glew.h>
#include "VertexAttribute.h"
#include "../Materials/Material.h"

template<typename VertexType, typename InstanceDataType>
class InstancedBuffer
{
private:
    GLuint _vao, _vbo, _instanceVbo;
    size_t _instancesMaxCount;
    Material* _material;
    std::vector<InstanceDataType> _instanceDataVector;
    size_t _vertexCount;
    
public:
    InstancedBuffer(const std::vector<VertexType>& vertices,
                    size_t instancesMaxCount,
                    const std::vector<VertexAttribute>& vertexAttributes,
                    const std::vector<VertexAttribute>& instanceAttributes,
                    Material* material)
        : _instancesMaxCount(instancesMaxCount), _material(material), _vertexCount(vertices.size())
    {
        glGenVertexArrays(1, &_vao);
        glBindVertexArray(_vao);

        glGenBuffers(1, &_vbo);
        glBindBuffer(GL_ARRAY_BUFFER, _vbo);
        glBufferData(GL_ARRAY_BUFFER, vertices.size() * sizeof(VertexType), vertices.data(), GL_STATIC_DRAW);

        for (const auto& attr : vertexAttributes) {
            glVertexAttribPointer(attr.index, attr.size, attr.type, attr.normalized, attr.stride, attr.pointer);
            glEnableVertexAttribArray(attr.index);
            glVertexAttribDivisor(attr.index, 0);
        }

        glGenBuffers(1, &_instanceVbo);
        glBindBuffer(GL_ARRAY_BUFFER, _instanceVbo);
        glBufferData(GL_ARRAY_BUFFER, instancesMaxCount * sizeof(InstanceDataType), nullptr, GL_DYNAMIC_DRAW);

        for (const auto& attr : instanceAttributes) {
            glVertexAttribPointer(attr.index, attr.size, attr.type, attr.normalized, attr.stride, attr.pointer);
            glEnableVertexAttribArray(attr.index);
            glVertexAttribDivisor(attr.index, attr.divisor);
        }

        glBindBuffer(GL_ARRAY_BUFFER, 0);
        glBindVertexArray(0);
    }

    ~InstancedBuffer() {
        Release();
    }

    void Add(const InstanceDataType& instanceData) {
        if (_instanceDataVector.size() >= _instancesMaxCount) {
            Flush();
        }
        _instanceDataVector.push_back(instanceData);
    }

    void Flush() {
        if (_instanceDataVector.empty()) return;

        glBindBuffer(GL_ARRAY_BUFFER, _instanceVbo);
        glBufferSubData(GL_ARRAY_BUFFER, 0, _instanceDataVector.size() * sizeof(InstanceDataType), _instanceDataVector.data());
        glBindBuffer(GL_ARRAY_BUFFER, 0);

        _material->Begin();
        glBindVertexArray(_vao);
        glDrawArraysInstanced(GL_TRIANGLES, 0, static_cast<GLsizei>(_vertexCount), static_cast<GLsizei>(_instanceDataVector.size()));
        glBindVertexArray(0);

        _instanceDataVector.clear();
    }

    void Release() {
        glDeleteBuffers(1, &_vbo);
        glDeleteBuffers(1, &_instanceVbo);
        glDeleteVertexArrays(1, &_vao);
        _instanceDataVector.clear();
        _instanceDataVector.shrink_to_fit();
    }
};