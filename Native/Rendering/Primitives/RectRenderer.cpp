#include "RectRenderer.h"
#include "../Renderer.h"

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
        {6, 4, GL_FLOAT, GL_FALSE, sizeof(RectData), reinterpret_cast<void*>(offsetof(RectData, borderColor)), 1},
        {7, 1, GL_FLOAT, GL_FALSE, sizeof(RectData), reinterpret_cast<void*>(offsetof(RectData, borderSize)), 1},
    };
    
    float size = 0.5f;

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

void RectRenderer::Filled(const Vector2& position, const Vector2& size, const Color& color) const
{
    _buffer2D->Add(RectData{Vector3(position.x, position.y, 0.0f), Vector3(size.x, size.y, 1.0f), color, Color{0.0f, 0.0f, 0.0f, 0.0f}, 0.0f});
}

void RectRenderer::Filled(const Vector3& position, const Vector2& size, const Color& color) const
{
    _buffer3D->Add(RectData{position, Vector3(size.x, 1.0f, size.y), color, Color{0.0f, 0.0f, 0.0f, 0.0f}, 0.0f});
}

float BorderSize(const Vector2& size, const float borderSize)
{
    const float min = size.x < size.y ? size.x : size.y;
    return borderSize / min;
}

void RectRenderer::FilledBordered(const Vector2& position, const Vector2& size, const Color& color, const Color& borderColor,
                                  const float borderSize) const
{
    _buffer2D->Add(RectData{Vector3(position.x, position.y, 0.0f), Vector3(size.x, size.y, 1.0f), color, borderColor, BorderSize(size, borderSize)});
}

void RectRenderer::FilledBordered(const Vector3& position, const Vector2& size, const Color& color, const Color& borderColor,
                                  const float borderSize) const
{
    _buffer3D->Add(RectData{position, Vector3(size.x, 1.0f, size.y), color, borderColor, BorderSize(size, borderSize)});
}

void RectRenderer::Border(const Vector2& position, const Vector2& size, const Color& color, const float borderSize) const
{
    _buffer2D->Add(RectData{Vector3(position.x, position.y, 0.0f), Vector3(size.x, size.y, 1.0f), Color{0.0f, 0.0f, 0.0f, 0.0f}, color, BorderSize(size, borderSize)});
}

void RectRenderer::Border(const Vector3& position, const Vector2& size, const Color& color, const float borderSize) const
{
    _buffer3D->Add(RectData{position, Vector3(size.x, 1.0f, size.y), Color{0.0f, 0.0f, 0.0f, 0.0f}, color, BorderSize(size, borderSize)});
}

void RectRenderer::Flush2D() const
{
    _material->SetMat4("viewProjectionMatrix", Renderer::Instance()->Get2DMatrix());
    _buffer2D->Flush();
}

void RectRenderer::Flush3D() const
{
    _material->SetMat4("viewProjectionMatrix", Renderer::Instance()->Get3DMatrix());
    _buffer3D->Flush();
}
