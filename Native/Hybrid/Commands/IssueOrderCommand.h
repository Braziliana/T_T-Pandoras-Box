#pragma once
#include <cstdint>
#include <iostream>

#include "../../Math/Vector2.h"
#include "../../Math/Vector3.h"

enum class IssueOrderCommandType
{
    MoveToScreenPosition,
    MoveToWorldPosition,
    MoveToObject,
    Attack
};

struct IssueOrderCommand
{
    IssueOrderCommandType commandType;
    Vector2 screenPosition;
    Vector3 worldPosition;
    uintptr_t objectPtr;

    IssueOrderCommand(const Vector2 screenPosition) : screenPosition(screenPosition)
    {
        //std::cout << "command " << (int) IssueOrderCommandType::MoveToScreenPosition << "sp: " << screenPosition.x << " " << screenPosition.y << std::endl;
        commandType = IssueOrderCommandType::MoveToScreenPosition;
        worldPosition = Vector3();
        objectPtr = 0x0;
    }

    IssueOrderCommand(const Vector3 worldPosition) : worldPosition(worldPosition)
    {
        commandType = IssueOrderCommandType::MoveToWorldPosition;
        screenPosition = Vector2();
        objectPtr = 0x0;
    }

    IssueOrderCommand(const uintptr_t objectPtr) : objectPtr(objectPtr)
    {
        commandType = IssueOrderCommandType::Attack;
        screenPosition = Vector2();
        worldPosition = Vector3();
    }
};
