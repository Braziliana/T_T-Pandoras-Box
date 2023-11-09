#pragma once
#include <iostream>

#include "Vector2.h"

struct Rect
{
public:
    float x;
    float y;
    float width;
    float height;

    Rect() : Rect(0, 0, 0, 0) {  }
    
    Rect(const float x, const float y, const float width, const float height) : x(x), y(y), width(width), height(height)
    {
        
    }

    Rect(const Vector2& position, const Vector2& size) : x(position.x), y(position.y), width(size.x), height(size.y)
    {
    }

    static Rect FromCenter(const Vector2& position, const Vector2& size)
    {
        const auto halfSize = size/2;
        return {position-halfSize, size};
    }
    
    bool Contains(const Vector2& position) const
    {
        return position.x >= x &&
                position.x <= x + width &&
                position.y >= y &&
                position.y <= y + height;
    }

    Vector2 GetStart() const
    {
        return {x, y};
    }
    
    Vector2 GetEnd() const
    {
        return {x+width, y+height};
    }

    Vector2 Center() const
    {
        return {x+width/2, y+height/2};
    }

    void Move(const Vector2 position)
    {
        x += position.x;
        y += position.y;
    }

    Vector2 Size() const
    {
        return {width, height};
    }

    Rect Padding(const float padding) const
    {
        return {x + padding, y + padding, width-padding, height-padding};
    }
    Rect Padding(const float paddingX, const float paddingY) const
    {
        return {x + paddingX, y + paddingY, width-paddingX*2, height-paddingY*2};
    }
    
    Rect Padding(const Vector2& padding) const
    {
        return Padding(padding.x, padding.y);
    }
};
