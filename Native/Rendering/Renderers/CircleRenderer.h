#pragma once
#include <d3d11.h>

#include "InstancedVertexBufferRenderer.h"
#include "../Vertex.h"
#include "../../Math/Color.h"
#include "../../Math/Vector2.h"
#include "../../Math/Vector3.h"
#include "../Materials/Material.h"

struct CircleData
{
    float radius;
    float width;
    Vector3 position;
    Color color;
};

class CircleRenderer
{
private:
    InstancedVertexBufferRenderer<Vertex, CircleData> _instancedVertexBuffer2D = {}; 
    InstancedVertexBufferRenderer<Vertex, CircleData> _instancedVertexBuffer3D = {}; 
    
public:
    bool Init(ID3D11Device* device, ID3D11DeviceContext* deviceContext, size_t maxItems, Material* material, int segments);
    void Draw(Vector2 position, float radius, float width, Color color);
    void Draw(Vector3 position, float radius, float width, Color color);
    void Release();
};
