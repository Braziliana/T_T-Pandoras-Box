#pragma once
#include <Windows.h>

#include "Commands/CastSpellCommand.h"
#include "Commands/Command.h"
#include "Commands/CommandType.h"
#include "Commands/IssueOrderCommand.h"
#include "Commands/LevelSpellCommand.h"
#include "Commands/PrintChatCommand.h"

struct alignas(1024) Packet
{
    CommandType commandType;
    int payloadSize;
    byte payload[1016];

    Packet() = delete;
    
    Packet(const PrintChatCommand& printChatCommand)
    {
        commandType = CommandType::PrintChat;
        payloadSize = sizeof(PrintChatCommand);
        if(payloadSize > 1016)
        {
            payloadSize = 1016;
        }
        memcpy_s(&payload, 1016, &printChatCommand, payloadSize);
    }

    Packet(const IssueOrderCommand& issueOrderCommand)
    {
        commandType = CommandType::IssueOrder;
        payloadSize = sizeof(IssueOrderCommand);
        if(payloadSize > 1016)
        {
            payloadSize = 1016;
        }
        memcpy_s(&payload, 1016, &issueOrderCommand, payloadSize);
    }

    Packet(const CastSpellCommand& castSpellCommand)
    {
        commandType = CommandType::CastSpell;
        payloadSize = sizeof(CastSpellCommand);
        if(payloadSize > 1016)
        {
            payloadSize = 1016;
        }
        memcpy_s(&payload, 1016, &castSpellCommand, payloadSize);
    }

    Packet(const LevelSpellCommand& levelSpellCommand)
    {
        commandType = CommandType::LevelSpell;
        payloadSize = sizeof(LevelSpellCommand);
        if(payloadSize > 1016)
        {
            payloadSize = 1016;
        }
        memcpy_s(&payload, 1016, &levelSpellCommand, payloadSize);
    }
};
