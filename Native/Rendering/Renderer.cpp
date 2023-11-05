#include "Renderer.h"

#include <iostream>
#include "Vertex.h"
#include "../Input/InputManager.h"
#include "../Math/Color.h"
#include "glm/ext/matrix_clip_space.hpp"

Renderer* Renderer::_instance = nullptr;

void RegisterRenderCallback(const RenderCallback callback)
{
    Renderer::Instance()->SetRenderCallback(callback);
}

void RegisterRenderHudCallback(const RenderCallback callback)
{
    Renderer::Instance()->SetRenderHudCallback(callback);
}

void RenderSetClearColor(const Color& color)
{
    Renderer::Instance()->SetClearColor(color);
}

void RendererDrawRect2D(Vector2* position, Vector2* size, Color* color) {
    Renderer::Instance()->DrawRect(*position, *size, *color);
}

void RendererDrawRect3D(Vector3* position, Vector2* size, Color* color) {
    Renderer::Instance()->DrawRect(*position, *size, *color);
}

void RendererDrawCircle2D(Vector2* position, Vector2* size, Color* color) {
    Renderer::Instance()->DrawCircle(*position, *size, *color);
}

void RendererDrawCircle3D(Vector3* position, Vector2* size, Color* color) {
    Renderer::Instance()->DrawCircle(*position, *size, *color);
}

void RendererText(const char* text, Vector2* position, float size, Color* color) {
    Renderer::Instance()->Text(std::string(text), *position, size, *color);
}

void RendererTextCenter(const char* text, Vector2* position, float size, Color* color) {
    Renderer::Instance()->TextCenter(std::string(text), *position, size, *color);
}

Renderer::Renderer(const HDC hdc) : _hdc(hdc)
{
}

Renderer::~Renderer()
{
    Release();
}

bool Renderer::Init(const int width, const int height)
{
    _width = width;
    _height = height;

    glewInit();
    if (glewInit() != GLEW_OK) {
        std::cerr << "Failed to initialize GLEW" << std::endl;
        return false;
    }

    _rectRenderer = new RectRenderer();
    _circleRenderer = new CircleRenderer();
    _textRenderer = new TextRenderer();
    
    glEnable(GL_BLEND);
    glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);
    
    return true;
}

void Renderer::SetRenderCallback(const RenderCallback callback)
{
    _renderCallback = callback;
}

void Renderer::SetRenderHudCallback(RenderCallback callback)
{
    _renderGuiCallback = callback;
}

void Renderer::Begin3D() const
{
   
}

void Renderer::Begin2D() const
{
    const glm::mat4 projection = glm::ortho(0.0f, static_cast<float>(_width), static_cast<float>(_height), 0.0f, -1.0f, 1.0f);
    ShaderManager::GetInstance().SetViewProjectionMatrix(projection);
}

void Renderer::DrawRect(const Vector2& position, const Vector2& size, const Color& color) const
{
    _rectRenderer->Draw(position, size, color);
}

void Renderer::DrawRect(const Vector3& position, const Vector2& size, const Color& color) const
{
    _rectRenderer->Draw(position, size, color);
}

void Renderer::DrawCircle(const Vector2& position, const Vector2& size, const Color& color) const
{
    _circleRenderer->Draw(position, size, color);
}

void Renderer::DrawCircle(const Vector3& position, const Vector2& size, const Color& color) const
{
    _circleRenderer->Draw(position, size, color);
}

void Renderer::Text(const std::string& text, const Vector2& position, const float size, const Color& color) const
{
    _textRenderer->Draw(text, position, size, color);
}

void Renderer::TextCenter(const std::string& text, const Vector2& position, const float size, const Color& color) const
{
    _textRenderer->DrawCenter(text, position, size, color);
}

Renderer* Renderer::CreateInstance(const HDC hdc, int width, int height)
{
    Destroy();
        
    _instance = new Renderer(hdc);
    _instance->Init(width, height);
    return _instance;
}

Renderer* Renderer::Instance()
{
    return _instance;
}

void Renderer::Destroy()
{
    if(_instance != nullptr)
    {
        _instance->Release();
        delete _instance;
        _instance=nullptr;
    }
}

void Renderer::Render(const float deltaTime)
{
    glClearColor(_clearColor.r, _clearColor.g, _clearColor.b, _clearColor.a);
    glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
    
    Begin3D();
    if(_renderCallback != nullptr)
    {
        _renderCallback(deltaTime);
    }
    
    Begin2D();    
    if(_renderGuiCallback != nullptr)
    {
        _renderGuiCallback(deltaTime);
    }
    DrawRect(Vector2(100, 100), Vector2(100, 26), Color(0.0f, 1.0f, 0.0f, 1.0f));
    _rectRenderer->Flush2D();
    DrawCircle(Vector2(500, 500), Vector2(100, 100), Color(1.0f, 0.0f, 1.0f, 1.0f));
    _circleRenderer->Flush2D();

    Text("Test", Vector2(100, 100), 21, Color(1.0f, 0.0f, 0.0f, 1.0f));
    Text("WWW", Vector2(300, 400), 36, Color(1.0f, 0.0f, 0.0f, 1.0f));
    TextCenter("WWW", Vector2(300, 400), 36, Color(0.0f, 1.0f, 0.0f, 1.0f));
    Text("WabTcdGe`?,.=+-0", Vector2(300, 500), 48, Color(1.0f, 0.0f, 0.0f, 1.0f));
    _textRenderer->Flush2D();
    SwapBuffers(_hdc);
}

void Renderer::Release()
{
    if(_rectRenderer != nullptr)
    {
        _rectRenderer->Release();
        delete _rectRenderer;
        _rectRenderer = nullptr;
    }

    if(_circleRenderer != nullptr)
    {
        _circleRenderer->Release();
        delete _circleRenderer;
        _circleRenderer=nullptr;
    }
    
    if(_textRenderer != nullptr)
    {
        _textRenderer->Release();
        delete _textRenderer;
        _textRenderer = nullptr;
    }
}

void Renderer::SetClearColor(const Color color)
{
    _clearColor = color;
}
