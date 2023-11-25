#pragma once
#include <mutex>
#include <string>

#include <windows.h>
#include "Packet.h"
#include "../Math/Vector2.h"
#include "../Math/Vector3.h"

class IPCClient
{
private:
    static IPCClient* _instance;
    static std::once_flag _initInstanceFlag;
    
    std::wstring pipeName;
    HANDLE pipe;
    bool SendPacket(const Packet& packet);
    IPCClient(std::wstring pipeName);
public:
    static IPCClient* GetInstance();
    bool IsConnected();
    bool Connect();
    void Disconnect();
    bool PrintChat(const std::string& message);
    bool CastSpell(int spellSlot);
    bool CastSpellSelf(int spellSlot);
    bool CastSpell(int spellSlot, Vector2 screenPosition);
    bool CastSpell(int spellSlot, Vector3 worldPosition);
    bool CastSpell(int spellSlot, uintptr_t target);
    bool MoveTo(Vector2 screenPosition);
    bool MoveTo(Vector3 worldPosition);
    bool Attack(uintptr_t target);

    bool LevelSpell(int spellSlot);
};


extern "C"
{
    __declspec(dllexport) IPCClient* IPCClient_GetInstance();
    __declspec(dllexport) bool IPCClient_IsConnected();
    __declspec(dllexport) bool IPCClient_Connect();
    __declspec(dllexport) void IPCClient_Disconnect();

    __declspec(dllexport) bool IPCClient_PrintChat(const char* message);
    
    __declspec(dllexport) bool IPCClient_CastSpell(int spellSlot);
    __declspec(dllexport) bool IPCClient_CastSpellSelf(int spellSlot);
    __declspec(dllexport) bool IPCClient_CastSpellScreenPosition(int spellSlot, const Vector2* screenPosition);
    __declspec(dllexport) bool IPCClient_CastSpellWorldPosition(int spellSlot, const Vector3* worldPosition);
    __declspec(dllexport) bool IPCClient_CastSpellTarget(int spellSlot, uintptr_t target);
    
    __declspec(dllexport) bool IPCClient_MoveTo(const Vector2* screenPosition);
    __declspec(dllexport) bool IPCClient_MoveToWorldPosition(const Vector3* worldPosition);
    __declspec(dllexport) bool IPCClient_Attack(uintptr_t target);

    __declspec(dllexport) bool IPCClient_LevelSpell(int spellSlot);
}
