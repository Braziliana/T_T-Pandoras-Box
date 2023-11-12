using System.Runtime.InteropServices;
using System.Text;
using Api.Game.Objects;
using Api.Game.Offsets;
using Api.GameProcess;
using Api.Internal.Game.Types;

namespace Api.Internal.Game.Readers;

public abstract class BaseReader : IDisposable
{
    private readonly byte[] _stringBuffer;
    private IMemoryBuffer? _memoryBuffer;
    protected readonly byte[] CharArray = new byte[64];
    protected readonly ITargetProcess TargetProcess;
    
    protected IMemoryBuffer MemoryBuffer => _memoryBuffer ??= CreateBatchReadContext();

    protected BaseReader(ITargetProcess targetProcess)
    {
        TargetProcess = targetProcess;
        _stringBuffer = new byte[255];
    }

    protected abstract IMemoryBuffer CreateBatchReadContext();

       
    protected bool StartRead(IBaseObject baseObject)
    {
        return StartRead(baseObject, MemoryBuffer);
    }
    
    protected T ReadOffset<T>(OffsetData offsetData) where T : unmanaged
    {
        return ReadOffset<T>(offsetData, MemoryBuffer);
    }
    
    public bool StartRead(IBaseObject baseObject, IMemoryBuffer memoryBuffer)
    {
        var result = ReadBuffer(baseObject.Pointer, memoryBuffer);
        baseObject.IsValid = result;

        return result;
    }
    
    public bool StartRead(IntPtr ptr)
    {
        return ReadBuffer(ptr, MemoryBuffer);
    }
    
    public bool ReadBuffer(IntPtr ptr, IMemoryBuffer memoryBuffer)
    {
        return TargetProcess.Read(ptr, memoryBuffer);
    }

    public T ReadOffset<T>(OffsetData offsetData, IMemoryBuffer memoryBuffer) where T : unmanaged
    {
        return memoryBuffer.Read<T>(offsetData.Offset);
    }

    public string ReadString(OffsetData offsetData, Encoding encoding, IMemoryBuffer memoryBuffer)
    {
        var ts = ReadOffset<TString>(offsetData, memoryBuffer);
        if (ts._maxContentLength <= 0 || ts._contentLength <= 0)
        {
            return string.Empty;
        }
        
        if (ts._maxContentLength < 16)
        {
            return encoding.GetString(ts.GetSpan());
        }

        var ptr = ts.GetPtr();
        if (ptr == IntPtr.Zero || !TargetProcess.Read(ptr, _stringBuffer))
        {
            return string.Empty;
        }
            
        var length = _stringBuffer.TakeWhile(t => t != 0).Count();
        return encoding.GetString(_stringBuffer, 0, length);
    }
    
    protected string ReadString(OffsetData offsetData, Encoding encoding)
    {
        return ReadString(offsetData, encoding, MemoryBuffer);
    }
    
    public string ReadString(IntPtr pointer, Encoding encoding)
    {
        if (!TargetProcess.Read<TString>(pointer, out var ts))
        {
            return string.Empty;
        }
        if (ts._maxContentLength <= 0 || ts._contentLength <= 0)
        {
            return string.Empty;
        }
        
        if (ts._maxContentLength < 16)
        {
            return encoding.GetString(ts.GetSpan());
        }

        var ptr = ts.GetPtr();
        if (ptr == IntPtr.Zero || !TargetProcess.Read(ptr, _stringBuffer))
        {
            return string.Empty;
        }
            
        var length = _stringBuffer.TakeWhile(t => t != 0).Count();
        return encoding.GetString(_stringBuffer, 0, length);
    }

    public string ReadCharArray(IntPtr strPtr, Encoding encoding)
    {
        if (!TargetProcess.Read(strPtr, CharArray))
        {
            return string.Empty;
        }
            
        var length = CharArray.TakeWhile(t => t != 0).Count();
        if (length <= 0)
        {
            return string.Empty;
        }
        return encoding.GetString(CharArray, 0, length);
    }
    
    public uint GetSize(IEnumerable<OffsetData> offsetData)
    {
        return offsetData.Select(data => data.Offset + data.TargetSize).Prepend((uint)0).Max();
    }
    
    public virtual void Dispose()
    {
        _memoryBuffer?.Dispose();
    }
}