#include "CircleRenderer.h"
constexpr float  PI_F = 3.14159265358979f;


bool CircleRenderer::Init(ID3D11Device* device, ID3D11DeviceContext* deviceContext, size_t maxItems, Material* material, int segments)
{
    std::vector<Vertex> circle2DVertices;
    std::vector<Vertex> circle3DVertices;
    circle2DVertices.push_back({{0.0f, 0.0f, 0.0f}, {0.5f, 0.5f}, {1.0f, 1.0f, 1.0f, 1.0f}});
    circle3DVertices.push_back({{0.0f, 0.0f, 0.0f}, {0.5f, 0.5f}, {1.0f, 1.0f, 1.0f, 1.0f}});
    for (int i = 0; i <= segments; ++i)
    {
        const float theta = (2.0f * PI_F * static_cast<float>(i)) / static_cast<float>(segments);
        float x = cosf(theta);
        float y = sinf(theta);
        circle2DVertices.push_back({{x, y, 0.0f}, {x * 0.5f + 0.5f, y * 0.5f + 0.5f}, {1.0f, 1.0f, 1.0f, 1.0f}});
        circle3DVertices.push_back({{x, 0.0f, y}, {x * 0.5f + 0.5f, y * 0.5f + 0.5f}, {1.0f, 1.0f, 1.0f, 1.0f}});
    }

    if(!_instancedVertexBuffer2D.Init(device, deviceContext, maxItems, circle2DVertices.data(), circle2DVertices.size(), D3D_PRIMITIVE_TOPOLOGY_TRIANGLELIST, material))
    {
        return false;
    }
    
    if(!_instancedVertexBuffer3D.Init(device, deviceContext, maxItems, circle3DVertices.data(), circle3DVertices.size(), D3D_PRIMITIVE_TOPOLOGY_TRIANGLELIST, material))
    {
        return false;
    }

    return true;
}

void CircleRenderer::Draw(const Vector2 position, const float radius, const float width, const Color color)
{
    _instancedVertexBuffer2D.Add({radius, width, Vector3(position.x, position.y, 0.0f), color});
}

void CircleRenderer::Draw(const Vector3 position, const float radius, const float width, const Color color)
{
    _instancedVertexBuffer3D.Add({radius, width, position, color});
}


void CircleRenderer::Release()
{
    _instancedVertexBuffer2D.Release();
    _instancedVertexBuffer3D.Release();
}
