using System.Numerics;
using System.Text;
using Api.Game.Objects;
using Api.Game.Offsets;
using Api.Game.Readers;
using Api.GameProcess;
using Api.Internal.Game.Objects;
using Api.Internal.Game.Offsets;
using Microsoft.Extensions.Options;
using NativeWarper;

namespace Api.Internal.Game.Readers;

internal class GameObjectReader : BaseReader, IGameObjectReader
{
    private readonly List<int> invalidObjectNames = new()
    {
        "cube".GetHashCode()
    };
    
    protected readonly IGameObjectOffsets GameObjectOffsets;
    
    public GameObjectReader(ITargetProcess targetProcess, IGameObjectOffsets gameObjectOffsets) : base(targetProcess)
    {
        GameObjectOffsets = gameObjectOffsets;
    }

    public bool ReadObject(IGameObject? gameObject)
    {
        if (gameObject is null || gameObject.Pointer.ToInt64() < 0x1000)
        {
            return false;
        }

        if (!StartRead(gameObject))
        {
            return false;
        }
        
        gameObject.IsVisible = ReadOffset<bool>(GameObjectOffsets.IsVisible);
        gameObject.Position = ReadOffset<Vector3>(GameObjectOffsets.Position);

        if (!gameObject.RequireFullUpdate)
        {
            return true;
        }
        
        gameObject.Name = ReadString(GameObjectOffsets.Name, Encoding.UTF8);
        gameObject.ObjectName = ReadString(GameObjectOffsets.ObjectName, Encoding.ASCII);
        if (string.IsNullOrWhiteSpace(gameObject.Name) && string.IsNullOrWhiteSpace(gameObject.ObjectName))
        {
            return false;
        }

        gameObject.ObjectNameHash = gameObject.ObjectName.GetHashCode();
        if (invalidObjectNames.Contains(gameObject.ObjectNameHash))
        {
            return false;
        }
        
        gameObject.Team = ReadOffset<int>(GameObjectOffsets.Team);
        gameObject.NetworkId = ReadOffset<int>(GameObjectOffsets.NetworkId);
        
        return true;
    }
    
    public bool ReadObject(IGameObject? gameObject, IMemoryBuffer memoryBuffer)
    {
        if (gameObject is null || gameObject.Pointer == IntPtr.Zero)
        {
            return false;
        }
        
        gameObject.IsVisible = ReadOffset<bool>(GameObjectOffsets.IsVisible, memoryBuffer);
        gameObject.Position = ReadOffset<Vector3>(GameObjectOffsets.Position, memoryBuffer);

        if (!gameObject.RequireFullUpdate)
        {
            return true;
        }
        
        gameObject.Name = ReadString(GameObjectOffsets.Name, Encoding.UTF8, memoryBuffer);
        gameObject.ObjectName = ReadString(GameObjectOffsets.ObjectName, Encoding.ASCII, memoryBuffer);
        if (string.IsNullOrWhiteSpace(gameObject.Name) && string.IsNullOrWhiteSpace(gameObject.ObjectName))
        {
            return false;
        }

        gameObject.ObjectNameHash = gameObject.ObjectName.GetHashCode();
        if (invalidObjectNames.Contains(gameObject.ObjectNameHash))
        {
            return false;
        }
        
        gameObject.Team = ReadOffset<int>(GameObjectOffsets.Team, memoryBuffer);
        gameObject.NetworkId = ReadOffset<int>(GameObjectOffsets.NetworkId, memoryBuffer);
        
        return true;
    }

    public string ReadObjectName(IMemoryBuffer batchReadContext)
    {
        return ReadString(GameObjectOffsets.ObjectName, Encoding.ASCII, batchReadContext);
    }

    public int ReadObjectNetworkId(IMemoryBuffer batchReadContext)
    {
        return ReadOffset<int>(GameObjectOffsets.NetworkId, batchReadContext);
    }

    public virtual uint GetBufferSize()
    {
        return GetSize(GameObjectOffsets.GetOffsets());
    }

    protected override IMemoryBuffer CreateBatchReadContext()
    {
        var size = GetBufferSize();
        return new MemoryBuffer(size);
    }
}