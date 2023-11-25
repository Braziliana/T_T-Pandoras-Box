#include "Offsets.h"

#include <psapi.h>
#include <vector>
Offsets* Offsets::_instance = nullptr;
std::once_flag Offsets::_onceFlag;

bool ScanByteArray(const BYTE* data, const std::vector<BYTE>& pattern, const std::string& mask) {
    for (size_t i = 0; i < pattern.size(); i++) {
        if (mask[i] != '?' && data[i] != pattern[i]) {
            return false;
        }
    }
    return true;
}

byte* PatternScan(byte* begin, size_t size, const std::vector<BYTE>& pattern, const std::string& mask, int pos)
{
    MEMORY_BASIC_INFORMATION mbi{};

    for (byte* curr = begin; curr < begin + size; curr += mbi.RegionSize)
    {
        if (!VirtualQuery(curr, &mbi, sizeof(mbi)) || mbi.State != MEM_COMMIT || mbi.Protect == PAGE_NOACCESS) continue;

        for(size_t i = 0; i < mbi.RegionSize - pattern.size(); i++)
        {
            if(ScanByteArray(curr + i, pattern, mask))
            {
                return curr + i + pos;
            }
        }
    }

    return nullptr;
}

Offsets::Offsets()
{
    const auto base = reinterpret_cast<uintptr_t>(GetModuleHandle(nullptr));

    MODULEINFO moduleInfo;
    if (GetModuleInformation(GetCurrentProcess(), reinterpret_cast<HMODULE>(base), &moduleInfo, sizeof(MODULEINFO)))
    {
        SpoofTrampoline = static_cast<void*>(PatternScan(reinterpret_cast<byte*>(base), moduleInfo.SizeOfImage, std::vector<BYTE>{0xFF, 0x23}, "xx", 0));
    }

    MouseScreenPosition += base;
    GameTime += base;
    LocalPlayer += base;
    
    ChatClient += base;
    PrintChat += base;

    ViewPort += base;
    WorldToScreen += base;
    
    HudInstance += base;
    IssueOrder += base;
    LevelSpell += base;
    CastSpell += base;
    CastSpellClick += base;
}

Offsets* Offsets::GetInstance()
{
    std::call_once(_onceFlag, []() {
        _instance = new Offsets();
    });
    return _instance;
}
