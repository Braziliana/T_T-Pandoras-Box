#include "ComboBox.h"

void ComboBox::SelectionChanged(const int index)
{
    this->_selectedIndex = index;
    if(_comboBoxSelectionChangedCallback != nullptr)
    {
        _comboBoxSelectionChangedCallback(index);
    }
}

ComboBox::ComboBox(MenuItem* parent, const std::string& title, const Rect& rect, const std::vector<std::string>& items, const int selectedIndex)
    : MenuBase(parent, MenuItemType::ComboBox, title, rect), _selectedIndex(selectedIndex), _comboBoxSelectionChangedCallback(nullptr)
{
    _selectionList = new SelectionList<std::string>(this, items, items, selectedIndex);
    
    _selectionList->OnSelectionChanged([this](const std::string& item, const int index) {
        this->SelectionChanged(index);
    });
}

ComboBox::~ComboBox()
{
    MenuBase::~MenuBase();
    delete _selectionList;
}

void ComboBox::Render()
{
    const auto renderer = Renderer::Instance();
    MenuItem::Render();  // NOLINT(bugprone-parent-virtual-call)
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

void ComboBox::SetSelection(const int index)
{
    _selectionList->SetSelectedItem(index);
}

void ComboBox::SetComboBoxSelectionChangedCallback(ComboBoxSelectionChangedCallback comboBoxSelectionChangedCallback)
{
    _comboBoxSelectionChangedCallback = comboBoxSelectionChangedCallback;
}

void ComboBox::SetComboBoxSelectionChangedCallback(std::function<void(int)> handler)
{
    _comboBoxSelectionChangedCallback = std::move(handler);
}

void RegisterComboBoxSelectionChangedCallback(ComboBox* instance, const ComboBoxSelectionChangedCallback callback)
{
    instance->SetComboBoxSelectionChangedCallback(callback);
}

int* ComboBoxGetSelectedIndexPointer(ComboBox* instance)
{
    return instance->GetSelectedIndexPointer();
}

void ComboBoxSetSelection(ComboBox* instance, const int index)
{
    instance->SetSelection(index);
}
