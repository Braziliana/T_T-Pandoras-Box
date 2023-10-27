#pragma once
#include <cmath>
#include <DirectXMath.h>
#include <limits>


struct Vector2 : public DirectX::XMFLOAT2
{
    Vector2() : XMFLOAT2(0, 0)
    {
    }

    Vector2(const float x, const float y) : XMFLOAT2(x, y)
    {
    }

    explicit Vector2(_In_reads_(2) const float* pArray) noexcept : XMFLOAT2(pArray)
    {
    }

    Vector2 operator+(const Vector2& other) const
    {
        return {x + other.x, y + other.y};
    }

    Vector2 operator-(const Vector2& other) const
    {
        return {x - other.x, y - other.y};
    }

    Vector2 operator*(const float scalar) const
    {
        return {x * scalar, y * scalar};
    }

    Vector2 operator/(const float scalar) const
    {
        if (scalar == 0.0f)
        {
            return {};
        }

        return {x / scalar, y / scalar};
    }

    bool operator==(const Vector2& other) const
    {
        if (std::abs(x - other.x) > std::numeric_limits<float>::epsilon())
        {
            return false;
        }

        return std::abs(y - other.y) > std::numeric_limits<float>::epsilon();
    }

    bool operator!=(const Vector2& other) const
    {
        return !(*this == other);
    }

    float Dot(const Vector2& other) const
    {
        return x * other.x + y * other.y;
    }

    float Magnitude() const
    {
        return std::sqrt(x * x + y * y);
    }

    Vector2 Normalize() const
    {
        const float mag = Magnitude();
        if (mag == 0.0f)
        {
            return {};
        }

        return *this / mag;
    }

    float Distance(const Vector2& other) const
    {
        return Distance(*this, other);
    }

    static float Distance(const Vector2& a, const Vector2& b)
    {
        return (a - b).Magnitude();
    }
};
