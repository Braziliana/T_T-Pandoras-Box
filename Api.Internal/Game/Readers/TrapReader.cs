using Api.Game.Objects;
using Api.Game.Offsets;
using Api.Game.Readers;
using Api.GameProcess;

namespace Api.Internal.Game.Readers;

internal class TrapReader : GameObjectReader, ITrapReader
{
    public TrapReader(ITargetProcess targetProcess, IGameObjectOffsets gameObjectOffsets) : base(targetProcess, gameObjectOffsets)
    {
    }

    public bool ReadTrap(ITrap? trap)
    {
        if (trap is null || !ReadObject(trap))
        {
            return false;
        }

        return true;
    }

    public bool ReadTrap(ITrap? trap, IMemoryBuffer memoryBuffer)
    {
        if (trap is null || !ReadObject(trap, memoryBuffer))
        {
            return false;
        }

        return true;
    }
}