using Api.Game.Calculations;
using Api.Game.GameInputs;
using Api.Game.Objects;
using Api.Internal.Game.Calculations;
using Api.Internal.Game.Objects;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Api.Internal.Game.GameInputs
{
    internal class SpellCaster : ISpellCaster
    {
        private readonly IPrediction _prediction;
        private readonly IGameState _gameState;
        private readonly IGameInput _gameInput;
        private readonly ILocalPlayer _localPlayer;

        private float _nextCast;

        public SpellCaster(IPrediction prediction, IGameState gameState, IGameInput gameInput, ILocalPlayer localPlayer)
        {
            _prediction = prediction;
            _gameState = gameState;
            _gameInput = gameInput;
            _localPlayer = localPlayer;
            _nextCast = 0;
        }

        public bool TryCastPredicted(ISpell spell, IHero target, float reactionTime, float dashTimeThreshold, float hitChance,
            CollisionType collisionType, PredictionType predictionType)
        {
            var spellData = spell.SpellData;
            if (spellData == null)
            {
                return false;
            }

            return TryCastPredicted(spell, target, spellData.CastDelayTime, spellData.Speed, spellData.Width,
                spellData.Range, reactionTime, dashTimeThreshold, hitChance, collisionType, predictionType);
        }

        public bool TryCastPredicted(ISpell spell, IHero target, float castDelayTime, float speed, float width, float range,
            float reactionTime, float dashTimeThreshold, float hitChance,
            CollisionType collisionType, PredictionType predictionType)
        {
            if (!CanCast(spell))
            {
                return false;
            }

            var prediction = _prediction.PredictPosition(
                target,
                _localPlayer.Position,
                castDelayTime,
                speed,
                width,
                range,
                reactionTime,
                dashTimeThreshold,
                collisionType,
                predictionType);

            if (prediction.HitChance < hitChance)
            {
                return false;
            }

            return TryCast(spell, target.Position);
        }

        public bool TryCast(ISpell spell, IAttackableUnit target)
        {
            return TryCast(spell, target.Position);
        }

        public bool TryCast(ISpell spell, Vector3 position)
        {
            if (!CanCast(spell) || !IsInRange(spell, position))
            {
                return false;
            }

            if (_gameInput.CastSpell(spell.SpellSlot, position))
            {
                _nextCast = _gameState.Time + spell.SpellData.CastDelayTime;
                return true;
            }

            return false;
        }

        public bool TryCast(ISpell spell)
        {
            if (!CanCast(spell))
            {
                return false;
            }

            if (_gameInput.CastSpell(spell.SpellSlot))
            {
                _nextCast = _gameState.Time + spell.SpellData.CastDelayTime;
                return true;
            }
            
            return false;
        }

        public bool TrySelfCast(ISpell spell)
        {
            if (!CanCast(spell))
            {
                return false;
            }

            if (_gameInput.SelfCastSpell(spell.SpellSlot))
            {
                _nextCast = _gameState.Time + spell.SpellData.CastDelayTime;
                return true;
            }

            return false;
        }

        public bool CanCast(ISpell spell)
        {
            return !IsCasting &&
                   spell.IsReady &&
                   spell.ManaCost < _localPlayer.Mana;
        }

        public bool IsInRange(ISpell spell, Vector3 position)
        {
            return Vector3.Distance(position, _localPlayer.Position) <= spell.Range;
        }
        
        public bool IsInRange(ISpell spell, IAttackableUnit attackableUnit)
        {
            return IsInRange(spell, attackableUnit.Position);
        }

        public bool IsCasting => IsCastingInternal();

        private bool IsCastingInternal()
        {
            if (_nextCast > _gameState.Time)
            {
                return true;
            }

            var activeSpell = _localPlayer.ActiveCastSpell;
            var activeSpellType = _localPlayer.ActiveCastSpell.Type;
            
            return activeSpell.IsActive && activeSpell.EndTime > _gameState.Time && activeSpellType is ActiveSpellType.Q or ActiveSpellType.W or ActiveSpellType.E or ActiveSpellType.R;
        }
    }
}
