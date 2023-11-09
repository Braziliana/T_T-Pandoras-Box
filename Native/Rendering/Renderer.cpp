#include "Renderer.h"

#include <iostream>
#include "Vertex.h"
#include "../Input/InputManager.h"
#include "../Math/Color.h"
#include "../Menus/Menu.h"
#include "../Menus/SubMenu.h"
#include "glm/ext/matrix_clip_space.hpp"

Renderer* Renderer::_instance = nullptr;
std::once_flag Renderer::_initInstanceFlag;

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
    Renderer::Instance()->RectFilled(*position, *size, *color);
}

void RendererDrawRect3D(Vector3* position, Vector2* size, Color* color) {
    Renderer::Instance()->RectFilled(*position, *size, *color);
}

void RendererDrawCircle2D(Vector2* position, Vector2* size, Color* color) {
    Renderer::Instance()->DrawCircle(*position, *size, *color);
}

void RendererDrawCircle3D(Vector3* position, Vector2* size, Color* color) {
    Renderer::Instance()->DrawCircle(*position, *size, *color);
}

void RendererText(const char* text, Vector2* position, float size, Color* color) {
    //Renderer::Instance()->Text(std::string(text), *position, size, *color);
}

void RendererTextCenter(const char* text, Vector2* position, float size, Color* color) {
    //Renderer::Instance()->TextCenter(std::string(text), *position, size, *color);
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

    glEnable(GL_MULTISAMPLE);

    glEnable(GL_BLEND);
    glBlendFuncSeparate(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA, GL_ONE, GL_ONE);
    
    glDisable(GL_DEPTH_TEST);
    glDepthMask(GL_FALSE);
    
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

void Renderer::DrawCircle(const Vector2& position, const Vector2& size, const Color& color) const
{
    _circleRenderer->Draw(position, size, color);
}

void Renderer::DrawCircle(const Vector3& position, const Vector2& size, const Color& color) const
{
    _circleRenderer->Draw(position, size, color);
}

void Renderer::Text(const std::string& text, const Vector2& position, const float size, const Color& color, const TextHorizontalOffset textHorizontalOffset, const TextVerticalOffset textVerticalOffset) const
{
    _textRenderer->Draw(text, position, size, color, textHorizontalOffset, textVerticalOffset);
}

auto Renderer::Text(const std::string& text, const Vector2& start, const Vector2& end, const float size, const Color& color,
                    const TextHorizontalOffset textHorizontalOffset, const TextVerticalOffset textVerticalOffset) const -> void
{
    _textRenderer->Draw(text, start, end, size, color, textHorizontalOffset, textVerticalOffset);
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

glm::mat4 Renderer::Get2DMatrix() const
{
    return glm::ortho(0.0f, static_cast<float>(_width), static_cast<float>(_height), 0.0f, -1.0f, 1.0f);    
}

glm::mat4 Renderer::Get3DMatrix() const
{
    return _viewProjectionMatrix;    
}

void Renderer::Set3DMatrix(const glm::mat4& matrix)
{
    _viewProjectionMatrix = matrix;
}

bool t1;
void Renderer::Render(const float deltaTime)
{
    glClearColor(_clearColor.r, _clearColor.g, _clearColor.b, _clearColor.a);
    glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
    
    if(_renderCallback != nullptr)
    {
        _renderCallback(deltaTime);
    }
    
    if(_renderGuiCallback != nullptr)
    {
        _renderGuiCallback(deltaTime);
    }
    
    if(!t1)
    {
        auto m = Menu::GetInstance();
        m->AddSubMenu("Test sub menu 1");
        auto mm = m->AddSubMenu("Test sub menu 2");
        mm->AddSubMenu("Sub sub menu 1");
        mm->AddToggle("Test toggle", false);
        mm->AddSubMenu("Sub sub menu 1");
        mm->AddToggle("Test toggle", false);
        mm->AddSubMenu("Sub sub menu 1");
        mm->AddToggle("Test toggle", false);
        m->AddSubMenu("Test sub menu - 3");
        m->AddFloatSlider("Test float slider", 0.532, 0.0f, 1.0f, 0.05f, 2);
        m->AddSubMenu("Test sub menu - 3");
        m->AddSubMenu("Test sub menu - 3");
        t1=true;
    }
    auto m = Menu::GetInstance();
    m->Render();
    
    _rectRenderer->Flush2D();
    _circleRenderer->Flush2D();
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

void Renderer::RectFilled(const Vector2& position, const Vector2& size, const Color& color) const
{
    _rectRenderer->Filled(position, size, color);
}

void Renderer::RectFilled(const Vector3& position, const Vector2& size, const Color& color) const
{
    _rectRenderer->Filled(position, size, color);
}

void Renderer::RectFilledBordered(const Vector2& position, const Vector2& size, const Color& color,
    const Color& borderColor, const float borderSize) const
{
    _rectRenderer->FilledBordered(position, size, color, borderColor, borderSize);
}

void Renderer::RectFilledBordered(const Vector3& position, const Vector2& size, const Color& color,
    const Color& borderColor, const float borderSize) const
{
    _rectRenderer->FilledBordered(position, size, color, borderColor, borderSize);
}

void Renderer::RectBorder(const Vector2& position, const Vector2& size, const Color& color, const float borderSize) const
{
    _rectRenderer->Border(position, size, color, borderSize);
}

void Renderer::RectBorder(const Vector3& position, const Vector2& size, const Color& color, const float borderSize) const
{
    _rectRenderer->Border(position, size, color, borderSize);
}