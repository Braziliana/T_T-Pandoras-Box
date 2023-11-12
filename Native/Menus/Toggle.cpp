#include "Toggle.h"

#include "../Rendering/Renderer.h"

bool* ToggleGetToggledPointer(Toggle* togglePointer)
{
    return togglePointer->GetToggledPointer();
}

Toggle::Toggle(MenuItem* parent, const std::string& title, const Rect& rect, const bool toggled): MenuItem(parent, MenuItemType::Toggle, title, rect), _toggled(toggled)
{
    _toggleElement =  DefaultMenuStyle.GetElementRect(_rect, 0);
}

bool Toggle::OnKeyStateEvent(const KeyStateEvent event)
{
    if(event.key == VK_LBUTTON && event.isDown && _toggleElement.Contains(InputManager::GetInstance()->GetMousePosition()))
    {
        _toggled = !_toggled;
        return true;
    }
        
    return false;
}

void Toggle::Render()
{
    const auto renderer = Renderer::Instance();
    MenuItem::Render();
    
    renderer->RectFilledBordered(_toggleElement.Center(), _toggleElement.Size(), _toggled ? DefaultMenuStyle.BorderColor : DefaultMenuStyle.ItemColor, DefaultMenuStyle.BorderColor, DefaultMenuStyle.ElementBorder);
}

bool* Toggle::GetToggledPointer()
{
    return &_toggled;
}

void Toggle::Move(const Vector2& position)
{
    MenuItem::Move(position);
    _toggleElement =  DefaultMenuStyle.GetElementRect(_rect, 0);
}

void Toggle::UpdatePosition(const Rect& rect)
{
    MenuItem::UpdatePosition(rect);
    _toggleElement =  DefaultMenuStyle.GetElementRect(_rect, 0);
}
