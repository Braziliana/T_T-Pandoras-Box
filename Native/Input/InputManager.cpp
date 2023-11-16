#include "InputManager.h"
#include <Windows.h>

#include <utility>

#include "../Rendering/Renderer.h"


void InputManagerReset()
{
    InputManager::GetInstance()->Reset();
}

bool InputManagerGetKeyState(unsigned short vkCode)
{
    return InputManager::GetInstance()->GetKeyState(vkCode);
}

void InputManagerGetMousePosition(Vector2* vector)
{
    if(vector == nullptr)
    {
        return;
    }
    
    *vector = InputManager::GetInstance()->GetMousePosition();
}


int InputManagerAddMouseMoveHandler(MouseMoveEventHandler handler)
{
    return InputManager::GetInstance()->AddMouseMoveHandler(handler);
}

void InputManagerRemoveMouseMoveHandler(int key)
{
    InputManager::GetInstance()->RemoveMouseMoveHandler(key);
}

int InputManagerAddKeyStateEventHandler(KeyStateEventHandler handler)
{
    return InputManager::GetInstance()->AddKeyStateEventHandler(handler);
}

void InputManagerRemoveKeyStateEventHandler(int key)
{
    InputManager::GetInstance()->RemoveKeyStateEventHandler(key);
}

InputManager* InputManager::_instance = nullptr;
std::once_flag InputManager::_initInstanceFlag;

LRESULT InputManager::LowLevelKeyboardProc(const int nCode, const WPARAM wParam, const LPARAM lParam)
{
    if (nCode == HC_ACTION) {
        const auto pKeyboard = reinterpret_cast<KBDLLHOOKSTRUCT*>(lParam);
        const auto key = pKeyboard->vkCode;
        const auto isDown = wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN;
        const auto isInjected = pKeyboard->flags & LLKHF_INJECTED || pKeyboard->flags & LLKHF_LOWER_IL_INJECTED;
        GetInstance()->UpdateKeyState(key, isDown, isInjected);

        if (isInjected) {
            pKeyboard->flags &= ~LLKHF_INJECTED;
            pKeyboard->flags &= ~LLKHF_LOWER_IL_INJECTED;
            return CallNextHookEx(nullptr, nCode, wParam, reinterpret_cast<LPARAM>(pKeyboard));
        }
    }
    return CallNextHookEx(nullptr, nCode, wParam, lParam);
}

LRESULT InputManager::LowLevelMouseProc(const int nCode, const WPARAM wParam, const LPARAM lParam)
{

    if (nCode == HC_ACTION) {
        const auto pMouse = reinterpret_cast<MSLLHOOKSTRUCT*>(lParam);
            
        const auto isInjected = pMouse->flags & LLMHF_INJECTED || pMouse->flags & LLMHF_LOWER_IL_INJECTED;
        auto instance = GetInstance();
        switch (wParam)
        {
        case WM_LBUTTONDOWN:
            instance->UpdateKeyState(VK_LBUTTON, true, isInjected);
            break;
        case WM_LBUTTONUP:
            instance->UpdateKeyState(VK_LBUTTON, false, isInjected);
            break;
        case WM_RBUTTONDOWN:
            instance->UpdateKeyState(VK_RBUTTON, true, isInjected);
            break;
        case WM_RBUTTONUP:
            instance->UpdateKeyState(VK_RBUTTON, false, isInjected);
            break;
        case WM_MBUTTONDOWN:
            instance->UpdateKeyState(VK_MBUTTON, true, isInjected);
            break;
        case WM_MBUTTONUP:
            instance->UpdateKeyState(VK_MBUTTON, false, isInjected);
            break;
        case WM_XBUTTONDOWN:
            if (HIWORD(pMouse->mouseData) == XBUTTON1)
            {
                instance->UpdateKeyState(VK_XBUTTON1, true, isInjected);
            }
            else if (HIWORD(pMouse->mouseData) == XBUTTON2)
            {
                instance->UpdateKeyState(VK_XBUTTON2, true, isInjected);
            }
            break;
        case WM_XBUTTONUP:
            if (HIWORD(pMouse->mouseData) == XBUTTON1)
            {
                instance->UpdateKeyState(VK_XBUTTON1, false, isInjected);
            }
            else if (HIWORD(pMouse->mouseData) == XBUTTON2)
            {
                instance->UpdateKeyState(VK_XBUTTON2, false, isInjected);
            }
            break;
        case WM_MOUSEMOVE:
            instance->UpdateMousePosition(static_cast<float>(pMouse->pt.x), static_cast<float>(pMouse->pt.y), isInjected);
            break;
        default:
            break;
        }
        
        if (isInjected) {
            pMouse->flags &= ~LLMHF_INJECTED;
            pMouse->flags &= ~LLMHF_LOWER_IL_INJECTED;

            return CallNextHookEx(nullptr, nCode, wParam, reinterpret_cast<LPARAM>(pMouse));
        }
        if(instance->_blockUserMouseInput)
        {
            return 1;
        }
    }
        
    return CallNextHookEx(nullptr, nCode, wParam, lParam);
}

