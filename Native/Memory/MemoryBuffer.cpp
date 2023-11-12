#include "MemoryBuffer.h"

#include <iostream>

#include "../Windows/AppWindow.h"

MemoryBuffer::MemoryBuffer(const unsigned int bufferSize)
{
    size = bufferSize;
    bytes = new unsigned char[bufferSize];
    //std::cout << "MemoryBuffer size: " <<  bufferSize << " address: " << std::hex << this << " buffer: " << reinterpret_cast<DWORD64>(bytes) << std::dec << std::endl;
}

void MemoryBuffer::Release()
{
    delete bytes;
    bytes = nullptr;
}

void MemoryBuffer::Resize(const unsigned int bufferSize)
{
    size = bufferSize;
    bytes = new unsigned char[bufferSize];
}

MemoryBuffer* MemoryBufferCreate(const unsigned int size)
{
    return new MemoryBuffer(size);
}

void MemoryBufferRelease(MemoryBuffer* memoryBuffer)
{
    if(memoryBuffer != nullptr)
    {
        memoryBuffer->Release();
        delete memoryBuffer;
    }
}

void MemoryBufferResize(MemoryBuffer* memoryBuffer, const unsigned int bufferSize)
{
    memoryBuffer->Resize(bufferSize);
}

unsigned int MemoryBufferSize(const MemoryBuffer* memoryBuffer)
{
    return memoryBuffer->size;
}

unsigned char* MemoryBufferBytes(const MemoryBuffer* memoryBuffer)
{
    return memoryBuffer->bytes;
}
