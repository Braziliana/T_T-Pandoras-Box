using System.Numerics;
using Api.Game.Objects;

namespace Api.Game.Calculations;

[Flags]
public enum CollisionType
{
    None,
    Minion,
    Hero,
    WindWall
}

public enum PredictionType
{
    Line,
    Point
}

public interface IPrediction
{
    PredictionResult PredictPosition(IHero target, Vector3 sourcePosition, float delay, float speed, float radius,
        float range, float reactionTime, float dashTimeThreshold, CollisionType collisionType,
        PredictionType predictionType);

    PredictionResult PredictImmobile(IHero target, Vector3 sourcePosition, float delay, float speed, float radius,
        float range, float immobileTime, CollisionType collisionType, PredictionType predictionType);

    PredictionResult PredictDashing(IHero target, Vector3 sourcePosition, float delay, float speed, float radius,
        float range, float dashTimeThreshold, CollisionType collisionType, PredictionType predictionType);

    PredictionResult PredictMobile(IHero target, Vector3 sourcePosition, float delay, float speed, float radius,
        float range, float reactionTime, CollisionType collisionType, PredictionType predictionType);
    
    float ImmobileDuration(IHero hero);
}