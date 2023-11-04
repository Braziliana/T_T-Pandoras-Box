#include "RectRenderer.h"

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

    std::vector<Vertex> verts3D;
    // First Triangle
    verts3D.push_back({{ -0.5f, 0.0f, -0.5f }, { 0.0f, 0.0f }, { 1.0f, 1.0f, 1.0f, 1.0f }});
    verts3D.push_back({{  0.5f, 0.0f, -0.5f }, { 1.0f, 0.0f }, { 1.0f, 1.0f, 1.0f, 1.0f }});
    verts3D.push_back({{ -0.5f,  0.0f, 0.5f }, { 0.0f, 1.0f }, { 1.0f, 1.0f, 1.0f, 1.0f }});

    // Second Triangle
    verts3D.push_back({{  0.5f, 0.0f, -0.5f }, { 1.0f, 0.0f }, { 1.0f, 1.0f, 1.0f, 1.0f }});
    verts3D.push_back({{ -0.5f, 0.0f, 0.5f }, { 0.0f, 1.0f }, { 1.0f, 1.0f, 1.0f, 1.0f }});
    verts3D.push_back({{  0.5f, 0.0f, 0.5f }, { 1.0f, 1.0f }, { 1.0f, 1.0f, 1.0f, 1.0f }});
    
    const auto shader = ShaderManager::GetInstance().CreateShader(L"Rect");
    _material = new Material(shader);
    _buffer2D = new InstancedBuffer<Vertex, RectData>(verts2D, 200, vertexAttributes, instanceAttributes, _material);
    _buffer3D = new InstancedBuffer<Vertex, RectData>(verts3D, 200, vertexAttributes, instanceAttributes, _material);
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

    if(_material != nullptr)
    {
        _material->Release();
        delete _material;
        _material = nullptr;
    }
}

void RectRenderer::Draw(const Vector2& position, const Vector2& size, const Color& color) const
{
    _buffer2D->Add(RectData{Vector3(position.x, position.y, 0.0f), Vector3(size.x, size.y, 1.0f), color});
}

void RectRenderer::Draw(const Vector3& position, const Vector2& size, const Color& color) const
{
    _buffer3D->Add(RectData{position, Vector3(size.x, 1.0f, size.y), color});
}

void RectRenderer::Flush2D() const
{
    _buffer2D->Flush();
}

void RectRenderer::Flush3D() const
{
    _buffer3D->Flush();
}
