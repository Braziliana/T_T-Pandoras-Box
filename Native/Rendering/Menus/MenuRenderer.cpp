#include "MenuRenderer.h"
#include "../Renderer.h"
#include "../../Input/InputManager.h"
#include "../../Menus/MenuItem.h"

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

void MenuRenderer::DrawItem(const Rect& rect, const std::string& text) const
{
    const auto renderer = Renderer::Instance();
    renderer->RectFilledBordered(rect.Center(), rect.Size(), _itemColor, _borderColor, MenuItem::BorderWidth);
    const auto itemsRect = rect.Padding(MenuItem::BorderWidth + 3, 0);
    DrawItemText(text, itemsRect, TextHorizontalOffset::Left, TextVerticalOffset::Center);
}

void MenuRenderer::DrawSubMenu(const Rect& rect, const std::string& text, const bool opened) const
{
    DrawItem(rect, text);
    const auto elementPosition = MenuItem::GetElementRect(rect, 0);
    DrawItemText(opened ? "<" : ">", elementPosition, TextHorizontalOffset::Center, TextVerticalOffset::Center);
}

void MenuRenderer::DrawToggle(const Rect& rect, const std::string& text, const bool toggled) const
{
    const auto renderer = Renderer::Instance();
    DrawItem(rect, text);

    const auto elementPosition = MenuItem::GetElementRect(rect, 0);
    renderer->RectFilledBordered(elementPosition.Center(), elementPosition.Size(), toggled ? _borderColor : _itemColor, _borderColor, MenuItem::BorderWidth);
}

void MenuRenderer::DrawItemText(const std::string& text, const Rect& rect, const TextHorizontalOffset textHorizontalOffset,
                                const TextVerticalOffset textVerticalOffset) const
{
    Renderer::Instance()->Text(text, rect.GetStart(), rect.GetEnd(), _fontSize, _textColor, textHorizontalOffset, textVerticalOffset);
}