using System.Diagnostics;
using System.Runtime.InteropServices;
using Api.GameProcess;

namespace NativeWarper;


public unsafe class MemoryBuffer : IMemoryBuffer, IDisposable
{
    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr MemoryBufferCreate(uint size);

    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void MemoryBufferRelease(MemoryBufferInternal* memoryBuffer);

    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void MemoryBufferResize(MemoryBufferInternal* memoryBuffer, uint size);

    private IntPtr _memoryBufferInternal;

    private MemoryBufferInternal* MemoryBufferInternal => (MemoryBufferInternal*)_memoryBufferInternal;

    public MemoryBuffer(uint size)
    {
        _memoryBufferInternal = MemoryBufferCreate(size);
    }

    ~MemoryBuffer()
    {
        ReleaseUnmanagedResources();
    }

    public T Read<T>(uint offset) where T : unmanaged
    {
        if (offset + sizeof(T) > MemoryBufferInternal->size)
        {
            Console.WriteLine($"Attempted to read beyond the buffer size. Required size: {(offset + sizeof(T)):X} bufferSize: {MemoryBufferInternal->size:X}");
            return default;
            //throw new ArgumentOutOfRangeException("Attempted to read beyond the buffer size.");
        }

        return *(T*)(MemoryBufferInternal->bytes + (int)offset);
    }
    
    public T? ReadManaged<T>(uint offset)
    {
        return Marshal.PtrToStructure<T>(MemoryBufferInternal->bytes + (int)offset);
    }

    public T Cast<T>() where T : unmanaged
    {
        return *(T*)(MemoryBufferInternal->bytes);
    }

    public MemoryBufferInternal* GetInternalBuffer()
    {
        return MemoryBufferInternal;
    }

    public void Resize(uint size)
    {
        MemoryBufferResize(MemoryBufferInternal, size);
    }

    private void ReleaseUnmanagedResources()
    {
        if (_memoryBufferInternal == IntPtr.Zero) return;
        
        MemoryBufferRelease(MemoryBufferInternal);
        _memoryBufferInternal = IntPtr.Zero;
    }

    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }
}


public unsafe class GameProcess : ITargetProcess
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
    
    [DllImport("Native.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr PatternScannerFindOffset(string patternStr, int pos);

    [DllImport("Native.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr PatternScannerFindOffsetWithMask(byte[] pattern, string mask, uint patternSize, int pos);

    private readonly Dictionary<uint, IMemoryBuffer> _buffers = new Dictionary<uint, IMemoryBuffer>();
    private readonly Dictionary<uint, byte[]> _byteBuffers = new Dictionary<uint, byte[]>();
    
    public GameProcess()
    {
        ProcessSetTargetProcessName("League of Legends.exe");
    }

    private IMemoryBuffer GetBuffer(uint size)
    {
        if (_buffers.TryGetValue(size, out var buffer))
        {
            return buffer;
        }

        buffer = new MemoryBuffer(size);
        _buffers.Add(size, buffer);
        
        return buffer;
    }
    
    private byte[] GetByteBuffer(uint size)
    {
        if (_byteBuffers.TryGetValue(size, out var buffer))
        {
            return buffer;
        }

        buffer = new byte[size];
        _byteBuffers.Add(size, buffer);
        
        return buffer;
    }
    
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

    public bool Read(IntPtr address, byte[] bytes)
    {
        return ProcessRead(address, (uint)bytes.Length, bytes);
    }

    public bool Read<T>(IntPtr address, out T value) where T : unmanaged
    {
        var buffer = GetBuffer((uint)sizeof(T));
        if (Read(address, buffer))
        {
            value = buffer.Cast<T>();
            return true;
        }

        value = default;
        return false;
    }

    public bool Read(IntPtr address, uint size, out byte[] bytes)
    {
        bytes = GetByteBuffer(size);
        return ProcessRead(address, size, bytes);
    }
    
    public bool Read(IntPtr address, IMemoryBuffer memoryBuffer)
    {
        return ProcessReadBuffer(address, memoryBuffer.GetInternalBuffer());
    }
    
    public bool ReadModule(uint offset, byte[] bytes)
    {
        return ProcessReadModule(offset, (uint)bytes.Length, bytes);
    }

    public bool ReadModule<T>(uint offset, out T value) where T : unmanaged
    {
        var buffer = GetBuffer((uint)sizeof(T));
        if (ReadModule(offset, buffer))
        {
            value = buffer.Cast<T>();
            return true;
        }

        value = default;
        return false;
    }

    public bool ReadModule(uint offset, uint size, out byte[] bytes)
    {
        bytes = GetByteBuffer(size);
        return ReadModule(offset, bytes);
    }
    
    public bool ReadModule(uint offset, IMemoryBuffer memoryBuffer)
    {
        return ProcessReadModuleBuffer(offset, memoryBuffer.GetInternalBuffer());
    }
    
    public bool ReadModulePointer(uint offset, out IntPtr value)
    {
        return ReadModule(offset, out value);
    }

    public bool ReadPointer(IntPtr memoryAddress, out IntPtr value)
    {
        return Read(memoryAddress, out value);
    }

    public IntPtr FindOffset(string patternStr, int pos)
    {
        return PatternScannerFindOffset(patternStr, pos);
    }

    public IntPtr FindOffset(byte[] pattern, string mask, uint patternSize, int pos)
    {
        return PatternScannerFindOffsetWithMask(pattern, mask, patternSize, pos);
    }
    
    public void Dispose()
    {
        foreach (var memoryBuffer in _buffers)
        {
            memoryBuffer.Value.Dispose();
        }
    }
}