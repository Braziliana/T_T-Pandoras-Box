﻿#include "Menu.h"
#include "../Rendering/Renderer.h"

Menu* Menu::_instance = nullptr;
std::once_flag Menu::_initInstanceFlag;

Menu::Menu(const std::string& title, const Rect rect): MenuBase(title, rect), _isMoving(false), _mouseMoveHandlerId(0), _keyStateEventHandlerId(0)
{
    const auto im = InputManager::GetInstance();
    _mouseMoveHandlerId = im->AddMouseMoveHandler([this](const MouseMoveEvent event) { this->OnMouseMoveEvent(event); });
    _keyStateEventHandlerId = im->AddKeyStateEventHandler([this](const KeyStateEvent event) { this->OnKeyStateEvent(event); });

    _headerRect = _rect;
    _nextChildPosition = Vector2(_headerRect.x, _headerRect.y + _headerRect.height - DefaultMenuStyle.Border);
}

Menu* Menu::GetInstance()
{
    std::call_once(_initInstanceFlag, []() {
        _instance = new Menu("T_T Pandoras box", Rect( Vector2(300, 300), DefaultMenuStyle.ItemSize));
    });
    return _instance;
}