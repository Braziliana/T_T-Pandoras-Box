#pragma once
#include <iostream>

#include "../Buffers/InstancedVertexBuffer.h"
#include "../Materials/Material.h"


template<typename TVertex, typename TBufferData>
class InstancedVertexBufferRenderer
{
protected:
    InstancedVertexBuffer<TVertex, TBufferData> _instancedVertexBuffer = {};
    ID3D11DeviceContext* _deviceContext = nullptr;
    D3D11_PRIMITIVE_TOPOLOGY _topology = D3D_PRIMITIVE_TOPOLOGY_TRIANGLELIST;
    Material* _material = nullptr;
    
public:
    InstancedVertexBufferRenderer(){}
    bool Init(ID3D11Device* device, ID3D11DeviceContext* deviceContext, size_t maxItems, const TVertex* vertexBuffer, size_t verticesCount, D3D11_PRIMITIVE_TOPOLOGY topology, Material* material);
    void Add(TBufferData item);
    void Add(TBufferData* items, size_t count);
    void Flush();
    void Release();
};

template <typename TVertex, typename TBufferData>
bool InstancedVertexBufferRenderer<TVertex, TBufferData>::Init(ID3D11Device* device, ID3D11DeviceContext* deviceContext,
    size_t maxItems, const TVertex* vertexBuffer, size_t verticesCount, D3D11_PRIMITIVE_TOPOLOGY topology,
    Material* material)
{
    if(!_instancedVertexBuffer.Init(device, deviceContext, maxItems, vertexBuffer, verticesCount))
    {
        return false;
    }

    _deviceContext = deviceContext;
    _topology = topology;
    _material = material;

    return true;
}

template <typename TVertex, typename TBufferData>
void InstancedVertexBufferRenderer<TVertex, TBufferData>::Add(TBufferData item)
{
    if(!_instancedVertexBuffer.Add(item))
    {
        Flush();
    }
    
    _instancedVertexBuffer.Add(item);
}

template <typename TVertex, typename TBufferData>
void InstancedVertexBufferRenderer<TVertex, TBufferData>::Add(TBufferData* items, size_t count)
{
    
    if(!_instancedVertexBuffer.Add(items, count))
    {
        Flush();
    }
    
    _instancedVertexBuffer.Add(items, count);
}

template <typename TVertex, typename TBufferData>
void InstancedVertexBufferRenderer<TVertex, TBufferData>::Flush()
{
    if(_instancedVertexBuffer.IsEmpty())
    {
        return;
    }

    auto count = _instancedVertexBuffer.Count();
    _instancedVertexBuffer.Flush();
    _material->Use(_deviceContext);

    _deviceContext->DrawInstanced(_instancedVertexBuffer.GetVerticesCount(), count, 0, 0);
}

template <typename TVertex, typename TBufferData>
void InstancedVertexBufferRenderer<TVertex, TBufferData>::Release()
{
    _instancedVertexBuffer.Release();
}
