#pragma once
#include <string>
#include "../Math/Rect.h"

#include "../../Input/InputManager.h"
#include "MenuStyle.h"

class MenuItem
{
protected:
    std::string _title;
    Rect _rect;
    
public:
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

    virtual void Move(const Vector2& position)
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
};
