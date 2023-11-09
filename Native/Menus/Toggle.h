#pragma once
#include "MenuItem.h"

class Toggle : public MenuItem
{
protected:
    bool _toggled;
    Rect _toggleElement;
    
public:
    Toggle(const std::string& title, const Rect& rect, const bool toggled)
        : MenuItem(title, rect), _toggled(toggled)
    {
        _toggleElement =  DefaultMenuStyle.GetElementRect(_rect, 0);
    }

    bool OnKeyStateEvent(const KeyStateEvent event) override
    {
        if(event.key == VK_LBUTTON && event.isDown && _toggleElement.Contains(InputManager::GetInstance()->GetMousePosition()))
        {
            _toggled = !_toggled;
            return true;
        }
        
        return false;
    }

    void Render() override;

    void Move(const Vector2& position) override
    {
        MenuItem::Move(position);
        _toggleElement =  DefaultMenuStyle.GetElementRect(_rect, 0);
    }

    void UpdatePosition(const Rect& rect) override
    {
        MenuItem::UpdatePosition(rect);
        _toggleElement =  DefaultMenuStyle.GetElementRect(_rect, 0);
    }
};
