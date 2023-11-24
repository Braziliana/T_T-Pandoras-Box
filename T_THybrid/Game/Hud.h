#pragma once
#include <cstdint>

#include "Spell.h"
#include "../Math/Vector3.h"

class HudMouseInfo
{
public:
    void SetMouseWorldPosition(Vector3 position);
    void SetTargetHandle(int targetHandle);
    Vector3 GetMousePosition();
    int GetUnderMouseObjectHandle();
};

class HudCastSpell
{
public:
    SpellInfo* GetSpellInfo();
    void SetSpellInfo(SpellInfo* spellInfo);
};

class Hud
{
public:
    HudMouseInfo* GetMouseInfo();
    HudCastSpell* GetCastHandle();
    uintptr_t GetOrderHandle();
};
