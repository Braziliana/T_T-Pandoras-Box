#pragma once
#include "../Math/Vector2.h"
#include "../Math/Vector3.h"

class Hud;
class GameObject;

class Functions
{
public:
    static void PrintChat(const char* msg);

    static Hud* GetHud();
    
    static void MoveTo(int x, int y);
    static void MoveTo(Vector2 position);
    static void MoveTo(Vector3 worldPosition);
    static void MoveToMouse();
    static void IssueOrder(bool isAttackCommand, int x, int y);
    static void Attack(GameObject* gameObject);

    static void CastSpell(int spellSlot, GameObject* target, Vector3 endPosition);
    static void CastSpell(int spellSlot, GameObject* gameObject);
    static void CastSpell(int spellSlot, Vector3 position);
    static void CastSpell(int spellSlot);
    static void SelfCast(int spellSlot);

    static void CastSpellClick(int spellSlot, Vector2 screenPosition);

    static bool WorldToScreen(Vector3 position, Vector2& out);
    static Vector3 WorldMousePosition();
    static Vector2 MousePosition();
    static void SetMousePosition(Vector2 position);
};
