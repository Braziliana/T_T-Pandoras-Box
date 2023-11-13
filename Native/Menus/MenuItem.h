#pragma once
#include <string>
#include "../Math/Rect.h"

#include "../../Input/InputManager.h"
#include "MenuStyle.h"

enum class MenuItemType
{
    Menu,
    SubMenu,
    Toggle,
    Hotkey,
    FloatSlider,
    ComboBox,
    HotkeySelector,
    SelectionItem
};

class MenuItem
{
protected:
    MenuItem* _parent;
    MenuItemType _menuItemType;
    std::string _title;
    Rect _rect;
    
public:
    
    MenuItem(MenuItem* parent, MenuItemType menuItemType, std::string title, Rect rect);
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
    MenuItemType GetType() const;

    virtual void Close();
    virtual void Open();
};

extern "C" {
    __declspec(dllexport) void MenuItemSetTitle(MenuItem* instance, const char* title);
    __declspec(dllexport) void MenuItemRender(MenuItem* instance);
}
