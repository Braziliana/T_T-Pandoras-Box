﻿using System.Numerics;
using Api.Game.Calculations;
using Api.Game.Managers;
using Api.Game.Objects;
using Api.Menus;
using Api.Scripts;

namespace Api.Internal.Game.Calculations;

public class Prediction : IPrediction
{
    private readonly IGameState _gameState;
    private readonly IHitChanceCalculator _hitChanceCalculator;

    
    private readonly IValueSlider _pingSlider;
    private readonly IValueSlider _rangePercent;
    private readonly IValueSlider _radiusPercent;
    private readonly IValueSlider _extraDelay;
    
    public Prediction(IGameState gameState, IHitChanceCalculator hitChanceCalculator, IMainMenu mainMenu)
    {
        _gameState = gameState;
        _hitChanceCalculator = hitChanceCalculator;

        var predictionMenu = mainMenu.CreateMenu("Prediction", ScriptType.Prediction);
        _pingSlider = predictionMenu.AddFloatSlider("Ping", 35, 0, 250, 1, 2);
        _rangePercent = predictionMenu.AddFloatSlider("Spell range modifier", 1.0f, 0, 1, 0.05f, 2);
        _radiusPercent = predictionMenu.AddFloatSlider("Spell width modifier", 1.0f, 0, 1, 0.05f, 2);
        _extraDelay = predictionMenu.AddFloatSlider("Spell extra delay", 0.0f, 0, 100, 1f, 2);
    }

    public float Ping { get; set; }

    public PredictionResult PredictPosition(IHero target, Vector3 sourcePosition, float delay, float speed, float radius, float range, float reactionTime, float dashTimeThreshold, CollisionType collisionType, PredictionType predictionType)
    {
        var immobileDuration = ImmobileDuration(target);
        if (immobileDuration > 0)
        {
            return PredictImmobile(target, sourcePosition, delay, speed, radius, range, immobileDuration, collisionType, predictionType);
        }
        
        if (target.AiManager.IsDashing)
        {
            return PredictDashing(target, sourcePosition, delay, speed, radius, range, dashTimeThreshold, collisionType, predictionType);
        }

        return PredictMobile(target, sourcePosition, delay, speed, radius,
            range, reactionTime, collisionType, predictionType);
    }

    public PredictionResult PredictImmobile(IHero target, Vector3 sourcePosition, float delay, float speed, float radius, float range, float immobileTime, CollisionType collisionType, PredictionType predictionType)
    {
        delay += _pingSlider.Value/2000.0f + _extraDelay.Value/1000.0f;
        radius *= _radiusPercent.Value;
        range *= _rangePercent.Value;
        if (radius <= 0)
        {
            radius = 30;
        }

        var distance = Vector3.Distance(target.AiManager.CurrentPosition, sourcePosition);
        if (distance > range)
        {
            return new PredictionResult(target.AiManager.CurrentPosition, 0.0f, PredictionResultType.Immobile);
        }
        
        var timeToImpact = GetTimeToHit(sourcePosition, target.AiManager.CurrentPosition, delay, speed, predictionType);
        var predictedPosition = target.AiManager.CurrentPosition;
        var hitChance = _hitChanceCalculator.CalculateImmobileHitChance(target, sourcePosition, radius, timeToImpact,
            immobileTime, collisionType, predictionType);
        
        return new PredictionResult(predictedPosition, hitChance, PredictionResultType.Immobile);
    }

    public PredictionResult PredictDashing(IHero target, Vector3 sourcePosition, float delay, float speed, float radius, float range, float dashTimeThreshold, CollisionType collisionType, PredictionType predictionType)
    {
        delay += _pingSlider.Value/2000.0f + _extraDelay.Value/1000.0f;
        radius *= _radiusPercent.Value;
        range *= _rangePercent.Value;
        if (radius <= 0)
        {
            radius = 30;
        }

        var distance = Vector3.Distance(target.AiManager.TargetPosition, sourcePosition);
        if (distance > range)
        {
            return new PredictionResult(target.AiManager.CurrentPosition, 0.0f, PredictionResultType.Immobile);
        }
        
        var timeToImpact = GetTimeToHit(sourcePosition, target.AiManager.TargetPosition, delay, speed, predictionType);
        
        var hitChance = _hitChanceCalculator.CalculateDashingHitChance(target, sourcePosition, radius, timeToImpact, dashTimeThreshold, collisionType, predictionType);
        return new PredictionResult(target.AiManager.TargetPosition, hitChance, PredictionResultType.Dashing);
    }

