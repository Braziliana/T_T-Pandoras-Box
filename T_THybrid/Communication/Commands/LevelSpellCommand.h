#pragma once
#include "../../Game/Functions.h"

struct LevelSpellCommand
{
    int spellSlot;
    void Handle() const;
};

inline void LevelSpellCommand::Handle() const
{
    Functions::LevelSpell(spellSlot);
}
