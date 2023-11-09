#pragma once
#include "MenuItem.h"

enum class HotkeyType
{
    Press,
    Toggle,
};

class Hotkey : MenuItem
{
private:
    unsigned int _hotkey;
    HotkeyType _hotkeyType;
    bool _toggled;
public:
    Hotkey(const std::string& title, const Rect& rect, const unsigned int hotkey, const HotkeyType hotkeyType, bool toggled)
        : MenuItem(title, rect), _hotkey(hotkey), _hotkeyType(hotkeyType), _toggled(toggled)
    {
    }

    bool* GetToggledPointer()
    {
        return &_toggled;
    }

    void Render() override;
};
