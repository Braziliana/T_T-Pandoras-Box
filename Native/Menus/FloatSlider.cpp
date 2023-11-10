#include "FloatSlider.h"

#include <iomanip>
#include <sstream>

#include "../Rendering/Renderer.h"

float* FloatSliderGetValuePointer(FloatSlider* instance)
{
    return instance->GetValuePointer();
}

float* FloatSliderGetMinValuePointer(FloatSlider* instance)
{
    return instance->GetMinValuePointer();
}

float* FloatSliderGetMaxValuePointer(FloatSlider* instance)
{
    return instance->GetMaxValuePointer();
}

float* FloatSliderGetStepValuePointer(FloatSlider* instance)
{
    return instance->GetStepPointer();
}

int* FloatSliderGetPrecisionPointer(FloatSlider* instance)
{
    return instance->GetPrecisionPointer();
}

std::string FloatString(const std::string& str, const float value, const int precision) {
    std::ostringstream out;
    out << str << ' ' << std::fixed << std::setprecision(precision) << value;
    return out.str();
}

void FloatSlider::Render()
{
    const auto renderer = Renderer::Instance();

    renderer->RectFilledBordered(_rect.Center(), _rect.Size(), DefaultMenuStyle.ItemColor, DefaultMenuStyle.BorderColor, DefaultMenuStyle.ElementBorder);

    renderer->Text(FloatString(_title, _value, _precision), topContentArea.GetStart(), topContentArea.GetEnd(), DefaultMenuStyle.FontSize, DefaultMenuStyle.TextColor, TextHorizontalOffset::Left, TextVerticalOffset::Center);
    
    renderer->RectBorder(increaseRect.Center(), increaseRect.Size(), DefaultMenuStyle.BorderColor, DefaultMenuStyle.ElementBorder);
    renderer->Text("+", increaseRect.Center(),  DefaultMenuStyle.FontSize + 3, DefaultMenuStyle.TextColor, TextHorizontalOffset::Center, TextVerticalOffset::Center);

    renderer->RectBorder(decreaseRect.Center(), decreaseRect.Size(), DefaultMenuStyle.BorderColor, DefaultMenuStyle.ElementBorder);
    renderer->Text("-", decreaseRect.Center() + Vector2(0, 6),  DefaultMenuStyle.FontSize + 3, DefaultMenuStyle.TextColor, TextHorizontalOffset::Center, TextVerticalOffset::None);


    renderer->RectFilledBordered(sliderTrack.Center(), sliderTrack.Size(), DefaultMenuStyle.BorderColor, DefaultMenuStyle.BorderColor, DefaultMenuStyle.ElementBorder);
    renderer->RectFilledBordered(sliderThumb.Center(), sliderThumb.Size(), DefaultMenuStyle.BorderColor, DefaultMenuStyle.BorderColor, DefaultMenuStyle.ElementBorder);
}