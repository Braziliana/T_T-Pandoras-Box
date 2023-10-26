using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Api.Game.Calculations;
using Api.Game.Data;
using Api.Game.GameInputs;
using Api.Game.Managers;
using Api.Game.ObjectNameMappers;
using Api.Game.Objects;
using Api.Game.Offsets;
using Api.Game.Readers;
using Api.GameProcess;
using Api.Inputs;
using Api.Internal.Game.Calculations;
using Api.Internal.Game.GameInputs;
using Api.Internal.Game.Managers;
using Api.Internal.Game.ObjectNameMappers;
using Api.Internal.Game.Objects;
using Api.Internal.Game.Offsets;
using Api.Internal.Game.Readers;
using Api.Internal.Game.Settings;
using Api.Internal.GameProcess;
using Api.Internal.Inputs;
using Api.Internal.Menus;
using Api.Internal.Utils;
using Api.Menus;
using Api.Settings;
using Api.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using WinApi;

namespace Api.Internal
{
    public static class InternalServiceInstaller
    {
        public static void InstallServices(IServiceCollection collection)
        {
            RegisterOffsets(collection);
            collection.TryAddSingleton<IRandomGenerator, RandomGenerator>();
            
            collection.TryAddSingleton<IManagerSettings, ManagerSettings>();
            collection.TryAddSingleton<ISettingsProvider, SettingsProvider>();
            
            collection.TryAddSingleton<ITargetProcess, TargetProcess>();
            collection.TryAddSingleton<IMemory, BasicMemory>();
            
            collection.TryAddSingleton<IInputManager, InputManager>();
            
            collection.TryAddSingleton<IMainMenu, MainMenu>();

            collection.TryAddSingleton<UnitDataDictionary>();
            collection.TryAddSingleton<SpellDataDictionary>();
            collection.TryAddSingleton<MissileDataDictionary>();

            RegisterCalculators(collection);
            RegisterObjects(collection);
            RegisterManagers(collection);
        }

        private static void RegisterManagers(IServiceCollection collection)
        {
            collection.TryAddSingleton<IGameCameraReader, GameCameraReader>();
            collection.TryAddSingleton<IGameStateReader, GameStateReader>();
            collection.TryAddSingleton<IGameObjectReader, GameObjectReader>();
            collection.TryAddSingleton<IAttackableUnitReader, AttackableUnitReader>();
            collection.TryAddSingleton<IAiBaseUnitReader, AiBaseUnitReader>();
            collection.TryAddSingleton<IMinionReader, MinionReader>();
            collection.TryAddSingleton<IMonsterReader, MonsterReader>();
            collection.TryAddSingleton<IPlantReader, PlantReader>();
            collection.TryAddSingleton<IWardReader, WardReader>();
            collection.TryAddSingleton<ITrapReader, TrapReader>();
            collection.TryAddSingleton<IGameReader, GameReader>();
            collection.TryAddSingleton<ITurretReader, TurretReader>();
            collection.TryAddSingleton<IInhibitorReader, InhibitorReader>();
            collection.TryAddSingleton<IMissileReader, MissileReader>();
            collection.TryAddSingleton<IHeroReader, HeroReader>();
            collection.TryAddSingleton<IBuffReader, BuffReader>();
            collection.TryAddSingleton<ISpellReader, SpellReader>();
            collection.TryAddSingleton<IAiManagerReader, AiManagerReader>();
            collection.TryAddSingleton<IActiveCastSpellReader, ActiveCastSpellReader>();
            
            collection.TryAddSingleton<IHeroManager, HeroManager>();
            collection.TryAddSingleton<IMinionManager, MinionManager>();
            collection.TryAddSingleton<IMonsterManager, MonsterManager>();
            collection.TryAddSingleton<IPlantManager, PlantManager>();
            collection.TryAddSingleton<IWardManager, WardManager>();
            collection.TryAddSingleton<ITrapManager, TrapManager>();
            collection.TryAddSingleton<IObjectManager, ObjectManager>();
            collection.TryAddSingleton<ITurretManager, TurretManager>();
            collection.TryAddSingleton<IInhibitorManager, InhibitorManager>();
            collection.TryAddSingleton<IMissileManager, MissileManager>();
            collection.TryAddSingleton<IGameManager, GameManager>();
            collection.TryAddSingleton<IGameInput, GameInput>();
        }

        private static void RegisterObjects(IServiceCollection collection)
        {
            collection.TryAddSingleton<IGameObjectTypeMapper, GameObjectTypeMapper>();
            collection.TryAddSingleton<IMinionNameTypeMapper, MinionNameTypeMapper>();
            collection.TryAddSingleton<IWardNameTypeMapper, WardNameTypeMapper>();
            collection.TryAddSingleton<IMonsterNameTypeMapper, MonsterNameTypeMapper>();
            collection.TryAddSingleton<IPlantNameTypeMapper, PlantNameTypeMapper>();
            
            
            collection.TryAddSingleton<IGameState, GameState>();
            collection.TryAddSingleton<ILocalPlayer, LocalPlayer>();
            collection.TryAddSingleton<IGameCamera, GameCamera>();
        }

        private static void RegisterOffsets(IServiceCollection collection)
        {
            collection.TryAddSingleton<IBaseOffsets, BaseOffsets>();
            collection.TryAddSingleton<IGameObjectOffsets, GameObjectOffsets>();
            collection.TryAddSingleton<IAttackableUnitOffsets, AttackableUnitOffsets>();
            collection.TryAddSingleton<IGameCameraOffsets, GameCameraOffsets>();
            collection.TryAddSingleton<IMissileOffsets, MissileOffsets>();
            collection.TryAddSingleton<IAiBaseUnitOffsets, AiBaseUnitOffsets>();
            collection.TryAddSingleton<IHeroOffsets, HeroOffsets>();
            collection.TryAddSingleton<IBuffOffsets, BuffOffsets>();
            collection.TryAddSingleton<ISpellOffsets, SpellOffsets>();
            collection.TryAddSingleton<IAiManagerOffsets, AiManagerOffsets>();
            collection.TryAddSingleton<IActiveCastSpellOffsets, ActiveCastSpellOffsets>();
        }

        private static void RegisterCalculators(IServiceCollection collection)
        {
            collection.TryAddSingleton<IDamageCalculator, DamageCalculator>();
            collection.TryAddSingleton<IDamagePrediction, DamagePrediction>();
            collection.TryAddSingleton<IPrediction, Prediction>();
        }
    }
}
