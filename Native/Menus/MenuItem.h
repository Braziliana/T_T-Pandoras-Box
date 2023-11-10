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

    bool Contains(const Vector2& position) const;
    const Rect& GetRect() const;
    void SetTitle(const std::string& title);
    const std::string& GetTitle();
    virtual void UpdatePosition(const Rect& rect);
    virtual void Move(const Vector2& position);
    virtual void Render();
    virtual bool OnMouseMoveEvent(MouseMoveEvent mouseMoveEvent);
    virtual bool OnKeyStateEvent(KeyStateEvent event);
};

extern "C" {
    __declspec(dllexport) void MenuItemSetTitle(MenuItem* instance, const char* title);
}
