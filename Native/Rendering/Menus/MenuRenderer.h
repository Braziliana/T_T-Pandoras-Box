#pragma once
#include <mutex>
#include <string>
#include <stack>

#include "../TextRendering/TextRenderer.h"
#include "../../Math/Color.h"
#include "../../Math/Vector2.h"

class MenuRenderer
{
private:

    static MenuRenderer* _instance;
    static std::once_flag _initInstanceFlag;

    int _menuOffset = 0;
    float _fontSize = 16;
    float _borderWidth = 3;
    Vector2 _itemSize = Vector2(300, 30);
    Vector2 _position = Vector2(_itemSize.x/2 + 20, _itemSize.y/2 + 20);
    Vector2 _nextPosition = _position;
    std::stack<Vector2> _prevPositionLayer;
    Color _itemColor = Color::FromByte(255, 108, 34);
    Color _textColor = Color::FromByte(255, 255, 255);
    Color _borderColor = Color::FromByte(43, 52, 153);
    
    MenuRenderer();
public:
    
    MenuRenderer(MenuRenderer const&) = delete;
    void operator=(MenuRenderer const&) = delete;
    static MenuRenderer* GetInstance();
    
    void Begin();
    void End();
    Vector2 DrawItem(const std::string& text);
    Vector2 DrawSubMenu(const std::string& text, bool opened);

    void DrawItemText(const std::string& text, Vector2 position, TextHorizontalOffset textHorizontalOffset, TextVerticalOffset textVerticalOffset) const;
};
