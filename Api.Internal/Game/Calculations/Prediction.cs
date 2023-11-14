using System.Numerics;
using Api.Game.Calculations;
using Api.Game.Objects;

namespace Api.Internal.Game.Calculations;

public class Prediction : IPrediction
{
    public PredictionResult PredictPosition(IHero target, Vector3 sourcePosition, float delay, float speed, float radius, float range)
    {
        var (predictedPosition, elapsedTime) = PredictPositionInternal(target, sourcePosition, delay, speed, radius, range);
        if (Vector3.Distance(sourcePosition, predictedPosition) > range)
        {
            return new PredictionResult(predictedPosition, 0.0f);
        }

        return new PredictionResult(predictedPosition, CalculateHitChance(target, sourcePosition, predictedPosition, radius, elapsedTime));
    }
    
    private (Vector3 position, float time) PredictPositionInternal(IHero target, Vector3 sourcePosition, float delay, float speed, float radius, float range)
    {
        var predictedPosition = target.AiManager.CurrentPosition;
        var currentWaypointIndex = 0;
        var timeStep = radius / speed;
        if (timeStep < 0.01f)
        {
            timeStep = 0.01f;
        }
        var totalSimulationTime = (range / speed) + delay;
        float elapsedTime = 0;

        var waypoints = target.AiManager.GetRemainingPath().ToList();

        while (elapsedTime < totalSimulationTime)
        {
            if (currentWaypointIndex >= waypoints.Count)
            {
                break;
            }

            var targetDirection = Vector3.Normalize(waypoints[currentWaypointIndex] - predictedPosition);
            var distanceToNextWaypoint = Vector3.Distance(predictedPosition, waypoints[currentWaypointIndex]);
            var distanceThisStep = target.AiManager.MovementSpeed * timeStep;

            if (distanceThisStep >= distanceToNextWaypoint)
            {
                predictedPosition = waypoints[currentWaypointIndex];
                currentWaypointIndex++;
            }
            else
            {
                predictedPosition += targetDirection * distanceThisStep;
            }

            elapsedTime += timeStep;
            if (Vector3.Distance(sourcePosition, predictedPosition) > range)
            {
                return (predictedPosition, elapsedTime);
            }
        }

        return (predictedPosition, elapsedTime);
    }

    
    private float CalculateHitChance(IHero target, Vector3 missileStartPosition, Vector3 predictedImpactPoint, float missileCollisionRadius, float timeToImpact)
    {
        // var initialDistanceToImpact = Vector3.Distance(missileStartPosition, predictedImpactPoint);
        // var targetPotentialMovementDistance = target.AiManager.MovementSpeed * timeToImpact;
        // var relativeDistance = targetPotentialMovementDistance / initialDistanceToImpact;
        //
        // float hitChance;
        // if (relativeDistance > 1)
        // {
        //     // If the target can move a greater distance than the distance to the impact point, lower hit chance
        //     hitChance = (1 / relativeDistance) * 100;
        // }
        // else
        // {
        //     // If the target's potential movement distance is less than the distance to the impact point, higher hit chance
        //     hitChance = (1 - relativeDistance) * 100;
        // }
        //
        // // Ensuring hit chance is within 0-100%
        // return Math.Max(0, Math.Min(100, hitChance));
        
        //2   
        // float targetMaxEvasionDistance = target.AiManager.MovementSpeed * timeToImpact;
        //
        // float effectiveHitRadius = missileCollisionRadius + target.CollisionRadius;
        //
        // float hitChance;
        // if (targetMaxEvasionDistance > effectiveHitRadius)
        // {
        //     hitChance = (effectiveHitRadius / targetMaxEvasionDistance) * 100;
        // }
        // else
        // {
        //     hitChance = 100;
        // }
        //
        // return Math.Max(0, Math.Min(100, hitChance));
        
        //3

        timeToImpact -= 0.1f;
        if (timeToImpact < 0)
        {
            timeToImpact = 0;
        }
        
        var targetMaxEvasionDistance = target.AiManager.MovementSpeed * timeToImpact;
        var effectiveHitRadius = missileCollisionRadius + target.CollisionRadius;

        float hitChance;
        if (targetMaxEvasionDistance > effectiveHitRadius)
        {
            hitChance = (effectiveHitRadius / targetMaxEvasionDistance) * 100;
        }
        else
        {
            hitChance = 100;
        }

        return Math.Max(0, Math.Min(100, hitChance));
    }
}