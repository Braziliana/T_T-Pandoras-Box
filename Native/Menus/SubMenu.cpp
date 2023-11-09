﻿#include "SubMenu.h"
#include "../Rendering/Renderer.h"

void SubMenu::Render()
{
    const auto renderer = Renderer::Instance();
    MenuItem::Render();  // NOLINT(bugprone-parent-virtual-call)
    if(!_items.empty())
    {
        const auto elementPosition = DefaultMenuStyle.GetElementRect(_rect, 0);
        renderer->Text(_open ? "<" : ">", elementPosition.GetStart(), elementPosition.GetEnd(), DefaultMenuStyle.FontSize, DefaultMenuStyle.TextColor, TextHorizontalOffset::Center, TextVerticalOffset::Center);
    }
    else
    {
        return;
    }

    MenuBase::Render();
}
