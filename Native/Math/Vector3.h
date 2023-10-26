#pragma once
#include <cmath>
#include <limits>
#include <DirectXMath.h>

struct Vector3 : public DirectX::XMFLOAT3 {


    Vector3() : XMFLOAT3(0, 0, 0) {}
    Vector3(const float x, const float y, const float z) : XMFLOAT3(x, y, z) {}
    explicit Vector3(_In_reads_(3) const float* pArray) noexcept : XMFLOAT3(pArray) {}

    Vector3 operator+(const Vector3& other) const {
        return {
            x + other.x,
            y + other.y,
            z + other.z
        };
    }

    Vector3 operator-(const Vector3& other) const {
        return {
            x - other.x,
            y - other.y,
            z - other.z
        };
    }

    Vector3 operator*(const float scalar) const {
        return {
            x * scalar,
            y * scalar,
            z * scalar
        };
    }

    Vector3 operator/(const float scalar) const {
        if(scalar == 0.0f) {
            return {};
        }
        
        return {
            x / scalar,
            y / scalar,
            z / scalar
        };
    }

    bool operator==(const Vector3& other) const {
        return std::abs(x - other.x) < std::numeric_limits<float>::epsilon() &&
               std::abs(y - other.y) < std::numeric_limits<float>::epsilon() &&
               std::abs(z - other.z) < std::numeric_limits<float>::epsilon();
    }

    bool operator!=(const Vector3& other) const {
        return !(*this == other);
    }
    
    float dot(const Vector3& other) const {
        return x * other.x + y * other.y + z * other.z;
    }

    Vector3 cross(const Vector3& other) const {
        return {
            y * other.z - z * other.y,
            z * other.x - x * other.z,
            x * other.y - y * other.x
        };
    }

    float magnitude() const {
        return std::sqrt(x * x + y * y + z * z);
    }

    Vector3 normalize() const {
        const float mag = magnitude();
        if (mag == 0.0f) {
            return {};
        }
        
        return *this / mag;
    }

    float distance(const Vector3& other) const {
        return distance(*this, other);
    }
    
    static float distance(const Vector3& a, const Vector3& b) {
        return (a - b).magnitude();
    }
};
