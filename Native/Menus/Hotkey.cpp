#include "Hotkey.h"
#include "../../Rendering/Renderer.h"

bool* HotkeyGetToggledPointer(Hotkey* instance)
{
    return instance->GetToggledPointer();
}

HotkeyType* HotkeyGetHotkeyTypePointer(Hotkey* instance)
{
    return instance->GetHotkeyTypePointer();
}

unsigned short* HotkeyGetHotkeyPointer(Hotkey* instance)
{
    return instance->GetHotkeyPointer();
}


Hotkey::Hotkey(const std::string& title, const Rect& rect, const unsigned short hotkey, const HotkeyType hotkeyType,
    bool toggled): MenuItem(MenuItemType::Hotkey, title, rect), _hotkey(hotkey), _hotkeyType(hotkeyType), _toggled(toggled)
{
}

bool* Hotkey::GetToggledPointer()
{
    return &_toggled;
}

HotkeyType* Hotkey::GetHotkeyTypePointer()
{
    return &_hotkeyType;
}

unsigned short* Hotkey::GetHotkeyPointer()
{
    return &_hotkey;
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