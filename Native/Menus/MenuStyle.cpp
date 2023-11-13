#include "MenuStyle.h"
#include <Windows.h>

MenuStyle DefaultMenuStyle = MenuStyle();

MenuStyle::MenuStyle()
{
    const auto screenWidth = static_cast<float>(GetSystemMetrics(SM_CXSCREEN));
    const auto screenHeight = static_cast<float>(GetSystemMetrics(SM_CYSCREEN));

    float scale = screenHeight/1080;
    ItemSize = {250 * scale, 30 * scale};

    if(static_cast<int>(ItemSize.x)%2 != 0)
    {
        ItemSize.x += 1;
    }
    
    if(static_cast<int>(ItemSize.y)%2 != 0)
    {
        ItemSize.y += 1;
    }
    
    Border = 4;
    ContentPadding = Vector2(Border + 8, Border + 8);
    
    ElementBorder = 2;
    ElementSpacing = ElementBorder + 6; 
    ElementSize = ItemSize.y - ContentPadding.y - 6;
    SliderThumbSize = Vector2(10, 20);
    
    FontSize = ItemSize.y - ContentPadding.y - 8;

    ItemColor = Color::FromByte(1, 1, 1, 200);
    ItemHooverColor = Color::FromByte(30, 30, 30, 200);
    TextColor = Color::FromByte(255, 108, 34);
    BorderColor = Color::FromByte(255, 108, 34, 200);
    ElementBorderColor = Color::FromByte(255, 108, 34);
    
}

Rect MenuStyle::GetElementRect(const Rect& rect, const int itemIndex) const
{
    const auto itemsRect = rect.Padding(ContentPadding);
    const auto center = itemsRect.Center();
    
    float x = itemsRect.x + itemsRect.width;
    float y = center.y-ElementSize/2;
    x -= (ElementSpacing + Border) + (Border + ElementSize) * static_cast<float>(itemIndex + 1);

    return {x, y, ElementSize, ElementSize};
}

Rect MenuStyle::GetMenuSlotRect(Rect rect, const int slot) const
{
    return {
        rect.x,
        rect.y + (ItemSize.y - Border) * static_cast<float>(slot),
        ItemSize.x,
        ItemSize.y
    };
}

Rect MenuStyle::GetNextItem(const Vector2& position, const int rows) const
{
    float extra = 0.0f;
    if(rows > 1)
    {
        extra = (static_cast<float>(rows)-1) * Border;
    }
    
    return {
        position.x,
        position.y,
        ItemSize.x,
        ItemSize.y * static_cast<float>(rows) - extra};
}

Rect MenuStyle::GetSlideAreaRect(const Rect& rect) const
{
    const auto bottomSlot = GetMenuSlotRect(rect, 1);
    return bottomSlot.Padding(ContentPadding);
}

Rect MenuStyle::GetSliderTrack(const Rect& slidingArea) const
{
    const auto middle = slidingArea.Center();
    return Rect{slidingArea.x, middle.y, slidingArea.width, SliderTrackHeight};
}

Rect MenuStyle::GetSliderThumbRect(const Rect& slidingArea, const float value, const float minValue,
    const float maxValue) const
{
    const float range = maxValue - minValue;
    const float normalizedValue = (value - minValue) / range;

    const auto middle = slidingArea.Center();
    const auto halfSize = SliderThumbSize/2;
    return Rect{
        slidingArea.x + normalizedValue * slidingArea.width - halfSize.x,
        middle.y-halfSize.y,
        SliderThumbSize.x,
        SliderThumbSize.y};
}
