#pragma once
#include <d3d11.h>

#include "BatchBuffer.h"

template<typename TVertex, typename TBufferData>
class InstancedVertexBuffer : public BatchBuffer<TBufferData>
{
protected:
    ID3D11Buffer* _vertexBuffer = nullptr;
    size_t _verticesCount = 0;
public:
    InstancedVertexBuffer() { }
    bool Init(ID3D11Device* device, ID3D11DeviceContext* deviceContext, size_t maxItems, const TVertex* vertexBuffer, size_t verticesCount);
    size_t GetVerticesCount() const;
    bool Bind() override;
    void Release() override;
};

template <typename TVertex, typename TBufferData>
bool InstancedVertexBuffer<TVertex, TBufferData>::Init(ID3D11Device* device, ID3D11DeviceContext* deviceContext,
    size_t maxItems, const TVertex* vertexBuffer, size_t verticesCount)
{
    if(!BatchBuffer<TBufferData>::Init(device, deviceContext, maxItems))
    {
        return false;
    }

    D3D11_BUFFER_DESC bufferDesc;
    ZeroMemory(&bufferDesc, sizeof(bufferDesc));
    bufferDesc.Usage = D3D11_USAGE_DEFAULT;
    bufferDesc.ByteWidth = static_cast<UINT>(sizeof(TVertex)) * static_cast<UINT>(verticesCount);
    bufferDesc.BindFlags = D3D11_BIND_VERTEX_BUFFER;

    D3D11_SUBRESOURCE_DATA initData;
    ZeroMemory(&initData, sizeof(initData));
    initData.pSysMem = vertexBuffer;

    const auto hr = device->CreateBuffer(&bufferDesc, &initData, &_vertexBuffer);
    
    if (FAILED(hr)) {
        return false;
    }

    _verticesCount = verticesCount;
    
    return true;
}

template <typename TVertex, typename TBufferData>
size_t InstancedVertexBuffer<TVertex, TBufferData>::GetVerticesCount() const
{
    return _verticesCount;
}

template <typename TVertex, typename TBufferData>
bool InstancedVertexBuffer<TVertex, TBufferData>::Bind()
{
    if(this->_buffer == nullptr || _vertexBuffer == nullptr)
    {
        return false;
    }
    
    UINT strides[2] = { sizeof(TVertex), sizeof(TBufferData) };
    UINT offsets[2] = { 0, 0 };
    ID3D11Buffer* buffers[2] = { _vertexBuffer, this->_buffer };

    this->_deviceContext->IASetVertexBuffers(0, 2, buffers, strides, offsets);
    return true;
}

template <typename TVertex, typename TBufferData>
void InstancedVertexBuffer<TVertex, TBufferData>::Release()
{
    BatchBuffer<TBufferData>::Release();
    if(_vertexBuffer != nullptr)
    {
        _vertexBuffer->Release();
        _vertexBuffer = nullptr;
    }
}
