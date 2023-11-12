#pragma once
#include "MenuItem.h"
#include "HotkeyType.h"
#include "MenuBase.h"

enum class VirtualKey : unsigned short;

class HotkeySelector;
class Hotkey : public MenuBase
{
    friend class HotkeySelector;
private:
    HotkeySelector* _hotkeySelector;
    unsigned short _hotkey;
    HotkeyType _hotkeyType;
    bool _toggled;
    
public:
    Hotkey(MenuItem* parent, const std::string& title, const Rect& rect, unsigned short hotkey, HotkeyType hotkeyType, bool toggled);
    ~Hotkey();
    bool* GetToggledPointer();
    HotkeyType* GetHotkeyTypePointer();
    unsigned short* GetHotkeyPointer();
    void Render() override;
    bool OnKeyStateEvent(KeyStateEvent event) override;
};


class HotkeySelector : public MenuItem
{
private:
    Hotkey* _hotkey;
    bool _isSelecting;
    
public:
    HotkeySelector(Hotkey* parent, const std::string& title, const Rect& rect)
        : MenuItem(parent, MenuItemType::HotkeySelector, title, rect), _hotkey(parent), _isSelecting(false)
    {
    }

    void Render() override;
    bool OnKeyStateEvent(KeyStateEvent event) override;
};


extern "C" {
    __declspec(dllexport) bool* HotkeyGetToggledPointer(Hotkey* instance);
    __declspec(dllexport) HotkeyType* HotkeyGetHotkeyTypePointer(Hotkey* instance);
    __declspec(dllexport) unsigned short* HotkeyGetHotkeyPointer(Hotkey* instance);
}

