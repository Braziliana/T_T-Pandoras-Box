#pragma once

struct MemoryBuffer
{
public:
    size_t size;
    unsigned char* bytes = nullptr;

    MemoryBuffer(size_t bufferSize);

    void Release();

    template <typename T>
    T Read(const size_t offset)
    {
        return static_cast<T>(bytes + offset);
    }
};

extern "C" {
    __declspec(dllexport) MemoryBuffer* Create(size_t size);
    __declspec(dllexport) void Release(MemoryBuffer* memoryBuffer);
}