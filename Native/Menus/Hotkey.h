#pragma once
#include "MenuItem.h"
#include "HotkeyType.h"

class Hotkey : public MenuItem
{
private:
    unsigned int _hotkey;
    HotkeyType _hotkeyType;
    bool _toggled;
    
public:
    Hotkey(const std::string& title, const Rect& rect, unsigned int hotkey, HotkeyType hotkeyType, bool toggled);
    bool* GetToggledPointer();
    void Render() override;
    bool OnKeyStateEvent(KeyStateEvent event) override;
};

extern "C" {
    __declspec(dllexport) bool* HotkeyGetToggledPointer(Hotkey* instance);
}