using System.Numerics;
using Api.Game.Calculations;
using Api.Game.Objects;

namespace Api.Game.GameInputs;

public interface ISpellCaster
{
    bool TryCastPredicted(ISpell spell, IHero target, float reactionTime, float dashTimeThreshold, float hitChance,
        CollisionType collisionType, PredictionType predictionType);
    bool TryCastPredicted(ISpell spell, IHero target, float castDelayTime, float speed, float width, float range,
        float reactionTime, float dashTimeThreshold, float hitChance,
        CollisionType collisionType, PredictionType predictionType);

    bool TryCast(ISpell spell, IAttackableUnit target);
    bool TryCast(ISpell spell, Vector3 position);
    bool TryCast(ISpell spell);
    bool TrySelfCast(ISpell spell);
    bool CanCast(ISpell spell);
    bool IsInRange(ISpell spell, Vector3 position);
    bool IsInRange(ISpell spell, IAttackableUnit attackableUnit);
    
    bool IsCasting { get; }
}