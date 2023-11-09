﻿#pragma once
#include <vector>

#include "MenuBase.h"
#include "MenuItem.h"

#include "../../Input/InputManager.h"

class Menu : public MenuBase
{
private:
    
    static Menu* _instance;
    static std::once_flag _initInstanceFlag;

    bool _isMoving;
    int _mouseMoveHandlerId;
    int _keyStateEventHandlerId;
    Menu(const std::string& title, Rect rect);
    
public:
    
    Menu(const Menu&) = delete;
    Menu& operator=(const Menu&) = delete;
    Menu(Menu&&) = delete;
    Menu& operator=(Menu&&) = delete;
    
    ~Menu() override
    {
        for (const MenuItem* item : _items) {
            delete item;
        }
        _items.clear();
    }

    static Menu* GetInstance();
    
    bool OnMouseMoveEvent(MouseMoveEvent event) override
    {
        if(!_open)
        {
            return false;
        }
        
        if(_isMoving)
        {
            Move(event.delta);    
            return true;
        }
        
        return MenuBase::OnMouseMoveEvent(event);
    }

    bool OnKeyStateEvent(KeyStateEvent event) override
    {
        if(event.key == VK_LSHIFT && event.isDown)
        {
            _open = !_open;
            return true;
        }
        if(event.key == VK_LBUTTON)
        {
            if(_isMoving && !event.isDown)
            {
                _isMoving = false;
            }
            if(event.isDown && Contains(InputManager::GetInstance()->GetMousePosition()))
            {
                _isMoving = true;
                return true;
            }
        }
        return MenuBase::OnKeyStateEvent(event);
    }
};
