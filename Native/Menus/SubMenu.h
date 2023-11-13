#pragma once
#include "MenuBase.h"
#include "MenuItem.h"

class SubMenu : public MenuBase
{
public:
    SubMenu(MenuItem* parent, const std::string& title, const Rect rect);

    void Render() override;
};
