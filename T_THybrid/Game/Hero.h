#pragma once
#include "GameObject.h"

class Spell;
class Hero : public GameObject
{
public:
    Spell* GetSpell(int slot);
    static Hero* LocalPlayer();
};
