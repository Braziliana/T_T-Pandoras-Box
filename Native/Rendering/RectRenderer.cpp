#include "RectRenderer.h"

#include "../Materials/MaterialManager.h"

RectRenderer::RectRenderer()
{
    const std::vector<VertexAttribute> vertexAttributes = {
        {0, 3, GL_FLOAT, GL_FALSE, sizeof(Vertex), reinterpret_cast<void*>(offsetof(Vertex, position)), 0},
        {1, 2, GL_FLOAT, GL_FALSE, sizeof(Vertex), reinterpret_cast<void*>(offsetof(Vertex, uv)), 0},
        {2, 4, GL_FLOAT, GL_FALSE, sizeof(Vertex), reinterpret_cast<void*>(offsetof(Vertex, color)), 0},
    };

    const std::vector<VertexAttribute> instanceAttributes = {
        {3, 3, GL_FLOAT, GL_FALSE, sizeof(RectData), reinterpret_cast<void*>(offsetof(RectData, position)), 1},
        {4, 3, GL_FLOAT, GL_FALSE, sizeof(RectData), reinterpret_cast<void*>(offsetof(RectData, scale)), 1},
        {5, 4, GL_FLOAT, GL_FALSE, sizeof(RectData), reinterpret_cast<void*>(offsetof(RectData, color)), 1},
    };
    
    std::vector<Vertex> verts2D;
    // First Triangle
    verts2D.push_back({{ -0.5f, -0.5f, 0.0f }, { 0.0f, 0.0f }, { 1.0f, 1.0f, 1.0f, 1.0f }});
    verts2D.push_back({{  0.5f, -0.5f, 0.0f }, { 1.0f, 0.0f }, { 1.0f, 1.0f, 1.0f, 1.0f }});
    verts2D.push_back({{ -0.5f,  0.5f, 0.0f }, { 0.0f, 1.0f }, { 1.0f, 1.0f, 1.0f, 1.0f }});

    // Second Triangle
    verts2D.push_back({{  0.5f, -0.5f, 0.0f }, { 1.0f, 0.0f }, { 1.0f, 1.0f, 1.0f, 1.0f }});
    verts2D.push_back({{ -0.5f,  0.5f, 0.0f }, { 0.0f, 1.0f }, { 1.0f, 1.0f, 1.0f, 1.0f }});
    verts2D.push_back({{  0.5f,  0.5f, 0.0f }, { 1.0f, 1.0f }, { 1.0f, 1.0f, 1.0f, 1.0f }});
    
    _buffer2D = new InstancedBuffer<Vertex, RectData>(verts2D, 200, vertexAttributes, instanceAttributes, MaterialManager::GetInstance().GetMaterial(L"Rect"));
    _buffer3D = new InstancedBuffer<Vertex, RectData>(verts2D, 200, vertexAttributes, instanceAttributes, MaterialManager::GetInstance().GetMaterial(L"Rect"));
}

RectRenderer::~RectRenderer()
{
    if(_buffer2D != nullptr)
    {
        _buffer2D->Release();
        delete _buffer2D;
        _buffer2D = nullptr;
    }
    
    if(_buffer3D != nullptr)
    {
        _buffer3D->Release();
        delete _buffer3D;
        _buffer3D = nullptr;
    }
}

void RectRenderer::Release()
{
    if(_buffer2D != nullptr)
    {
        _buffer2D->Release();
        _buffer2D = nullptr;
    }
        
    if(_buffer3D != nullptr)
    {
        _buffer3D->Release();
        _buffer3D = nullptr;
    }
}

void RectRenderer::DrawRect(const Vector2& position, const Vector2& size, const Color& color)
{
    _buffer2D->Add(RectData{Vector3(position.x, position.y, 0.0f), Vector3(size.x, size.y, 1.0f), color});
}

void RectRenderer::DrawRect(const Vector3& position, const Vector2& size, const Color& color)
{
    _buffer3D->Add(RectData{position, Vector3(size.x, 1.0f, size.y), color});
}

void RectRenderer::Flush2D()
{
    _buffer2D->Flush();
}

void RectRenderer::Flush3D()
{
    _buffer3D->Flush();
}
