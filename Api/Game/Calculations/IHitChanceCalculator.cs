using System.Numerics;
using Api.Game.Objects;

namespace Api.Game.Calculations;

public interface IHitChanceCalculator
{
    public float CalculateHitChance(
        IHero target,
        Vector3 sourcePosition,
        Vector3 predictedPosition,
        float collisionRadius, //radious of collision
        float timeToImpact, //time how long it takes to for spell to hit position
        float reactionTime,
        float immobileTime, //time how long target will be immobile
        float dashTimeThreshold, //100% hit chance if spell lands before target
        CollisionType collisionType,
        PredictionType predictionType,
        PredictionResultType predictionResultType);

    float CalculateMobileHitChance(IHero target,
        Vector3 sourcePosition,
        float collisionRadius,
        float timeToImpact,
        float reactionTime,
        CollisionType collisionType,
        PredictionType predictionType);
    
    public float CalculateDashingHitChance(
        IHero target,
        Vector3 sourcePosition,
        float collisionRadius,
        float timeToImpact,
        float dashTimeThreshold,
        CollisionType collisionType,
        PredictionType predictionType
    );
    
    public float CalculateImmobileHitChance(
        IHero target,
        Vector3 sourcePosition,
        float collisionRadius,
        float timeToImpact,
        float immobileTime,
        CollisionType collisionType,
        PredictionType predictionType);
}