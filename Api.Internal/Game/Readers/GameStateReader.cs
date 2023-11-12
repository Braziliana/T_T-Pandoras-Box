using Api.Game.Objects;
using Api.Game.Offsets;
using Api.Game.Readers;
using Api.GameProcess;

namespace Api.Internal.Game.Readers;

internal class GameStateReader : IGameStateReader
{
    private readonly ITargetProcess _targetProcess;
    private readonly IBaseOffsets _baseOffsets;
    
    public GameStateReader(ITargetProcess targetProcess, IBaseOffsets baseOffsets)
    {
        _targetProcess = targetProcess;
        _baseOffsets = baseOffsets;
    }
    
    public void ReadGameState(IGameState gameState)
    {
        if (_targetProcess.ReadModule<float>(_baseOffsets.GameTime, out var time))
        {
            gameState.Time = time;
        }
        else
        {
            gameState.Time = 0;
        }
    }
}