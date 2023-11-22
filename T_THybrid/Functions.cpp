#include <iostream>

#include "Functions.h"

#include <sstream>

#include "SpoofCall.h"

#include "Offsets.h"

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

    //printChat(offsets->ChatClient, msg, 0);
    spoof_call(offsets->SpoofTrampoline, printChat, offsets->ChatClient, msg, 0);
}

void Functions::RightClick(int x, int y)
{
    static auto offsets = Offsets::GetInstance();
    typedef bool(__fastcall* RightClickFunc)(uintptr_t playerInstance, unsigned int* params);
    static RightClickFunc rightClick = reinterpret_cast<RightClickFunc>(offsets->RightClick);

    unsigned int* params = new unsigned int[20];
    params[17] = x;
    params[18] = y;
    params[19] = 2;

    spoof_call(offsets->SpoofTrampoline, rightClick, offsets->LocalPlayer, params);
}
