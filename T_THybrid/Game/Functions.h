#pragma once
#include "../Math/Vector2.h"
#include "../Math/Vector3.h"

class GameObject;

class Functions
{
public:
    static void PrintChat(const char* msg);
    static void MoveTo(int x, int y);
    static void MoveToMouse();
    static void IssueOrder(bool isAttackCommand, int x, int y);
    static void Attack(GameObject* gameObject);

    static bool WorldToScreen(Vector3 position, Vector2& out);
    static Vector3 WorldMousePosition();
    static Vector2 MousePosition();
};
