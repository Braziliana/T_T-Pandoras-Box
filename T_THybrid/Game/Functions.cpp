﻿#include <iostream>

#include "Functions.h"
#include "GameObject.h"
#include <sstream>
#include "../SpoofCall.h"
#include "../Offsets.h"

std::string GetHexString(int hexNumber)
{
    std::stringstream ss;
    ss << std::hex << hexNumber;
    return ss.str();
}

std::string GetString(Vector2 pos)
{
    std::stringstream ss;
    ss << "x: " << pos.x << " y: " << pos.y;
    return ss.str();
}

std::string GetString(Vector3 pos)
{
    std::stringstream ss;
    ss << "x: " << pos.x << " y: " << pos.y << " z: " << pos.z;
    return ss.str();
}

void Functions::PrintChat(const char* msg)
{
    static auto offsets = Offsets::GetInstance();
    typedef void(__thiscall* PrintChatFunc)(uintptr_t chatInstance, const char* msg, int type);
    static PrintChatFunc printChat = reinterpret_cast<PrintChatFunc>(offsets->PrintChat);

    spoof_call(offsets->SpoofTrampoline, printChat, offsets->ChatClient, msg, 0);
}

void Functions::MoveTo(const int x, const int y)
{
    PrintChat(GetString(Vector2(x, y)).c_str());
    IssueOrder(false, x, y);
}

void Functions::MoveTo(const Vector2 position)
{
    MoveTo(static_cast<int>(position.x), static_cast<int>(position.y));
}

void Functions::MoveTo(const Vector3 worldPosition)
{
    Vector2 screenPosition;
    if(WorldToScreen(worldPosition, screenPosition))
    {
        MoveTo(screenPosition);
    }
}

void Functions::MoveToMouse()
{
    MoveTo(MousePosition());
}

void Functions::IssueOrder(const bool isAttackCommand, const int x, const int y)
{
    static auto offsets = Offsets::GetInstance();
    typedef bool(__fastcall* IssueOrderFunc)(uintptr_t hudInput, int state, int isAttack, int isAttackCommand, int x, int y, int attackAndMove);
    auto hudInput = *reinterpret_cast<uintptr_t*>(*reinterpret_cast<uintptr_t*>(offsets->HudInstance) + 0x48);
    static auto issueOrder = reinterpret_cast<IssueOrderFunc>(offsets->IssueOrder);
    spoof_call(offsets->SpoofTrampoline, issueOrder, hudInput, 0, 0, static_cast<int>(isAttackCommand), x, y, 0);
    spoof_call(offsets->SpoofTrampoline, issueOrder, hudInput, 1, 0, static_cast<int>(isAttackCommand), x, y, 0);
}

void Functions::Attack(GameObject* gameObject)
{
    Vector2 position;
    if(WorldToScreen(gameObject->GetPosition(), position))
    {
        IssueOrder(true, static_cast<int>(position.x), static_cast<int>(position.y));
    }
}

bool Functions::WorldToScreen(Vector3 position, Vector2& out)
{
    PrintChat(GetString(position).c_str());
    static auto offsets = Offsets::GetInstance();
    typedef bool(__fastcall* WorldToScreenFunc)(uintptr_t* viewport, Vector3* in, Vector3* out);
    static auto worldToScreenFunc = reinterpret_cast<WorldToScreenFunc>(offsets->WorldToScreen);
    Vector3 funcOut;

    uintptr_t* viewport = *reinterpret_cast<uintptr_t**>(offsets->ViewPort);
    viewport = reinterpret_cast<uintptr_t*>(reinterpret_cast<uintptr_t>(viewport) + 0x270);
    auto result = spoof_call(offsets->SpoofTrampoline, worldToScreenFunc, viewport, &position, &funcOut);
    PrintChat(result ? "true" : "false");
    out.x = funcOut.x;
    out.y = funcOut.y;
    return result;
}

Vector3 Functions::WorldMousePosition()
{
    static auto offsets = Offsets::GetInstance();
    uintptr_t MousePtr = *reinterpret_cast<uintptr_t*>(*reinterpret_cast<uintptr_t*>(offsets->HudInstance) + 0x28);
     
    Vector3 mousePos;
     
    mousePos.x = *reinterpret_cast<float*>(MousePtr + 0x20);
    mousePos.y = *reinterpret_cast<float*>(MousePtr + 0x30);
    mousePos.z = *reinterpret_cast<float*>(MousePtr + 0x28);
    return mousePos;
}

Vector2 Functions::MousePosition()
{
    Vector2 mousePosition;
    WorldToScreen(WorldMousePosition(), mousePosition);
    return mousePosition;
}
