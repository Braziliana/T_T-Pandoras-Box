using System.Runtime.InteropServices;

namespace Api.GameProcess;

[StructLayout(LayoutKind.Sequential)]
public struct MemoryBufferInternal
{
    public uint size;
    public IntPtr bytes;
}

public unsafe interface IMemoryBuffer : IDisposable
{
    public T Read<T>(uint offset) where T : unmanaged;
    public T? ReadManaged<T>(uint offset);
    public T Cast<T>() where T : unmanaged;
    public MemoryBufferInternal* GetInternalBuffer();
    void Resize(uint size);
}

public interface ITargetProcess : IDisposable
{
    void SetTargetProcessName(string processName);
    bool Hook();
    bool IsProcessRunning();
    bool Read(IntPtr address, byte[] bytes);
    bool Read<T>(IntPtr address, out T value) where T : unmanaged;
    bool Read(IntPtr address, uint size, out byte[] bytes);
    bool Read(IntPtr address, IMemoryBuffer memoryBuffer);
    bool ReadModule(uint offset, byte[] bytes);
    bool ReadModule<T>(uint offset, out T value) where T : unmanaged;
    bool ReadModule(uint offset, uint size, out byte[] bytes);
    bool ReadModule(uint offset, IMemoryBuffer memoryBuffer);
    bool ReadPointer(IntPtr memoryAddress, out IntPtr value);
    bool ReadModulePointer(uint offset, out IntPtr pointer);
    IntPtr FindOffset(string patternStr, int pos);
    IntPtr FindOffset(byte[] pattern, string mask, uint patternSize, int pos);
}