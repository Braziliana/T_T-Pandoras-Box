using System.Runtime.InteropServices;

namespace Api.Game.Offsets;

public struct OffsetData
{
    public string Name { get; }
    public uint Offset { get; }
    public uint TargetSize { get; }

    public OffsetData(string name, uint offset, Type type)
    {
        Name = name;
        Offset = offset;
        TargetSize = (uint)Marshal.SizeOf(type);
    }
}