#pragma once
#include "MenuItem.h"
#include "HotkeyType.h"

class Hotkey : public MenuItem
{
private:
    unsigned short _hotkey;
    HotkeyType _hotkeyType;
    bool _toggled;
    
public:
    Hotkey(const std::string& title, const Rect& rect, unsigned short hotkey, HotkeyType hotkeyType, bool toggled);
    bool* GetToggledPointer();
    HotkeyType* GetHotkeyTypePointer();
    unsigned short* GetHotkeyPointer();
    void Render() override;
    bool OnKeyStateEvent(KeyStateEvent event) override;
};

extern "C" {
    __declspec(dllexport) bool* HotkeyGetToggledPointer(Hotkey* instance);
    __declspec(dllexport) HotkeyType* HotkeyGetHotkeyTypePointer(Hotkey* instance);
    __declspec(dllexport) unsigned short* HotkeyGetHotkeyPointer(Hotkey* instance);
}