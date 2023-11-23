#pragma once
#include <Windows.h>

#include "Commands/Command.h"
#include "Commands/CommandType.h"
#include "Commands/PrintChatCommand.h"

struct alignas(1024) Packet
{
    CommandType commandType;
    int payloadSize;
    byte payload[1016];

    PrintChatCommand& ToPrintChatCommand()
    {
        return *reinterpret_cast<PrintChatCommand*>(payload);
    }
};
