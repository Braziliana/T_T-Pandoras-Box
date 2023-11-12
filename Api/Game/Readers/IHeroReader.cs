using Api.Game.Objects;
using Api.GameProcess;

namespace Api.Game.Readers;

public interface IHeroReader : IAiBaseUnitReader
{
    bool ReadHero(IHero? hero);
    bool ReadHero(IHero? hero, IMemoryBuffer memoryBuffer);
}