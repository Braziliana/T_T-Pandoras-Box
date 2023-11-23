#include "Hero.h"
#include "../Offsets.h"

Spell* Hero::GetSpell(const int slot)
{
    return *reinterpret_cast<Spell**>(reinterpret_cast<uintptr_t>(this) + HeroOffsets::SpellBook + HeroOffsets::SpellBookSpellSlot + (sizeof(uintptr_t) * slot));
}

Hero* Hero::LocalPlayer()
{
    static auto offsets = Offsets::GetInstance();
    return *reinterpret_cast<Hero**>(offsets->LocalPlayer);
}
