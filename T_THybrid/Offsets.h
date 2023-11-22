#pragma once
#include <cstdint>
#include <mutex>
#include <Windows.h>

class Offsets
{
    Offsets()
    {
        const auto base = reinterpret_cast<uintptr_t>(GetModuleHandle(nullptr));
        ChatClient += base;
        PrintChat += base;
        GameTime += base;
    }
    
    static Offsets* _instance;
    static std::once_flag _onceFlag;
public:

    Offsets(const Offsets&) = delete;
    Offsets& operator=(const Offsets&) = delete;
    
    static Offsets* GetInstance()
    {
        std::call_once(_onceFlag, []() {
            _instance = new Offsets();
        });
        return _instance;
    }
    
    uintptr_t ChatClient = 0x2211A60;
    uintptr_t PrintChat = 0x86ACB0;
    uintptr_t GameTime = 0x21FE6F8;
};
