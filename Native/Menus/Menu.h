#pragma once
#include <vector>

#include "MenuBase.h"
#include "MenuItem.h"

#include "../../Input/InputManager.h"

extern "C"{
    typedef void (*MenuCallback)();
}

class Menu : public MenuBase
{
private:
    static Menu* _instance;
    static std::once_flag _initInstanceFlag;
    MenuCallback _closeCallback = nullptr;

    bool _isMoving;
    int _mouseMoveHandlerId;
    int _keyStateEventHandlerId;
    Menu(const std::string& title, Rect rect);
    
public:
    
    Menu(const Menu&) = delete;
    Menu& operator=(const Menu&) = delete;
    Menu(Menu&&) = delete;
    Menu& operator=(Menu&&) = delete;
    
    ~Menu() override;

    static Menu* GetInstance();
    bool OnMouseMoveEvent(MouseMoveEvent event) override;
    bool OnKeyStateEvent(KeyStateEvent event) override;

    void SetCloseCallback(MenuCallback callback);
};

extern "C" {
    __declspec(dllexport) Menu* MenuGetInstance();
    __declspec(dllexport) void MenuSetCloseCallback(Menu* instance, MenuCallback callback);
}
