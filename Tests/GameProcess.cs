using System.Runtime.InteropServices;

namespace Tests;

[StructLayout(LayoutKind.Sequential)]
public struct MemoryBufferInternal
{
    public uint size;
    public IntPtr bytes;
}

public unsafe class MemoryBuffer : IDisposable
{
    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern MemoryBufferInternal* Create(uint size);

    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void Release(MemoryBufferInternal* memoryBuffer);

    private MemoryBufferInternal* _memoryBufferInternal;

    public MemoryBuffer(uint size)
    {
        _memoryBufferInternal = Create(size);
    }

    ~MemoryBuffer()
    {
        ReleaseUnmanagedResources();
    }

    public T Read<T>(int offset) where T : unmanaged
    {
        if (offset + sizeof(T) > _memoryBufferInternal->size)
            throw new ArgumentOutOfRangeException("Attempted to read beyond the buffer size.");

        return *(T*)(_memoryBufferInternal->bytes + offset);
    }
    
    public T? ReadManaged<T>(int offset)
    {
        return Marshal.PtrToStructure<T>(_memoryBufferInternal->bytes + offset);
    }

    public MemoryBufferInternal* GetInternalBuffer()
    {
        return _memoryBufferInternal;
    }

    private void ReleaseUnmanagedResources()
    {
        if (_memoryBufferInternal == null) return;
        
        Release(_memoryBufferInternal);
        _memoryBufferInternal = null;
    }

    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }
}


public unsafe class GameProcess
{
    [DllImport("Native.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
    private static extern void ProcessSetTargetProcessName(string processName);

    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool ProcessRead(IntPtr address, uint size, byte[] result);

    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool ProcessReadBuffer(IntPtr address, MemoryBufferInternal* memoryBufferInternal);

    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool ProcessReadModule(uint offset, uint size, byte[] result);

    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool ProcessReadModuleBuffer(uint offset, MemoryBufferInternal* memoryBufferInternal);

    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool ProcessHook();
    
    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool ProcessIsRunning();

    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern uint ProcessGetId();

    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr ProcessGetModuleBase();

    public void SetTargetProcessName(string processName)
    {
        ProcessSetTargetProcessName(processName);
    }
    
    public bool Hook()
    {
        return ProcessHook();
    }

    public bool IsProcessRunning()
    {
        return ProcessIsRunning();
    }

    public bool Read(IntPtr address, byte[] bytes, uint size)
    {
        return ProcessRead(address, size, bytes);
    }
    
    public bool Read(IntPtr address, MemoryBuffer memoryBuffer)
    {
        return ProcessReadBuffer(address, memoryBuffer.GetInternalBuffer());
    }
    
    public bool ReadModule(uint offset, byte[] bytes, uint size)
    {
        return ProcessReadModule(offset, size, bytes);
    }
    
    public bool ReadModule(uint offset, MemoryBuffer memoryBuffer)
    {
        return ProcessReadModuleBuffer(offset, memoryBuffer.GetInternalBuffer());
    }
}