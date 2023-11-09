#include "MenuBase.h"
#include "../Rendering/Renderer.h"
#include "SubMenu.h"
#include "Toggle.h"
#include "FloatSlider.h"

Rect MenuBase::GetChildRect(float slots) const
{
    float extra = 0.0f;
    if(slots>1)
    {
        extra = (slots-1)*DefaultMenuStyle.Border;
    }
    return {_nextChildPosition.x, _nextChildPosition.y, DefaultMenuStyle.ItemSize.x, DefaultMenuStyle.ItemSize.y * slots - extra};
}

void MenuBase::UpdateNextChildPosition()
{
    const auto rect = _items.back()->GetRect();
    _nextChildPosition.y = rect.y + rect.height - DefaultMenuStyle.Border;
}

void MenuBase::DrawHeader() const
{
    const auto renderer = Renderer::Instance();
    renderer->RectFilledBordered(_headerRect.Center(), _headerRect.Size(), DefaultMenuStyle.ItemColor, DefaultMenuStyle.BorderColor, DefaultMenuStyle.Border);
    const auto itemsRect = _headerRect.Padding(DefaultMenuStyle.ContentPadding);
    renderer->Text(_title, itemsRect.GetStart(), itemsRect.GetEnd(), DefaultMenuStyle.FontSize, DefaultMenuStyle.TextColor, TextHorizontalOffset::Center, TextVerticalOffset::Center);
}

void MenuBase::Render()
{
    if(!_open)
    {
        return;
    }

    DrawHeader();
    
    for(const auto item : _items)
    {
        item->Render();
    }
}

SubMenu* MenuBase::AddSubMenu(const std::string& title)
{
    const auto item = new SubMenu(title, GetChildRect(1));
    AddItem(item);
    return item;
}

Toggle* MenuBase::AddToggle(const std::string& title, bool toggled)
{
    const auto item = new Toggle(title, GetChildRect(1), toggled);
    AddItem(item);
    return item;
}

FloatSlider* MenuBase::AddFloatSlider(const std::string& title, const float value, const float minValue,
    const float maxValue, const float step, const int precision)
{
    const auto item = new FloatSlider(title, GetChildRect(2), value, minValue, maxValue, step, precision);
    AddItem(item);
    return item;
}
