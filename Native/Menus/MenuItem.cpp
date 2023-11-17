#include "MenuItem.h"

#include <utility>
#include "../Rendering/Renderer.h"

void MenuItemSetTitle(MenuItem* instance, const char* title)
{
    instance->SetTitle(title);
}

void MenuItemRender(MenuItem* instance)
{
    instance->Render();
}
const char* MenuItemGetId(MenuItem* instance)
{
    const std::string id = instance->GetId();
    const auto result = new char[id.size() + 1];
    strcpy_s(result, id.size() + 1, id.c_str());
    return result;
}


void FreeMenuItemIdBuffer(const char* instance)
{
    delete[] instance;
}


MenuItem::MenuItem(MenuItem* parent, const MenuItemType menuItemType, std::string title, const Rect rect)
    : _parent(parent), _menuItemType(menuItemType), _title(std::move(title)), _rect(rect)
{  }

bool MenuItem::Contains(const Vector2& position) const
{
    return _rect.Contains(position);
}

const Rect& MenuItem::GetRect() const
{
    return _rect;
}

void MenuItem::SetTitle(const std::string& title)
{
    _title = std::string(title);
}

const std::string& MenuItem::GetTitle()
{
    return _title;
}

void MenuItem::UpdatePosition(const Rect& rect)
{
    _rect = rect;
}

void MenuItem::Move(const Vector2& position)
{
    _rect.Move(position);
}

void MenuItem::Render()
{
    const auto renderer = Renderer::Instance();
    renderer->RectFilledBordered(_rect.Center(), _rect.Size(), DefaultMenuStyle.ItemColor, DefaultMenuStyle.BorderColor, DefaultMenuStyle.Border);
    const auto itemsRect = _rect.Padding(DefaultMenuStyle.ContentPadding);
    
    renderer->Text(_title, itemsRect.GetStart(), itemsRect.GetEnd(), DefaultMenuStyle.FontSize, DefaultMenuStyle.TextColor, TextHorizontalOffset::Left, TextVerticalOffset::Center);
}

bool MenuItem::OnMouseMoveEvent(MouseMoveEvent mouseMoveEvent)
{
    return false;
}

bool MenuItem::OnKeyStateEvent(KeyStateEvent event)
{
    return false;
}

MenuItemType MenuItem::GetType() const
{
    return _menuItemType;
}

void MenuItem::Close()
{
}

void MenuItem::Open()
{
}

std::string MenuItem::GetId()
{
    if(_parent == nullptr)
    {
        return _title;
    }

    return _parent->GetId() + "." + _title;
}