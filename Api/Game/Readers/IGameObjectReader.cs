using System.Numerics;
using System.Text;
using Api.Game.Objects;
using Api.Game.Offsets;
using Api.GameProcess;

namespace Api.Game.Readers;

public interface IGameObjectReader
{
    bool ReadBuffer(IntPtr ptr, IMemoryBuffer memoryBuffer);
    T ReadOffset<T>(OffsetData offsetData, IMemoryBuffer memoryBuffer) where T : unmanaged;
    string ReadString(OffsetData offsetData, Encoding encoding, IMemoryBuffer memoryBuffer);
    bool ReadObject(IGameObject? gameObject);
    /// <summary>
    /// batchReadContext must be initialized and read manully before with ReadBuffer(IntPtr ptr, BatchReadContext batchReadContext)
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="memoryBuffer"></param>
    /// <returns></returns>
    bool ReadObject(IGameObject? gameObject, IMemoryBuffer memoryBuffer);
    string ReadObjectName(IMemoryBuffer memoryBuffer);
    int ReadObjectNetworkId(IMemoryBuffer memoryBuffer);
    public uint GetSize(IEnumerable<OffsetData> offsetData);
    public uint GetBufferSize();
}