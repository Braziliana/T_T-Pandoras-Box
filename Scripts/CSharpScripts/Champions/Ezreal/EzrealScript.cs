using Api.Game.Calculations;
using Api.Game.GameInputs;
using Api.Game.Objects;
using Api.Menus;
using Api;
using Api.Scripts;

using Scripts.Utils;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Api.Game.Managers;
using System.Numerics;

namespace Scripts.CSharpScripts.Champions.Ezreal
{
    internal class EzrealScript : IChampionScript
    {
        public string Name => "Ezreal";

        public string Champion => "Ezreal";
        public ScriptType ScriptType => ScriptType.Champion;
        public bool Enabled { get; set; }

        private readonly IMainMenu _mainMenu;
        private IMenu? _menu = null;
        private readonly ILocalPlayer _localPlayer;
        private readonly IScriptingState _scriptingState;
        private readonly ITargetSelector _targetSelector;
        private readonly IPrediction _prediction;
        private readonly IGameInput _gameInput;
        private readonly IRenderer _renderer;
        private readonly IGameState _gameState;
        private readonly IHeroManager _heroManager;
        private readonly ISpellCaster _spellCaster;

        private IToggle _useQInCombo;
        private IToggle _useWInCombo;
        //private IToggle _useEInCombo;

        private IToggle _useQInHarass;
        private IToggle _useWInHarass;
        //private IToggle _useEInHarass;

        private IValueSlider _QHitChance;
        private IValueSlider _WHitChance;
        private IValueSlider _RHitChance;

        private IValueSlider _QReactionTime;
        private IValueSlider _WReactionTime;
        private IValueSlider _RReactionTime;

        private IToggle _autoQCC;
        private IToggle _autoWCC;
        private IToggle _autoRCC;
        private IToggle _autoQDashing;

        public EzrealScript(
            IMainMenu mainMenu,
            ILocalPlayer localPlayer,
            IScriptingState scriptingState,
            ITargetSelector targetSelector,
            IPrediction prediction,
            IGameInput gameInput,
            IRenderer renderer,
            IGameState gameState,
            IHeroManager heroManager,
            ISpellCaster spellCaster)
        {
            _mainMenu = mainMenu;
            _localPlayer = localPlayer;
            _scriptingState = scriptingState;
            _targetSelector = targetSelector;
            _prediction = prediction;
            _gameInput = gameInput;
            _renderer = renderer;
            _gameState = gameState;
            _heroManager = heroManager;
            _spellCaster = spellCaster;
        }

        public void OnLoad()
        {
            _menu = _mainMenu.CreateMenu("Ezreal", ScriptType.Champion);
            var comboMenu = _menu.AddSubMenu("Combo");
            _useQInCombo = comboMenu.AddToggle("Use Q in combo", true);
            _useWInCombo = comboMenu.AddToggle("Use W in combo", true);

            var harassMenu = _menu.AddSubMenu("Harass");
            _useQInHarass = harassMenu.AddToggle("Use Q in harass", true);
            _useWInHarass = harassMenu.AddToggle("Use W in harass", true);

            var hitChanceMenu = _menu.AddSubMenu("Hit chance");
            _QHitChance = hitChanceMenu.AddFloatSlider("Q hit chance", 0.8f, 0.0f, 1.0f, 0.05f, 2);
            _WHitChance = hitChanceMenu.AddFloatSlider("W hit chance", 0.9f, 0.0f, 1.0f, 0.05f, 2);
            _RHitChance = hitChanceMenu.AddFloatSlider("R hit chance", 0.9f, 0.0f, 1.0f, 0.05f, 2);

            _QReactionTime = hitChanceMenu.AddFloatSlider("Q reaction time", 50f, 0.0f, 300f, 5f, 2);
            _WReactionTime = hitChanceMenu.AddFloatSlider("W reaction time", 0.00f, 0.0f, 300f, 5f, 2);
            _RReactionTime = hitChanceMenu.AddFloatSlider("R reaction time", 50f, 0.0f, 300f, 5f, 2);

            var autoMenu = _menu.AddSubMenu("Auto");
            _autoQCC = autoMenu.AddToggle("Auto Q CC enemy", true);
            _autoWCC = autoMenu.AddToggle("Auto W CC enemy", true);
            _autoRCC = autoMenu.AddToggle("Auto R CC enemy", true);
            _autoQDashing = autoMenu.AddToggle("Auto Q dashing enemy", true);
        }

