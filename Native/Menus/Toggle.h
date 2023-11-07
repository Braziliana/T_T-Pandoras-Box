#pragma once
#include "MenuItem.h"
#include "../Rendering/Menus/MenuRenderer.h"

class Toggle : public MenuItem
{
private:
    bool _toggled;
public:
    Toggle(const std::string& title, const Rect& rect, bool toggled)
        : MenuItem(title, rect), _toggled(toggled)
    {
    }

    bool OnKeyStateEvent(KeyStateEvent event) override
    {
        const auto togglePosition = GetElementRect(0);
        if(event.key == VK_LBUTTON && event.isDown && togglePosition.Contains(InputManager::GetInstance()->GetMousePosition()))
        {
            _toggled = !_toggled;
            return true;
        }
        
        return false;
    }

    void Render() override
    {
        MenuRenderer::GetInstance()->DrawToggle(_rect, _title, _toggled);
    }
};
