using System.Numerics;
using System.Runtime.InteropServices;
using Api.Game.Objects;
using Api.Game.Offsets;
using Api.Game.Readers;
using Api.GameProcess;
using NativeWarper;

namespace Api.Internal.Game.Readers;

internal class GameCameraReader : IGameCameraReader
{
	private readonly ITargetProcess _targetProcess;
	private readonly IGameCameraOffsets _gameCameraOffsets;
	private readonly IMemoryBuffer _matrixMemoryBuffer;
	private readonly IMemoryBuffer _doubleIntMemoryBuffer;
	private readonly uint _matrixSize;
	private readonly uint _intSize;
	
	public GameCameraReader(ITargetProcess targetProcess, IGameCameraOffsets gameCameraOffsets)
	{
		_targetProcess = targetProcess;
		_gameCameraOffsets = gameCameraOffsets;
		_matrixSize = (uint)Marshal.SizeOf<Matrix4x4>();
		_intSize = (uint)Marshal.SizeOf<int>();
		_matrixMemoryBuffer = new MemoryBuffer(_matrixSize * 2);
		_doubleIntMemoryBuffer = new MemoryBuffer(_intSize * 2);
	}
	
    public bool ReadCamera(IGameCamera? gameCamera)
    {
        if (gameCamera is null)
        {
            return false;
        }

        if (!ReadMatrices(gameCamera))
        {
	        gameCamera.IsValid = false;
	        return false;
        }

        if (!ReadSize(gameCamera))
        {
	        gameCamera.IsValid = false;
	        return false;
        }
        
        return true;
    }

    private bool ReadSize(IGameCamera gameCamera)
    {
	    if (!gameCamera.RequireFullUpdate)
	    {
		    return true;
	    }
	    
	    if(!_targetProcess.ReadModulePointer(_gameCameraOffsets.Renderer.Offset, out var rendererPtr))
	    {
		    return false;
	    }
	    
	    if (!_targetProcess.Read(rendererPtr + (int)_gameCameraOffsets.RendererWidth.Offset, _doubleIntMemoryBuffer))
	    {
		    return false;
	    }

	    gameCamera.RendererWidth = _doubleIntMemoryBuffer.Read<int>(0);
	    gameCamera.RendererHeight = _doubleIntMemoryBuffer.Read<int>(_intSize);
	    gameCamera.RequireFullUpdate = false;
	    
	    return true;
    }
    
    private bool ReadMatrices(IGameCamera gameCamera)
    {
	    if (!_targetProcess.ReadModule(_gameCameraOffsets.ViewProjMatrix.Offset, _matrixMemoryBuffer))
	    {
		    return false;
	    }

	    var viewMatrix = _matrixMemoryBuffer.Read<Matrix4x4>(0);
	    var projMatrix = _matrixMemoryBuffer.Read<Matrix4x4>(_matrixSize);

	    gameCamera.ViewProjMatrix = viewMatrix * projMatrix;

	    return true;
    }
}