using System.Numerics;

namespace Api.Game.Calculations;
public enum PredictionResultType
{
    Stationary,
    Moving,
    Immobile,
    Dashing,
    OutOfRange
}

public struct PredictionResult
{
    public Vector3 Position { get; }
    public float HitChance { get; }
    public PredictionResultType PredictionResultType { get; }
    public PredictionResult(Vector3 position, float hitChance, PredictionResultType predictionResultType)
    {
        Position = position;
        HitChance = hitChance;
        PredictionResultType = predictionResultType;
    }
}