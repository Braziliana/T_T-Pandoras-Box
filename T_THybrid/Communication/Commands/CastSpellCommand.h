#pragma once
#include "../../Math/Vector2.h"
#include "../../Game/Functions.h"
#include "../../Game/GameObject.h"

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
    
    void Handle() const;
};

inline void CastSpellCommand::Handle() const
{
    switch (castSpellType)
    {
    case CastSpellType::Self:
        Functions::CastSpellSelf(spellSlot);
        break;
    case CastSpellType::MousePosition:
        Functions::CastSpellClick(spellSlot, Functions::MousePosition());
        break;
    case CastSpellType::ScreenPosition:
        Functions::CastSpellClick(spellSlot, screenPosition);
        break;
    case CastSpellType::WorldPosition:
        Functions::CastSpell(spellSlot, worldPosition);
        break;
    case CastSpellType::Target:
        Functions::CastSpell(spellSlot, reinterpret_cast<GameObject*>(objectPtr));
        break;
    }
}
