using Api.Game.Offsets;
using Microsoft.Extensions.Configuration;

namespace Api.Internal.Game.Offsets;

internal class GameCameraOffsets : IGameCameraOffsets
{
    public OffsetData ViewProjMatrix { get; }
    public OffsetData Renderer { get; }
    public OffsetData RendererWidth { get; }
    public OffsetData RendererHeight { get; }

    public GameCameraOffsets(IConfiguration configuration)
    {
        var cs = configuration.GetSection(nameof(GameCameraOffsets));
        ViewProjMatrix = new OffsetData(nameof(ViewProjMatrix), Convert.ToUInt32(cs[nameof(ViewProjMatrix)], 16), typeof(int));
        Renderer = new OffsetData(nameof(Renderer), Convert.ToUInt32(cs[nameof(Renderer)], 16), typeof(int));
        RendererWidth = new OffsetData(nameof(RendererWidth), Convert.ToUInt32(cs[nameof(RendererWidth)], 16), typeof(int));
        RendererHeight = new OffsetData(nameof(RendererHeight), Convert.ToUInt32(cs[nameof(RendererHeight)], 16), typeof(int));
    }
}