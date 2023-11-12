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

MenuItem::MenuItem(const MenuItemType menuItemType, std::string title, const Rect rect): _menuItemType(menuItemType), _title(std::move(title)), _rect(rect)
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
