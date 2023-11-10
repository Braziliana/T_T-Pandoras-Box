#pragma once
#include "MenuItem.h"

class Toggle : public MenuItem
{
protected:
    bool _toggled;
    Rect _toggleElement;
    
public:
    Toggle(const std::string& title, const Rect& rect, const bool toggled);

    bool OnKeyStateEvent(KeyStateEvent event) override;
    void Render() override;
    bool* GetToggledPointer();
    void Move(const Vector2& position) override;
    void UpdatePosition(const Rect& rect) override;
};

extern "C" {
    __declspec(dllexport) bool* ToggleGetToggledPointer(Toggle* togglePointer);
}