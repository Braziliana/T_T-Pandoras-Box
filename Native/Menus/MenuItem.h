#pragma once
#include <string>

class MenuItem
{
private:
    std::string _title;
public:
    virtual void Render();
};
