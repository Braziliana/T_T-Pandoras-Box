#pragma once
#include <array>
#include <windows.h>
#include <thread>
#include <atomic>
#include <functional>
#include <mutex>
#include <queue>

#include "../Math/Vector2.h"

struct MouseMoveEvent
{
    Vector2 position;
    Vector2 delta;
};

struct KeyStateEvent
{
    unsigned int key;
    bool isDown;
};

class InputManager
{
public:
    using MouseMoveEventHandler = std::function<void(MouseMoveEvent)>;
    using KeyStateEventHandler = std::function<void(KeyStateEvent)>;
    
private:
    static InputManager* _instance;
    static std::once_flag _initInstanceFlag;

    std::thread _hookThread;
    std::atomic<bool> _running;
    std::array<std::atomic<bool>, 256> _keyStates; 
    std::atomic<Vector2> _mousePosition{Vector2(0, 0)};
    std::atomic<Vector2> _mouseMoveDelta{Vector2(0, 0)};
    
    MouseMoveEventHandler _onMouseMoveEvent;
    KeyStateEventHandler _onKeyStateEvent;
    
    HHOOK _keyboardHook;
    HHOOK _mouseHook;

    std::queue<MouseMoveEvent> _mouseMoveEventQueue;
    std::mutex _mouseMoveEventQueueMutex;
    std::condition_variable _mouseMoveEventQueueCondition;

    std::queue<KeyStateEvent> _keyStateEventQueue;
    std::mutex _keyStateEventQueueMutex;
    std::condition_variable _keyStateEventQueueCondition;

    static LRESULT CALLBACK LowLevelKeyboardProc(int nCode, WPARAM wParam, LPARAM lParam);
    static LRESULT CALLBACK LowLevelMouseProc(int nCode, WPARAM wParam, LPARAM lParam);
    
    InputManager();
    
    void HookThreadFunction();
    void UpdateKeyState(unsigned int vkCode, bool isPressed, bool isInjected);
    void PostKeyStateEvent(const KeyStateEvent& event);
    void UpdateMousePosition(float x, float y, bool isInjected);
    void PostMouseMoveEvent(const MouseMoveEvent& event);

public:
    InputManager(InputManager const&) = delete; // Prevent copying
    void operator=(InputManager const&) = delete; // Prevent assignment

    static InputManager* GetInstance();
    
    void Start();
    void Stop();
    void Reset();
    bool GetKeyState(unsigned int vkCode) const;
    Vector2 GetMousePosition() const;
    void ProcessInputEvents();
    void SetOnMouseMoveEvent(MouseMoveEventHandler handler);
    void SetKeyStateEvent(KeyStateEventHandler handler);

    ~InputManager();
};
