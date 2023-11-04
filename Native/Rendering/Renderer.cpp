#include "Renderer.h"

#include <iostream>
#include "Vertex.h"
#include "../Math/Color.h"
#include "glm/ext/matrix_clip_space.hpp"

Renderer* Renderer::_instance = nullptr;

void RegisterRenderCallback(Renderer* renderer, const RenderCallback callback)
{
    if(renderer == nullptr) return;
    
    renderer->SetRenderCallback(callback);
}

void RegisterRenderGuiCallback(Renderer* renderer, const RenderCallback callback)
{
    if(renderer == nullptr) return;
    
    renderer->SetRenderGuiCallback(callback);
}

void RenderSetClearColor(Renderer* renderer, Color color)
{
    if(renderer == nullptr) return;
    
    renderer->SetClearColor(color);
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
    _textRenderer = new TextRenderer();
    
    glEnable(GL_BLEND);
    glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);
    
    glClearColor(0.0f, 0.0f, 0.0f, 0.0f);
    return true;
}

void Renderer::SetRenderCallback(const RenderCallback callback)
{
    _renderCallback = callback;
}

void Renderer::SetRenderGuiCallback(RenderCallback callback)
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

void Renderer::DrawCircle(const Vector2& position, const Vector2& size, const Color& color)
{
    DrawCircle(Vector3(position.x, position.y, 0.0f), size, color);
}

void Renderer::DrawCircle(const Vector3& position, const Vector2& size, const Color& color)
{
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
    
    _textRenderer->DrawCenter("Test", Vector2(100, 100), 21, Color(1.0f, 0.0f, 0.0f, 1.0f));
    _textRenderer->Draw("WWW", Vector2(300, 400), 36, Color(1.0f, 0.0f, 0.0f, 1.0f));
    _textRenderer->DrawCenter("WWW", Vector2(300, 400), 36, Color(0.0f, 1.0f, 0.0f, 1.0f));
    _textRenderer->Draw("WabTcdGe`?,.=+-0", Vector2(300, 500), 48, Color(1.0f, 0.0f, 0.0f, 1.0f));
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
