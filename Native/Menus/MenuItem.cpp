#include "MenuItem.h"
#include "../Rendering/Menus/MenuRenderer.h"

Vector2 MenuItem::ItemSize = Vector2(300, 34);

float MenuItem::BorderWidth = 4;
float MenuItem::ElementSpacing = 5;
float MenuItem::ElementSize = 20;

/*
float _elementSize = 18;
    float _elementSpacing = 5;
    float _borderWidth = 3;
 */

void MenuItem::Render()
{
    MenuRenderer::GetInstance()->DrawItem(_rect, _title);
}
