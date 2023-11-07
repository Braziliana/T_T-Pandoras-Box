#pragma once
#include <string>
#include "../Math/Rect.h"

#include "../../Input/InputManager.h"
class MenuItem
{
protected:
    std::string _title;
    Rect _rect;
    
public:
    
    static Vector2 ItemSize;
    static float BorderWidth;
    static float ElementSpacing;
    static float ElementSize;
    
    MenuItem(std::string title, const Rect rect) : _title(std::move(title)), _rect(rect) {  }
    virtual ~MenuItem() = default;

    bool Contains(const Vector2& position) const
    {
        return _rect.Contains(position);
    }

    const Rect& GetRect() const
    {
        return _rect;
    }

    const std::string& GetTitle()
    {
        return _title;
    }
    
    virtual void UpdatePosition(const Rect& rect)
    {
        _rect = rect;
    }

    virtual void Move(Vector2 position)
    {
        _rect.Move(position);
    }
    
    virtual void Render();
    
    virtual bool OnMouseMoveEvent(MouseMoveEvent mouseMoveEvent)
    {
        return false;
    }
    
    virtual bool OnKeyStateEvent(KeyStateEvent event)
    {
        return false;
    }

    Rect GetElementRect(const int itemIndex) const
    {
        return GetElementRect(_rect, itemIndex);
    }

    static Rect GetElementRect(const Rect& rect, const int itemIndex)
    {
        const auto itemsRect = rect.Padding(BorderWidth + 3);
        float x = itemsRect.x + itemsRect.width;
        x -= (ElementSpacing + BorderWidth) + (BorderWidth + ElementSize) * static_cast<float>(itemIndex + 1);

        return {x, itemsRect.y, ElementSize, ElementSize};
    }
    
    Rect GetMenuSlotRect(const int slot) const
    {
        return GetMenuSlotRect(_rect, slot);
    }

    static Rect GetMenuSlotRect(Rect rect, const int slot)
    {
        return {rect.x, rect.y + (ItemSize.y - BorderWidth) * static_cast<float>(slot), ItemSize.x, ItemSize.y};
    }
};
