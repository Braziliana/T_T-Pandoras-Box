#pragma once
#include <Windows.h>

#include "Commands/Command.h"
#include "Commands/CommandType.h"
#include "Commands/PrintChatCommand.h"

struct Packet
{
    CommandType commandType;
    int payloadSize;
    byte payload[256];

    PrintChatCommand& ToPrintChatCommand()
    {
        return *reinterpret_cast<PrintChatCommand*>(payload);
    }
};