void InputManager::HookThreadFunction()
{
    _keyboardHook = SetWindowsHookEx(WH_KEYBOARD_LL, LowLevelKeyboardProc, nullptr, 0);
    _mouseHook = SetWindowsHookEx(WH_MOUSE_LL, LowLevelMouseProc, nullptr, 0);

    MSG msg;
    ZeroMemory(&msg, sizeof(MSG));
    while (msg.message != WM_QUIT && _running) {
        if (PeekMessage(&msg, nullptr, 0U, 0U, PM_REMOVE))
        {
            TranslateMessage(&msg);
            DispatchMessage(&msg);
        }

        if(!_running)
        {
            break;
        }
    }

    UnhookWindowsHookEx(_keyboardHook);
    UnhookWindowsHookEx(_mouseHook);
}

InputManager::InputManager(): _running(false), _keyboardHook(nullptr), _mouseHook(nullptr)
{

}

void InputManager::UpdateKeyState(const unsigned short vkCode, const bool isPressed, const bool isInjected)
{
    //TODO Test if we want store injected states
    const bool wasPressed = _keyStates[vkCode].exchange(isPressed, std::memory_order_relaxed);
    if(isInjected)
    {
        return;
    }
        
    if (!wasPressed && isPressed)
    {
        PostKeyStateEvent({vkCode, true});
    }
    else if (wasPressed && !isPressed)
    {
        PostKeyStateEvent({vkCode, false});
    }
}

void InputManager::PostKeyStateEvent(const KeyStateEvent& event)
{
    std::lock_guard<std::mutex> lock(_keyStateEventQueueMutex);
    _keyStateEventQueue.push(event);
    _keyStateEventQueueCondition.notify_one();
}

void InputManager::UpdateMousePosition(const float x, const float y, const bool isInjected)
{
    //TODO Test if we want store injected states
    const auto position = Vector2(x, y);
    const auto previousPosition = _mousePosition.exchange(position, std::memory_order_relaxed);
    const auto delta = position - previousPosition;
    _mouseMoveDelta.store(delta, std::memory_order_relaxed);

    if(isInjected)
    {
        return;
    }

    PostMouseMoveEvent({position, delta});
}

void InputManager::PostMouseMoveEvent(const MouseMoveEvent& event)
{
    std::lock_guard<std::mutex> lock(_mouseMoveEventQueueMutex);
    _mouseMoveEventQueue.push(event);
    _mouseMoveEventQueueCondition.notify_one();
}

InputManager* InputManager::GetInstance()
{
    std::call_once(_initInstanceFlag, []() {
        _instance = new InputManager();
    });
    return _instance;
}

void InputManager::Start()
{
    if (!_running) {
        _running = true;
        _hookThread = std::thread(&InputManager::HookThreadFunction, this);
    }
}

void InputManager::Stop()
{
    if (_running) {
        _running = false;
        if (_hookThread.joinable()) {
            _hookThread.join();
        }
    }
}

