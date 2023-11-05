#include "MemoryBuffer.h"

MemoryBuffer::MemoryBuffer(unsigned int bufferSize)
{
    size = bufferSize;
    bytes = new unsigned char[bufferSize];
}

void MemoryBuffer::Release()
{
    delete bytes;
    bytes = nullptr;
}

MemoryBuffer* Create(unsigned int size)
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
