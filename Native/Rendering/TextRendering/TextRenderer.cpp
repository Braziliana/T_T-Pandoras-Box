#include "TextRenderer.h"

TextRenderer::TextRenderer()
{
    if(LoadFont("default", "Resources/Fonts/OpenSans-Regular.ttf"))
    {
        _default = _fontRenderers["default"];
    }
}

bool TextRenderer::LoadFont(const std::string& name, const std::string& fontFilePath)
{
    FT_Library ft;
    if (FT_Init_FreeType(&ft)) {
        std::cout << "ERROR::FREETYPE: Could not init FreeType Library\n" << std::endl;
        return false;
    }

    FT_Face face;
    if (FT_New_Face(ft, fontFilePath.c_str(), 0, &face)) {
        std::cout << "ERROR::FREETYPE: Failed to load font\n" << std::endl;
        return false;
    }

    FT_Set_Pixel_Sizes(face, 0, _fontSize);
    const auto font = new Font(face);
    const auto fontRenderer = new FontRenderer(font);
    _fontRenderers[name] = fontRenderer;
       
    FT_Done_Face(face);
    FT_Done_FreeType(ft);
        
    return  true;
}

void TextRenderer::Render(const std::string& text, const Vector2& position, float size, const Color& color) const
{
    const float scaleFactor = size / static_cast<float>(_fontSize);
    _default->Draw(text, position, scaleFactor, color);
}

void TextRenderer::Release()
{
    _default = nullptr;
    for (const auto& font : _fontRenderers)
    {
        font.second->Release();
        delete font.second;
    }
    _fontRenderers.clear();
}

void TextRenderer::Flush2D() const
{
    _default->Flush2D();
}

void TextRenderer::Flush3D() const
{
    _default->Flush3D();
}
