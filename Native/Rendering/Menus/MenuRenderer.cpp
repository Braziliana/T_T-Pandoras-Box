#include "MenuRenderer.h"
#include "../Renderer.h"
#include "../../Input/InputManager.h"
#include "../../Menus/MenuItem.h"
#include <sstream>
#include <iomanip>

#include "../../Menus/FloatSlider.h"

std::string FloatString(const std::string& str, const float value, const int precision) {
    std::ostringstream out;
    out << str << ' ' << std::fixed << std::setprecision(precision) << value;
    return out.str();
}

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
    renderer->RectFilledBordered(elementPosition.Center(), elementPosition.Size(), toggled ? _borderColor : _itemColor, _borderColor, MenuItem::BorderWidth -1);
}

void MenuRenderer::DrawFloatSlider(const Rect& rect, const std::string& text, const float value, const float minValue,
    const float maxValue, const float step, const int precision) const
{
    const auto renderer = Renderer::Instance();
    auto topSlot = MenuItem::GetMenuSlotRect(rect, 0);
    auto bottomSlot = MenuItem::GetMenuSlotRect(rect, 1);

    renderer->RectFilledBordered(rect.Center(), rect.Size(), _itemColor, _borderColor, MenuItem::BorderWidth);
    auto itemsRect = topSlot.Padding(MenuItem::BorderWidth + 3, 0);
    DrawItemText(FloatString(text, value, precision), itemsRect, TextHorizontalOffset::Left, TextVerticalOffset::Center);
    
    auto elementPosition = MenuItem::GetElementRect(topSlot, 0);
    renderer->RectBorder(elementPosition.Center(), elementPosition.Size(), _borderColor, MenuItem::BorderWidth-1);
    //DrawItemText("-", elementPosition, TextHorizontalOffset::Center, TextVerticalOffset::Center);
    renderer->Text("-", elementPosition.Center() + Vector2(0, 6),  _fontSize + 3, _textColor, TextHorizontalOffset::Center, TextVerticalOffset::None);
    
    elementPosition = MenuItem::GetElementRect(topSlot, 1);
    renderer->RectBorder(elementPosition.Center(), elementPosition.Size(), _borderColor, MenuItem::BorderWidth-1);
    DrawItemText("+", elementPosition, TextHorizontalOffset::Center, TextVerticalOffset::Center);
    
    itemsRect = FloatSlider::GetSlideAreaRect(rect);
    renderer->RectFilledBordered(itemsRect.Center(), Vector2(itemsRect.width, 5), _borderColor, _borderColor, MenuItem::BorderWidth);

    const auto knobRect = FloatSlider::GetKnobRect(itemsRect, value, minValue, maxValue);
    renderer->RectFilledBordered(knobRect.Center(), knobRect.Size(), _borderColor, _borderColor, MenuItem::BorderWidth);
}

void MenuRenderer::DrawItemText(const std::string& text, const Rect& rect, const TextHorizontalOffset textHorizontalOffset,
                                const TextVerticalOffset textVerticalOffset) const
{
    Renderer::Instance()->Text(text, rect.GetStart(), rect.GetEnd(), _fontSize, _textColor, textHorizontalOffset, textVerticalOffset);
}
