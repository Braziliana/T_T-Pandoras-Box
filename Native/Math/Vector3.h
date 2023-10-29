#pragma once
#include <cmath>
#include <limits>

struct Vector3
{
    union
    {
        struct
        {
            float x;
            float y;
            float z;
        };
        float xyz[3];
    };

    
    Vector3() : Vector3(0, 0, 0)
    {
    }

    Vector3(const float x, const float y, const float z) : x(x), y(y), z(z)
    {
    }

    Vector3 operator+(const Vector3& other) const
    {
        return {
            x + other.x,
            y + other.y,
            z + other.z
        };
    }

    Vector3 operator-(const Vector3& other) const
    {
        return {
            x - other.x,
            y - other.y,
            z - other.z
        };
    }

    Vector3 operator*(const float scalar) const
    {
        return {
            x * scalar,
            y * scalar,
            z * scalar
        };
    }

    Vector3 operator/(const float scalar) const
    {
        if (scalar == 0.0f)
        {
            return {};
        }

        return {
            x / scalar,
            y / scalar,
            z / scalar
        };
    }

    bool operator==(const Vector3& other) const
    {
        return std::abs(x - other.x) < std::numeric_limits<float>::epsilon() &&
            std::abs(y - other.y) < std::numeric_limits<float>::epsilon() &&
            std::abs(z - other.z) < std::numeric_limits<float>::epsilon();
    }

    bool operator!=(const Vector3& other) const
    {
        return !(*this == other);
    }

    float Dot(const Vector3& other) const
    {
        return x * other.x + y * other.y + z * other.z;
    }

    Vector3 Cross(const Vector3& other) const
    {
        return {
            y * other.z - z * other.y,
            z * other.x - x * other.z,
            x * other.y - y * other.x
        };
    }

    float Magnitude() const
    {
        return std::sqrt(x * x + y * y + z * z);
    }

    Vector3 Normalize() const
    {
        const float mag = Magnitude();
        if (mag == 0.0f)
        {
            return {};
        }

        return *this / mag;
    }

    float Distance(const Vector3& other) const
    {
        return Distance(*this, other);
    }

    static float Distance(const Vector3& a, const Vector3& b)
    {
        return (a - b).Magnitude();
    }
};
