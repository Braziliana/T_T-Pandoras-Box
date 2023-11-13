using Api.Game.Objects;
using Api.GameProcess;

namespace Api.Game.Readers;

public interface IPlantReader : IAttackableUnitReader
{
    bool ReadPlant(IPlant? plant);
    bool ReadPlant(IPlant? plant, IMemoryBuffer memoryBuffer);
}