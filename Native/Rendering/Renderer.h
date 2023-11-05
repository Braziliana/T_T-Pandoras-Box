﻿#pragma once
#include <Windows.h>

#include "Primitives/RectRenderer.h"
#include "TextRendering/TextRenderer.h"
#include "../Math/Color.h"
#include "../Math/Vector2.h"
#include "Primitives/CircleRenderer.h"


extern "C"{
    typedef void (*RenderCallback)(float deltaTime);
}


class Renderer
{
private:
    static Renderer* _instance;
    static std::once_flag _initInstanceFlag;
    
    HDC _hdc;
    int _width = 0;
    int _height = 0;
    
    Color _clearColor = {0, 0, 0, 1.0};
    RenderCallback _renderCallback = nullptr;
    RenderCallback _renderGuiCallback = nullptr;

    RectRenderer* _rectRenderer = nullptr;
    CircleRenderer* _circleRenderer = nullptr;
    TextRenderer* _textRenderer = nullptr;
    
    Renderer(HDC hdc);
public:
    ~Renderer();
    
    bool Init(int width, int height);
    void SetRenderCallback(RenderCallback callback);
    void SetRenderHudCallback(RenderCallback callback);
    void Render(float deltaTime);
    void Release();

    void SetClearColor(Color color);

    void Begin3D() const;
    void Begin2D() const;
    
    void DrawRect(const Vector2& position, const Vector2& size, const Color& color) const;
    void DrawRect(const Vector3& position, const Vector2& size, const Color& color) const;
    void DrawCircle(const Vector2& position, const Vector2& size, const Color& color) const;
    void DrawCircle(const Vector3& position, const Vector2& size, const Color& color) const;
    void Text(const std::string& text, const Vector2& position, float size, const Color& color) const;
    void TextCenter(const std::string& text, const Vector2& position, float size, const Color& color) const;
    
    
    static Renderer* CreateInstance(HDC hdc, int width, int height);
    static Renderer* Instance();
    static void Destroy();
};

extern "C" {
    __declspec(dllexport) void RegisterRenderCallback(RenderCallback callback);
    __declspec(dllexport) void RegisterRenderHudCallback(RenderCallback callback);
    
    __declspec(dllexport) void RenderSetClearColor(const Color& color);

    __declspec(dllexport) void RendererDrawRect2D(Vector2* position, Vector2* size, Color* color);
    __declspec(dllexport) void RendererDrawRect3D(Vector3* position, Vector2* size, Color* color);
    __declspec(dllexport) void RendererDrawCircle2D(Vector2* position, Vector2* size, Color* color);
    __declspec(dllexport) void RendererDrawCircle3D(Vector3* position, Vector2* size, Color* color);
    __declspec(dllexport) void RendererText(const char* text, Vector2* position, float size, Color* color);
    __declspec(dllexport) void RendererTextCenter(const char* text, Vector2* position, float size, Color* color);

}