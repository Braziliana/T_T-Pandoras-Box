#pragma once
#include <algorithm>
#include <cmath>
#include <array>

struct Color
{
    union {
        struct {
            float r, g, b, a;
        };
        float rgba[4];
    };

    Color() : r(0.0f), g(0.0f), b(0.0f), a(1.0f) {}

    Color(const float r, const float g, const float b) : r(r), g(g), b(b), a(1.0f)
    {
    }
    
    Color(const float r, const float g, const float b, const float a) : r(r), g(g), b(b), a(a)
    {
    }

    Color& operator*=(const float scalar)
    {
        r *= scalar;
        g *= scalar;
        b *= scalar;
        a *= scalar;
        return clamp();
    }

    Color& operator+=(const float scalar)
    {
        r += scalar;
        g += scalar;
        b += scalar;
        a += scalar;
        return clamp();
    }

    Color& operator-=(const float scalar)
    {
        r -= scalar;
        g -= scalar;
        b -= scalar;
        a -= scalar;
        return clamp();
    }
    
    Color operator*(const float scalar) const {
        return Color(r * scalar, g * scalar, b * scalar, a * scalar).clamp();
    }

    Color operator+(const Color& other) const {
        return Color(r + other.r, g + other.g, b + other.b, a + other.a).clamp();
    }

    Color operator-(const Color& other) const {
        return Color(r - other.r, g - other.g, b - other.b, a - other.a).clamp();
    }
    
    bool operator==(const Color& other) const {
        return std::abs(r - other.r) < std::numeric_limits<float>::epsilon() &&
               std::abs(g - other.g) < std::numeric_limits<float>::epsilon() &&
               std::abs(b - other.b) < std::numeric_limits<float>::epsilon() &&
               std::abs(a - other.a) < std::numeric_limits<float>::epsilon();
    }

    bool operator!=(const Color& other) const {
        return !(*this == other);
    }

    Color& clamp() {
        r = (r < 0.0f) ? 0.0f : (r > 1.0f) ? 1.0f : r;
        g = (g < 0.0f) ? 0.0f : (g > 1.0f) ? 1.0f : g;
        b = (b < 0.0f) ? 0.0f : (b > 1.0f) ? 1.0f : b;
        a = (a < 0.0f) ? 0.0f : (a > 1.0f) ? 1.0f : a;
        return *this;
    }

    static Color lerp(const Color& start, const Color& end, const float t) {
        const float inverse = 1.0f - t;
        return Color(
            inverse * start.r + t * end.r,
            inverse * start.g + t * end.g,
            inverse * start.b + t * end.b,
            inverse * start.a + t * end.a
        ).clamp();
    }
};