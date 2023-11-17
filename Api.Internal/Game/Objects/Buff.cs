using Api.Game.Objects;

namespace Api.Internal.Game.Objects;

internal class Buff : IBuff
{
    public IntPtr Pointer { get; set; }
    public string Name { get; set; } = string.Empty;
    public float StartTime { get; set; }
    public float EndTime { get; set; }
    public int Count { get; set; }
    public int CountAlt1 { get; set; }
    public int CountAlt2 { get; set; }
    public BuffType BuffType { get; set; }

    public void CloneFrom(IBuff buff)
    {
        Pointer = buff.Pointer;
        Name = buff.Name;
        StartTime = buff.StartTime;
        EndTime = buff.EndTime;
        Count = buff.Count;
        CountAlt1 = buff.CountAlt1;
        CountAlt2 = buff.CountAlt2;
        BuffType = buff.BuffType;
    }
    
    public bool IsHardCC()
    {
        return BuffType is BuffType.Snare or BuffType.Stun or BuffType.Knockup or BuffType.Asleep;
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }
}