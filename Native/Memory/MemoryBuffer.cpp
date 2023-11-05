#include "MemoryBuffer.h"

MemoryBuffer::MemoryBuffer(size_t bufferSize)
{
    size = bufferSize;
    bytes = new unsigned char[bufferSize];
}

void MemoryBuffer::Release()
{
    delete bytes;
    bytes = nullptr;
}

MemoryBuffer* Create(size_t size)
{
    return new MemoryBuffer(size);
}

void Release(MemoryBuffer* memoryBuffer)
{
    if(memoryBuffer != nullptr)
    {
        memoryBuffer->Release();
        delete memoryBuffer;
    }
}
