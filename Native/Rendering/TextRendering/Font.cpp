#include "Font.h"

Font::Font(const FT_Face face)
{
    int maxWidth = 0;
    int maxHeight = 0;
    int numGlyphs = 0;
    for (unsigned char c = 0; c < 127; ++c) {
        if (FT_Load_Char(face, c, FT_LOAD_RENDER)) {
            std::cerr << "Failed to load Glyph" << std::endl;
            continue;
        }
        maxWidth = std::max(maxWidth, static_cast<int>(face->glyph->bitmap.width));
        maxHeight = std::max(maxHeight, static_cast<int>(face->glyph->bitmap.rows));
        numGlyphs++;
    }

    const unsigned int itemSize = std::max(maxWidth, maxHeight);
    const unsigned int side = static_cast<unsigned int>(std::ceil(std::sqrt(numGlyphs)));
    const unsigned int atlasSize = NextPowerOfTwo(itemSize * side);
    const unsigned int dataSize = atlasSize*atlasSize;
    auto* data = new unsigned char[dataSize];
    memset(data, 0, dataSize);
        
    unsigned int x = 0, y = 0;
    for (char c = 0; c < 127; ++c) {
        if (FT_Load_Char(face, c, FT_LOAD_RENDER)) continue;

        const auto& glyph = face->glyph;

        const FT_Bitmap bitmap = glyph->bitmap;
        for (unsigned int i = 0; i < bitmap.rows; i++)
        {
            for (unsigned int j = 0; j < bitmap.width; j++) {
                const unsigned int index = i * abs(bitmap.pitch) + j;
                const unsigned char pixelValue = bitmap.buffer[index];

                const unsigned int atlasRow = y * itemSize + i;
                const unsigned int atlasCol = x * itemSize + j;
                const unsigned int dataIndex = atlasRow * atlasSize + atlasCol;

                data[dataIndex] = pixelValue;
            }
        }

        _characters[c] = {
            //size
            {static_cast<float>(glyph->bitmap.width), static_cast<float>(glyph->bitmap.rows)},
            //bearing
            {static_cast<float>(glyph->bitmap_left), static_cast<float>(glyph->bitmap_top)},
            // UV (bottom-left of the glyph in the atlas)
            {
                static_cast<float>(x * itemSize) / static_cast<float>(atlasSize),
                static_cast<float>(y * itemSize) / static_cast<float>(atlasSize)
            },
            // UV Size (how big the glyph is in the atlas)
            {
                static_cast<float>(glyph->bitmap.width) / static_cast<float>(atlasSize),
                static_cast<float>(glyph->bitmap.rows) / static_cast<float>(atlasSize)
            },
            //advance
            static_cast<float>(glyph->advance.x >> 6)
        };

        x++;
        if (x >= side) {
            x = 0;
            y++;
        }
    }
        
    glPixelStorei(GL_PACK_ALIGNMENT, 1);
    _textureAtlas = new TextureAtlas(static_cast<int>(atlasSize), static_cast<int>(atlasSize), TextureType::Single, static_cast<int>(side), static_cast<int>(side), data);
    _textureAtlas->SaveToFile();
}

FontCharacterInstance Font::GetInstance(char c, Vector2& position, float scale, Color color)
{
    if(c < 0 || c >= 127)
    {
        c = '?';
    }
        
    const Character ch = _characters[c];

    float xPos = position.x + (ch.bearing.x * scale);
    float yPos = position.y - ch.bearing.y * scale;

    position.x += ch.advance * scale; 
    return  {
        {xPos, yPos, 0.0f},
        {ch.size.x * scale, ch.size.y * scale, 1.0f},
        ch.uv,
        ch.uvSize,
        color
    };

}

FontCharacterInstance Font::GetInstance(char c, Vector3& position, const float scale, const Color color)
{
    if(c < 0 || c >= 127)
    {
        c = '?';
    }
        
    const Character ch = _characters[c];

    float xPos = position.x + (ch.bearing.x * scale);
    float zPos = position.z - ch.bearing.y * scale;

    position.x += ch.advance * scale; 
    return  {
            {xPos, 0.0f, zPos},
            {ch.size.x * scale, 1.0f, ch.size.y * scale},
            ch.uv,
            ch.uvSize,
            color
        };
}

Vector2 Font::GetTextSize(const std::string& text, const float scale)
{
    float totalWidth = 0;
    float maxHeight = 0;
    for (char c : text) {
        if(c < 0 || c >= 127)
        {
            c = '?';
        }
        const Character ch = _characters[c];

        totalWidth += ch.advance * scale;

        float height = (ch.size.y + ch.bearing.y) * scale;
        maxHeight = std::max(maxHeight, height);
    }
        
    return {totalWidth, maxHeight};
}

void Font::Release()
{
    _characters.clear();
    if(_textureAtlas != nullptr)
    {
        _textureAtlas->Release();
        delete _textureAtlas;
        _textureAtlas = nullptr;
    }
}

TextureAtlas* Font::GetFontTexture() const
{
    return _textureAtlas;
}

std::string Font::GetName()
{
    return _name;
}
