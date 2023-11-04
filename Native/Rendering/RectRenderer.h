#pragma once
#include "Vertex.h"
#include "../Buffers/InstancedBatchBuffer.h"
#include "../Math/Color.h"
#include "../Math/Vector3.h"

struct RectData
{
    Vector3 position;
    Vector3 scale;
    Color color;
};

class RectRenderer
{
private:
    InstancedBuffer<Vertex, RectData>* _buffer2D = nullptr;
    InstancedBuffer<Vertex, RectData>* _buffer3D = nullptr;
public:
    RectRenderer();
    ~RectRenderer();
    void Release();
    
    void Draw(const Vector2& position, const Vector2& size, const Color& color);
    void Draw(const Vector3& position, const Vector2& size, const Color& color);
    void Flush2D();
    void Flush3D();
};
