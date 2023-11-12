#pragma once
#include "MenuItem.h"

class ComboBox : MenuItem
{
public:
    ComboBox(MenuItem* parent, const std::string& title, const Rect& rect);
};
