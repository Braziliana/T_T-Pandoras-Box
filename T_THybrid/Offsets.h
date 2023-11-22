#pragma once
#include <cstdint>
#include <mutex>
#include <Windows.h>

class Offsets
{
    Offsets();

    static Offsets* _instance;
    static std::once_flag _onceFlag;
public:

    Offsets(const Offsets&) = delete;
    Offsets& operator=(const Offsets&) = delete;
    
    static Offsets* GetInstance();

    void* SpoofTrampoline = 0x0;
    
    uintptr_t GameTime = 0x21FE6F8;
    uintptr_t LocalPlayer = 0x22118D8;
    
    uintptr_t ChatClient = 0x2211A60;
    uintptr_t PrintChat = 0x86ACB0;

    uintptr_t RightClick = 0x8B6C50;
};
