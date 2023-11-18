#pragma once
#include <Windows.h>

#include "Primitives/RectRenderer.h"
#include "TextRendering/TextRenderer.h"
#include "../Math/Color.h"
#include "../Math/Vector2.h"
#include "glm/ext/matrix_clip_space.hpp"
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
    
    Color _clearColor = {0, 0, 0, 0.0};
    RenderCallback _renderCallback = nullptr;
    RenderCallback _renderGuiCallback = nullptr;

    RectRenderer* _rectRenderer = nullptr;
    CircleRenderer* _circleRenderer = nullptr;
    TextRenderer* _textRenderer = nullptr;
    glm::mat4 _viewProjectionMatrix;
    
    Renderer(HDC hdc);
public:
    ~Renderer();
    
    bool Init(int width, int height);
    void SetRenderCallback(RenderCallback callback);
    void SetRenderHudCallback(RenderCallback callback);
    void Render(float deltaTime);
    void Release();

    void SetClearColor(Color color);

    void RectFilled(const Vector2& position, const Vector2& size, const Color& color) const;
    void RectFilled(const Vector3& position, const Vector2& size, const Color& color) const;
    void RectFilledBordered(const Vector2& position, const Vector2& size, const Color& color, const Color& borderColor, float borderSize) const;
    void RectFilledBordered(const Vector3& position, const Vector2& size, const Color& color, const Color& borderColor, float borderSize) const;
    void RectBorder(const Vector2& position, const Vector2& size, const Color& color, float borderSize) const;
    void RectBorder(const Vector3& position, const Vector2& size, const Color& color, float borderSize) const;
    
    void CircleFilled(const Vector2& position, float size, const Color& color) const;
    void CircleFilled(const Vector3& position, float size, const Color& color) const;
    void CircleFilledBordered(const Vector2& position, float size, const Color& color, const Color& borderColor, float borderSize) const;
    void CircleFilledBordered(const Vector3& position, float size, const Color& color, const Color& borderColor, float borderSize) const;
    void CircleBorder(const Vector2& position, float size, const Color& color, float borderSize) const;
    void CircleBorder(const Vector3& position, float size, const Color& color, float borderSize) const;

    
    void Text(const std::string& text, const Vector2& position, float size, const Color& color, TextHorizontalOffset textHorizontalOffset, TextVerticalOffset textVerticalOffset) const;
    void Text(const std::string& text, const Vector2& start, const Vector2& end, float size, const Color& color, TextHorizontalOffset textHorizontalOffset, TextVerticalOffset textVerticalOffset) const;

    void Text(const std::string& text, const Vector3& position, float size, const Color& color, TextHorizontalOffset textHorizontalOffset, TextVerticalOffset textVerticalOffset) const;

    static Renderer* CreateInstance(HDC hdc, int width, int height);
    static Renderer* Instance();
    static void Destroy();

    glm::mat4 Get2DMatrix() const;
    glm::mat4 Get3DMatrix() const;
    void Set3DMatrix(const glm::mat4& matrix);
};

extern "C" {
    __declspec(dllexport) void RegisterRenderCallback(RenderCallback callback);
    __declspec(dllexport) void RegisterRenderHudCallback(RenderCallback callback);
    
    __declspec(dllexport) void RenderSetClearColor(const Color* color);

    __declspec(dllexport) void RendererRectFilled2D(const Vector2* position, const Vector2* size, const Color* color);
    __declspec(dllexport) void RendererRectFilled3D(const Vector3* position, const Vector2* size, const Color* color);
    __declspec(dllexport) void RendererRectFilledBordered2D(const Vector2* position, const Vector2* size, const Color* color, const Color* borderColor, float borderSize);
    __declspec(dllexport) void RendererRectFilledBordered3D(const Vector3* position, const Vector2* size, const Color* color, const Color* borderColor, float borderSize);
    __declspec(dllexport) void RendererRectBorder2D(const Vector2* position, const Vector2* size, const Color* color, float borderSize);
    __declspec(dllexport) void RendererRectBorder3D(const Vector3* position, const Vector2* size, const Color* color, float borderSize);
    
    __declspec(dllexport) void RendererCircleFilled2D(const Vector2* position, float size, const Color* color);
    __declspec(dllexport) void RendererCircleFilled3D(const Vector3* position, float size, const Color* color);
    __declspec(dllexport) void RendererCircleFilledBordered2D(const Vector2* position, float size, const Color* color, const Color* borderColor, float borderSize);
    __declspec(dllexport) void RendererCircleFilledBordered3D(const Vector3* position, float size, const Color* color, const Color* borderColor, float borderSize);
    __declspec(dllexport) void RendererCircleBorder2D(const Vector2* position, float size, const Color* color, float borderSize);
    __declspec(dllexport) void RendererCircleBorder3D(const Vector3* position, float size, const Color* color, float borderSize);
    
    __declspec(dllexport) void RendererText2D(const char* text, const Vector2* position, float size, const Color* color, TextHorizontalOffset textHorizontalOffset, TextVerticalOffset textVerticalOffset);
    __declspec(dllexport) void RendererTextRect2D(const char* text, const Vector2* start, const Vector2* end, float size, const Color* color, TextHorizontalOffset textHorizontalOffset, TextVerticalOffset textVerticalOffset);

    __declspec(dllexport) void RendererText3D(const char* text, const Vector3* position, float size, const Color* color, TextHorizontalOffset textHorizontalOffset, TextVerticalOffset textVerticalOffset);

    __declspec(dllexport) void RendererSet3DMatrix(const glm::mat4* matrix);
}