#include "FloatSlider.h"

#include "../Rendering/Menus/MenuRenderer.h"

void FloatSlider::Render()
{
    MenuRenderer::GetInstance()->DrawFloatSlider(_rect, _title, _value, _minValue, _maxValue, _step, _precision);
}
