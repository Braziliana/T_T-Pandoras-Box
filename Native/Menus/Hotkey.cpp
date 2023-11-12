#include "Hotkey.h"
#include "../../Rendering/Renderer.h"

bool* HotkeyGetToggledPointer(Hotkey* instance)
{
    return instance->GetToggledPointer();
}

Hotkey::Hotkey(const std::string& title, const Rect& rect, const unsigned hotkey, const HotkeyType hotkeyType,
    bool toggled): MenuItem(MenuItemType::Hotkey, title, rect), _hotkey(hotkey), _hotkeyType(hotkeyType), _toggled(toggled)
{
}

bool* Hotkey::GetToggledPointer()
{
    return &_toggled;
}

void Hotkey::Render()
{
    auto renderer = Renderer::Instance();

    
}

bool Hotkey::OnKeyStateEvent(const KeyStateEvent event)
{
    if(event.key != _hotkey)
    {
        return false;
    }
    
    switch (_hotkeyType)
    {
    case HotkeyType::Press:
        _toggled = event.isDown;
        break;
    case HotkeyType::Toggle:
        if(!event.isDown)
        {
            _toggled = !_toggled;
        }
        break;
    }
    
    return false;
}
