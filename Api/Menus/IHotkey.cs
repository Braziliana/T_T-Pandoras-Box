using Api.Inputs;

namespace Api.Menus;

public enum HotkeyType
{
    Press = 0,
    Toggle = 1,
}

public interface IHotkey : IMenuElement
{
    VirtualKey VirtualKey { get; set; }
    HotkeyType HotkeyType { get; set; } 
    bool Enabled { get; }
}