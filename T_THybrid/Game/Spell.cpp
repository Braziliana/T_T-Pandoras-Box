#include "Spell.h"

void SpellInput::SetCaster(const int handle)
{
    *reinterpret_cast<int*>(this + 0x10) = handle;
}

void SpellInput::SetTarget(const int handle)
{
    *reinterpret_cast<int*>(this + 0x14) = handle;
}

void SpellInput::SetStartPosition(const Vector3 position)
{
    *reinterpret_cast<Vector3*>(this + 0x18) = position;
}

void SpellInput::SetEndPosition(const Vector3 position)
{
    *reinterpret_cast<Vector3*>(this + 0x24) = position;
}

void SpellInput::SetClickPosition(const Vector3 position)
{
    *reinterpret_cast<Vector3*>(this + 0x24 + sizeof(Vector3)) = position;
}

void SpellInput::SetEndPosition2(const Vector3 position)
{
    *reinterpret_cast<Vector3*>(this + 0x24 + sizeof(Vector3) * 2) = position;
}

SpellInput* Spell::GetSpellInput()
{
    return reinterpret_cast<SpellInput*>(this + 0x128);
}
