#include "RectRenderer.h"

bool RectRenderer::Init(ID3D11Device* device, ID3D11DeviceContext* deviceContext, size_t maxItems, Material* material)
{
    auto size = 0.5f;
    const auto rect2DVertex = new Vertex[]
    {
        // Triangle 1
        {Vector3(-size,  size, 0.0f), Vector2(0.0f, 0.0f), Color(1.0f, 1.0f, 1.0f, 1.0f)}, // Top-left
        {Vector3( size,  size, 0.0f), Vector2(1.0f, 0.0f), Color(1.0f, 1.0f, 1.0f, 1.0f)}, // Top-right
        {Vector3(size, -size, 0.0f), Vector2(1.0f, 1.0f), Color(1.0f, 1.0f, 1.0f, 1.0f)}, // Bottom-left

        // Triangle 2
        {Vector3(size, -size, 0.0f), Vector2(1.0f, 1.0f), Color(1.0f, 1.0f, 1.0f, 1.0f)}, // Bottom-left
        {Vector3( -size,  -size, 0.0f), Vector2(0.0f, 1.0f), Color(1.0f, 1.0f, 1.0f, 1.0f)}, // Top-right
        {Vector3( -size,  size, 0.0f), Vector2(0.0f, 0.0f), Color(1.0f, 1.0f, 1.0f, 1.0f)}, // Bottom-right
    };

    const auto rect3DVertex = new Vertex[]
    {
        // Triangle 1
        {Vector3(-size,  0.0f, size), Vector2(0.0f, 0.0f), Color(1.0f, 1.0f, 1.0f, 1.0f)}, // Top-left
        {Vector3( size,  0.0f, size), Vector2(1.0f, 0.0f), Color(1.0f, 1.0f, 1.0f, 1.0f)}, // Top-right
        {Vector3(size, 0.0f, -size), Vector2(1.0f, 1.0f), Color(1.0f, 1.0f, 1.0f, 1.0f)}, // Bottom-left

        // Triangle 2
        {Vector3(size, 0.0f, -size), Vector2(1.0f, 1.0f), Color(1.0f, 1.0f, 1.0f, 1.0f)}, // Bottom-left
        {Vector3( -size,  0.0f, -size), Vector2(0.0f, 1.0f), Color(1.0f, 1.0f, 1.0f, 1.0f)}, // Top-right
        {Vector3( -size,  0.0f, size), Vector2(0.0f, 0.0f), Color(1.0f, 1.0f, 1.0f, 1.0f)}, // Bottom-right
    };

    if(!_instancedVertexBuffer2D.Init(device, deviceContext, maxItems, rect2DVertex, 6, D3D_PRIMITIVE_TOPOLOGY_TRIANGLELIST, material))
    {
        return false;
    }
    
    if(!_instancedVertexBuffer3D.Init(device, deviceContext, maxItems, rect3DVertex, 6, D3D_PRIMITIVE_TOPOLOGY_TRIANGLELIST, material))
    {
        return false;
    }

    return true;
}

void RectRenderer::Draw(Vector2 position, float width, float height, Color color)
{
    
    DirectX::XMMATRIX modelMatrix = DirectX::XMMatrixScaling(width, height, 1.0f) *
        DirectX::XMMatrixTranslation(position.x, position.y, 0.0f);

    _instancedVertexBuffer2D.Add({DirectX::XMMatrixTranspose(modelMatrix), color});
}

void RectRenderer::Draw(Vector3 position, float width, float height, Color color)
{
    DirectX::XMMATRIX modelMatrix = DirectX::XMMatrixScaling(width, 1.0, height) *
        DirectX::XMMatrixTranslation(position.x, position.y, position.z);
    
    _instancedVertexBuffer3D.Add({modelMatrix, color});
}

void RectRenderer::Flush2D()
{
    _instancedVertexBuffer2D.Flush();
}
void RectRenderer::Flush3D()
{
    _instancedVertexBuffer3D.Flush();
}

void RectRenderer::Release()
{
    _instancedVertexBuffer2D.Release();
    _instancedVertexBuffer3D.Release();
}
