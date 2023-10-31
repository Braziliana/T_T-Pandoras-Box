#pragma once
#include <iostream>
#include <string>
#include <ft2build.h>
#include <map>

#include <glm/vec2.hpp>

#include "../Materials/Shader.h"
#include "../Materials/ShaderManager.h"
#include "GL/glew.h"

#include FT_FREETYPE_H

struct Character
{
    unsigned int textureId; // ID handle of the glyph texture
    Vector2 size; // Size of glyph
    Vector2 bearing; // Offset from baseline to left/top of glyph
    unsigned int advance; // Offset to advance to next glyph
};

class Font
{
private:
    unsigned int _vao, _vbo;
    Shader* _textShader;
    std::string _name;
    std::map<char, Character> _characters;

public:
    Font(Shader* shader, FT_Face face)
    {
        _textShader = shader;
        FT_Set_Pixel_Sizes(face, 0, 48);

        glPixelStorei(GL_UNPACK_ALIGNMENT, 1); // disable byte-alignment restriction
        glPixelStorei(GL_UNPACK_ALIGNMENT, 1); // Disable byte-alignment restriction

        for (GLubyte c = 0; c < 128; c++) {
            if (FT_Load_Char(face, c, FT_LOAD_RENDER)) {
                std::cout << "ERROR::FREETYTPE: Failed to load Glyph\n" << std::endl;
                continue;
            }

            GLuint texture;
            glGenTextures(1, &texture);
            glBindTexture(GL_TEXTURE_2D, texture);
            glTexImage2D(GL_TEXTURE_2D, 0, GL_RED,
                static_cast<GLsizei>(face->glyph->bitmap.width), static_cast<GLsizei>(face->glyph->bitmap.rows),
                0, GL_RED, GL_UNSIGNED_BYTE, face->glyph->bitmap.buffer);

            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_EDGE);
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_EDGE);
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);

            Character character = {
                texture,
                Vector2(static_cast<float>(face->glyph->bitmap.width), static_cast<float>(face->glyph->bitmap.rows)),
                Vector2(static_cast<float>(face->glyph->bitmap_left), static_cast<float>(face->glyph->bitmap_top)),
                static_cast<GLuint>(face->glyph->advance.x)
            };
            _characters.insert(std::pair<char, Character>(c, character));
        }

        FT_Done_Face(face);

        glGenVertexArrays(1, &_vao);
        glGenBuffers(1, &_vbo);
        glBindVertexArray(_vao);
        glBindBuffer(GL_ARRAY_BUFFER, _vbo);
        glBufferData(GL_ARRAY_BUFFER, sizeof(float) * 6 * 4, nullptr, GL_DYNAMIC_DRAW);
        glEnableVertexAttribArray(0);
        glVertexAttribPointer(0, 4, GL_FLOAT, GL_FALSE, 4 * sizeof(float), 0);
        glBindBuffer(GL_ARRAY_BUFFER, 0);
        glBindVertexArray(0);      
    }
    
    void RenderText(std::string text, float x, float y, float scale, Color color)
    {
        // activate corresponding render state	
        _textShader->Use();
        _textShader->SetColor("textColor", color);
        glActiveTexture(GL_TEXTURE0);
        glBindVertexArray(_vao);

        // iterate through all characters
        std::string::const_iterator c;
        for (c = text.begin(); c != text.end(); c++)
        {
            const Character ch = _characters[*c];

            const float xPos = x + static_cast<float>(ch.bearing.x) * scale;
            const float yPos = y - (static_cast<float>(ch.bearing.y) * scale);
            
            const float w = ch.size.x * scale;
            const float h = ch.size.y * scale;
            // update VBO for each character
            const float vertices[6][4] = {
                { xPos,     yPos + h,   0.0f, 1.0f },            
                { xPos,     yPos,       0.0f, 0.0f },
                { xPos + w, yPos,       1.0f, 0.0f },

                { xPos,     yPos + h,   0.0f, 1.0f },
                { xPos + w, yPos,       1.0f, 0.0f },
                { xPos + w, yPos + h,   1.0f, 1.0f }           
            };
            // render glyph texture over quad
            glBindTexture(GL_TEXTURE_2D, ch.textureId);
            // update content of VBO memory
            glBindBuffer(GL_ARRAY_BUFFER, _vbo);
            glBufferSubData(GL_ARRAY_BUFFER, 0, sizeof(vertices), vertices); 
            glBindBuffer(GL_ARRAY_BUFFER, 0);
            // render quad
            glDrawArrays(GL_TRIANGLES, 0, 6);
            // now advance cursors for next glyph (note that advance is number of 1/64 pixels)
            x += static_cast<float>(ch.advance >> 6) * scale; // bitshift by 6 to get value in pixels (2^6 = 64)
        }
        glBindVertexArray(0);
        glBindTexture(GL_TEXTURE_2D, 0);
    }

    void Release()
    {
        for (auto& character : _characters)
        {
            glDeleteTextures(1, &character.second.textureId);
        }
        _characters.clear();
    }
};


class TextRenderer
{
private:
    Font* _default; 
    std::map<std::string, Font*> _fonts;
public:
    TextRenderer()
    {
        if(LoadFont("default", "Resources/Fonts/OpenSans-Regular.ttf", ShaderManager::GetInstance().CreateShader(L"Font")))
        {
            _default = _fonts["default"];
        }
    }
    
    bool LoadFont(const std::string& name, const std::string& fontFilePath, Shader* shader)
    {
        FT_Library ft;
        if (FT_Init_FreeType(&ft)) {
            std::cout << "ERROR::FREETYPE: Could not init FreeType Library\n" << std::endl;
            return false;
        }

        FT_Face face;
        if (FT_New_Face(ft, fontFilePath.c_str(), 0, &face)) {
            std::cout << "ERROR::FREETYPE: Failed to load font\n" << std::endl;
            return false;
        }

        _fonts.insert({name, new Font(shader, face)});

        FT_Done_FreeType(ft);
        
        return  true;
    }

    void Render(const std::string& text, Vector2 position)
    {
        _default->RenderText(text, position.x, position.y, 1, Color(1.0f, 1.0f, 1.0f, 1.0f));
    }

    void Release()
    {
        _default = nullptr;
        for (const auto& font : _fonts)
        {
            font.second->Release();
        }
        _fonts.clear();
    }
};
