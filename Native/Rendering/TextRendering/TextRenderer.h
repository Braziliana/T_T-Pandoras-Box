#pragma once
#include <string>
#include <map>

#include "Font.h"
#include "FontRenderer.h"

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
    void Draw(const std::string& text, const Vector2& position, float size, const Color& color) const;
    void Draw(const std::string& text, const Vector3& position, float size, const Color& color) const;
    void DrawCenter(const std::string& text, const Vector2& position, float size, const Color& color) const;
    void DrawCenter(const std::string& text, const Vector3& position, float size, const Color& color) const;
    void Release();
    void Flush2D() const;
    void Flush3D() const;
};
