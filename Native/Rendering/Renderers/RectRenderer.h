#pragma once

#include "InstancedVertexBufferRenderer.h"
#include "../Vertex.h"
#include "../../Math/Color.h"
#include "../../Math/Vector2.h"
#include "../../Math/Vector3.h"
#include "../Materials/Material.h"

struct RectData
{
    Vector3 position;
    Vector3 scale;
    Color color;
};

class RectRenderer
{
private:
    InstancedVertexBufferRenderer<Vertex, RectData> _instancedVertexBuffer2D = {}; 
    InstancedVertexBufferRenderer<Vertex, RectData> _instancedVertexBuffer3D = {}; 
    
public:
    bool Init(ID3D11Device* device, ID3D11DeviceContext* deviceContext, size_t maxItems, Material* material);
    void Draw(Vector2 position, float width, float height, Color color);
    void Draw(Vector3 position, float width, float height, Color color);
    void Flush2D();
    void Flush3D();
    void Release();
};