    public PredictionResult PredictMobile(IHero target, Vector3 sourcePosition, float delay, float speed, float radius, float range, float reactionTime, CollisionType collisionType, PredictionType predictionType)
    {
        delay += _pingSlider.Value/2000.0f + _extraDelay.Value/1000.0f;
        radius *= _radiusPercent.Value;
        range *= _rangePercent.Value;
        if (radius <= 0)
        {
            radius = 30;
        }

        var (predictedPosition, timeToImpact) = PredictPositionInternal(target, sourcePosition, delay, speed, radius, range, predictionType);
        if (Vector3.Distance(sourcePosition, predictedPosition) > range)
        {
            return new PredictionResult(predictedPosition, 0.0f, PredictionResultType.OutOfRange);
        }

        var hitChance = _hitChanceCalculator.CalculateMobileHitChance(target, sourcePosition, radius, timeToImpact, reactionTime, collisionType, predictionType);
        return new PredictionResult(predictedPosition, hitChance, target.AiManager.IsMoving ? PredictionResultType.Moving : PredictionResultType.Stationary);
    }

    private float TravelTime(Vector3 start, Vector3 end, float speed)
    {
        var distance = Vector3.Distance(start, end);
        return distance / speed;
    }
    
    private float GetTimeToHit(Vector3 start, Vector3 end, float delay, float speed, PredictionType predictionType)
    {
        return predictionType switch
        {
            PredictionType.Line => delay + TravelTime(start, end, speed),
            PredictionType.Point => delay,
            _ => throw new ArgumentOutOfRangeException(nameof(predictionType), predictionType, null)
        };
    }

    public float ImmobileDuration(IHero hero)
    {
        return MathF.Max(GetCastDuration(hero), GetImmobileBuffDuration(hero));
    }

    private float GetCastDuration(IHero hero)
    {
        if (hero.ActiveCastSpell.IsActive)
        {
            return hero.ActiveCastSpell.EndTime - _gameState.Time;
        }
        return 0;
    }

    private float GetImmobileBuffDuration(IHero hero)
    {
        float duration = 0;
        foreach (var buff in hero.Buffs.Where(x => x.IsHardCC()))
        {
            var buffDuration = buff.EndTime - _gameState.Time;
            if (duration < buff.EndTime - _gameState.Time)
            {
                duration = buffDuration;
            }
        }
        return duration;
    }
    
    private (Vector3 position, float time) PredictPositionInternal(IHero target, Vector3 sourcePosition, float delay, float speed, float radius, float range, PredictionType predictionType)
    {
        if (radius <= 0)
        {
            radius = 30;
        }

        if (!target.AiManager.IsMoving)
        {
            return (target.Position, GetTimeToHit(target.Position, sourcePosition, delay, speed, predictionType));
        }
        
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

        var halfTargetCollision = target.CollisionRadius / 2;
        var halfMissileCollision = radius / 2;
        
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

            var distanceFromSource = Vector3.Distance(sourcePosition, predictedPosition);
            if (distanceFromSource > range)
            {
                return (predictedPosition, elapsedTime);
            }
            
            elapsedTime += timeStep;
            
            if (elapsedTime >= delay)
            {
                if (predictionType == PredictionType.Point)
                {
                    return (predictedPosition, elapsedTime);
                }

                var missileTravel = elapsedTime * speed;
                if (missileTravel + halfTargetCollision + halfMissileCollision >= distanceFromSource)
                {
                    return (predictedPosition, elapsedTime);
                }
            }
        }

        return (predictedPosition, elapsedTime);
    }
}