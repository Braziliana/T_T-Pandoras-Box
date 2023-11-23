#pragma once
#include "Command.h"

#include "../../Game/Functions.h"
struct PrintChatCommand : Command
{
    char message[1000];

    void Handle() const;
};

inline auto PrintChatCommand::Handle() const -> void
{
    Functions::PrintChat(message);
}
