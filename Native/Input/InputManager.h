#pragma once
#include <array>
#include <windows.h>
#include <thread>
#include <atomic>
#include <functional>
#include <mutex>
#include <queue>
#include <map>

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


extern "C"{
    typedef void (*MouseMoveEventHandler)(MouseMoveEvent mouseMoveEvent);
    typedef void (*KeyStateEventHandler)(KeyStateEvent mouseMoveEvent);
}


class InputManager
{
private:
    static InputManager* _instance;
    static std::once_flag _initInstanceFlag;

    std::thread _hookThread;
    std::atomic<bool> _running;
    std::array<std::atomic<bool>, 256> _keyStates; 
    std::atomic<Vector2> _mousePosition{Vector2(0, 0)};
    std::atomic<Vector2> _mouseMoveDelta{Vector2(0, 0)};

    int _mouseEventListenerId = 1;
    std::map<int, std::function<void(MouseMoveEvent)>> _onMouseMoveEvent;
    
    int keyStateEventId = 1;
    std::map<int, std::function<void(KeyStateEvent)>> _onKeyStateEvent;
    
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
    InputManager(InputManager const&) = delete;
    void operator=(InputManager const&) = delete;

    static InputManager* GetInstance();
    
    void Start();
    void Stop();
    void Reset();
    bool GetKeyState(unsigned int vkCode) const;
    Vector2 GetMousePosition() const;
    void ProcessInputEvents();

    int AddMouseMoveHandler(MouseMoveEventHandler handler);
    int AddMouseMoveHandler(std::function<void(MouseMoveEvent)> handler);
    void RemoveMouseMoveHandler(int key);
    int AddKeyStateEventHandler(KeyStateEventHandler handler);
    int AddKeyStateEventHandler(std::function<void(KeyStateEvent)> handler);
    void RemoveKeyStateEventHandler(int key);

    ~InputManager();
};


extern "C" {
    __declspec(dllexport) void InputManagerReset();
    __declspec(dllexport) bool InputManagerGetKeyState(unsigned int vkCode);
    __declspec(dllexport) void InputManagerGetMousePosition(Vector2* vector);
    
    __declspec(dllexport) int InputManagerAddMouseMoveHandler(MouseMoveEventHandler handler);
    __declspec(dllexport) void InputManagerRemoveMouseMoveHandler(int key);
    __declspec(dllexport) int InputManagerAddKeyStateEventHandler(KeyStateEventHandler handler);
    __declspec(dllexport) void InputManagerRemoveKeyStateEventHandler(int key);
}