        public void OnRender(float deltaTime)
        {
        }

        public void OnUnload()
        {
            if (_menu is not null)
            {
                _mainMenu.RemoveMenu(_menu);
            }
        }

        public void OnUpdate(float deltaTime)
        {
            if (!_localPlayer.IsAlive || _spellCaster.IsCasting)
            {
                return;
            }

            if (Combo())
            {
                return;
            }

            if (Harass())
            {
                return;
            }

            if (Auto())
            {
                return;
            }
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

        private bool Auto()
        {
            var enemies = _heroManager.GetEnemyHeroes();

            foreach (var enemy in enemies)
            {
                var immobileTime = GetImmobileBuffDuration(enemy);
                var distance = Vector3.Distance(_localPlayer.Position, enemy.Position);

                if (distance <= _localPlayer.R.Range && _autoRCC.Toggled && CanCast(_localPlayer.R))
                {
                    if (immobileTime > _localPlayer.R.SpellData.CastDelayTime)
                    {
                        if (CastR(enemy))
                        {
                            return true;
                        }
                    }
                }

                if (distance <= _localPlayer.W.Range && _autoWCC.Toggled && CanCast(_localPlayer.W))
                {
                    if (immobileTime > _localPlayer.W.SpellData.CastDelayTime)
                    {
                        if (CastW(enemy))
                        {
                            return true;
                        }
                    }
                }

                if (distance <= _localPlayer.Q.Range && _autoQCC.Toggled && CanCast(_localPlayer.Q))
                {
                    if (immobileTime > _localPlayer.Q.SpellData.CastDelayTime)
                    {
                        if (CastQ(enemy))
                        {
                            return true;
                        }
                    }
                }

                if (_autoQDashing.Toggled && enemy.AiManager.IsDashing && CanCast(_localPlayer.Q))
                {
                    if (CastQ(enemy))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool Combo()
        {
            if (_scriptingState.IsCombo == false)
            {
                return false;
            }

            var target = _targetSelector.GetTarget();
            if (target == null)
            {
                return false;
            }

            var distance = Vector3.Distance(_localPlayer.Position, target.Position);


            if (_useWInCombo.Toggled && distance <= _localPlayer.W.Range && CanCast(_localPlayer.W) && CastW(target))
            {
                return true;
            }

            if (_useQInCombo.Toggled && distance <= _localPlayer.Q.Range && CanCast(_localPlayer.Q) && CastQ(target))
            {
                return true;
            }

            return false;
        }

        private bool Harass()
        {
            if (_scriptingState.IsHaras == false)
            {
                return false;
            }

            var target = _targetSelector.GetTarget();
            if (target == null)
            {
                return false;
            }

            var distance = Vector3.Distance(_localPlayer.Position, target.Position);

            if (_useWInHarass.Toggled && distance <= _localPlayer.W.Range && CanCast(_localPlayer.W) && CastW(target))
            {
                return true;
            }

            if (_useQInHarass.Toggled && distance <= _localPlayer.Q.Range && CanCast(_localPlayer.Q) && CastQ(target))
            {
                return true;
            }

            return false;
        }

        private bool CanCast(ISpell spell)
        {
            return _spellCaster.CanCast(spell);
        }

        private bool CastQ(IHero target)
        {
            var spell = _localPlayer.Q;
            var spellData = spell.SpellData;
            if (spellData == null) return false;
            return _spellCaster.TryCastPredicted(spell, target, spellData.CastDelayTime, 2000.0f,
                spellData.Width, spellData.Range, _QReactionTime.Value / 1000.0f, 0.0f,
                _QHitChance.Value, CollisionType.Minion, PredictionType.Line);
        }

        private bool CastW(IHero target)
        {
            return _spellCaster.TryCastPredicted(_localPlayer.W, target, _WReactionTime.Value / 1000.0f, 0.0f,
                _WHitChance.Value, CollisionType.None, PredictionType.Line);
        }

        private bool CastR(IHero target)
        {
            return _spellCaster.TryCastPredicted(_localPlayer.R, target, _RReactionTime.Value / 1000.0f, 0.0f,
                _RHitChance.Value, CollisionType.None, PredictionType.Line);
        }
    }
}
