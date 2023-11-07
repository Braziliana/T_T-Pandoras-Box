#pragma once
#include "MenuItem.h"

class FloatSlider : public MenuItem
{
private:
    float _value;
    float _minValue;
    float _maxValue;
    float _step;
    int _precision;
    
public:
    FloatSlider(const std::string& title, const Rect& rect, const float value, const float minValue, const float maxValue, const float step, const int precision)
        : MenuItem(title, rect), _value(value), _minValue(minValue), _maxValue(maxValue), _step(step), _precision(precision)
    {
    }

    float* GetValuePointer()
    {
        return &_value; 
    }

    float GetValue() const
    {
        return _value;
    }

    float GetMinValue() const
    {
        return _minValue;
    }

    float GetMaxValue() const
    {
        return _maxValue;
    }

    float GetStep() const
    {
        return _step;
    }

    float GetPrecision() const
    {
        return _precision;
    }

    static Rect GetSlideAreaRect(const Rect& rect)
    {
        const auto bottomSlot = GetMenuSlotRect(rect, 1);
        return bottomSlot.Padding(MenuItem::BorderWidth + 10, 5);
    }

    Rect GetSlideAreaRect() const
    {
        return GetSlideAreaRect(_rect);
    }
    
    Rect GetKnobRect(const Rect& slidingArea) const
    {
        return GetKnobRect(slidingArea, _value, _minValue, _maxValue);
    }

    static Rect GetKnobRect(const Rect& slidingArea, const float value, const float minValue, const float maxValue) {
        const float range = maxValue - minValue;
        const float normalizedValue = (value - minValue) / range;
        constexpr float knobWidth = 10;

        return Rect{slidingArea.x + normalizedValue * slidingArea.width - knobWidth/2, slidingArea.y, knobWidth, slidingArea.height};
    }

    float CalculateValueFromKnobPosition(const Rect& knobRect, const Rect& slidingArea) {

        const float knobCenter = knobRect.Center().x;
        float normalizedPosition = (knobCenter - slidingArea.x) / slidingArea.width;

        if(normalizedPosition < 0.0f)
        {
            normalizedPosition = 0.0f;
        }
        else if(normalizedPosition > 1.0f)
        {
            normalizedPosition = 1.0f;
        }
        
        const float range = _maxValue - _minValue;
        const float value = normalizedPosition * range + _minValue;

        return value;
    }

    
    void Render() override;

    bool OnMouseMoveEvent(MouseMoveEvent mouseMoveEvent) override
    {
        if(!InputManager::GetInstance()->GetKeyState(VK_LBUTTON))
        {
            return false;
        }
        
        auto slidingArea = GetSlideAreaRect();
        auto knobRect = GetKnobRect(slidingArea);

        if(knobRect.Contains(mouseMoveEvent.position))
        {
            knobRect.Move(Vector2(mouseMoveEvent.delta.x, 0));
            _value = CalculateValueFromKnobPosition(knobRect, slidingArea);
        }
        
        return false;
    }

    bool OnKeyStateEvent(KeyStateEvent event) override
    {
        const auto decreasePosition = GetElementRect(0);
        if(event.key == VK_LBUTTON && event.isDown && decreasePosition.Contains(InputManager::GetInstance()->GetMousePosition()))
        {
            _value -= _step;
            if(_value < _minValue)
            {
                _value = _minValue;
            }
            return true;
        }
        const auto increasePosition = GetElementRect(1);
        if(event.key == VK_LBUTTON && event.isDown && increasePosition.Contains(InputManager::GetInstance()->GetMousePosition()))
        {
            _value += _step;
            if(_value > _maxValue)
            {
                _value = _maxValue;
            }
            return true;
        }
        return false;
    }
};
