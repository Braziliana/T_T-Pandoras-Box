#include <iostream>

#include "Functions.h"
#include "GameObject.h"
#include <sstream>

#include "Hero.h"
#include "Spell.h"
#include "../SpoofCall.h"
#include "../Offsets.h"
#include "Hud.h"

std::string GetHexString(uintptr_t hexNumber)
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

Hud* Functions::GetHud()
{
    static auto offsets = Offsets::GetInstance();
    return *reinterpret_cast<Hud**>(offsets->HudInstance);
}

void Functions::MoveTo(const int x, const int y)
{
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

    auto hud = GetHud();
    auto hudInput = hud->GetOrderHandle();
    static auto issueOrder = reinterpret_cast<IssueOrderFunc>(offsets->IssueOrder);
    
    const auto mousePos = MousePosition();
    SetMousePosition(Vector2(static_cast<float>(x), static_cast<float>(y)));
    spoof_call(offsets->SpoofTrampoline, issueOrder, hudInput, 0, 0, static_cast<int>(isAttackCommand), x, y, 0);
    spoof_call(offsets->SpoofTrampoline, issueOrder, hudInput, 1, 0, static_cast<int>(isAttackCommand), x, y, 0);
    SetMousePosition(mousePos);
}

void Functions::Attack(GameObject* gameObject)
{
    Vector2 position;
    if(WorldToScreen(gameObject->GetPosition(), position))
    {
        IssueOrder(true, static_cast<int>(position.x), static_cast<int>(position.y));
    }
}

void Functions::CastSpell(const int spellSlot, GameObject* target, const Vector3 endPosition)
{
	static auto offsets = Offsets::GetInstance();

    const auto hudInput = GetHud()->GetCastHandle();
    
    const auto hudMouseInfo = GetHud()->GetMouseInfo();
	const auto localPlayer = Hero::LocalPlayer();
    const auto spell = localPlayer->GetSpell(spellSlot);
	const auto spellInput = spell->GetSpellInput();
    const auto spellInfo = spell->GetSpellInfo();
    const auto hudMouseWorldPosition = hudMouseInfo->GetMousePosition();
    const auto hudMouseTarget = hudMouseInfo->GetUnderMouseObjectHandle();
    
	spellInput->SetCaster(localPlayer->GetHandle());

	if(target == nullptr)
	{
		spellInput->SetTarget(0);
	    hudMouseInfo->SetTargetHandle(0);
	}
	else
	{
		spellInput->SetTarget(target->GetHandle());
	    hudMouseInfo->SetTargetHandle(target->GetHandle());
	}

	spellInput->SetEndPosition(endPosition);
	spellInput->SetClickPosition(endPosition);
	spellInput->SetEndPosition2(endPosition);

    hudInput->SetSpellInfo(spellInfo);
    hudMouseInfo->SetMouseWorldPosition(endPosition);
    
	typedef void(__fastcall* HudCastSpellFunc)(HudCastSpell* hudInput, SpellInfo* spellInfo);
	static auto castSpellFunc = reinterpret_cast<HudCastSpellFunc>(offsets->CastSpell);
	spoof_call(offsets->SpoofTrampoline, castSpellFunc, hudInput, spellInfo);
    
    hudInput->SetSpellInfo(nullptr);
    hudMouseInfo->SetMouseWorldPosition(hudMouseWorldPosition);
    hudMouseInfo->SetTargetHandle(hudMouseTarget);
}

void Functions::CastSpell(const int spellSlot, GameObject* gameObject)
{
	CastSpell(spellSlot, gameObject, gameObject->GetPosition());
}

void Functions::CastSpell(const int spellSlot, const Vector3 position)
{
	CastSpell(spellSlot, nullptr, position);
}

void Functions::CastSpell(const int spellSlot)
{
	CastSpell(spellSlot, nullptr, WorldMousePosition());
}

void Functions::CastSpellSelf(const int spellSlot)
{
    const auto localPlayer = Hero::LocalPlayer();
	CastSpell(spellSlot, localPlayer, localPlayer->GetPosition());
}

void Functions::CastSpellClick(int spellSlot, Vector2 screenPosition)
{
    static auto offsets = Offsets::GetInstance();

    const auto hudInput = GetHud()->GetCastHandle();
    
    const auto localPlayer = Hero::LocalPlayer();
    const auto spellInfo = localPlayer->GetSpell(spellSlot)->GetSpellInfo();

    typedef void(__fastcall* HudCastSpellFunc)(HudCastSpell* hudInput, SpellInfo* spellInfo, int isDown);
    static auto castSpellFunc = reinterpret_cast<HudCastSpellFunc>(offsets->CastSpellClick);

    const auto mousePos = MousePosition();
    SetMousePosition(screenPosition);
    spoof_call(offsets->SpoofTrampoline, castSpellFunc, hudInput, spellInfo, 0);
    spoof_call(offsets->SpoofTrampoline, castSpellFunc, hudInput, spellInfo, 1);
    SetMousePosition(mousePos);
}

bool Functions::WorldToScreen(Vector3 position, Vector2& out)
{
    static auto offsets = Offsets::GetInstance();
    typedef bool(__fastcall* WorldToScreenFunc)(uintptr_t* viewport, Vector3* in, Vector3* out);
    static auto worldToScreenFunc = reinterpret_cast<WorldToScreenFunc>(offsets->WorldToScreen);
    Vector3 funcOut;

    uintptr_t* viewport = *reinterpret_cast<uintptr_t**>(offsets->ViewPort);
    viewport = reinterpret_cast<uintptr_t*>(reinterpret_cast<uintptr_t>(viewport) + 0x270);
    auto result = spoof_call(offsets->SpoofTrampoline, worldToScreenFunc, viewport, &position, &funcOut);
    out.x = funcOut.x;
    out.y = funcOut.y;
    return result;
}

Vector3 Functions::WorldMousePosition()
{
    return GetHud()->GetMouseInfo()->GetMousePosition();
}

Vector2 Functions::MousePosition()
{
    static auto offsets = Offsets::GetInstance();
    const auto mouse = *reinterpret_cast<uintptr_t*>(offsets->MouseScreenPosition);
    const int x = *reinterpret_cast<int*>(mouse + 0xC);
    const int y = *reinterpret_cast<int*>(mouse + 0x10);
    return {static_cast<float>(x), static_cast<float>(y)};
}

void Functions::SetMousePosition(Vector2 position)
{
    static auto offsets = Offsets::GetInstance();
    const auto mouse = *reinterpret_cast<uintptr_t*>(offsets->MouseScreenPosition);
    *reinterpret_cast<int*>(mouse + 0xC) = static_cast<int>(position.x);
    *reinterpret_cast<int*>(mouse + 0x10) = static_cast<int>(position.y);
}