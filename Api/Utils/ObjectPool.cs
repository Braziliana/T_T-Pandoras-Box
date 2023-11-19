namespace Api.Utils;

/// <summary>
/// FUCK GC FOR FREEZING APP
/// </summary>
/// <typeparam name="T"></typeparam>
public class ObjectPool<T>
{
    private int _currentIndex = 0;
    private readonly T[] _items;
    private readonly Func<T> _create;
    
    public ObjectPool(int size, Func<T> create)
    {
        _create = create;
        _items = new T[size];
        _currentIndex = size-1;
        for (var i = 0; i < size; i++)
        {
            _items[i] = _create();
        }
    }

    public T Get()
    {
        if (_currentIndex < 1)
        {
            return _create();
        }
        
        var item = _items[_currentIndex];
        _currentIndex--;
        return item;
    }

    public void Stash(T item)
    {
        if(_currentIndex + 1 >= _items.Length) return;
        _currentIndex++;
        _items[_currentIndex] = item;
    }
}