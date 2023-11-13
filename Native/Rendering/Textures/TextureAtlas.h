#pragma once
#include <complex.h>

#include "Texture.h"
#include "../../Math/Vector2.h"

class TextureAtlas : public Texture
{
public:
    int rows;
    int cols;
    int itemWidth;
    int itemHeight;
    
    TextureAtlas(const int width, const int height, const unsigned int textureId, const TextureType type, int rows, int cols)
        : Texture(width, height, textureId, type),
        rows(rows),
        cols(cols),
        itemWidth(static_cast<int>(static_cast<float>(width) / static_cast<float>(cols))),
        itemHeight(static_cast<int>(static_cast<float>(height) / static_cast<float>(rows)))
    {
    }

    TextureAtlas(const int width, const int height, const TextureType type, int rows, int cols, unsigned char* data)
        : Texture(width, height, type, data),
        rows(rows),
        cols(cols),
        itemWidth(static_cast<int>(static_cast<float>(width) / static_cast<float>(cols))),
        itemHeight(static_cast<int>(static_cast<float>(height) / static_cast<float>(rows)))
    {
    }
    
    void SetItem(const int row, const int col, const unsigned char* data)
    {
        if (row < 0 || row >= rows || col < 0 || col >= cols)
        {
            return;
        }

        const int xOffset = col * itemWidth;
        const int yOffset = row * itemHeight;

        SetBytes(data, xOffset, yOffset, itemWidth, itemHeight);
    }

    void SetItem(const int row, const int col, int width, int height, const unsigned char* data)
    {
        if (row < 0 || row >= rows || col < 0 || col >= cols)
        {
            return;
        }

        const int xOffset = col * itemWidth;
        const int yOffset = row * itemHeight;

        SetBytes(data, xOffset, yOffset, width, height);
    }
    
    Vector2 GetUv(const int row, const int col) const
    {
        if (row < 0 || row >= rows || col < 0 || col >= cols)
        {
            // Handle error: row or col out of range
            return {0.0f, 0.0f};
        }

        float u = static_cast<float>(col) / static_cast<float>(cols);
        float v = static_cast<float>(row) / static_cast<float>(rows);

        return {u, v};
    }
};
