using System.Numerics;
using System.Runtime.InteropServices;
using Api.Game.Objects;
using Api.Game.Offsets;
using Api.Game.Readers;
using Api.GameProcess;
using NativeWarper;

namespace Api.Internal.Game.Readers;

internal class AiManagerReader : BaseReader, IAiManagerReader
{
	private const int MaxSegmentsToRead = 12;
	private readonly uint _vectorSize = (uint)Marshal.SizeOf<Vector3>();
	private readonly IAiManagerOffsets _aiManagerOffsets;
	private readonly IMemoryBuffer _segmentsBatchReadContext;

	public AiManagerReader(IAiManagerOffsets aiManagerOffsets, ITargetProcess targetProcess) : base(targetProcess)
	{
		_aiManagerOffsets = aiManagerOffsets;
		//We will read max 15 waypoints i dont see any reason to read more when i saw 5-8 typicly
		_segmentsBatchReadContext = new MemoryBuffer(MaxSegmentsToRead * _vectorSize);
	}

	public void ReadAiManager(IHero hero, IntPtr aiManagerPointer)
	{
		if (!TargetProcess.ReadPointer(aiManagerPointer, out var aiManager))
		{
			return;
		}

		if (!StartRead(aiManager))
		{
			return;
		}
		
	    hero.AiManager.Pointer = aiManager;
	    
	    hero.AiManager.TargetPosition = ReadOffset<Vector3>(_aiManagerOffsets.TargetPosition);
	    hero.AiManager.PathStart = ReadOffset<Vector3>(_aiManagerOffsets.PathStart);
	    hero.AiManager.PathEnd = ReadOffset<Vector3>(_aiManagerOffsets.PathEnd);
	    hero.AiManager.CurrentPathSegment = ReadOffset<int>(_aiManagerOffsets.CurrentPathSegment);
	    hero.AiManager.PathSegmentsCount = ReadOffset<int>(_aiManagerOffsets.PathSegmentsCount);
	    hero.AiManager.CurrentPosition = ReadOffset<Vector3>(_aiManagerOffsets.CurrentPosition);
	    hero.AiManager.IsDashing = ReadOffset<bool>(_aiManagerOffsets.IsDashing);
	    hero.AiManager.DashSpeed = ReadOffset<float>(_aiManagerOffsets.DashSpeed);
	    hero.AiManager.IsMoving = ReadOffset<bool>(_aiManagerOffsets.IsMoving);
	    hero.AiManager.MovementSpeed = ReadOffset<float>(_aiManagerOffsets.MovementSpeed);

	    hero.AiManager.PathSegments.Clear();
	    if (hero.AiManager.PathSegmentsCount > MaxSegmentsToRead)
	    {
		    hero.AiManager.PathSegmentsCount = MaxSegmentsToRead;
	    }
	    
	    var pathSegmentsPtr = ReadOffset<IntPtr>(_aiManagerOffsets.PathSegments);
	    if (ReadBuffer(pathSegmentsPtr, _segmentsBatchReadContext))
	    {
		    for (uint i = 0; i < hero.AiManager.PathSegmentsCount; i++)
		    {
			    hero.AiManager.PathSegments.Add(_segmentsBatchReadContext.Read<Vector3>(_vectorSize*i));
		    }
	    }
	}
    
    protected override IMemoryBuffer CreateBatchReadContext()
    {
	    var size = GetSize(_aiManagerOffsets.GetOffsets());
	    return new MemoryBuffer(size);
    }
    
    public override void Dispose()
    {
	    base.Dispose();
	    _segmentsBatchReadContext.Dispose();
    }
    
    /*
  int64 fastcall sub_7FF7A3AD5F00(int64 a1)
{
  unsigned int8 v1; // rdx
  unsigned int64 v2; // rcx
  unsigned int64 v3; // r8
  int64 v4; // r9
  unsigned int64 v5; // rax
  int64 v7; // [rsp+8h] [rbp+8h]

  v1 = (unsigned int8)(a1 + 0x3700);
  v2 = 0i64;
  v3 = v1[1];
  v4 = (_QWORD)&v1[8 * v1[16] + 24];
  v7 = v4;
  if ( v3 )
  {
    do
    {
      (&v7 + v2) ^= ~(_QWORD )&v1[8 v2 + 8];
      ++v2;
    }
    while ( v2 < v3 );
    v4 = v7;
  }
  if ( !v1[2] )
    return (_QWORD)(v4 + 16);
  v5 = 8i64 - v1[2];
  if ( v5 >= 8 )
    return (_QWORD)(v4 + 16);
  do
  {
    ((_BYTE)&v7 + v5) ^= ~v1[v5 + 8];
    ++v5;
  }
  while ( v5 < 8 );
  return (_QWORD)(v7 + 16);
}
     
     */
}