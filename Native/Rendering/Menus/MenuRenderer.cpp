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

void MenuRenderer::DrawItem(const std::string& text)
{
    const auto renderer = Renderer::Instance();
    renderer->RectFilledBordered(_nextPosition, _itemSize, _itemColor, _borderColor, _borderWidth);

    auto textPosition = _nextPosition;
    textPosition.x -= _itemSize.x / 2 - 10;
    textPosition.y += _itemSize.y / 2 - _fontSize / 2;
    renderer->Text(text, textPosition, _fontSize, _textColor);
    _nextPosition.y += _itemSize.y - _borderWidth;
}
