#include "TextRenderer.h"

TextRenderer::TextRenderer()
{
    if(LoadFont("default", "Resources/Fonts/Roboto-Regular.ttf"))
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

float TextRenderer::GetFontScale(const float size) const
{
    return size / static_cast<float>(_fontSize);
}

Vector2 TextRenderer::CalculatePosition(const std::string& text, const Vector2& position, float scaleFactor, TextHorizontalOffset textHorizontalOffset, TextVerticalOffset textVerticalOffset) const
{
    const Vector2 textSize = _default->GetTextSize(text, scaleFactor);
    const Vector2 textHalfSize = textSize / 2.0f;
    Vector2 finalPosition = position;

    switch (textHorizontalOffset)
    {
    case TextHorizontalOffset::Center:
        finalPosition.x -= textHalfSize.x;
        break;
    case TextHorizontalOffset::Right:
        finalPosition.x -= textSize.x;
        break;
    case TextHorizontalOffset::Left:
    case TextHorizontalOffset::None:
        // No adjustment needed
        break;
    }

    // Adjust for vertical offset
    switch (textVerticalOffset)
    {
    case TextVerticalOffset::Center:
        finalPosition.y += textHalfSize.y/2;
        break;
    case TextVerticalOffset::Top:
        finalPosition.y -= textSize.y;
        break;
    case TextVerticalOffset::Bottom:
    case TextVerticalOffset::None:
        // No adjustment needed
        break;
    }
    
    return finalPosition;
}

Vector2 TextRenderer::CalculatePosition(const std::string& text, const Vector2& start, const Vector2& end, float scaleFactor, TextHorizontalOffset textHorizontalOffset, TextVerticalOffset textVerticalOffset) const
{
    Vector2 textSize = _default->GetTextSize(text, scaleFactor);
    Vector2 textHalfSize = textSize / 2.0f;
    Vector2 midpoint = (start + end) / 2.0f;
    Vector2 finalPosition = midpoint;

    switch (textHorizontalOffset) {
    case TextHorizontalOffset::Center:
        finalPosition.x -= textHalfSize.x;
        break;
    case TextHorizontalOffset::Right:
        finalPosition.x = end.x - textSize.x;
        break;
    case TextHorizontalOffset::Left:
        finalPosition.x = start.x;
        break;
    case TextHorizontalOffset::None:
        break;
    }

    switch (textVerticalOffset) {
    case TextVerticalOffset::Center:
        finalPosition.y += textHalfSize.y / 2; //position.y + textSize.y/2
        break;
    case TextVerticalOffset::Top:
        finalPosition.y = start.y - textSize.y;
        break;
    case TextVerticalOffset::Bottom:
        finalPosition.y = end.y;
        break;
    case TextVerticalOffset::None:
        break;
    }

    return finalPosition;
}

void TextRenderer::Draw(const std::string& text, const Vector2& position, const float size, const Color& color, TextHorizontalOffset textHorizontalOffset, TextVerticalOffset textVerticalOffset) const
{
    const float scaleFactor = GetFontScale(size);
    _default->Draw(text, CalculatePosition(text, position, scaleFactor, textHorizontalOffset, textVerticalOffset), scaleFactor, color);
}

void TextRenderer::Draw(const std::string& text, const Vector2& start, const Vector2& end, float size,
    const Color& color, TextHorizontalOffset textHorizontalOffset, TextVerticalOffset textVerticalOffset) const
{
    const float scaleFactor = GetFontScale(size);
    _default->Draw(text, CalculatePosition(text, start, end, scaleFactor, textHorizontalOffset, textVerticalOffset), scaleFactor, color);
}

void TextRenderer::Draw(const std::string& text, const Vector3& position, const float size, const Color& color, TextHorizontalOffset textHorizontalOffset, TextVerticalOffset textVerticalOffset) const
{
    const float scaleFactor = GetFontScale(size);
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
