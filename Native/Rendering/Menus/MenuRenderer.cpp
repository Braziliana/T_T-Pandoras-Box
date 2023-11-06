#include "MenuRenderer.h"
#include "../Renderer.h"

MenuRenderer* MenuRenderer::_instance = nullptr;
std::once_flag MenuRenderer::_initInstanceFlag;

MenuRenderer::MenuRenderer() = default;

MenuRenderer* MenuRenderer::GetInstance()
{
    std::call_once(_initInstanceFlag, []() {
        _instance = new MenuRenderer();
    });
    return _instance;
}

void MenuRenderer::Begin()
{
    if(_menuOffset == 0)
    {
        _nextPosition = _position;
        _prevPositionLayer.push(_nextPosition);
    }
    else
    {
        _prevPositionLayer.push(_nextPosition);
        _nextPosition.x += _itemSize.x - _borderWidth;
        _nextPosition.y -= _itemSize.y - _borderWidth;
    }
    _menuOffset++;
}

void MenuRenderer::End()
{
    _nextPosition = _prevPositionLayer.top();
    _prevPositionLayer.pop();
    _menuOffset--;
}

Vector2 MenuRenderer::DrawItem(const std::string& text)
{
    const auto position = _nextPosition;
    const auto renderer = Renderer::Instance();
    renderer->RectFilledBordered(position, _itemSize, _itemColor, _borderColor, _borderWidth);

    DrawItemText(text, position, TextHorizontalOffset::Left, TextVerticalOffset::Center);
    
    _nextPosition.y += _itemSize.y - _borderWidth;
    return position;
}

Vector2 MenuRenderer::DrawSubMenu(const std::string& text, bool opened)
{
    const auto position = DrawItem(text);

    DrawItemText(opened ? "<" : ">", position, TextHorizontalOffset::Right, TextVerticalOffset::Center);
    
    return position;
}

void MenuRenderer::DrawItemText(const std::string& text, const Vector2 position, const TextHorizontalOffset textHorizontalOffset,
                                const TextVerticalOffset textVerticalOffset) const
{
    const auto halfSize = _itemSize/2;
    const auto start = position - halfSize + Vector2(10, 10);
    const auto end = position + halfSize - Vector2(10, 10);
    Renderer::Instance()->Text(text, start, end, _fontSize, _textColor, textHorizontalOffset, textVerticalOffset);
}
