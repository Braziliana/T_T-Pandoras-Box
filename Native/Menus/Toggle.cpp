#include "Toggle.h"

#include "../Rendering/Renderer.h"
void Toggle::Render()
{
    const auto renderer = Renderer::Instance();
    MenuItem::Render();
    
    renderer->RectFilledBordered(_toggleElement.Center(), _toggleElement.Size(), _toggled ? DefaultMenuStyle.BorderColor : DefaultMenuStyle.ItemColor, DefaultMenuStyle.BorderColor, DefaultMenuStyle.ElementBorder);
}
