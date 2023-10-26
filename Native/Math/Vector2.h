#pragma once
#include <cmath>
#include <limits>


struct Vector2 {
    float x;
    float y;

    Vector2() : x(0.0f), y(0.0f) {}

    Vector2(const float x, const float y) : x(x), y(y) {}

    Vector2 operator+(const Vector2& other) const {
        return {x + other.x, y + other.y};
    }

    Vector2 operator-(const Vector2& other) const {
        return {x - other.x, y - other.y};
    }

    Vector2 operator*(const float scalar) const {
        return {x * scalar, y * scalar};
    }

    Vector2 operator/(const float scalar) const {
        if(scalar == 0.0f) {
            return {};
        }
        
        return {x / scalar, y / scalar};
    }

    bool operator==(const Vector2& other) const {

        if(std::abs(x - other.x) > std::numeric_limits<float>::epsilon())
        {
            return false;
        }
        
        return std::abs(y - other.y) > std::numeric_limits<float>::epsilon();
    }

    bool operator!=(const Vector2& other) const {
        return !(*this == other);
    }

    float dot(const Vector2& other) const {
        return x * other.x + y * other.y;
    }

    float magnitude() const {
        return std::sqrt(x * x + y * y);
    }

    Vector2 normalize() const {
        const float mag = magnitude();
        if (mag == 0.0f) {
            return {};
        }
        
        return *this / mag;
    }

    float distance(const Vector2& other) const {
        return distance(*this, other);
    }
    
    static float distance(const Vector2& a, const Vector2& b) {
        return (a - b).magnitude();
    }
};
