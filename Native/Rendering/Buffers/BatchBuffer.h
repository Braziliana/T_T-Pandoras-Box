#pragma once
#include <d3d11.h>
#include <vector>

template<typename T>
class BatchBuffer
{
protected:
    ID3D11DeviceContext* _deviceContext = nullptr;
    ID3D11Buffer* _buffer = nullptr;
    std::vector<T> _items;
    size_t _count = 0;
    size_t _maxItems = 0;
    
public:
    BatchBuffer() {  }
    virtual ~BatchBuffer()
    {
        
    }
    
    BatchBuffer(const BatchBuffer&) = delete;
    BatchBuffer& operator=(const BatchBuffer&) = delete;
    BatchBuffer(BatchBuffer&&) = delete;
    BatchBuffer& operator=(BatchBuffer&&) = delete;

    bool Init(ID3D11Device* device, ID3D11DeviceContext* deviceContext, size_t maxItems);
    bool IsEmpty() const;
    size_t Count() const;
    bool Add(const T& item);
    bool Add(const T* items, size_t itemsCount);
    void Clear();

    virtual bool Bind();
    virtual bool Flush();
    virtual void Release();
};

template <typename T>
bool BatchBuffer<T>::Init(ID3D11Device* device, ID3D11DeviceContext* deviceContext, size_t maxItems)
{
    _deviceContext = deviceContext;
    _maxItems = maxItems;
    _items.reserve(maxItems);
    
    D3D11_BUFFER_DESC bufferDesc;
    ZeroMemory(&bufferDesc, sizeof(bufferDesc));
    bufferDesc.Usage = D3D11_USAGE_DYNAMIC;
    bufferDesc.ByteWidth = static_cast<UINT>(sizeof(T)) * static_cast<UINT>(maxItems);
    bufferDesc.BindFlags = D3D11_BIND_VERTEX_BUFFER;
    bufferDesc.CPUAccessFlags = D3D11_CPU_ACCESS_WRITE;

    const auto hr = device->CreateBuffer(&bufferDesc, nullptr, &_buffer);
    
    if (FAILED(hr)) {
        return false;
    }
    
    return true;
}

template <typename T>
bool BatchBuffer<T>::IsEmpty() const
{
    return _count < 1;
}

template <typename T>
size_t BatchBuffer<T>::Count() const
{
    return _count;
}

template <typename T>
bool BatchBuffer<T>::Add(const T& item)
{
    if(_count + 1 >= _maxItems)
    {
        std::cout << "c: " << _count << " m: " << _maxItems << std::endl;
        return false;
    }
    
    _items[_count] = item;
    _count++;
    return true;
}

template <typename T>
bool BatchBuffer<T>::Add(const T* items, const size_t itemsCount)
{
    if(_count + itemsCount >= _maxItems)
    {
        return false;
    }
    
    std::memcpy(&_items[_count], items, itemsCount * sizeof(T));
    _count += itemsCount;
    return true;
}

template <typename T>
void BatchBuffer<T>::Clear()
{
    _count = 0;
    _items.clear();
}

template <typename T>
bool BatchBuffer<T>::Bind()
{
    if(_buffer == nullptr)
    {
        return false;
    }

    constexpr UINT stride = sizeof(T);
    constexpr UINT offset = 0;

    _deviceContext->IASetVertexBuffers(0, 1, &_buffer, &stride, &offset);
    
    return true;
}

template <typename T>
bool BatchBuffer<T>::Flush()
{
    D3D11_MAPPED_SUBRESOURCE mappedResource;
    auto hr = _deviceContext->Map(_buffer, 0, D3D11_MAP_WRITE_DISCARD, 0, &mappedResource);

    if (FAILED(hr)) {
        Clear();
        return false;
    }
    
    const auto dataPtr = static_cast<T*>(mappedResource.pData);
    memcpy(dataPtr, _items.data(), sizeof(T) * _count);
    _deviceContext->Unmap(_buffer, 0);

    Bind();

    Clear();
    return true;
}

template <typename T>
void BatchBuffer<T>::Release()
{
    if(_buffer != nullptr)
    {
        _buffer->Release();
        _buffer = nullptr;
        _items.clear();
        _items.shrink_to_fit();
    }
}
