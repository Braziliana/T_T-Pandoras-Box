﻿using System.Numerics;
using Api.Game.Calculations;
using Api.Game.Managers;
using Api.Game.Objects;

namespace Api.Internal.Game.Calculations;

public class HitChanceCalculator : IHitChanceCalculator
{
    private readonly IGameState _gameState;
    private readonly IMinionManager _minionManager;
    private readonly IHeroManager _heroManager;

    public HitChanceCalculator(IGameState gameState, IMinionManager minionManager, IHeroManager heroManager)
    {
        _gameState = gameState;
        _minionManager = minionManager;
        _heroManager = heroManager;
    }

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
        PredictionResultType predictionResultType)
    {
        switch (predictionResultType)
        {
            case PredictionResultType.Stationary:
            case PredictionResultType.Moving:
                return CalculateMobileHitChance(target, sourcePosition, collisionRadius, timeToImpact, reactionTime,
                    collisionType, predictionType);
            case PredictionResultType.Immobile:
                return CalculateImmobileHitChance(target, sourcePosition, collisionRadius,
                    timeToImpact, immobileTime, collisionType, predictionType);
            
            case PredictionResultType.Dashing:
                return CalculateDashingHitChance(target, sourcePosition, collisionRadius, timeToImpact,
                    dashTimeThreshold, collisionType, predictionType);
            case PredictionResultType.OutOfRange:
            default:
                throw new ArgumentOutOfRangeException(nameof(predictionResultType), predictionResultType, null);
        }

        return 0;
    }

    public float CalculateMobileHitChance(IHero target,
        Vector3 sourcePosition,
        float collisionRadius,
        float timeToImpact,
        float reactionTime,
        CollisionType collisionType,
        PredictionType predictionType)
    {
        if (predictionType == PredictionType.Line && collisionType != CollisionType.None)
        {
            if (!CollidesWithTarget(collisionType, sourcePosition, target.AiManager.CurrentPosition, collisionRadius))
            {
                return 0;
            }
        }
        var hitChance = GetEvasionHitChance(timeToImpact - reactionTime, target.AiManager.MovementSpeed,
            target.CollisionRadius, collisionRadius);
        return hitChance;
    }
    
    public float CalculateImmobileHitChance(
        IHero target,
        Vector3 sourcePosition,
        float collisionRadius,
        float timeToImpact,
        float immobileTime,
        CollisionType collisionType,
        PredictionType predictionType)
    {
        if (timeToImpact <= immobileTime)
        {
            return 100.0f;
        }

        if (predictionType == PredictionType.Line && collisionType != CollisionType.None)
        {
            if (!CollidesWithTarget(collisionType, sourcePosition, target.AiManager.CurrentPosition, collisionRadius))
            {
                return 0;
            }
        }
        
        return GetEvasionHitChance(timeToImpact - immobileTime, target.AiManager.MovementSpeed,
            target.CollisionRadius, collisionRadius);
    }

    public float CalculateDashingHitChance(
        IHero target,
        Vector3 sourcePosition,
        float collisionRadius,
        float timeToImpact,
        float dashTimeThreshold,
        CollisionType collisionType,
        PredictionType predictionType
        )
    {
        if (predictionType == PredictionType.Line && collisionType != CollisionType.None)
        {
            if (!CollidesWithTarget(collisionType, sourcePosition, target.AiManager.CurrentPosition, collisionRadius))
            {
                return 0;
            }
        }
        
        var dashEndTime = TravelTime(target.AiManager.TargetPosition, target.AiManager.CurrentPosition,
            target.AiManager.DashSpeed);
        
        if (timeToImpact < dashEndTime)
        {
            if (dashEndTime - timeToImpact <= dashTimeThreshold)
            {
                return 100.0f;
            }

            return 0.0f;
        }

        if (Math.Abs(timeToImpact - dashEndTime) < float.Epsilon)
        {
            return 100.0f;
        }

        return GetEvasionHitChance(timeToImpact - dashEndTime, target.AiManager.MovementSpeed,
            target.CollisionRadius, collisionRadius);
    }
    
    private float GetEvasionHitChance(float time, float targetMovementSpeed, float targetCollisionRadius, float collisionRadius)
    {
        var area = time * targetMovementSpeed;
        var collisionArea = targetCollisionRadius + collisionRadius;
        if (area <= collisionArea)
        {
            return 100;
        }

        return (1.0f - collisionArea / area) * 100;
    }
    private float TravelTime(Vector3 start, Vector3 end, float speed)
    {
        var distance = Vector3.Distance(start, end);
        return distance / speed;
    }
    
     private bool DoesLineIntersectCircle(Vector3 start, Vector3 end, Vector3 objectPosition,
        float objectCollisionRadius)
    {
        var a = end.Z - start.Z;
        var b = start.X - end.X;
        var c = a * start.X + b * start.Z;
        
        var distance = Math.Abs(a * objectPosition.X + b * objectPosition.Z + c) / Math.Sqrt(a * a + b * b);
        return distance <= objectCollisionRadius;
    }
    
    private bool WillCollide(Vector3 start, Vector3 end, float projectileWidth, Vector3 objectPosition,
        float objectCollisionRadius)
    {     
        // Define the line as start + t * dir for a scalar parameter t
        Vector3 dir = Vector3.Normalize(end - start);
    
        // Compute the vector from the line starting point to the object position
        Vector3 fromStartToObject = objectPosition - start;
   
        // Compute the projection length of fromStartToObject onto the line 
        float t = Vector3.Dot(dir, fromStartToObject);

        // Compute the closest point on the line to the center of the circle
        Vector3 closest = start + t * dir;

        // Compute the vector from the center of the circle to this closest point
        Vector3 fromObjectToClosest = closest - objectPosition;

        // If the length of this vector is less than or equal to the collision circle radius, the line intersects the circle
        return fromObjectToClosest.Length() <= projectileWidth + objectCollisionRadius;
    }
    
    private bool CollidesWithTarget(CollisionType collisionType, Vector3 start, Vector3 end, float width)
    {
        if (collisionType == CollisionType.None)
        {
            return false;
        }

        //var center = (start + end) / 2;
        var range = Vector3.Distance(start, end);
        if (collisionType.HasFlag(CollisionType.Minion))
        {
            var minions = _minionManager.GetEnemyMinions(range);
            if (minions.Any(minion => WillCollide(start, end, width, minion.Position, minion.CollisionRadius)))
            {
                return false;
            }
        }
        
        if (collisionType.HasFlag(CollisionType.Hero))
        {
            var heroes = _heroManager.GetEnemyHeroes(range);
            if (heroes.Any(hero => WillCollide(start, end, width, hero.Position, hero.CollisionRadius)))
            {
                return false;
            }
        }

        return true;
    }
}