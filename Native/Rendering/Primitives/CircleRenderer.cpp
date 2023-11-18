#include "CircleRenderer.h"
#include "../Renderer.h"

CircleRenderer::CircleRenderer()
{
    const std::vector<VertexAttribute> vertexAttributes = {
        {0, 3, GL_FLOAT, GL_FALSE, sizeof(Vertex), reinterpret_cast<void*>(offsetof(Vertex, position)), 0},
        {1, 2, GL_FLOAT, GL_FALSE, sizeof(Vertex), reinterpret_cast<void*>(offsetof(Vertex, uv)), 0},
        {2, 4, GL_FLOAT, GL_FALSE, sizeof(Vertex), reinterpret_cast<void*>(offsetof(Vertex, color)), 0},
    };
    
    const std::vector<VertexAttribute> instanceAttributes = {
        {3, 3, GL_FLOAT, GL_FALSE, sizeof(CircleData), reinterpret_cast<void*>(offsetof(CircleData, position)), 1},
        {4, 3, GL_FLOAT, GL_FALSE, sizeof(CircleData), reinterpret_cast<void*>(offsetof(CircleData, scale)), 1},
        {5, 4, GL_FLOAT, GL_FALSE, sizeof(CircleData), reinterpret_cast<void*>(offsetof(CircleData, color)), 1},
        {6, 4, GL_FLOAT, GL_FALSE, sizeof(CircleData), reinterpret_cast<void*>(offsetof(CircleData, borderColor)), 1},
        {7, 1, GL_FLOAT, GL_FALSE, sizeof(CircleData), reinterpret_cast<void*>(offsetof(CircleData, borderSize)), 1},
    };
    
    float size = 1.0f;

    std::vector<Vertex> verts2D;
    // First Triangle
    verts2D.push_back({{ -size, -size, 0.0f }, { 0.0f, 0.0f }, { 1.0f, 1.0f, 1.0f, 1.0f }});
    verts2D.push_back({{  size, -size, 0.0f }, { 1.0f, 0.0f }, { 1.0f, 1.0f, 1.0f, 1.0f }});
    verts2D.push_back({{ -size,  size, 0.0f }, { 0.0f, 1.0f }, { 1.0f, 1.0f, 1.0f, 1.0f }});

    // Second Triangle
    verts2D.push_back({{  size, -size, 0.0f }, { 1.0f, 0.0f }, { 1.0f, 1.0f, 1.0f, 1.0f }});
    verts2D.push_back({{ -size,  size, 0.0f }, { 0.0f, 1.0f }, { 1.0f, 1.0f, 1.0f, 1.0f }});
    verts2D.push_back({{  size,  size, 0.0f }, { 1.0f, 1.0f }, { 1.0f, 1.0f, 1.0f, 1.0f }});

    std::vector<Vertex> verts3D;
    // First Triangle
    verts3D.push_back({{ -size, 0.0f, -size }, { 0.0f, 0.0f }, { 1.0f, 1.0f, 1.0f, 1.0f }});
    verts3D.push_back({{  size, 0.0f, -size }, { 1.0f, 0.0f }, { 1.0f, 1.0f, 1.0f, 1.0f }});
    verts3D.push_back({{ -size, 0.0f,  size }, { 0.0f, 1.0f }, { 1.0f, 1.0f, 1.0f, 1.0f }});

    // Second Triangle
    verts3D.push_back({{  size, 0.0f, -size }, { 1.0f, 0.0f }, { 1.0f, 1.0f, 1.0f, 1.0f }});
    verts3D.push_back({{ -size, 0.0f,  size }, { 0.0f, 1.0f }, { 1.0f, 1.0f, 1.0f, 1.0f }});
    verts3D.push_back({{  size, 0.0f,  size }, { 1.0f, 1.0f }, { 1.0f, 1.0f, 1.0f, 1.0f }});

    
    const auto shader = ShaderManager::GetInstance().CreateShader(L"Circle");
    _material = new Material(shader);
    _buffer2D = new InstancedBuffer<Vertex, CircleData>(verts2D, 200, vertexAttributes, instanceAttributes, _material);
    _buffer3D = new InstancedBuffer<Vertex, CircleData>(verts3D, 200, vertexAttributes, instanceAttributes, _material);
}


CircleRenderer::~CircleRenderer()
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

void CircleRenderer::Release()
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

void CircleRenderer::Filled(const Vector2& position, const float size, const Color& color) const
{
    if(!_buffer2D->CanAdd())
    {
        Flush2D();
    }
    _buffer2D->Add(CircleData{Vector3(position.x, position.y, 0.0f), Vector3(size, size, 1.0f), color, Color(0, 0, 0, 0), 0});
}

void CircleRenderer::Filled(const Vector3& position, const float size, const Color& color) const
{
    if(!_buffer3D->CanAdd())
    {
        Flush3D();
    }
    _buffer3D->Add(CircleData{position, Vector3(size, 1.0f, size), color, Color(0, 0, 0, 0), 0});
}

void CircleRenderer::FilledBordered(const Vector2& position, const float size, const Color& color, const Color& borderColor,
    float borderSize) const
{
    if(!_buffer2D->CanAdd())
    {
        Flush2D();
    }
    _buffer2D->Add(CircleData{Vector3(position.x, position.y, 0.0f), Vector3(size, size, 1.0f), color, borderColor, borderSize});
}

void CircleRenderer::FilledBordered(const Vector3& position, const float size, const Color& color, const Color& borderColor,
                                    const float borderSize) const
{
    if(!_buffer3D->CanAdd())
    {
        Flush3D();
    }
    _buffer3D->Add(CircleData{position, Vector3(size, 1.0f, size), color, borderColor, borderSize});
}

void CircleRenderer::Border(const Vector2& position, const float size, const Color& color, const float borderSize) const
{
    _buffer2D->Add(CircleData{Vector3(position.x, position.y, 0.0f), Vector3(size, size, 1.0f), Color(0, 0, 0, 0), color, borderSize});
}

void CircleRenderer::Border(const Vector3& position, const float size, const Color& color, const float borderSize) const
{
    if(!_buffer3D->CanAdd())
    {
        Flush3D();
    }
    _buffer3D->Add(CircleData{position, Vector3(size, 1.0f, size), Color(0, 0, 0, 0), color, borderSize});
}

void CircleRenderer::Flush2D() const
{
    _material->SetMat4("viewProjectionMatrix", Renderer::Instance()->Get2DMatrix());
    _buffer2D->Flush();
}

void CircleRenderer::Flush3D() const
{
    _material->SetMat4("viewProjectionMatrix", Renderer::Instance()->Get3DMatrix());
    _buffer3D->Flush();
}
