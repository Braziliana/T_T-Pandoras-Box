﻿using System.Numerics;
using Api.Game.Objects;

namespace Api.Internal.Game.Objects;

internal class AiManager : IAiManager
{
    public IntPtr Pointer { get; set; }
    public Vector3 TargetPosition { get; set; }
    public Vector3 PathStart { get; set; }
    public Vector3 PathEnd { get; set; }
    public int CurrentPathSegment { get; set; }
    public List<Vector3> PathSegments { get; set; } = new List<Vector3>();
    public int PathSegmentsCount { get; set; }
    public Vector3 CurrentPosition { get; set; }
    public bool IsDashing { get; set; }
    public float DashSpeed { get; set; }
    public bool IsMoving { get; set; }
    public float MovementSpeed { get; set; }
    public IEnumerable<Vector3> RemainingPath => GetRemainingPath();
    public IEnumerable<Vector3> GetRemainingPath()
    {
        yield return CurrentPosition;
        if (CurrentPathSegment + 1 >= PathSegmentsCount)
        {
            yield return TargetPosition;
            yield break;
        }
        for (var i = CurrentPathSegment + 1; i < PathSegmentsCount; i++)
        {
            yield return PathSegments[i];
        }
    }
}