void InputManager::Reset()
{
    Stop();
    Start();
}

bool InputManager::GetKeyState(const unsigned short vkCode) const
{
    if (vkCode < _keyStates.size()) {
        return _keyStates[vkCode].load(std::memory_order_relaxed);
    }
    return false;
}

Vector2 InputManager::GetMousePosition() const
{
    return _mousePosition.load(std::memory_order_relaxed);
}

void InputManager::ProcessInputEvents()
{
    if(!_onMouseMoveEvent.empty())
    {
        std::unique_lock<std::mutex> mouseMoveLock(_mouseMoveEventQueueMutex);
        while (!_mouseMoveEventQueue.empty()) {
            const auto event = _mouseMoveEventQueue.front();
            _mouseMoveEventQueue.pop();
            
            mouseMoveLock.unlock();
            for (const auto& handler: _onMouseMoveEvent) {
                handler.second(event);
            }
            mouseMoveLock.lock();
        }
    }

    if(!_onKeyStateEvent.empty())
    {
        std::unique_lock<std::mutex> keyStateLock(_keyStateEventQueueMutex);
        while (!_keyStateEventQueue.empty()) {
            const auto event = _keyStateEventQueue.front();
            _keyStateEventQueue.pop();
            
            keyStateLock.unlock();
            for (const auto& handler : _onKeyStateEvent) {
                handler.second(event);
            }
            keyStateLock.lock();
        }
    }
}

int InputManager::AddMouseMoveHandler(MouseMoveEventHandler handler)
{
    const auto key = _mouseEventListenerId;
    _onMouseMoveEvent[key] = handler;
    _mouseEventListenerId++;
    return key;
}

int InputManager::AddMouseMoveHandler(std::function<void(MouseMoveEvent)> handler)
{
    const auto key = _mouseEventListenerId;
    _onMouseMoveEvent[key] = std::move(handler);
    _mouseEventListenerId++;
    return key;
}

void InputManager::RemoveMouseMoveHandler(const int key)
{
    _onMouseMoveEvent.erase(key);
}

int InputManager::AddKeyStateEventHandler(KeyStateEventHandler handler)
{
    const auto key = keyStateEventId;
    _onKeyStateEvent[key] = handler;
    keyStateEventId++;
    return key;
}

int InputManager::AddKeyStateEventHandler(std::function<void(KeyStateEvent)> handler)
{
    const auto key = keyStateEventId;
    _onKeyStateEvent[key] = std::move(handler);
    keyStateEventId++;
    return key;
}

void InputManager::RemoveKeyStateEventHandler(int key)
{
    _onKeyStateEvent.erase(key);
}

void InputManager::SetBlockUserMouseInput(const bool blockInput)
{
    _blockUserMouseInput = blockInput;
}

INPUT InputManager::CreateMouseClickInput(unsigned short vkCode, bool down)
{
    INPUT input;
    ZeroMemory(&input, sizeof(INPUT));

    input.type = INPUT_MOUSE;
    input.mi.mouseData = 0;
    input.mi.dwFlags = (vkCode == VK_LBUTTON) ? (down ? MOUSEEVENTF_LEFTDOWN : MOUSEEVENTF_LEFTUP) : 
                       (vkCode == VK_RBUTTON) ? (down ? MOUSEEVENTF_RIGHTDOWN : MOUSEEVENTF_RIGHTUP) : 0;
    return input;
}

INPUT InputManager::CreateMouseMoveInput(const Vector2& position)
{
    static auto screenWidth = static_cast<float>(GetSystemMetrics(SM_CXSCREEN));
    static auto screenHeight = static_cast<float>(GetSystemMetrics(SM_CYSCREEN));
    INPUT input;
    ZeroMemory(&input, sizeof(INPUT));

    input.type = INPUT_MOUSE;
    input.mi.dx = static_cast<int>((position.x * 65535) / screenWidth);
    input.mi.dy = static_cast<int>((position.y * 65535) / screenHeight);
    input.mi.dwFlags = MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE;
    return input;
}

