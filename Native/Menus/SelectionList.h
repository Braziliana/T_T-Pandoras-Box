#pragma once
#include <string>
#include <vector>
#include <functional>
#include <utility>

#include "MenuBase.h"
#include "../Rendering/Renderer.h"

#include "MenuItem.h"
template<typename T>
class SelectionList;

template<typename T>
class SelectionItem : public MenuItem
{
public:
    SelectionList<T>* selectionList;
    bool isSelected;
    T item;
    int index;

    SelectionItem(MenuItem* parent, const std::string& title, const Rect& rect,
         SelectionList<T>* selectionList, const T& item, const bool isSelected, const int index)
        : MenuItem(parent, MenuItemType::SelectionItem, title, rect), selectionList(selectionList), isSelected(isSelected), item(item), index(index)
    {
    }

    void Render() override
    {
        auto color = DefaultMenuStyle.BorderColor;
        color.a = 1.0f;
        
        const auto renderer = Renderer::Instance();
        renderer->RectFilledBordered(_rect.Center(), _rect.Size(), isSelected ? color : DefaultMenuStyle.ItemColor, DefaultMenuStyle.BorderColor, DefaultMenuStyle.Border);
        const auto itemsRect = _rect.Padding(DefaultMenuStyle.ContentPadding);
        
        renderer->Text(_title, itemsRect.GetStart(), itemsRect.GetEnd(), DefaultMenuStyle.FontSize, isSelected ? Color(1, 1, 1, 1) : DefaultMenuStyle.TextColor, TextHorizontalOffset::Center, TextVerticalOffset::Center);
    }

    bool OnKeyStateEvent(KeyStateEvent event) override
    {
        if(event.key == VK_LBUTTON && event.isDown && _rect.Contains(InputManager::GetInstance()->GetMousePosition()))
        {
            if(!isSelected)
            {
                selectionList->SetSelectedItem(index);
            }
            return true;
        }
        
        return false;
    }
};

template<typename T>
class SelectionList
{
public:
    using EventHandler = std::function<void(const T&, int)>;
    
private:
    MenuBase* _menuBase;
    std::vector<EventHandler> _eventHandlers;
    int _selectedItem;
    std::vector<SelectionItem<T>*> _items;
    
public:
    SelectionList(MenuBase* menuBase, std::vector<T> items, std::vector<std::string> itemNames, const int selectedItem) : _menuBase(menuBase), _selectedItem(selectedItem)
    {
        for (size_t i = 0; i < items.size(); ++i)
        {
            auto& item = items[i];
            auto selectionItem = new SelectionItem<T>(menuBase, itemNames[i], menuBase->GetChildRect(1), this, item, i == selectedItem, i);
            _menuBase->AddItem(selectionItem);
            _items.push_back(selectionItem);
        }
    }

    ~SelectionList()
    {
        for (size_t i = 0; i < _items.size(); ++i)
        {
            delete _items[i];
        }
        _items.clear();
    }

    void OnSelectionChanged(EventHandler handler)
    {
        _eventHandlers.push_back(handler);
    }
    
    void SetSelectedItem(int index)
    {
        if (index >= 0 && index < _items.size() && index != _selectedItem)
        {
            _items[_selectedItem]->isSelected = false;
            _selectedItem = index;
         
            auto selectedItem = _items[_selectedItem];   
            selectedItem->isSelected = true;
            for (auto& handler : _eventHandlers)
            {
                handler(selectedItem->item, index);
            }
        }
    }

    bool IsSelected(const int index) const
    {
        return _selectedItem == index;
    }
};
