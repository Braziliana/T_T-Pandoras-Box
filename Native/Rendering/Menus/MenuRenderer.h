#pragma once
#include <mutex>
#include <string>
#include <stack>

#include "../TextRendering/TextRenderer.h"
#include "../../Math/Color.h"
#include "../../Math/Vector2.h"
#include "../../Math/Rect.h"

class MenuRenderer
{
private:

    static MenuRenderer* _instance;
    static std::once_flag _initInstanceFlag;
    
    float _fontSize = 16;
    Color _itemColor = Color::FromByte(255, 108, 34);
    Color _textColor = Color::FromByte(255, 255, 255);
    Color _borderColor = Color::FromByte(43, 52, 153);
    
    MenuRenderer();
public:
    
    MenuRenderer(MenuRenderer const&) = delete;
    void operator=(MenuRenderer const&) = delete;
    static MenuRenderer* GetInstance();
    
    void DrawItem(const Rect& rect, const std::string& text) const;
    void DrawSubMenu(const Rect& rect, const std::string& text, bool opened) const;
    void DrawToggle(const Rect& rect, const std::string& text, bool toggled) const;

    void DrawItemText(const std::string& text, const Rect& rect, TextHorizontalOffset textHorizontalOffset, TextVerticalOffset textVerticalOffset) const;
};
