#include "FontRenderer.h"


FontRenderer::FontRenderer(Font* font)
{
    _font = font;
    const std::vector<VertexAttribute> vertexAttributes = {
        {0, 3, GL_FLOAT, GL_FALSE, sizeof(VertexPositionUv), reinterpret_cast<void*>(offsetof(VertexPositionUv, position)), 0},
        {1, 2, GL_FLOAT, GL_FALSE, sizeof(VertexPositionUv), reinterpret_cast<void*>(offsetof(VertexPositionUv, uv)), 0},
    };

    const std::vector<VertexAttribute> instanceAttributes = {
        {2, 3, GL_FLOAT, GL_FALSE, sizeof(FontCharacterInstance), reinterpret_cast<void*>(offsetof(FontCharacterInstance, position)), 1},
        {3, 3, GL_FLOAT, GL_FALSE, sizeof(FontCharacterInstance), reinterpret_cast<void*>(offsetof(FontCharacterInstance, scale)), 1},
        {4, 2, GL_FLOAT, GL_FALSE, sizeof(FontCharacterInstance), reinterpret_cast<void*>(offsetof(FontCharacterInstance, uv)), 1},
        {5, 2, GL_FLOAT, GL_FALSE, sizeof(FontCharacterInstance), reinterpret_cast<void*>(offsetof(FontCharacterInstance, uvSize)), 1},
        {6, 4, GL_FLOAT, GL_FALSE, sizeof(FontCharacterInstance), reinterpret_cast<void*>(offsetof(FontCharacterInstance, color)), 1},
    };
    
    constexpr float size = 1.0f;

    std::vector<VertexPositionUv> verts2D;
    // First Triangle
    verts2D.push_back({{ 0.0f, 0.0f, 0.0f }, { 0.0f, 0.0f }});
    verts2D.push_back({{ size, 0.0f, 0.0f }, { 1.0f, 0.0f }});
    verts2D.push_back({{ 0.0f, size, 0.0f }, { 0.0f, 1.0f }});

    // Second Triangle
    verts2D.push_back({{ size, 0.0f, 0.0f }, { 1.0f, 0.0f }});
    verts2D.push_back({{ 0.0f, size, 0.0f }, { 0.0f, 1.0f }});
    verts2D.push_back({{ size, size, 0.0f }, { 1.0f, 1.0f }});

    std::vector<VertexPositionUv> verts3D;
    // First Triangle
    verts3D.push_back({{ 0.0f, 0.0f, 0.0f }, { 0.0f, 0.0f }});
    verts3D.push_back({{ size, 0.0f, 0.0f }, { 1.0f, 0.0f }});
    verts3D.push_back({{ 0.0f, 0.0f, size }, { 0.0f, 1.0f }});

    // Second Triangle
    verts3D.push_back({{ size, 0.0f, 0.0f }, { 1.0f, 0.0f }});
    verts3D.push_back({{ 0.0f, 0.0f, size }, { 0.0f, 1.0f }});
    verts3D.push_back({{ size, 0.0f, size }, { 1.0f, 1.0f }});
    
    const auto shader = ShaderManager::GetInstance().CreateShader(L"Font");
    _fontMaterial = new TexturedMaterial(_font->GetFontTexture(), shader);
    _buffer2D = new InstancedBuffer<VertexPositionUv, FontCharacterInstance>(verts2D, 1000, vertexAttributes, instanceAttributes, reinterpret_cast<Material*>(_fontMaterial));
    _buffer3D = new InstancedBuffer<VertexPositionUv, FontCharacterInstance>(verts3D, 1000, vertexAttributes, instanceAttributes, reinterpret_cast<Material*>(_fontMaterial));
}

Vector2 FontRenderer::GetTextSize(const std::string& text, const float size) const
{
    return _font->GetTextSize(text, size);
}

void FontRenderer::Draw(const std::string& text, const Vector2& position, const float& scale, const Color& color) const
{
    Vector2 characterPosition = position;
    for(const auto& c : text)
    {
        auto instanceData = _font->GetInstance(c, characterPosition, scale, color);
        _buffer2D->Add(instanceData);
    }
}

void FontRenderer::Draw(const std::string& text, const Vector3& position, const float& scale, const Color& color) const
{
    Vector3 characterPosition = position;
    for(const auto& c : text)
    {
        auto instanceData = _font->GetInstance(c, characterPosition, scale, color);
        _buffer3D->Add(instanceData);
    }
}

void FontRenderer::Flush2D() const
{
    _buffer2D->Flush();
}

void FontRenderer::Flush3D() const
{
    _buffer3D->Flush();
}

void FontRenderer::Release()
{
    if(_fontMaterial != nullptr)
    {
        _fontMaterial->Release();
        delete _fontMaterial;
        _fontMaterial = nullptr;
    }
}
