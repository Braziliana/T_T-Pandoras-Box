#include "IPCClient.h"

#include <iostream>

#include "Commands/CastSpellCommand.h"
#include "Commands/LevelSpellCommand.h"
#include "Commands/PrintChatCommand.h"

IPCClient* IPCClient::_instance = nullptr;
std::once_flag IPCClient::_initInstanceFlag;

bool IPCClient::SendPacket(const Packet& packet)
{
    if (pipe == INVALID_HANDLE_VALUE) {
        return false;
    }
    
    constexpr DWORD size = sizeof(Packet);
    byte buffer[size];
    DWORD bytesWritten;
    
    memcpy(buffer, &packet, size);
  
    auto result = WriteFile(
        pipe, 
        buffer, 
        size, 
        &bytesWritten, 
        nullptr);

    if(!result)
    {
        printf("WriteFile failed, GetLastError: %d\n", GetLastError());
    }
    
    return result;
}

IPCClient::IPCClient(std::wstring pipeName): pipeName(std::move(pipeName))
{
    pipe = INVALID_HANDLE_VALUE;
    std::cout << "IPCClient created" << std::endl;
}

IPCClient* IPCClient::GetInstance()
{
    std::call_once(_initInstanceFlag, []() {
        _instance = new IPCClient(L"HelloWorld");
   });
    return _instance;
}

bool IPCClient::Connect()
{
    pipe = CreateFile(
      (L"\\\\.\\pipe\\" + pipeName).c_str(),
      GENERIC_READ | GENERIC_WRITE,
      0,
      nullptr,
      OPEN_EXISTING,
      0,
      nullptr);

    if (pipe == INVALID_HANDLE_VALUE) {
        printf("CreateFile failed, GetLastError: %d\n", GetLastError());
        std::wcout << "Failed to connect to " << pipeName << std::endl;
        pipe = INVALID_HANDLE_VALUE;
        return false;
    }
    
    std::cout << "Connected to ipc" << std::endl;
    return true;
}

void IPCClient::Disconnect()
{
    if(pipe != INVALID_HANDLE_VALUE) {
        CloseHandle(pipe);
        pipe = INVALID_HANDLE_VALUE;
    }
}

bool IPCClient::PrintChat(const std::string& message)
{
    const auto command = PrintChatCommand(message);
    return SendPacket(Packet(command));
}

bool IPCClient::CastSpell(int spellSlot)
{
    const auto command = CastSpellCommand(spellSlot, false);
    return SendPacket(Packet(command));
}

bool IPCClient::CastSpellSelf(int spellSlot)
{
    const auto command = CastSpellCommand(spellSlot, true);
    return SendPacket(Packet(command));
}

bool IPCClient::CastSpell(int spellSlot, Vector2 screenPosition)
{
    const auto command = CastSpellCommand(spellSlot, screenPosition);
    return SendPacket(Packet(command));
}

bool IPCClient::CastSpell(int spellSlot, Vector3 worldPosition)
{
    const auto command = CastSpellCommand(spellSlot, worldPosition);
    return SendPacket(Packet(command));
}

bool IPCClient::CastSpell(int spellSlot, uintptr_t target)
{
    const auto command = CastSpellCommand(spellSlot, target);
    return SendPacket(Packet(command));
}

bool IPCClient::MoveTo(const Vector2 screenPosition)
{
    const auto command = IssueOrderCommand(screenPosition);
    return SendPacket(Packet(command));
}

bool IPCClient::MoveTo(Vector3 worldPosition)
{    const auto command = IssueOrderCommand(worldPosition);
    return SendPacket(Packet(command));
}

bool IPCClient::Attack(const uintptr_t target)
{
    const auto command = IssueOrderCommand(target);
    return SendPacket(Packet(command));
}

bool IPCClient::LevelSpell(int spellSlot)
{
    const auto command = LevelSpellCommand(spellSlot);
    return SendPacket(Packet(command));
}

bool IPCClient::IsConnected()
{
    return pipe != INVALID_HANDLE_VALUE;
}

///////

IPCClient* IPCClient_GetInstance()
{
    return IPCClient::GetInstance();
}

bool IPCClient_Connect()
{
    return IPCClient::GetInstance()->Connect();
}

void IPCClient_Disconnect()
{
    IPCClient::GetInstance()->Disconnect();
}

bool IPCClient_PrintChat(const char* message)
{
    return IPCClient::GetInstance()->PrintChat(std::string(message));
}

bool IPCClient_CastSpellScreenPosition(const int spellSlot, const Vector2* screenPosition)
{
    return IPCClient::GetInstance()->CastSpell(spellSlot, *screenPosition);
}

bool IPCClient_MoveTo(const Vector2* screenPosition)
{
    return IPCClient::GetInstance()->MoveTo(*screenPosition);
}

bool IPCClient_Attack(const uintptr_t target)
{
    return IPCClient::GetInstance()->Attack(target);
}

bool IPCClient_IsConnected()
{
    return IPCClient::GetInstance()->IsConnected();
}

bool IPCClient_CastSpellSelf(int spellSlot)
{
    return IPCClient::GetInstance()->CastSpellSelf(spellSlot);
}

bool IPCClient_CastSpellWorldPosition(int spellSlot, const Vector3* worldPosition)
{
    return IPCClient::GetInstance()->CastSpell(spellSlot, *worldPosition);
}

bool IPCClient_CastSpellTarget(int spellSlot, uintptr_t target)
{
    return IPCClient::GetInstance()->CastSpell(spellSlot, target);
}

bool IPCClient_CastSpell(int spellSlot)
{
    return IPCClient::GetInstance()->CastSpell(spellSlot);
}

bool IPCClient_MoveToWorldPosition(const Vector3* worldPosition)
{
    return IPCClient::GetInstance()->MoveTo(*worldPosition);
}

bool IPCClient_LevelSpell(int spellSlot)
{
    return IPCClient::GetInstance()->LevelSpell(spellSlot);
}
