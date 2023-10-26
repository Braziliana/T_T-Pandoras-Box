#pragma once
#include "Rendering/Renderer.h"

class AppWindow
{
private:
    int _width;
    int _height;
    HWND _hWnd;
    Renderer* _renderer;
    
public:
    AppWindow(int width, int height);
    ~AppWindow();

    Renderer* GetRenderer() const;
};
