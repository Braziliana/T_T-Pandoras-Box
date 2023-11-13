using System.Runtime.InteropServices;
using Api.Menus;

namespace NativeWarper.Menus;

public unsafe class ComboBox : MenuBase, IComboBox
{    
    [DllImport("Native.dll")]
    public static extern int* ComboBoxGetSelectedIndexPointer(IntPtr instance);
    
    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void RegisterComboBoxSelectionChangedCallback(IntPtr instance, IntPtr handler);
    
    
    private readonly int* _selectedIndexPointer;
    
    public int SelectedIndex { get => *_selectedIndexPointer; set => *_selectedIndexPointer = value; }
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
}