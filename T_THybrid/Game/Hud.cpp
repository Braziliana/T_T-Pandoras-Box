#include "Hud.h"

void HudMouseInfo::SetMouseWorldPosition(Vector3 position)
{
    *reinterpret_cast<Vector3*>(this + 0x20) = position;
    *reinterpret_cast<Vector3*>(this + 0x2C) = position; 
}

void HudMouseInfo::SetTargetHandle(int targetHandle)
{
    *reinterpret_cast<int*>(this + 0x4C) = targetHandle;
    *reinterpret_cast<int*>(this + 0x50) = targetHandle; 
}

Vector3 HudMouseInfo::GetMousePosition()
{
    return *reinterpret_cast<Vector3*>(this + 0x20);
}

int HudMouseInfo::GetUnderMouseObjectHandle()
{
    return *reinterpret_cast<int*>(this + 0x4C);
}

///////HudCastSpell

SpellInfo* HudCastSpell::GetSpellInfo()
{
    return *reinterpret_cast<SpellInfo**>(this + 0x8);
}

void HudCastSpell::SetSpellInfo(SpellInfo* spellInfo)
{
    *reinterpret_cast<SpellInfo**>(this + 0x8) = spellInfo;
}

/////////Hud

HudMouseInfo* Hud::GetMouseInfo()
{
    return *reinterpret_cast<HudMouseInfo**>(this + 0x28);
}

HudCastSpell* Hud::GetCastHandle()
{
    return *reinterpret_cast<HudCastSpell**>(this + 0x68);
}

uintptr_t Hud::GetOrderHandle()
{
    return *reinterpret_cast<uintptr_t*>(this + 0x48);
}
