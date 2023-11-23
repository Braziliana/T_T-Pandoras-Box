#pragma once
#include <cstdint>

#include "../../Game/Functions.h"
#include "../../Game/GameObject.h"
#include "../../Math/Vector2.h"
#include "../../Math/Vector3.h"

enum class MoveToCommandType
{
    ScreenPosition,
    WorldPosition,
    Object
};

struct MoveToCommand
{
    MoveToCommandType moveToCommand;
    Vector2 screenPosition;
    Vector3 worldPosition;
    uintptr_t objectPtr;

    void Handle()
    {
        switch (moveToCommand) {
        case MoveToCommandType::ScreenPosition:
            Functions::MoveTo(screenPosition);
            break;
        case MoveToCommandType::WorldPosition:
            Functions::MoveTo(worldPosition);
            break;
        case MoveToCommandType::Object:
            Functions::MoveTo(reinterpret_cast<GameObject*>(objectPtr)->GetPosition());
            break;
        default: ;
        }
    }
};
