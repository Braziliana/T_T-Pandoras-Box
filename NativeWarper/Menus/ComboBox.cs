using System.Runtime.InteropServices;
using Api.Menus;
using Api.Settings;

using Newtonsoft.Json.Linq;

namespace NativeWarper.Menus;

public unsafe class ComboBox : MenuBase, IComboBox
{    
    [DllImport("Native.dll")]
    public static extern int* ComboBoxGetSelectedIndexPointer(IntPtr instance);
    
    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void RegisterComboBoxSelectionChangedCallback(IntPtr instance, IntPtr handler);

    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void ComboBoxSetSelection(IntPtr instance, int index);



    private readonly int* _selectedIndexPointer;
    
    public int SelectedIndex { get => *_selectedIndexPointer; set => ComboBoxSetSelection(Ptr, value); }
    public string[] Items { get; set; }
    public event Action<int>? SelectionChanged;
    
    
    public delegate void OnSelectionChangedDelegate(int selectedIndex);
    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    private readonly OnSelectionChangedDelegate _onSelectionChanged;
    
    public ComboBox(IntPtr comboBoxPointer, string title, string[] items, int selectedIndex) : base(comboBoxPointer, title)
    {
        _selectedIndexPointer = ComboBoxGetSelectedIndexPointer(comboBoxPointer);
        Items = items;
        SelectedIndex = selectedIndex;
        _onSelectionChanged = new OnSelectionChangedDelegate(OnSelectionChanged);
        RegisterComboBoxSelectionChangedCallback(comboBoxPointer, Marshal.GetFunctionPointerForDelegate(_onSelectionChanged));
    }

    private void OnSelectionChanged(int selectedIndex)
    {
        SelectionChanged?.Invoke(selectedIndex);
    }

    public override void SaveSettings(ISettingsProvider settingsProvider)
    {
        settingsProvider.SetValue(SaveId, SelectedIndex);
    }

    public override void LoadSettings(ISettingsProvider settingsProvider)
    {
        if (settingsProvider.ReadValue(SaveId, out int selectedIndex))
        {
            SelectedIndex = selectedIndex;
        }
    }
}

public unsafe class EnumComboBox<T> : MenuBase, IEnumComboBox<T> where T : Enum
{    
    
    private readonly int* _selectedIndexPointer;
    private readonly T[] _items;
    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    private readonly ComboBox.OnSelectionChangedDelegate _onSelectionChanged;
    
    public EnumComboBox(IntPtr comboBoxPointer, string title, T selectedItem) : base(comboBoxPointer, title)
    {
        _items = (T[])Enum.GetValues(typeof(T));
        _selectedIndexPointer = ComboBox.ComboBoxGetSelectedIndexPointer(comboBoxPointer);
        SelectedItem = selectedItem;
        _onSelectionChanged = new ComboBox.OnSelectionChangedDelegate(OnSelectionChanged);
        ComboBox.RegisterComboBoxSelectionChangedCallback(comboBoxPointer, Marshal.GetFunctionPointerForDelegate(_onSelectionChanged));
    }

    private void OnSelectionChanged(int selectedIndex)
    {
        SelectionChanged?.Invoke(_items[selectedIndex]);
    }

    public T SelectedItem { get => _items[*_selectedIndexPointer]; set => *_selectedIndexPointer = GetIndexOfSelectedItem(value); }
    public event Action<T>? SelectionChanged;
    
    public int GetIndexOfSelectedItem(T selectedItem)
    {
        for (var i = 0; i < _items.Length; i++)
        {
            if (_items[i].Equals(selectedItem))
            {
                return i;
            }
        }

        return -1;
    }
    
    public override void SaveSettings(ISettingsProvider settingsProvider)
    {
        var index = *_selectedIndexPointer;
        settingsProvider.SetValue(SaveId, index);
    }

    public override void LoadSettings(ISettingsProvider settingsProvider)
    {
        if (settingsProvider.ReadValue(SaveId, out int selectedIndex))
        {
            ComboBox.ComboBoxSetSelection(Ptr, selectedIndex);
        }
    }
}