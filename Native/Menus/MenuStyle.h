#pragma once

#include "../../Math/Color.h"
#include "../../Math/Vector2.h"
#include "../../Math/Rect.h"

struct MenuStyle
{
    Vector2 ItemSize;
    float Border;
    Vector2 ContentPadding;
    float ElementSize;
    float ElementBorder;
    float ElementSpacing;
    float FontSize;

    Vector2 SliderThumbSize;
    float SliderTrackHeight = 8;
    
    Color ItemColor;
    Color ItemHooverColor;
    Color TextColor;
    Color BorderColor;
    Color ElementBorderColor;

    MenuStyle();

    Rect GetElementRect(const Rect& rect, int itemIndex) const;
    Rect GetMenuSlotRect(Rect rect, int slot) const;
    Rect GetNextItem(const Vector2& position, int rows) const;
    
    Rect GetSlideAreaRect(const Rect& rect) const;
    Rect GetSliderTrack(const Rect& slidingArea) const;
    Rect GetSliderThumbRect(const Rect& slidingArea, float value, float minValue, float maxValue) const;
};

extern MenuStyle DefaultMenuStyle;