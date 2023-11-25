#pragma once
#include <cstdint>
#include <sstream>

#include "../../Game/Functions.h"
#include "../../Game/GameObject.h"
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

    void Handle() const;
};

inline auto IssueOrderCommand::Handle() const -> void
{
    switch (commandType) {
    case IssueOrderCommandType::MoveToScreenPosition:
        Functions::MoveTo(screenPosition);
        break;
    case IssueOrderCommandType::MoveToWorldPosition:
        Functions::MoveTo(worldPosition);
        break;
    case IssueOrderCommandType::MoveToObject:
        Functions::MoveTo(reinterpret_cast<GameObject*>(objectPtr)->GetPosition());
        break;
    case IssueOrderCommandType::Attack:
        Functions::Attack(reinterpret_cast<GameObject*>(objectPtr));
        break;
    }
}
