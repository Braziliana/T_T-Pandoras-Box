#include "MenuBase.h"
#include "../Rendering/Renderer.h"
#include "SubMenu.h"
#include "Toggle.h"
#include "FloatSlider.h"
#include "Hotkey.h"

SubMenu* MenuBaseAddSubMenu(MenuBase* instance, const char* title)
{
    return instance->AddSubMenu(std::string(title));
}

Toggle* MenuBaseAddToggle(MenuBase* instance, const char* title, const bool toggled)
{
    return instance->AddToggle(std::string(title), toggled);
}

FloatSlider* MenuBaseAddFloatSlider(MenuBase* instance, const char* title, const float value, const float minValue, const float maxValue,
                                    const float step, const int precision)
{
    return instance->AddFloatSlider(std::string(title), value, minValue, maxValue, step, precision);
}

Hotkey* MenuBaseAddHotkey(MenuBase* instance, const char* title, const unsigned short hotkey, const int hotkeyType, const bool toggled)
{
    return instance->AddHotkey(title, hotkey, static_cast<HotkeyType>(hotkeyType), toggled);
}


//----------------------

Rect MenuBase::GetChildRect(const float slots) const
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

MenuBase::MenuBase(MenuItemType menuItemType, const std::string& title, const Rect rect): MenuItem(menuItemType, title, rect)
{
    _headerRect = Rect(_rect.x + DefaultMenuStyle.ItemSize.x - DefaultMenuStyle.Border, rect.y, DefaultMenuStyle.ItemSize.x, DefaultMenuStyle.ItemSize.y);
    _nextChildPosition = Vector2(_headerRect.x, _headerRect.y + _headerRect.height - DefaultMenuStyle.Border);
}

MenuBase::~MenuBase()
{
    for (const MenuItem* item : _items) {
        delete item;
    }
    _items.clear();
    _menus.clear();
    _hotkeys.clear();
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

void MenuBase::UpdatePosition(const Rect& rect)
{
    const auto movePosition = Vector2(rect.x - _rect.x, rect.y - _rect.y);
    MenuItem::UpdatePosition(rect);
    _headerRect.Move(movePosition);
    for (const auto item : _items)
    {
        item->Move(movePosition);
    }
}

void MenuBase::Move(const Vector2& position)
{
    MenuItem::Move(position);
    _headerRect.Move(position);
    for (const auto item : _items)
    {
        item->Move(position);
    }
}

SubMenu* MenuBase::AddSubMenu(const std::string& title)
{
    const auto item = new SubMenu(title, GetChildRect(1));
    AddItem(item);
    _menus.push_back(item);
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

Hotkey* MenuBase::AddHotkey(const std::string& title, const unsigned short hotkey, const HotkeyType hotkeyType, bool toggled)
{
    const auto item = new Hotkey(title, GetChildRect(2), hotkey, hotkeyType, toggled);
    AddItem(item);
    _hotkeys.push_back(item);
    return item;
}

void MenuBase::AddItem(MenuItem* item)
{
    _items.push_back(item);
    UpdateNextChildPosition();
}

void MenuBase::RemoveItem(const MenuItem* itemToRemove)
{
    const auto it = std::find(_items.begin(), _items.end(), itemToRemove);
    if (it != _items.end()) {
        delete *it;
        _items.erase(it);
    }
}

bool MenuBase::OnMouseMoveEvent(const MouseMoveEvent event)
{
    if(!_open)
    {
        return false;
    }
        
    for (const auto item : _items)
    {
        if(item->OnMouseMoveEvent(event))
        {
            return true;
        }
    }
        
    return false;
}

bool MenuBase::OnKeyStateEvent(const KeyStateEvent event)
{
    if(event.isDown && event.key == VK_LBUTTON && _rect.Contains(InputManager::GetInstance()->GetMousePosition()))
    {
        _open = !_open;
        return true;
    }
        
    if(!_open)
    {
        return false;
    }

    for (const auto item : _items)
    {
        if(item->GetType() != MenuItemType::Hotkey && item->OnKeyStateEvent(event))
        {
            return true;
        }
    }
        
    return false;
}

void MenuBase::HandleHotkeys(const KeyStateEvent event) const
{
    for (const auto item : _hotkeys)
    {
        item->OnKeyStateEvent(event);
    }
}
