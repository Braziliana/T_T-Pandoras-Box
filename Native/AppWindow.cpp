#include "AppWindow.h"

AppWindow::AppWindow(const int width, const int height) : _width(width), _height(height)
{
    _renderer = new Renderer();
    _renderer->Init(_hWnd, width, height);
}

AppWindow::~AppWindow()
{
    delete _renderer;
}

Renderer* AppWindow::GetRenderer() const
{
    return _renderer;
}
