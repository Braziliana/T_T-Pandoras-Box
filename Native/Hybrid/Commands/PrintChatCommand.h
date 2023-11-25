#pragma once
#include "Command.h"

struct PrintChatCommand : Command
{
    PrintChatCommand(const std::string& message)
    {
        memcpy_s(this->message, 998, message.c_str(), message.length());
        if(999 > message.length())
        {
            this->message[message.length()] = '\0';    
        }
        else
        {
            this->message[999] = '\0';
        }
    }
    char message[1000];
};