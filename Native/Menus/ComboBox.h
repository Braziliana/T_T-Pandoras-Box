#pragma once
#include "MenuItem.h"
#include "SelectionList.h"

extern "C"{
    typedef void (*ComboBoxSelectionChangedCallback)(int selectedIndex);
}

class ComboBox : public MenuBase
{
private:
    SelectionList<std::string>* _selectionList;
    int _selectedIndex;
    std::function<void(int)> _comboBoxSelectionChangedCallback;
    void SelectionChanged(int index);
    
public:
    ComboBox(MenuItem* parent, const std::string& title, const Rect& rect, const std::vector<std::string>& items, int selectedIndex);
    ~ComboBox() override;

    int* GetSelectedIndexPointer()
    {
        return &_selectedIndex;
    }

    void Render() override;
    void SetSelection(int index);
    
    void SetComboBoxSelectionChangedCallback(ComboBoxSelectionChangedCallback comboBoxSelectionChangedCallback);
    void SetComboBoxSelectionChangedCallback(std::function<void(int)> handler);
};

extern "C"
{
    __declspec(dllexport) void RegisterComboBoxSelectionChangedCallback(ComboBox* instance, ComboBoxSelectionChangedCallback callback);
    __declspec(dllexport) int* ComboBoxGetSelectedIndexPointer(ComboBox* instance);
    __declspec(dllexport) void ComboBoxSetSelection(ComboBox* instance, int index);
}