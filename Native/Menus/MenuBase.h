#pragma once
#include <vector>

#include "../../Input/InputManager.h"
#include "MenuItem.h"

class FloatSlider;
class Toggle;
class SubMenu;

class MenuBase : public MenuItem
{
protected:
    bool _open = false;
    std::vector<MenuItem*> _items;

    Vector2 _nextChildPosition;
    virtual Rect GetChildRect(float slots) const;
    virtual void UpdateNextChildPosition();
public:
    MenuBase(const std::string& title, const Rect rect) : MenuItem(title, rect)
    {
        _nextChildPosition = Vector2(_rect.x + ItemSize.x - BorderWidth, _rect.y);
    }
    
    MenuBase(const MenuBase&) = delete;
    MenuBase& operator=(const MenuBase&) = delete;
    MenuBase(MenuBase&&) = delete;
    MenuBase& operator=(MenuBase&&) = delete;
    
    ~MenuBase() override
    {
        for (const MenuItem* item : _items) {
            delete item;
        }
        _items.clear();
    }

    void Render() override;

    void UpdatePosition(const Rect& rect) override
    {
        const auto movePosition = Vector2(rect.x - _rect.x, rect.y - _rect.y);
        MenuItem::UpdatePosition(rect);

        for (const auto item : _items)
        {
            item->Move(movePosition);
        }
    }

    void Move(const Vector2 position) override
    {
        MenuItem::Move(position);
        for (const auto item : _items)
        {
            item->Move(position);
        }
    }

    SubMenu* AddSubMenu(const std::string& title);
    Toggle* AddToggle(const std::string& title, bool toggled);
    FloatSlider* AddFloatSlider(const std::string& title, float value, float minValue, float maxValue, float step, int precision);

    void AddItem(MenuItem* item) {
        _items.push_back(item);
        UpdateNextChildPosition();
    }

    void RemoveItem(const MenuItem* itemToRemove) {
        const auto it = std::find(_items.begin(), _items.end(), itemToRemove);
        if (it != _items.end()) {
            delete *it;
            _items.erase(it);
        }
    }

    bool OnMouseMoveEvent(MouseMoveEvent event) override
    {
        if(!_open)
        {
            return false;
        }
        
        for (const auto item : _items)
        {
            if(item->OnMouseMoveEvent(event))
            {
                return true;
            }
        }
        
        return false;
    }

    bool OnKeyStateEvent(KeyStateEvent event) override
    {
        if(event.isDown && event.key == VK_LBUTTON && _rect.Contains(InputManager::GetInstance()->GetMousePosition()))
        {
            _open = !_open;
            return true;
        }
        
        if(!_open)
        {
            return false;
        }

        for (const auto item : _items)
        {
            if(item->OnKeyStateEvent(event))
            {
                return true;
            }
        }
        
        return false;
    }
    
};
