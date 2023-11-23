#pragma once
#include "Command.h"

struct PrintChatCommand : Command
{
    char message[1000];
};
