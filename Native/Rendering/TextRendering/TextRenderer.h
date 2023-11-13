#pragma once
#include <string>
#include <map>

#include "Font.h"
#include "FontRenderer.h"

enum class TextVerticalOffset
{
    None,
    Top,
    Center,
    Bottom
};

enum class TextHorizontalOffset
{
    None,
    Left,
    Center,
    Right
};

class TextRenderer
{
private:
    FontRenderer* _default; 
    std::map<std::string, FontRenderer*> _fontRenderers;
    int _fontSize = 21;
    
public:
    TextRenderer();
    bool LoadFont(const std::string& name, const std::string& fontFilePath);
    float GetFontScale(float size) const;
    Vector2 CalculatePosition(const std::string& text, const Vector2& position, float scaleFactor, TextHorizontalOffset textHorizontalOffset, TextVerticalOffset textVerticalOffset) const;
    Vector2 CalculatePosition(const std::string& text, const Vector2& start, const Vector2& end, float scaleFactor, TextHorizontalOffset textHorizontalOffset, TextVerticalOffset textVerticalOffset) const;
    void Draw(const std::string& text, const Vector2& position, float size, const Color& color, TextHorizontalOffset textHorizontalOffset, TextVerticalOffset textVerticalOffset) const;
    void Draw(const std::string& text, const Vector2& start, const Vector2& end, float size, const Color& color, TextHorizontalOffset textHorizontalOffset, TextVerticalOffset textVerticalOffset) const;
    void Draw(const std::string& text, const Vector3& position, float size, const Color& color, TextHorizontalOffset textHorizontalOffset, TextVerticalOffset textVerticalOffset) const;
    void Release();
    void Flush2D() const;
    void Flush3D() const;
};
