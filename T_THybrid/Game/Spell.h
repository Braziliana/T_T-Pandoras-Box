#pragma once
#include "../Math/Vector3.h"

class SpellInput
{
public:
    
    void SetCaster(int handle);
    void SetTarget(int handle);
    void SetStartPosition(Vector3 position);
    void SetEndPosition(Vector3 position);
    void SetClickPosition(Vector3 position);
    void SetEndPosition2(Vector3 position);
};

class Spell
{
public:
    SpellInput* GetSpellInput();
};

