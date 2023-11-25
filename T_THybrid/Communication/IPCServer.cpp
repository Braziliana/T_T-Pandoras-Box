#include "IPCServer.h"

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
        //Functions::PrintChat("Failed to create pipe");
        CloseHandle(pipe);
        return false;
    }
        
    //Functions::PrintChat("Pipe created");
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
        switch (packet.commandType)
        {
        case CommandType::PrintChat:
            {
                const auto printChatCommand = packet.ToPrintChatCommand();
                printChatCommand.Handle();
                break;
            }
        case CommandType::IssueOrder:
            {
                const auto issueOrderCommand = packet.ToIssueOrderCommand();
                issueOrderCommand.Handle();
                break;
            }
        case CommandType::CastSpell:
            {
                const auto spellCastCommand = packet.ToCastSpellCommand();
                spellCastCommand.Handle();
                break;
            }
        case CommandType::LevelSpell:
            {
                const auto levelSpellCommand = packet.ToLevelSpellCommand();
                levelSpellCommand.Handle();
                break;
            }
        }
    }
}

bool IPCServer::IsRunning()
{
    return pipe != INVALID_HANDLE_VALUE;
}
