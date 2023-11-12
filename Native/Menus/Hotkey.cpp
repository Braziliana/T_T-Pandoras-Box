#include "Hotkey.h"

#include <sstream>

#include "../../Rendering/Renderer.h"


bool* HotkeyGetToggledPointer(Hotkey* instance)
{
    return instance->GetToggledPointer();
}

HotkeyType* HotkeyGetHotkeyTypePointer(Hotkey* instance)
{
    return instance->GetHotkeyTypePointer();
}

unsigned short* HotkeyGetHotkeyPointer(Hotkey* instance)
{
    return instance->GetHotkeyPointer();
}

const std::string VirtualKeyNames[] = {
    "0", "LeftButton", "RightButton", "Cancel", "MiddleButton", "XButton1", "XButton2", "7", "Back", "Tab", "10", 
    "11", "Clear", "Return", "14", "15", "Shift", "Control", "Menu", "Pause", "Capital", 
    "Hangul", "22", "Junja", "Final", "Hanja", "26", "Escape", "Convert", "NonConvert", "Accept", 
    "ModeChange", "Space", "Prior", "Next", "End", "Home", "Left", "Up", "Right", "Down", 
    "Select", "Print", "Execute", "Snapshot", "Insert", "Delete", "Help", "Key0", "Key1", "Key2", 
    "Key3", "Key4", "Key5", "Key6", "Key7", "Key8", "Key9", "58", "59", "60", 
    "61", "62", "63", "64", "A", "B", "C", "D", "E", "F", 
    "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", 
    "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z",
    "LeftWin", "RightWin", "Apps", "94", "Sleep", "Numpad0", "Numpad1", "Numpad2", "Numpad3", "Numpad4",
    "Numpad5", "Numpad6", "Numpad7", "Numpad8", "Numpad9", "Multiply", "Add", "Separator", "Subtract", "Decimal",
    "Divide", "F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9",
    "F10", "F11", "F12", "F13", "F14", "F15", "F16", "F17", "F18", "F19",
    "F20", "F21", "F22", "F23", "F24", "136", "137", "138", "139", "140",
    "141", "142", "143", "NumLock", "Scroll", "146", "147", "148", "149", "150",
    "151", "152", "153", "154", "155", "156", "157", "158", "159", "LeftShift",
    "RightShift", "LeftControl", "RightControl", "LeftAlt", "RightAlt", "BrowserBack", "BrowserForward", "BrowserRefresh", "BrowserStop", "BrowserSearch",
    "BrowserFavorites", "BrowserHome", "VolumeMute", "VolumeDown", "VolumeUp", "MediaNextTrack", "MediaPrevTrack", "MediaStop", "MediaPlayPause", "LaunchMail",
    "SelectMedia", "LaunchApplication1", "LaunchApplication2", "184", "185", "Oem1", "OemPlus", "OemComma", "OemMinus", "OemPeriod",
    "Oem2", "Backtick", "193", "194", "195", "196", "197", "198", "199", "200",
    "201", "202", "203", "204", "205", "206", "207", "208", "209", "210",
    "211", "212", "213", "214", "215", "216", "217", "218", "Oem4", "Oem5",
    "Oem6", "Oem7", "Oem8", "224", "225", "Oem102", "227", "228", "ProcessKey", "230",
    "Packet", "232", "233", "234", "235", "236", "237", "238", "239", "240",
    "241", "242", "243", "244", "245", "Attn", "Crsel", "Exsel", "Ereof", "Play",
    "Zoom", "Noname", "Pa1", "OemClear",
};



std::string HotkeyString(const std::string& str, const VirtualKey value) {
    std::ostringstream out;
    out << str << ' ' << VirtualKeyNames[static_cast<int>(value)];
    return out.str();
}

Hotkey::Hotkey(MenuItem* parent, const std::string& title, const Rect& rect, const unsigned short hotkey, const HotkeyType hotkeyType,
    bool toggled): MenuBase(parent, MenuItemType::Hotkey, title, rect), _hotkey(hotkey), _hotkeyType(hotkeyType), _toggled(toggled)
{
    /*
    const auto item = new Toggle(this, title, GetChildRect(1), toggled);
    AddItem(item); 
     */
    _hotkeySelector = new HotkeySelector(this, "Hotkey", MenuBase::GetChildRect(1));
    AddItem(_hotkeySelector);
    AddToggle("V1", true);
    AddToggle("V2", false);
}

Hotkey::~Hotkey()
{
    delete _hotkeySelector;
}

bool* Hotkey::GetToggledPointer()
{
    return &_toggled;
}

HotkeyType* Hotkey::GetHotkeyTypePointer()
{
    return &_hotkeyType;
}

unsigned short* Hotkey::GetHotkeyPointer()
{
    return &_hotkey;
}

void Hotkey::Render()
{
    const auto renderer = Renderer::Instance();

    renderer->RectFilledBordered(_rect.Center(), _rect.Size(), DefaultMenuStyle.ItemColor, DefaultMenuStyle.BorderColor, DefaultMenuStyle.Border);
    const auto itemsRect = _rect.Padding(DefaultMenuStyle.ContentPadding);
    
    renderer->Text(HotkeyString(_title, static_cast<VirtualKey>(_hotkey)), itemsRect.GetStart(), itemsRect.GetEnd(), DefaultMenuStyle.FontSize, DefaultMenuStyle.TextColor, TextHorizontalOffset::Left, TextVerticalOffset::Center);

    
    if(!_items.empty())
    {
        const auto elementPosition = DefaultMenuStyle.GetElementRect(_rect, 0);
        renderer->Text(_open ? "<" : ">", elementPosition.GetStart(), elementPosition.GetEnd(), DefaultMenuStyle.FontSize, DefaultMenuStyle.TextColor, TextHorizontalOffset::Center, TextVerticalOffset::Center);
    }
    else
    {
        return;
    }

    MenuBase::Render();
}

bool Hotkey::OnKeyStateEvent(const KeyStateEvent event)
{
    MenuBase::OnKeyStateEvent(event);
    
    if(event.key != _hotkey)
    {
        return false;
    }
    
    switch (_hotkeyType)
    {
    case HotkeyType::Press:
        _toggled = event.isDown;
        break;
    case HotkeyType::Toggle:
        if(!event.isDown)
        {
            _toggled = !_toggled;
        }
        break;
    }
    
    return false;
}


void HotkeySelector::Render()
{
    const auto renderer = Renderer::Instance();
    renderer->RectFilledBordered(_rect.Center(), _rect.Size(), DefaultMenuStyle.ItemColor, DefaultMenuStyle.BorderColor, DefaultMenuStyle.Border);
    const auto itemsRect = _rect.Padding(DefaultMenuStyle.ContentPadding);


    const std::string title = _isSelecting ? "Select hotkey" : HotkeyString(_title, static_cast<VirtualKey>(_hotkey->_hotkey));
    
    renderer->Text(title, itemsRect.GetStart(), itemsRect.GetEnd(), DefaultMenuStyle.FontSize, DefaultMenuStyle.TextColor, TextHorizontalOffset::Left, TextVerticalOffset::Center);
}

bool HotkeySelector::OnKeyStateEvent(KeyStateEvent event)
{
    if(event.isDown && event.key == VK_LBUTTON && _rect.Contains(InputManager::GetInstance()->GetMousePosition()) && !_isSelecting)
    {
        _isSelecting = true;
        return true;
    }
    if(event.isDown && _isSelecting)
    {
        _isSelecting = false;
        _hotkey->_hotkey = event.key;
       return true; 
    }
    return false;
}
