#pragma once
#include "../../Math/Vector2.h"
#include "../../Math/Vector3.h"

enum class CastSpellType
{
    Self,
    MousePosition,
    ScreenPosition,
    WorldPosition,
    Target
};

struct CastSpellCommand
{
    CastSpellType castSpellType;
    int spellSlot;
    Vector2 screenPosition;
    Vector3 worldPosition;
    uintptr_t objectPtr;
    
    CastSpellCommand(const int spellSlot, bool isSelfCast): spellSlot(spellSlot)
    {
        castSpellType = isSelfCast ? CastSpellType::Self : CastSpellType::MousePosition;
        screenPosition = Vector2();
        worldPosition = Vector3();
        objectPtr = 0x0;
    }
    
    CastSpellCommand(const int spellSlot, const Vector2 screenPosition) : spellSlot(spellSlot), screenPosition(screenPosition)
    {
        castSpellType = CastSpellType::ScreenPosition;
        worldPosition = Vector3();
        objectPtr = 0x0;
    }
    
    CastSpellCommand(const int spellSlot, const Vector3 worldPosition) : spellSlot(spellSlot), worldPosition(worldPosition)
    {
        castSpellType = CastSpellType::WorldPosition;
        screenPosition = Vector2();
        objectPtr = 0x0;
    }
    
    CastSpellCommand(const int spellSlot, uintptr_t objectPtr) : spellSlot(spellSlot), objectPtr(objectPtr)
    {
        castSpellType = CastSpellType::WorldPosition;
        screenPosition = Vector2();
        worldPosition = Vector3();
    }
};