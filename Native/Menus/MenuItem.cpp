#include "MenuItem.h"
#include "../Rendering/Renderer.h"

void MenuItem::Render()
{
    const auto renderer = Renderer::Instance();
    renderer->RectFilledBordered(_rect.Center(), _rect.Size(), DefaultMenuStyle.ItemColor, DefaultMenuStyle.BorderColor, DefaultMenuStyle.Border);
    const auto itemsRect = _rect.Padding(DefaultMenuStyle.ContentPadding);
    
    renderer->Text(_title, itemsRect.GetStart(), itemsRect.GetEnd(), DefaultMenuStyle.FontSize, DefaultMenuStyle.TextColor, TextHorizontalOffset::Left, TextVerticalOffset::Center);
}
