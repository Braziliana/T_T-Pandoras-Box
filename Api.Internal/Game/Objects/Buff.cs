using Api.Game.Objects;

namespace Api.Internal.Game.Objects;

internal class Buff : IBuff
{
    public IntPtr Pointer { get; set; }
    public string Name { get; set; } = string.Empty;
    public float StartTime { get; set; }
    public float EndTime { get; set; }
    public int Count { get; set; }
    public int CountAlt { get; set; }
    public BuffType BuffType { get; set; }

    public void CloneFrom(IBuff buff)
    {
        Pointer = buff.Pointer;
        Name = buff.Name;
        StartTime = buff.StartTime;
        EndTime = buff.EndTime;
        Count = buff.Count;
        CountAlt = buff.CountAlt;
        BuffType = buff.BuffType;
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }
}