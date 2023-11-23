#include <iostream>

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

void Functions::PrintChat(const char* msg)
{
    static auto offsets = Offsets::GetInstance();
    typedef void(__thiscall* PrintChatFunc)(uintptr_t chatInstance, const char* msg, int type);
    static PrintChatFunc printChat = reinterpret_cast<PrintChatFunc>(offsets->PrintChat);

    spoof_call(offsets->SpoofTrampoline, printChat, offsets->ChatClient, msg, 0);
}

void Functions::MoveTo(int x, int y)
{
    IssueOrder(false, x, y);
}

void Functions::MoveToMouse()
{
    const auto mousePos = MousePosition();
    MoveTo(static_cast<int>(mousePos.x), static_cast<int>(mousePos.y));
}

void Functions::IssueOrder(bool isAttackCommand, int x, int y)
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
    static auto offsets = Offsets::GetInstance();
    typedef bool(__fastcall* WorldToScreenFunc)(uintptr_t viewport, Vector3* in, Vector3* out);
    static auto worldToScreenFunc = reinterpret_cast<WorldToScreenFunc>(offsets->WorldToScreen);
    Vector3 funcOut;
    auto result = spoof_call(offsets->SpoofTrampoline, worldToScreenFunc, (offsets->ViewPort + 0x270), &position, &funcOut);
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
