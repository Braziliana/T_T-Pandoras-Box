using System.Runtime.InteropServices;
using Api.GameProcess;
using NativeWarper;

namespace Api.Internal.Game.Types;

public class TArray : IDisposable
{
    private readonly ITargetProcess _targetProcess;
    private readonly uint _offset;
    private readonly IMemoryBuffer _doubleIntMemoryBuffer;
    private IMemoryBuffer? _memoryBuffer;
    private readonly uint _intPtrSize;

    private uint _size;
    public uint Size => _size;

    private IntPtr _listPtr;
    public IntPtr ListPtr => _listPtr;
    
    public TArray(ITargetProcess targetProcess, uint offset)
    {
        _targetProcess = targetProcess;
        _offset = offset;

        _listPtr = IntPtr.Zero;
        _size = 0;
        
        _intPtrSize = (uint)Marshal.SizeOf<IntPtr>();
        _doubleIntMemoryBuffer = new MemoryBuffer(_intPtrSize + (uint)Marshal.SizeOf<int>());
        _memoryBuffer = null;
    }

    
    public bool Read()
    {
        if (!_targetProcess.ReadModulePointer(_offset, out var tArray))
        {
            return false;
        }
        
        if (!_targetProcess.Read(tArray + 0x8, _doubleIntMemoryBuffer))
        {
         return false;
        }
        
        _listPtr = _doubleIntMemoryBuffer.Read<IntPtr>(0);
        _size = _doubleIntMemoryBuffer.Read<uint>(_intPtrSize);
        
        if (_memoryBuffer is null)
        {
            _memoryBuffer = new MemoryBuffer(_size * _intPtrSize);
        }
        else
        {
            _memoryBuffer.Resize(_size * _intPtrSize);
        }

        return _targetProcess.Read(_listPtr, _memoryBuffer);
    }

    public IEnumerable<IntPtr> GetPointers()
    {
        if (_memoryBuffer is null)
        {
            yield break;
        }
        
        for (uint i = 0; i < _size; i++)
        {
            var ptr = _memoryBuffer.Read<IntPtr>(_intPtrSize * i);
            if (ptr != IntPtr.Zero && ptr.ToInt64() > 0x1000)
            {
                yield return ptr;
            }
        }
    }
    
    public void Dispose()
    {
        _memoryBuffer?.Dispose();
    }
}