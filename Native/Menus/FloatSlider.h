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
    bool _isSliding;

    Rect topContentArea;
    Rect topArea;
    Rect bottomArea;
    Rect increaseRect;
    Rect decreaseRect;

    Rect sliderArea;
    Rect sliderThumb;
    Rect sliderTrack;

    void UpdateContentPositions()
    {
        topArea = DefaultMenuStyle.GetMenuSlotRect(_rect, 0);
        bottomArea = DefaultMenuStyle.GetMenuSlotRect(_rect, 1);

        topContentArea = topArea.Padding(DefaultMenuStyle.ContentPadding);
        increaseRect = DefaultMenuStyle.GetElementRect(topArea, 0);
        decreaseRect = DefaultMenuStyle.GetElementRect(topArea, 1);

        sliderArea = DefaultMenuStyle.GetSlideAreaRect(_rect);
        sliderThumb = DefaultMenuStyle.GetSliderThumbRect(sliderArea, _value, _minValue, _maxValue);
        sliderTrack = DefaultMenuStyle.GetSliderTrack(sliderArea);
    }
    
public:
    FloatSlider(const std::string& title, const Rect& rect, const float value, const float minValue, const float maxValue, const float step, const int precision)
        : MenuItem(title, rect), _value(value), _minValue(minValue), _maxValue(maxValue), _step(step), _precision(precision), _isSliding(false)
    {
        UpdateContentPositions();
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

    int GetPrecision() const
    {
        return _precision;
    }

    void SetValue(const float value)
    {
        _value = value;
        sliderThumb = DefaultMenuStyle.GetSliderThumbRect(sliderArea, _value, _minValue, _maxValue);
    }
    
    float CalculateValueFromThumbPosition(const Rect& knobRect, const Rect& slidingArea) const
    {

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

    void SetNormalizedValue(float value)
    {
        if(value < 0.0f)
        {
            value = 0.0f;
        }
        else if(value > 1.0f)
        {
            value = 1.0f;
        }

        SetValue(value * (_maxValue-_minValue) + _minValue);
    }
    
    void Render() override;

    bool OnMouseMoveEvent(const MouseMoveEvent mouseMoveEvent) override
    {
        if(!_isSliding)
        {
            return false;
        }

        const float normalizedPosition = (mouseMoveEvent.position.x - sliderTrack.x) / sliderTrack.width;
        SetNormalizedValue(normalizedPosition);
        return true;
    }

    bool OnKeyStateEvent(const KeyStateEvent event) override
    {
        const auto mousePosition = InputManager::GetInstance()->GetMousePosition();
        if(event.key == VK_LBUTTON)
        {
            if(_isSliding && !event.isDown)
            {
                _isSliding = false;
            }
            else if(event.isDown)
            {
               
                if(sliderThumb.Contains(mousePosition))
                {
                    _isSliding = true;
                    return true;
                }
                
                if(sliderArea.Contains(mousePosition))
                {
                    const float normalizedPosition = (mousePosition.x - sliderTrack.x) / sliderTrack.width;
                    SetNormalizedValue(normalizedPosition);
                    return true;
                }
            }
        }

       
        if(event.key == VK_LBUTTON && event.isDown)
        {
            if(increaseRect.Contains(mousePosition))
            {
                _value += _step;
                if(_value > _maxValue)
                {
                    _value = _maxValue;
                }
                SetValue(_value);
                return true;
            }
            if(decreaseRect.Contains(mousePosition))
            {
                _value -= _step;
                if(_value < _minValue)
                {
                    _value = _minValue;
                }
                SetValue(_value);
                return true;
            }
        }
        
        return false;
    }

    void Move(const Vector2& position) override
    {
        MenuItem::Move(position);
        UpdateContentPositions();
    }

    void UpdatePosition(const Rect& rect) override
    {
        MenuItem::UpdatePosition(rect);
        UpdateContentPositions();
    }
};
