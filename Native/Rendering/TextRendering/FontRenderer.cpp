#include "FontRenderer.h"

#include "../../Materials/MaterialManager.h"
#include "../../Materials/TexturedMaterial.h"


FontRenderer::FontRenderer(Font* font)
{

    _font = font;
    _textureId = 0;
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
    
    std::vector<VertexPositionUv> verts2D;
    constexpr float size = 1.0f;

    // First Triangle
    verts2D.push_back({{ 0.0f, 0.0f, 0.0f }, { 0.0f, 0.0f }});
    verts2D.push_back({{ size, 0.0f, 0.0f }, { 1.0f, 0.0f }});
    verts2D.push_back({{ 0.0f, size, 0.0f }, { 0.0f, 1.0f }});

    // Second Triangle
    verts2D.push_back({{ size, 0.0f, 0.0f }, { 1.0f, 0.0f }});
    verts2D.push_back({{ 0.0f, size, 0.0f }, { 0.0f, 1.0f }});
    verts2D.push_back({{ size, size, 0.0f }, { 1.0f, 1.0f }});
    
    auto shader = ShaderManager::GetInstance().CreateShader(L"Font");
    auto fontMaterial = new TexturedMaterial(_font->GetFontTexture(), shader);
    _buffer2D = new InstancedBuffer<VertexPositionUv, FontCharacterInstance>(verts2D, 1000, vertexAttributes, instanceAttributes, reinterpret_cast<Material*>(fontMaterial));
    _buffer3D = new InstancedBuffer<VertexPositionUv, FontCharacterInstance>(verts2D, 1000, vertexAttributes, instanceAttributes, reinterpret_cast<Material*>(fontMaterial));
}

void FontRenderer::Draw(std::string text, const Vector2& position, const float& scale, const Color& color)
{
    Vector2 characterPosition = position;
    for(const auto& c : text)
    {
        auto instanceData = _font->GetInstance(c, characterPosition, scale, color);
        _buffer2D->Add(instanceData);
    }
}

void FontRenderer::Draw(std::string text, const Vector3& position, const Vector3& scale, const Color& color)
{
    //_buffer3D->Add(FontCharacterInstance{position, Vector3(scale.x, 1.0f, scale.y), color});
}

void FontRenderer::Flush2D()
{
    _buffer2D->Flush();
}

void FontRenderer::Flush3D()
{
    _buffer3D->Flush();
}
