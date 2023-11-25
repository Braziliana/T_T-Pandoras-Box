#pragma once
#include <windows.h>
#include <string>
#include <iostream>

#include "Packet.h"

class IPCServer
{
private:
    std::wstring pipeName;
    HANDLE pipe;
public:
    IPCServer(std::wstring name);
    ~IPCServer();

    bool Start();
    void Run() const;
    bool IsRunning();
};
