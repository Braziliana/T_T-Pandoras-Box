#include "MenuBase.h"
#include "../Rendering/Menus/MenuRenderer.h"
#include "SubMenu.h"
#include "Toggle.h"

Rect MenuBase::GetChildRect() const
{
    return {_rect.x + ItemSize.x - BorderWidth, _rect.y + static_cast<float>(_items.size()) * (ItemSize.y - BorderWidth), ItemSize.x, ItemSize.y};
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
    const auto item = new SubMenu(title, GetChildRect());
    AddItem(item);
    return item;
}

Toggle* MenuBase::AddToggle(const std::string& title, bool toggled)
{
    const auto item = new Toggle(title, GetChildRect(), toggled);
    AddItem(item);
    return item;
}
