#pragma once
#include "../Math/Vector3.h"
#include "../Offsets.h"

class GameObject
{
public:
    Vector3 GetPosition()
    {
        return *reinterpret_cast<Vector3*>(this + GameObjectOffsets::Position);
    }

    int GetHandle()
    {
        return *reinterpret_cast<int*>(this + GameObjectOffsets::Handle);
    }
};