enum class VirtualKey : unsigned short
{
    VK_LeftButton = 0x01,
    VK_RightButton = 0x02,
    VK_Cancel = 0x03,
    VK_MiddleButton = 0x04,
    VK_XButton1 = 0x05,
    VK_XButton2 = 0x06,
    VK_Back = 0x08,
    VK_Tab = 0x09,
    VK_Clear = 0x0C,
    VK_Return = 0x0D,
    VK_Shift = 0x10,
    VK_Control = 0x11,
    VK_Menu = 0x12,
    VK_Pause = 0x13,
    VK_Capital = 0x14,
    VK_Kana = 0x15,
    VK_Hanguel = 0x15,
    VK_Hangul = 0x15,
    VK_Junja = 0x17,
    VK_Final = 0x18,
    VK_Hanja = 0x19,
    VK_Kanji = 0x19,
    VK_Escape = 0x1B,
    VK_Convert = 0x1C,
    VK_NonConvert = 0x1D,
    VK_Accept = 0x1E,
    VK_ModeChange = 0x1F,
    VK_Space = 0x20,
    VK_Prior = 0x21,
    VK_Next = 0x22,
    VK_End = 0x23,
    VK_Home = 0x24,
    VK_Left = 0x25,
    VK_Up = 0x26,
    VK_Right = 0x27,
    VK_Down = 0x28,
    VK_Select = 0x29,
    VK_Print = 0x2A,
    VK_Execute = 0x2B,
    VK_Snapshot = 0x2C,
    VK_Insert = 0x2D,
    VK_Delete = 0x2E,
    VK_Help = 0x2F,
    VK_Key0 = 0x30,
    VK_Key1 = 0x31,
    VK_Key2 = 0x32,
    VK_Key3 = 0x33,
    VK_Key4 = 0x34,
    VK_Key5 = 0x35,
    VK_Key6 = 0x36,
    VK_Key7 = 0x37,
    VK_Key8 = 0x38,
    VK_Key9 = 0x39,
    VK_A = 0x41,
    VK_B = 0x42,
    VK_C = 0x43,
    VK_D = 0x44,
    VK_E = 0x45,
    VK_F = 0x46,
    VK_G = 0x47,
    VK_H = 0x48,
    VK_I = 0x49,
    VK_J = 0x4A,
    VK_K = 0x4B,
    VK_L = 0x4C,
    VK_M = 0x4D,
    VK_N = 0x4E,
    VK_O = 0x4F,
    VK_P = 0x50,
    VK_Q = 0x51,
    VK_R = 0x52,
    VK_S = 0x53,
    VK_T = 0x54,
    VK_U = 0x55,
    VK_V = 0x56,
    VK_W = 0x57,
    VK_X = 0x58,
    VK_Y = 0x59,
    VK_Z = 0x5A,
    VK_LeftWin = 0x5B,
    VK_RightWin = 0x5C,
    VK_Apps = 0x5D,
    VK_SleepKey = 0x5F,
    VK_Numpad0 = 0x60,
    VK_Numpad1 = 0x61,
    VK_Numpad2 = 0x62,
    VK_Numpad3 = 0x63,
    VK_Numpad4 = 0x64,
    VK_Numpad5 = 0x65,
    VK_Numpad6 = 0x66,
    VK_Numpad7 = 0x67,
    VK_Numpad8 = 0x68,
    VK_Numpad9 = 0x69,
    VK_Multiply = 0x6A,
    VK_Add = 0x6B,
    VK_Separator = 0x6C,
    VK_Subtract = 0x6D,
    VK_Decimal = 0x6E,
    VK_Divide = 0x6F,
    F1 = 0x70,
    F2 = 0x71,
    F3 = 0x72,
    F4 = 0x73,
    F5 = 0x74,
    F6 = 0x75,
    F7 = 0x76,
    F8 = 0x77,
    F9 = 0x78,
    F10 = 0x79,
    F11 = 0x7A,
    F12 = 0x7B,
    F13 = 0x7C,
    F14 = 0x7D,
    F15 = 0x7E,
    F16 = 0x7F,
    F17 = 0x80,
    F18 = 0x81,
    F19 = 0x82,
    F20 = 0x83,
    F21 = 0x84,
    F22 = 0x85,
    F23 = 0x86,
    F24 = 0x87,
    NumLock = 0x90,
    Scroll = 0x91,
    LeftShift = 0xA0,
    RightShift = 0xA1,
    LeftControl = 0xA2,
    RightControl = 0xA3,
    LeftAlt = 0xA4,
    RightAlt = 0xA5,
    BrowserBack = 0xA6,
    BrowserForward = 0xA7,
    BrowserRefresh = 0xA8,
    BrowserStop = 0xA9,
    BrowserSearch = 0xAA,
    BrowserFavorites = 0xAB,
    BrowserHome = 0xAC,
    VolumeMute = 0xAD,
    VolumeDown = 0xAE,
    VolumeUp = 0xAF,
    MediaNextTrack = 0xB0,
    MediaPrevTrack = 0xB1,
    MediaStop = 0xB2,
    MediaPlayPause = 0xB3,
    LaunchMail = 0xB4,
    SelectMedia = 0xB5,
    LaunchApplication1 = 0xB6,
    LaunchApplication2 = 0xB7,
    Oem1 = 0xBA, // For US ";:"
    OemPlus = 0xBB, // For any country/region "+"
    OemComma = 0xBC, // For any country/region ","
    OemMinus = 0xBD, // For any country/region "-"
    OemPeriod = 0xBE, // For any country/region "."
    Oem2 = 0xBF, // For US "/?"
    Backtick = 0xC0, // For US "`~"
    Oem3 = 0xC0, // For US "`~"
    Oem4 = 0xDB, // For US "[{"
    Oem5 = 0xDC, // For US "\|"
    Oem6 = 0xDD, // For US "]}"
    Oem7 = 0xDE, // For US "'\""
    Oem8 = 0xDF,
    Oem102 = 0xE2, // Angle bracket or backslash on RT 102-key keyboard
    ProcessKey = 0xE5,
    Packet = 0xE7,
    Attn = 0xF6,
    Crsel = 0xF7,
    Exsel = 0xF8,
    Ereof = 0xF9,
    Play = 0xFA,
    Zoom = 0xFB,
    Noname = 0xFC,
    Pa1 = 0xFD,
    OemClear = 0xFE
};