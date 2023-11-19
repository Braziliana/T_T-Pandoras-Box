using System.Runtime.InteropServices;
using System.Text;
using Api.Game.Objects;
using Api.Game.Offsets;
using Api.Game.Readers;
using Api.GameProcess;
using Api.Internal.Game.Objects;
using Api.Utils;
using NativeWarper;

namespace Api.Internal.Game.Readers;

internal class BuffReader : BaseReader, IBuffReader
{
	private readonly IBuffOffsets _buffOffsets;
	private readonly IGameState _gameState;
	private readonly ObjectPool<IBuff> _buffPool = new ObjectPool<IBuff>(400, () => new Buff());

	public BuffReader(
		ITargetProcess targetProcess,
		IBuffOffsets buffOffsets,
		IGameState gameState) : base(targetProcess)
	{
		_buffOffsets = buffOffsets;
		_gameState = gameState;
	}

	public void ReadBuffs(IDictionary<string, IBuff> buffDictionary, IntPtr start, IntPtr end)
    {
	    foreach (var buff in buffDictionary)
	    {
		    _buffPool.Stash(buff.Value);
	    }
	    buffDictionary.Clear();
	    
        if (start.ToInt64() < 0x1000 || end.ToInt64() < 0x1000 || end.ToInt64() < start.ToInt64())
        {
	        return;
        }
        
        var size = ((int)(end.ToInt64() - start.ToInt64()) / 0x8) + 1;
        if (size > 255)
        {
	        return;
        }

        for (var i = 0; i < size; i+=2)
        {
	        var ptr = start + 0x8 * i;
	        if (ptr.ToInt64() < 0x1000)
	        {
		        continue;
	        }

	        var buff = ReadBuff(ptr);
	        if (buff != null)
	        {
		        if (buffDictionary.TryGetValue(buff.Name, out var altBuf))
		        {
					if (altBuf.Count < buff.Count || altBuf.CountAlt2 < buff.CountAlt2)
					{
						altBuf.CloneFrom(buff);
					}
                }
		        else
		        {
			        buffDictionary.Add(buff.Name, buff);
		        }
            }
        }
    }
    
    private IBuff? ReadBuff(IntPtr ptr)
    {
	    if(!TargetProcess.ReadPointer(ptr, out ptr))
	    {
		    return null;
	    }
	    
		if (!StartRead(ptr))
	    {
		    return null;
	    }

		var buffType = ReadOffset<sbyte>(_buffOffsets.BuffType);
		if(buffType < 0 || buffType > 60)
		{
			return null;
		}

        var count = ReadOffset<byte>(_buffOffsets.BuffEntryBuffCount);
	    if (count < 1)
	    {
            return null;
	    }

	    var countAlt1 = ReadOffset<byte>(_buffOffsets.BuffEntryBuffCountAlt1);
        var countAlt2 = ReadOffset<byte>(_buffOffsets.BuffEntryBuffCountAlt2);

        var startTime = ReadOffset<float>(_buffOffsets.BuffEntryBuffStartTime);
	    if (startTime < 0)
        {
            return null;
	    }
	    
	    var endTime = ReadOffset<float>(_buffOffsets.BuffEntryBuffEndTime);

		var buffInfoPtr = ReadOffset<IntPtr>(_buffOffsets.BuffInfo);
		if (buffInfoPtr.ToInt64() < 0x1000)
		{
			return null;
		}

		string name = string.Empty;
		if (TargetProcess.ReadPointer(buffInfoPtr + (int)_buffOffsets.BuffInfoName.Offset, out var buffNamePtr))
		{
			name = ReadCharArray(buffNamePtr, Encoding.ASCII);
			if (string.IsNullOrWhiteSpace(name) || name.Count(char.IsLetter) < 3)
			{
				return null;
			}
		}

		var buff = _buffPool.Get();
	    buff.Pointer = ptr;
	    buff.StartTime = startTime;
	    buff.EndTime = endTime;
	    buff.Name = name;
	    buff.Count = count;
	    buff.CountAlt1 = countAlt1;
        buff.CountAlt2 = countAlt2;
        buff.BuffType = (BuffType)buffType;

        return buff;
    }

    protected override IMemoryBuffer CreateBatchReadContext()
    {
	    var size = GetSize(_buffOffsets.GetOffsets());
	    return new MemoryBuffer(size);
    }
}