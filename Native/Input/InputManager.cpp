#include "InputManager.h"
#include <Windows.h>


bool InputManagerGetKeyState(unsigned vkCode)
{
    return InputManager::GetInstance()->GetKeyState(vkCode);
}

Vector2 InputManagerGetMousePosition()
{
    return InputManager::GetInstance()->GetMousePosition();
}

void InputManagerSetOnMouseMoveEvent(void(* handler)(MouseMoveEvent))
{
    auto lambda = [handler](MouseMoveEvent event) {
        handler(event);
    };
    InputManager::GetInstance()->SetOnMouseMoveEvent(lambda);
}

void InputManagerSetKeyStateEvent(void(* handler)(KeyStateEvent))
{
    auto lambda = [handler](KeyStateEvent event) {
        handler(event);
    };
    InputManager::GetInstance()->SetKeyStateEvent(lambda);
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
            
        switch (wParam)
        {
        case WM_LBUTTONDOWN:
            GetInstance()->UpdateKeyState(VK_LBUTTON, true, isInjected);
            break;
        case WM_LBUTTONUP:
            GetInstance()->UpdateKeyState(VK_LBUTTON, false, isInjected);
            break;
        case WM_RBUTTONDOWN:
            GetInstance()->UpdateKeyState(VK_RBUTTON, true, isInjected);
            break;
        case WM_RBUTTONUP:
            GetInstance()->UpdateKeyState(VK_RBUTTON, false, isInjected);
            break;
        case WM_MBUTTONDOWN:
            GetInstance()->UpdateKeyState(VK_MBUTTON, true, isInjected);
            break;
        case WM_MBUTTONUP:
            GetInstance()->UpdateKeyState(VK_MBUTTON, false, isInjected);
            break;
        case WM_XBUTTONDOWN:
            if (HIWORD(pMouse->mouseData) == XBUTTON1)
            {
                GetInstance()->UpdateKeyState(VK_XBUTTON1, true, isInjected);
            }
            else if (HIWORD(pMouse->mouseData) == XBUTTON2)
            {
                GetInstance()->UpdateKeyState(VK_XBUTTON2, true, isInjected);
            }
            break;
        case WM_XBUTTONUP:
            if (HIWORD(pMouse->mouseData) == XBUTTON1)
            {
                GetInstance()->UpdateKeyState(VK_XBUTTON1, false, isInjected);
            }
            else if (HIWORD(pMouse->mouseData) == XBUTTON2)
            {
                GetInstance()->UpdateKeyState(VK_XBUTTON2, false, isInjected);
            }
            break;
        case WM_MOUSEMOVE:
            GetInstance()->UpdateMousePosition(static_cast<float>(pMouse->pt.x), static_cast<float>(pMouse->pt.y), isInjected);
            break;
        default:
            break;
        }
        
        if (isInjected) {
            pMouse->flags &= ~LLMHF_INJECTED;
            pMouse->flags &= ~LLMHF_LOWER_IL_INJECTED;

            return CallNextHookEx(nullptr, nCode, wParam, reinterpret_cast<LPARAM>(pMouse));
        }
    }
        
    return CallNextHookEx(nullptr, nCode, wParam, lParam);
}

void InputManager::HookThreadFunction()
{
    _keyboardHook = SetWindowsHookEx(WH_KEYBOARD_LL, LowLevelKeyboardProc, nullptr, 0);
    _mouseHook = SetWindowsHookEx(WH_MOUSE_LL, LowLevelMouseProc, nullptr, 0);

    MSG msg;
    while (_running && GetMessage(&msg, nullptr, 0, 0)) {
        TranslateMessage(&msg);
        DispatchMessage(&msg);
    }

    UnhookWindowsHookEx(_keyboardHook);
    UnhookWindowsHookEx(_mouseHook);
}

InputManager::InputManager(): _running(false), _keyboardHook(nullptr), _mouseHook(nullptr)
{

}

void InputManager::UpdateKeyState(const unsigned vkCode, const bool isPressed, const bool isInjected)
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

bool InputManager::GetKeyState(const unsigned vkCode) const
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
    if(_onMouseMoveEvent)
    {
        std::unique_lock<std::mutex> mouseMoveLock(_mouseMoveEventQueueMutex);
        while (!_mouseMoveEventQueue.empty()) {
            const auto event = _mouseMoveEventQueue.front();
            _mouseMoveEventQueue.pop();
            
            mouseMoveLock.unlock(); // Unlock while processing to prevent blocking other threads
            _onMouseMoveEvent(event);
            mouseMoveLock.lock(); // Re-lock to check the queue
        }
    }

    if(_onKeyStateEvent)
    {
        std::unique_lock<std::mutex> keyStateLock(_keyStateEventQueueMutex);
        while (!_keyStateEventQueue.empty()) {
            const auto event = _keyStateEventQueue.front();
            _keyStateEventQueue.pop();
            
            keyStateLock.unlock(); // Unlock while processing to prevent blocking other threads
            _onKeyStateEvent(event);
            keyStateLock.lock(); // Re-lock to check the queue
        }
    }
}

void InputManager::SetOnMouseMoveEvent(MouseMoveEventHandler handler)
{
    _onMouseMoveEvent = std::move(handler);
}

void InputManager::SetKeyStateEvent(KeyStateEventHandler handler)
{
    _onKeyStateEvent = std::move(handler);
}

InputManager::~InputManager()
{
    Stop();
}
