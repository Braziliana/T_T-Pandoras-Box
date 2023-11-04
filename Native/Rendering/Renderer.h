#pragma once
#include <Windows.h>

#define GLEW_STATIC
#include <GL/glew.h>
#include "RectRenderer.h"
#include "TextRendering/TextRenderer.h"
#include "Vertex.h"
#include "../Math/Color.h"
#include "../Math/Vector2.h"


extern "C"{
    typedef void (*RenderCallback)(float deltaTime);
}


class Renderer
{
private:
    static Renderer* _instance;
    
    HDC _hdc;
    int _width = 0;
    int _height = 0;
    
    Color _clearColor = {0, 0, 0, 1.0};
    RenderCallback _renderCallback = nullptr;
    RenderCallback _renderGuiCallback = nullptr;

    RectRenderer* _rectRenderer = nullptr;
    TextRenderer* _textRenderer = nullptr;
    
public:
    Renderer(HDC hdc);
    ~Renderer();
    bool Init(int width, int height);
    void SetRenderCallback(RenderCallback callback);
    void SetRenderGuiCallback(RenderCallback callback);
    void Render(float deltaTime);
    void Release();

    void SetClearColor(Color color);

    void Begin3D() const;
    void Begin2D() const;
    
    void DrawRect(const Vector2& position, const Vector2& size, const Color& color);
    void DrawRect(const Vector3& position, const Vector2& size, const Color& color);
    void DrawCircle(const Vector2& position, const Vector2& size, const Color& color);
    void DrawCircle(const Vector3& position, const Vector2& size, const Color& color);

    static Renderer* CreateInstance(HDC hdc, int width, int height);
    static Renderer* Instance();
    static void Destroy();
};

extern "C" {
    __declspec(dllexport) void RegisterRenderCallback(Renderer* renderer, RenderCallback callback);
    __declspec(dllexport) void RegisterRenderGuiCallback(Renderer* renderer, RenderCallback callback);
    __declspec(dllexport) void RenderSetClearColor(Renderer* renderer, Color color);
}