void InputManager::MouseSendDown(unsigned short vkCode)
{
    MouseSend(vkCode, true);
}

void InputManager::MouseSendUp(unsigned short vkCode)
{
    MouseSend(vkCode, false);
}

void InputManager::MouseMove(const Vector2& position)
{
    INPUT input = CreateMouseMoveInput(position);
    SendInput(1, &input, sizeof(INPUT));
}

void InputManager::MouseSend(unsigned short vkCode, bool down)
{
    INPUT input = CreateMouseClickInput(vkCode, down);
    SendInput(1, &input, sizeof(INPUT));
}

void InputManager::MouseSend(unsigned short vkCode)
{
    INPUT inputs[2];
    inputs[0] = CreateMouseClickInput(vkCode, true);
    inputs[1] = CreateMouseClickInput(vkCode, false);
    SendInputs(inputs, 2);
}

INPUT InputManager::CreateKeyboardInput(unsigned short vkCode, bool down)
{
    INPUT input;
    ZeroMemory(&input, sizeof(INPUT));

    input.type = INPUT_KEYBOARD;
    input.ki.wVk = vkCode;
    input.ki.wScan = MapVirtualKey(vkCode, MAPVK_VK_TO_VSC);
    input.ki.dwFlags = KEYEVENTF_SCANCODE | (down ? 0 : KEYEVENTF_KEYUP);
    return input;
}

void InputManager::KeyboardSendDown(unsigned short vkCode)
{
    KeyboardSend(vkCode, true);
}

void InputManager::KeyboardSendUp(unsigned short vkCode)
{
    KeyboardSend(vkCode, false);
}

void InputManager::KeyboardSend(unsigned short vkCode, bool down)
{
    INPUT input = CreateKeyboardInput(vkCode, down);
    SendInput(1, &input, sizeof(INPUT));
}

void InputManager::KeyboardSend(unsigned short vkCode)
{
    INPUT inputs[2];
    inputs[0] = CreateKeyboardInput(vkCode, true);
    inputs[1] = CreateKeyboardInput(vkCode, false);
    SendInputs(inputs, 2);
}

void InputManager::SendInputs(INPUT* inputs, unsigned count)
{
    SendInput(count, inputs, sizeof(INPUT));
}

InputManager::~InputManager()
{
    Stop();
}


INPUT InputManagerCreateMouseClickInput(const unsigned short vkCode, const bool down)
{
    return InputManager::CreateMouseClickInput(vkCode, down);
}

INPUT InputManagerCreateMouseMoveInput(const Vector2* position)
{
    return InputManager::CreateMouseMoveInput(*position);
}

void InputManagerMouseSendDown(const unsigned short vkCode)
{
    InputManager::MouseSendDown(vkCode);
}

void InputManagerMouseSendUp(const unsigned short vkCode)
{
    InputManager::MouseSendUp(vkCode);
}

void InputManagerMouseMove(const Vector2* position)
{
    InputManager::MouseMove(*position);
}

void InputManagerMouseSend(const unsigned short vkCode)
{
    InputManager::MouseSend(vkCode);
}

INPUT InputManagerCreateKeyboardInput(unsigned short vkCode, bool down)
{
    return InputManager::CreateKeyboardInput(vkCode, down);
}

void InputManagerKeyboardSendDown(unsigned short vkCode)
{
    InputManager::KeyboardSendDown(vkCode);
}

void InputManagerKeyboardSendUp(unsigned short vkCode)
{
    InputManager::KeyboardSendUp(vkCode);
}

void InputManagerKeyboardSend(unsigned short vkCode)
{
    InputManager::KeyboardSend(vkCode);
}

void InputManagerSendInputs(INPUT* inputs, unsigned count)
{
    InputManager::SendInputs(inputs, count);
}

void InputManagerSetBlockUserMouseInput(const bool blockInput)
{
    InputManager::GetInstance()->SetBlockUserMouseInput(blockInput);
}
