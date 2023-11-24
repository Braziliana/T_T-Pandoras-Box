#pragma once
#include <cstdint>
#include <mutex>
#include <Windows.h>

namespace GameObjectOffsets
{
    constexpr uintptr_t Handle = 0x0010;
    constexpr uintptr_t Name = 0x0060;
    constexpr uintptr_t Team = 0x003C;
    constexpr uintptr_t IsVisible = 0x0340;
    constexpr uintptr_t Expiry = 0x0298;
    constexpr uintptr_t Position = 0x0220;
    constexpr uintptr_t ObjectName = 0x38A8;
}

namespace HeroOffsets
{
    constexpr uintptr_t SpellBook = 0x2A38;
    constexpr uintptr_t SpellBookSpellSlot = 0x6D0;
}

class Offsets
{
    Offsets();

    static Offsets* _instance;
    static std::once_flag _onceFlag;
public:

    Offsets(const Offsets&) = delete;
    Offsets& operator=(const Offsets&) = delete;
    
    static Offsets* GetInstance();

    void* SpoofTrampoline = nullptr;

    uintptr_t MouseScreenPosition = 0x21f6f08;
    uintptr_t GameTime = 0x21FE6F8;
    uintptr_t LocalPlayer = 0x22118D8;
    
    uintptr_t ChatClient = 0x2211A60;
    uintptr_t PrintChat = 0x86ACB0;
    
    uintptr_t ViewPort = 0x21F6F00;
    uintptr_t WorldToScreen = 0xE4B480;
    
    uintptr_t HudInstance = 0x21F3ED0;
    uintptr_t IssueOrder = 0x8CDE50;
    uintptr_t IssueMove = 0x8B6C50;
    uintptr_t CastSpell = 0x8c2880;
    uintptr_t CastSpellClick = 0x8b9750;
};
