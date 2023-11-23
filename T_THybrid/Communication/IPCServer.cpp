#include "IPCServer.h"

#include "../Game//Functions.h"

IPCServer::IPCServer(std::wstring name): pipeName(std::move(name)), pipe(INVALID_HANDLE_VALUE)
{}

IPCServer::~IPCServer()
{
    if (pipe != INVALID_HANDLE_VALUE) {
        DisconnectNamedPipe(pipe);
        CloseHandle(pipe);
    }
}

bool IPCServer::Start()
{
    constexpr DWORD size = sizeof(Packet);
    pipe = CreateNamedPipe(
        (L"\\\\.\\pipe\\" + pipeName).c_str(), 
        PIPE_ACCESS_DUPLEX, 
        PIPE_TYPE_BYTE | PIPE_READMODE_BYTE | PIPE_WAIT, //PIPE_TYPE_MESSAGE | PIPE_READMODE_MESSAGE | PIPE_WAIT
        PIPE_UNLIMITED_INSTANCES, 
        size * 16,
        size * 16, 
        NMPWAIT_USE_DEFAULT_WAIT,
        nullptr);

    if (pipe == INVALID_HANDLE_VALUE) {
        return false;
    }

    const BOOL connected = ConnectNamedPipe(pipe, nullptr) ? TRUE : (GetLastError() == ERROR_PIPE_CONNECTED);
    if (!connected) {
        CloseHandle(pipe);
        return false;
    }
        
    return true;
}

void IPCServer::Run() const
{
    constexpr DWORD size = sizeof(Packet);
    byte buffer[size];
    DWORD bytesRead;
    if(ReadFile(pipe, buffer, size, &bytesRead, nullptr))
    {
        Packet packet = *reinterpret_cast<Packet*>(buffer);
        switch (packet.commandType) {
        case CommandType::PrintChat:
                auto printChatCommand = packet.ToPrintChatCommand();
                Functions::PrintChat(printChatCommand.message);
            break;
        default: ;
        }
    }
}
