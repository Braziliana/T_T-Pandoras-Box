#pragma once
#include "GL/glew.h"
#include <iostream>
#include <vector>
#include <fstream>

enum class TextureType {
    Single,
    Rgb,
    Rgba
};

class Texture
{
private:
    unsigned int GenerateTexture(unsigned char* data) const
    {
        unsigned int texId;
        glGenTextures(1, &texId);
        glBindTexture(GL_TEXTURE_2D, texId);

        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_EDGE);
        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_EDGE);
        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);

        std::cout << "Texture: " << texId << " " << width << " " << height << std::endl;
        glTexImage2D(GL_TEXTURE_2D,
            0,
            static_cast<GLint>(GetFormat()),
            width, height,
            0,
            static_cast<GLint>(GetFormat()),
            GL_UNSIGNED_BYTE,
            data);
        
        glBindTexture(GL_TEXTURE_2D, 0);

        return texId;
    }
    
public:
    int width;
    int height;
    unsigned int textureId;
    TextureType type;
    
    unsigned char* d;

    Texture() = delete;
    Texture(const int width, const int height, const unsigned int textureId, const TextureType type) : width(width), height(height), textureId(textureId), type(type)
    {
        d = nullptr;
    }
    
    Texture(const int width, const int height, const TextureType type, unsigned char* data) : width(width), height(height), type(type)
    {
        textureId = GenerateTexture(data);
        d=data;
    }
    
    void Bind() const
    {
        glActiveTexture(GL_TEXTURE0);
        glBindTexture(GL_TEXTURE_2D, textureId);
    }

    static void UnBind()
    {
        glBindTexture(GL_TEXTURE_2D, 0);
    }

    unsigned char* GetBytes() const
    {
        const int size = width * height * PixelSize();
        auto* data = new unsigned char[size];
        
        Bind();
        glPixelStorei(GL_PACK_ALIGNMENT, 1);
        glGetTexImage(GL_TEXTURE_2D, 0, GetFormat(), GL_UNSIGNED_BYTE, data);
        UnBind();
        
        return data;
    }

    void SetBytes(const unsigned char* data, int xOffset, int yOffset, int bWidth, int bHeight)
    {
        Bind();
        glPixelStorei(GL_PACK_ALIGNMENT, 1);
        glTexSubImage2D(GL_TEXTURE_2D, 0, xOffset, yOffset, bWidth, bHeight, GetFormat(), GL_UNSIGNED_BYTE, data);
        UnBind();
    }

    int PixelSize() const
    {
        switch (type)
        {
        case TextureType::Single:
            return 1;
        case TextureType::Rgb:
            return 3;
        case TextureType::Rgba:
            return 4;
        }
        
        return 4;
    }

    GLenum GetFormat() const
    {
        switch (type)
        {
        case TextureType::Single:
            return GL_RED;
        case TextureType::Rgb:
            return GL_RGB;
        case TextureType::Rgba:
            return GL_RGBA;
        }
        
        return GL_RGBA;
    }
    
    void SaveToFile()
    {
        //Use STB https://github.com/nothings/stb

        // BMP Header
        unsigned char header[54] = {
            'B', 'M',             // Signature
            0, 0, 0, 0,           // Image file size in bytes
            0, 0, 0, 0,           // Reserved
            54, 0, 0, 0,          // Start of pixel array
            40, 0, 0, 0,          // Info header size
            0, 0, 0, 0,           // Image width
            0, 0, 0, 0,           // Image height
            1, 0,                 // Number of color planes
            24, 0,                // Bits per pixel
            0, 0, 0, 0,           // Compression
            0, 0, 0, 0,           // Image size
            0, 0, 0, 0,           // Horizontal resolution
            0, 0, 0, 0,           // Vertical resolution
            0, 0, 0, 0,           // Colors in color table
            0, 0, 0, 0,           // Important color count
        };

        // Filling in the header information
        int fileSize = 54 + 3 * width * height;
        header[2] = (unsigned char)(fileSize);
        header[3] = (unsigned char)(fileSize >> 8);
        header[4] = (unsigned char)(fileSize >> 16);
        header[5] = (unsigned char)(fileSize >> 24);

        header[18] = (unsigned char)(width);
        header[19] = (unsigned char)(width >> 8);
        header[20] = (unsigned char)(width >> 16);
        header[21] = (unsigned char)(width >> 24);

        header[22] = (unsigned char)(height);
        header[23] = (unsigned char)(height >> 8);
        header[24] = (unsigned char)(height >> 16);
        header[25] = (unsigned char)(height >> 24);

        std::ofstream file;
        file.open("Test.bmp", std::ios::out | std::ios::binary);
        if (!file) {
            std::cerr << "Failed to open file for writing: " << "Test.bmp" << std::endl;
            return;
        }

        // Write the header
        file.write(reinterpret_cast<char*>(header), 54);

        // Write the pixel data
        auto buffer = d;//GetBytes();

        unsigned char* bmpData = new unsigned char[width * height * 3];

        // Copy the red data to the BMP data, setting green and blue to 0
        for (int i = 0; i < width * height; ++i) {
            bmpData[i * 3 + 0] = buffer[i]; // Red
            bmpData[i * 3 + 1] = buffer[i];          // Green
            bmpData[i * 3 + 2] = buffer[i];          // Blue
        }

        
        for (int y = height - 1; y >= 0; y--) { // BMP files are stored upside-down
            file.write(reinterpret_cast<char*>(bmpData + y * width * 3), width * 3);
        }

        file.write(reinterpret_cast<char*>(bmpData), width*height * 3);
        
        file.close();
    }

    void Release()
    {
        if (textureId != 0)
        {
            glDeleteTextures(1, &textureId);
            textureId = 0;
        }
    }
};
