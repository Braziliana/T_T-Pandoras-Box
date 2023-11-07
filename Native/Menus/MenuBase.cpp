#include "MenuBase.h"
#include "../Rendering/Menus/MenuRenderer.h"
#include "SubMenu.h"
#include "Toggle.h"
#include "FloatSlider.h"

Rect MenuBase::GetChildRect(float slots) const
{
    float extra = 0.0f;
    if(slots>1)
    {
        extra = (slots-1)*BorderWidth;
    }
    return {_nextChildPosition.x, _nextChildPosition.y, ItemSize.x, ItemSize.y * slots - extra};
}

void MenuBase::UpdateNextChildPosition()
{
    auto rect = _items.back()->GetRect();
    _nextChildPosition.y = rect.y + rect.height - BorderWidth;
}

void MenuBase::Render()
{
    const auto mr = MenuRenderer::GetInstance();

    mr->DrawSubMenu(_rect, _title, _open);
    
    if(!_open)
    {
        return;
    }
    
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
