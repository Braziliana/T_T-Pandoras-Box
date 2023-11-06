#pragma once
#include "../Vertex.h"
#include "../Buffers/InstancedBatchBuffer.h"
#include "../../Math/Color.h"
#include "../../Math/Vector3.h"

struct RectData
{
    Vector3 position;
    Vector3 scale;
    Color color;
    Color borderColor;
    float borderSize;
};

class RectRenderer
{
private:
    Material* _material = nullptr;
    InstancedBuffer<Vertex, RectData>* _buffer2D = nullptr;
    InstancedBuffer<Vertex, RectData>* _buffer3D = nullptr;
public:
    RectRenderer();
    ~RectRenderer();
    void Release();
    
    void Filled(const Vector2& position, const Vector2& size, const Color& color) const;
    void Filled(const Vector3& position, const Vector2& size, const Color& color) const;
    void FilledBordered(const Vector2& position, const Vector2& size, const Color& color, const Color& borderColor, float borderSize) const;
    void FilledBordered(const Vector3& position, const Vector2& size, const Color& color, const Color& borderColor, float borderSize) const;
    void Border(const Vector2& position, const Vector2& size, const Color& color, float borderSize) const;
    void Border(const Vector3& position, const Vector2& size, const Color& color, float borderSize) const;
    
    void Flush2D() const;
    void Flush3D() const;
};
