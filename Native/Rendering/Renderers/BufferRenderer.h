#pragma once
#include "../Buffers/BatchBuffer.h"
#include "../Materials/Material.h"

template<typename T>
class BufferRenderer
{
protected:
    BatchBuffer<T> _batchBuffer;
    ID3D11DeviceContext* _deviceContext = nullptr;
    D3D11_PRIMITIVE_TOPOLOGY _topology = D3D_PRIMITIVE_TOPOLOGY_TRIANGLELIST;
    Material* _material = nullptr;
    
public:
    bool Init(ID3D11Device* device, ID3D11DeviceContext* deviceContext, size_t bufferSize, D3D11_PRIMITIVE_TOPOLOGY topology, Material* material);
    void Add(T item);
    void Add(T* items, size_t count);
    void Flush();
    void Release();
};

template <typename T>
bool BufferRenderer<T>::Init(ID3D11Device* device, ID3D11DeviceContext* deviceContext, size_t bufferSize,
                             const D3D11_PRIMITIVE_TOPOLOGY topology, Material* material)
{
    if(!_batchBuffer.Init(device, deviceContext, bufferSize))
    {
        return false;
    }

    _deviceContext = deviceContext;
    _topology = topology;
    _material = material;

    return true;
}

template <typename T>
void BufferRenderer<T>::Add(T item)
{
    if(!_batchBuffer.Add(item))
    {
        Flush();
    }
    
    _batchBuffer.Add(item);
}

template <typename T>
void BufferRenderer<T>::Add(T* items, size_t count)
{
    if(!_batchBuffer.Add(items, count))
    {
        Flush();
    }
    
    _batchBuffer.Add(items, count);
}

template <typename T>
void BufferRenderer<T>::Flush()
{
    if(_batchBuffer.IsEmpty())
    {
        return;
    }

    auto count = _batchBuffer.Count();
    _batchBuffer.Flush();
    _material->Use(_deviceContext);
    _deviceContext->Draw(count, 0);
}

template <typename T>
void BufferRenderer<T>::Release()
{
    _batchBuffer.Release();
}
