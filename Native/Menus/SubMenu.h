﻿#pragma once
#include "MenuBase.h"
#include "MenuItem.h"

class SubMenu : public MenuBase
{
public:
    SubMenu(const std::string& title, const Rect rect) : MenuBase(title, rect) {  }
};