#include "Menu.h"
#include "../Rendering/Renderer.h"

Menu* MenuGetInstance()
{
    return Menu::GetInstance();
}

Menu* Menu::_instance = nullptr;
std::once_flag Menu::_initInstanceFlag;

Menu::Menu(const std::string& title, const Rect rect): MenuBase(nullptr, MenuItemType::Menu, title, rect), _isMoving(false), _mouseMoveHandlerId(0), _keyStateEventHandlerId(0)
{
    const auto im = InputManager::GetInstance();
    _mouseMoveHandlerId = im->AddMouseMoveHandler([this](const MouseMoveEvent event) { this->OnMouseMoveEvent(event); });
    _keyStateEventHandlerId = im->AddKeyStateEventHandler([this](const KeyStateEvent event) { this->OnKeyStateEvent(event); });

    _headerRect = _rect;
    _nextChildPosition = Vector2(_headerRect.x, _headerRect.y + _headerRect.height - DefaultMenuStyle.Border);
}

Menu::~Menu()
{
    for (const MenuItem* item : _items) {
        delete item;
    }
    _items.clear();
}

Menu* Menu::GetInstance()
{
    std::call_once(_initInstanceFlag, []() {
        _instance = new Menu("T_T Pandoras box", Rect( Vector2(300, 300), DefaultMenuStyle.ItemSize));
    });
    return _instance;
}

bool Menu::OnMouseMoveEvent(MouseMoveEvent event)
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

bool Menu::OnKeyStateEvent(KeyStateEvent event)
{
    for (const auto item : _menus)
    {
        item->HandleHotkeys(event);
    }
    
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
