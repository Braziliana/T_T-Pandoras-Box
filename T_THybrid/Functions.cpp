#include "Functions.h"

#include "Offsets.h"

void Functions::PrintChat(const char* msg)
{
    static auto offsets = Offsets::GetInstance();
    typedef void(__thiscall* PrintChatFunc)(uintptr_t thisPtr, const char* msg, int type);
    static const auto printChat = reinterpret_cast<PrintChatFunc>(offsets->PrintChat);
    printChat(offsets->ChatClient, msg, 0);
}
