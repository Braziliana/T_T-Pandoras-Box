#pragma once

struct MemoryBuffer
{
public:
    unsigned int size;
    unsigned char* bytes = nullptr;

    MemoryBuffer(unsigned int bufferSize);
    void Release();
    void Resize(unsigned int bufferSize);

    template <typename T>
    T Read(const unsigned int offset)
    {
        return static_cast<T>(bytes + offset);
    }
};

extern "C" {
    __declspec(dllexport) MemoryBuffer* MemoryBufferCreate(unsigned int size);
    __declspec(dllexport) void MemoryBufferRelease(MemoryBuffer* memoryBuffer);
    __declspec(dllexport) void MemoryBufferResize(MemoryBuffer* memoryBuffer, unsigned int bufferSize);

    
    __declspec(dllexport) unsigned int MemoryBufferSize(const MemoryBuffer* memoryBuffer);
    __declspec(dllexport) unsigned char* MemoryBufferBytes(const MemoryBuffer* memoryBuffer);
}