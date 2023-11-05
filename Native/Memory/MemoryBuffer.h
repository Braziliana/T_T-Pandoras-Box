#pragma once

struct MemoryBuffer
{
public:
    unsigned int size;
    unsigned char* bytes = nullptr;

    MemoryBuffer(unsigned int bufferSize);

    void Release();

    template <typename T>
    T Read(const unsigned int offset)
    {
        return static_cast<T>(bytes + offset);
    }
};

extern "C" {
    __declspec(dllexport) MemoryBuffer* Create(unsigned int size);
    __declspec(dllexport) void Release(MemoryBuffer* memoryBuffer);
}