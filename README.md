# T_T Pandora's Box
## Alpha release
<br />

**Disclaimer: This code is for educational purposes only. It is not intended for use in any games or competitions where it may violate the terms and conditions or be considered unfair to other players. Please use this code responsibly and at your own risk.**

---

## Creedits
- UC forum users active in League of Legends Reversal and thread starters with solutions
- Koelion

---
## Info
- Requires .NET 6
- Run as admin
- Before first game launch and go to Data downloader to download some data from https://raw.communitydragon.org/latest/game/data/characters/
- Unmap in league settings keys like X, C, V and other which can create problems
- Disable auto attacks, enable attack move on cursor, Bind Target Champions Only in Hotkeys/Abilities and Summoner Spells to `
- Run game in Window Mode: Borderless
---

## Updating offsets
Offsets are located in appsettings.json in T_T_PandorasBox.

---

# SDK/API

Its not final and changes will be made.

---

## IRenderer

### Methods

- `RectFilled2D(Vector2 position, Vector2 size, Color color)`
- `RectFilled3D(Vector3 position, Vector2 size, Color color)`
- `RectFilledBordered2D(Vector2 position, Vector2 size, Color color, Color borderColor, float borderSize)`
- `RectFilledBordered3D(Vector3 position, Vector2 size, Color color, Color borderColor, float borderSize)`
- `RectBorder2D(Vector2 position, Vector2 size, Color color, float borderSize)`
- `RectBorder3D(Vector3 position, Vector2 size, Color color, float borderSize)`
-
- `CircleFilled2D(Vector2 position, float size, Color color)`
- `CircleFilled3D(Vector3 position, float size, Color color)`
- `CircleFilledBordered2D(Vector2 position, float size, Color color, Color borderColor, float borderSize)`
- `CircleFilledBordered3D(Vector3 position, float size, Color color, Color borderColor, float borderSize)`
- `CircleBorder2D(Vector2 position, float size, Color color, float borderSize)`
- `CircleBorder3D(Vector3 position, float size, Color color, float borderSize)`
-
- `Text(string text, Vector2 position, float size, Color color)`
- `Text(string text, Vector2 start, Vector2 end, float size, Color color)`
- `Text(string text, Vector2 position, float size, Color color, TextHorizontalOffset textHorizontalOffset, TextVerticalOffset textVerticalOffset)`
- `Text(string text, Vector2 start, Vector2 end, float size, Color color, TextHorizontalOffset textHorizontalOffset, TextVerticalOffset textVerticalOffset)`

---

## VirtualKey

An enumeration representing various virtual key codes for key inputs.

### Key Values

- `SelectKey`: Represents the select key.
- `MouseLeft`: Represents the left mouse button.
- `MouseRight`: Represents the right mouse button.
- `MouseMiddle`: Represents the middle mouse button.
- `MouseX1`: Represents the X1 mouse button.
- `MouseX2`: Represents the X2 mouse button.
- `Spacebar`: Represents the spacebar key.
- `Insert`: Represents the insert key.
- `Delete`: Represents the delete key.
- `A` to `Z`: Represents the alphabetical keys (A to Z).
- `Key1` to `Key9`: Represents the numerical keys (1 to 9).
- `Key0`: Represents the numerical key 0.
- `F1` to `F12`: Represents the function keys (F1 to F12).
- `LeftShift`: Represents the left shift key.
- `RightShift`: Represents the right shift key.
- `LeftControl`: Represents the left control key.
- `RightControl`: Represents the right control key.
- `LeftMenu`/`LeftAlt`: Represents the left menu (alt) key.
- `RightMenu`: Represents the right menu (alt) key.
- `Backtick`: Represents the backtick (`) key.

---

## KeyState

### Key State Values

- `KeyDown`: Represents that a key is in the "pressed down" state.
- `KeyUp`: Represents that a key is in the "released" state.

---

## MouseButton

### Mouse Button Values

- `Left`: Represents the left mouse button.
- `Right`: Represents the right mouse button.
- `Middle`: Represents the middle mouse button.
- `X1`: Represents the X1 mouse button.
- `X2`: Represents the X2 mouse button.

---

# GameObjects

---
## IBaseObject

### Properties

- `Pointer` (IntPtr): A pointer or memory address that references the base object. This is often used in low-level operations or integrations with native systems.
- `RequireFullUpdate` (bool): Indicates if the object requires a complete update, which may involve refreshing or recalculating various properties or states associated with the object.
- `IsValid` (bool): A flag determining if the object is in a valid state or if it has been correctly initialized and hasn't been destroyed or corrupted.
- `NetworkId` (int): A unique identifier for the object, typically used in networking operations to ensure consistency and synchronization across different game clients or server instances.

---

## IGameObject
Extends: [IBaseObject](#ibaseobject)

### Properties

- `Team` (int): Represents the team the game object belongs to.
- `Position` (Vector3): The game object's 3D position.
- `IsVisible` (bool): Determines if the game object is visible.
- `Name` (string): Descriptive name of the game object.
- `ObjectName` (string): Specific name tied to the game object type.
- `ObjectNameHash` (int): Hashed representation of the object's name.
- `GameObjectType` ([GameObjectType](#gameobjecttype)): Enum defining the game object's type.

### Methods
- `IsEnemy(IGameObject gameObject) -> bool`: Checks if a given game object is an enemy.
- `AsGameObject() -> IGameObject`: Returns the current instance as a game object.
- `Distance(IGameObject gameObject) -> float`: Calculates distance from a given game object.
- `Distance(Vector3 position) -> float`: Calculates distance from a given 3D position.

## GameObjectType

### Values

- `Unknown`: Represents an unidentified or undefined game object.
- `Minion`: A smaller AI-controlled unit, typically spawned in lanes.
- `Monster`: AI-controlled units found in the jungle or special game events.
- `Ward`: A vision-granting object, usually placed by players.
- `Plant`: Interactive entities in the jungle that provide various effects.
- `Trap`: Objects that can cause damage or other effects when triggered.
- `Turret`: Defensive structures found in lanes.
- `Inhibitor`: Structures that, when destroyed, allow the spawning of super minions.
- `Clone`: Duplicates of heroes, often created by certain abilities.
- `Hero`: The main player-controlled characters in the game.

---

## IAttackableUnit
Extends [IGameObject](#igameobject)

### Properties

- `IsDead` (bool): Indicates if the unit is dead.
- `Mana` (float): Current mana of the unit.
- `MaxMana` (float): Maximum mana the unit can have.
- `Health` (float): Current health of the unit.
- `MaxHealth` (float): Maximum health the unit can have.
- `Armor` (float): Unit's base armor.
- `BonusArmor` (float): Additional armor points the unit has.
- `TotalArmor` (float): The sum of base and bonus armor (readonly).
- `MagicResistance` (float): Base magic resistance of the unit.
- `BonusMagicResistance` (float): Additional magic resistance points.
- `TotalMagicResistance` (float): Sum of base and bonus magic resistance (readonly).
- `Targetable` (bool): Indicates if the unit can be targeted by attacks or spells.
- `CollisionRadius` (float): Radius used for collision detection.
- `IsAlive` (bool): Indicates if the unit is alive (readonly).
- `UnitData` (UnitData?): Contains additional data about the unit (e.g., stats, abilities).

---

## IAiBaseUnit
Extends [IAttackableUnit](#iattackableunit)

### Properties

- `CurrentTargetIndex` (int): Index of the unit's current target.
- `BaseAttackRange` (float): Unit's base attack range.
- `AttackRange` (float): Effective attack range of the unit (readonly).
- `AttackSpeed` (float): Effective attack speed of the unit (readonly).
- `AttackSpeedRatio` (float): Ratio of the unit's attack speed (readonly).
- `BonusAttackSpeed` (float): Additional attack speed points.
- `BaseAttackDamage` (float): Unit's base attack damage.
- `BonusAttackDamage` (float): Additional attack damage points.
- `TotalAttackDamage` (float): Total attack damage considering base and bonus values (readonly).
- `BasicAttackWindup` (float): Time taken for the basic attack windup.
- `AbilityPower` (float): The unit's ability power.
- `MagicPenetration` (float): Magic penetration value of the unit.
- `Lethality` (float): Lethality value of the unit.
- `Level` (int): Current level of the AI unit.

---

## IWard
Extends [IAttackableUnit](#iattackableunit)

### Properties

- `WardType` ([WardType](#wardtype)): Type of the ward, as defined by the `WardType` enum.

---

## WardType

Enumerates the possible types of wards.

- `Unknown`: The type of the ward is not recognized.
- `Yellow`: Standard vision-providing ward.
- `Pink` (also known as `JammerDevice`): Ward that reveals and disables other wards.
- `Blue`: A ward with a longer vision radius but shorter duration.
- `Crab`: Ward spawned from carb.

---

## IPlant
Extends [IAttackableUnit](#iattackableunit)

### Properties

- `PlantType` ([PlantType](#planttype)): Specifies the type of the plant, as defined by the `PlantType` enum.

---

## PlantType

Enumerates the possible types of plants.

- `Unknown`: The type of the plant is not recognized or specified.
- `Vision`: A plant that provides vision or sight in a specific area.
- `Health`: A plant that offers health benefits or healing properties when interacted with.
- `Satchel`: A plant that knocks player back

---

## ITrap
Extends [IGameObject](#igameobject)

---

## IInhibitor
Extends [IAttackableUnit](#iattackableunit)

---

## IMinion
Extends [IAiBaseUnit](#iaibaseunit)

### Properties

- `MinionType` ([MinionType](#miniontype)): Specifies the type of the minion, as defined by the `MinionType` enum.

---

## MinionType

Enumerates the various possible types of minions.

- `Unknown`
- `Melee`
- `Ranged`
- `Canon`
- `Super`
- `Herald`: Herald which is walking on lanes and attacking towers
- `Creature`: Minions spawned by heroes or main characters.

---

## IMonster
Extends [IAiBaseUnit](#iaibaseunit)

The `IMonster` interface characterizes a type of AI-controlled creature or entity in the game, often found within jungles, lairs, or special map points. It inherits properties and methods from the `IAiBaseUnit` interface.

### Properties

- `MonsterType` ([MonsterType](#monstertype)): Determines the specific type of the monster, as described by the `MonsterType` enum.

---

## MonsterType

Enumerates the diverse possible types of in-game monsters.

- `Unknown`: The monster's type is unspecified or unrecognized.
- `Dragon`
- `Herald`
- `Baron`
- `Crab`
- `Red`: Associated with the "Red Buff", granting unique abilities to its vanquisher.
- `Blue`: Associated with the "Blue Buff", offering special bonuses upon defeat.
- `Gromp`: A solitary jungle creature.
- `Razorbeak`: A bird-like monster residing in the jungle.
- `RazorbeakMini`: Smaller variant of the Razorbeak.
- `Murkwolf`: A jungle creature resembling a wolf.
- `MurkwolfMini`: The lesser counterpart of the Murkwolf.
- `Krug`: A rocky creature found in the jungle terrain.
- `KrugMini`: A smaller version of the Krug.
- `KrugMiniMini`: An even smaller variant of the Krug.

---

## ITurret
Extends [IAiBaseUnit](#iaibaseunit)

---

## IHero
Extends [IAiBaseUnit](#iaibaseunit)

### Properties

- `IsLocalHero` (bool): Indicates if the hero is controlled by the local player.
- `SpawnCount` (int): Number of times the hero has spawned or respawned in the game.
- `AutoAttack` ([ISpell](#ispell)): Basic attack.
- `Q`, `W`, `E`, `R` ([ISpell](#ispell)): Hero spells
- `Summoner1`, `Summoner2` ([ISpell](#ispell)): Summoner Spells
- `ActiveCastSpell` ([IActiveCastSpell](#iactivecastspell)): The spell currently being cast by the hero.
- `AiManager` ([IAiManager](#iaimanager)): Contains movement data, Path, Dashing etc
- `BuffsDictionary` (IDictionary<string, [IBuff](#ibuff)>): Dictionary holding all the buffs applied to the hero, indexed by their names.
- `Buffs` (IEnumerable<[IBuff](#ibuff)>): Provides an iterable list of all the buffs currently applied to the hero.

### Methods

- `HasBuff(string name) -> bool`: Checks if the hero currently has a specific buff applied.
- `GetBuff(string name) -> IBuff?`: Fetches the details of a specific buff applied to the hero, or null if not found.

### Dependencies

- [ISpell](#ispell): Interface representing the various spells or abilities a hero can cast.
- [IActiveCastSpell](#iactivecastspell): Represents details about a spell that's actively being cast.
- [IAiManager](#iaimanager): Manages AI-specific behaviors and decisions for the hero.
- [IBuff](#ibuff): Represents buffs or debuffs that can be applied to heroes, altering their stats, behavior, or abilities.

---

## ILocalPlayer
Extends [IHero](#ihero)

---

## ISpellInput

### Properties

- `Pointer` (IntPtr): A pointer or reference to the memory location associated with this input.
- `SpellInputStartPosition` (Vector3): 3D position where the spell casting starts.
- `SpellInputEndPosition` (Vector3): 3D position where the spell casting ends or aims towards.
- `SpellInputTargetId` (int): Identifier for a specific target the spell is cast on.

---

## SpellData

### Properties

- `Name` (string): The name of the spell.
- `SpellFlags` (SpellFlags): Flags that represent specific attributes or behaviors associated with the spell.
- `AffectFlags` (AffectFlags): Flags that define which units or targets are affected by the spell.
- `Range` (float): The effective range of the spell. It determines how far the spell can reach.
- `ManaCost` (float[]?): An array representing the mana cost at different levels of the spell. Null if not applicable.
- `CastTime` (float): The time it takes to cast the spell.
- `Speed` (float): The speed at which the spell travels or is executed.
- `Width` (float): The width or area of effect of the spell.
- `MissileData` (MissileData?): Additional information about the spell if it involves a missile or projectile. Null if not applicable.
- `TargetingTypeData` (string?): Data about the types of targets the spell can affect. Null if not applicable.
- `NameHash` (int): A hash representation of the spell's name for efficient comparisons or lookups.
- `CastType` (int): Cast Type needs some reversing to create enum

### Dependencies

- `SpellFlags`: An enumeration or class that defines various attributes of the spell.
- `AffectFlags`: An enumeration or class that details which units or targets the spell affects.
- `MissileData`: A class or structure encapsulating data about the spell's missile or projectile (if applicable).

---

## ISpell

### Properties

- `Pointer` (IntPtr): A pointer or reference to the memory location associated with this spell.
- `SpellSlot` ([SpellSlot](#spellslot-enum)): The slot or key assigned to this spell for quick activation.
- `Name` (string): Name of the spell.
- `NameHash` (int): Hashed representation of the spell's name for efficient comparisons.
- `Level` (int): Current level or rank of the spell, usually improving its effects or reducing its costs.
- `Cooldown` (float): Duration in seconds before the spell can be cast again.
- `SmiteCooldown` (float): Specific cooldown duration for the Smite spell.
- `Damage` (float): Damage value dealt by the spell.
- `ManaCost` (float): Mana consumed when the spell is cast.
- `IsReady` (bool): Indicates if the spell is available for casting.
- `Stacks` (int): Number of times the spell can be cast or its charges.
- `SpellInput` ([ISpellInput](#ispellinput)): Associated spell input details.
- `SmiteIsReady` (bool): Indicates if the Smite spell is available for casting.
- `SpellData` ([SpellData](#spelldata)?): Additional data about the spell (e.g., effects, interactions, lore).
- `Range` (float): Maximum distance from the caster at which the spell can be cast or its effects can reach.

### Dependencies

- [ISpellInput](#ispellinput)
- [SpellData](#spelldata)
- [SpellSlot](#spellslot-enum)

---

## SpellSlot Enum

The `SpellSlot` enumeration represents the slots or keys to which a character's spells or abilities are assigned for quick activation. These slots commonly correspond to the primary abilities and attacks of characters in many games, especially in the context of multiplayer online battle arenas (MOBAs) or role-playing games (RPGs).

### Values

- `Q`: The first primary ability or spell for a character.
- `W`: The second primary ability or spell for a character.
- `E`: The third primary ability or spell for a character.
- `R`: The ultimate or special ability for a character, often more powerful and with longer cooldowns.
- `Summoner1`: One of the two summoner spells that a player can choose before starting a game. These are usually game-changing abilities with long cooldowns.
- `Summoner2`: The other summoner spell slot.
- `AutoAttack`: Represents the basic or default attack for a character.

---

## IActiveCastSpell

The `IActiveCastSpell` interface represents an actively casting or executing spell in the game. It provides properties detailing the spell's status, source, target, positions, and timing.

### Properties

- `Pointer` (IntPtr): A pointer to the underlying memory structure or object for this spell.
- `IsActive` (bool): Indicates if the spell is currently active or being cast.
- `Type` ([ActiveSpellType](#activespelltype)): The type or slot of the spell being cast (e.g., Q, W, E, R, AutoAttack).
- `SourceId` (int): The unique identifier for the source unit or caster of the spell.
- `TargetId` (int): The unique identifier for the target unit or object of the spell.
- `StartPosition` (Vector3): The 3D starting position of the spell, typically where the caster is located.
- `EndPosition` (Vector3): The 3D end position of the spell, indicating where the spell is directed or will land.
- `StartTime` (float): The time (in game ticks or seconds) when the spell started casting.
- `EndTime` (float): The time (in game ticks or seconds) when the spell will finish or hit its target.
- `Name` (string): The name of the spell being cast.

---

## ActiveSpellType

### Values

- `AutoAttack` (-1): Represents basic auto attacks.
- `Q` (0): Represents the Q spell slot.
- `W` (1): Represents the W spell slot.
- `E` (2): Represents the E spell slot.
- `R` (3): Represents the R spell slot.
- `Unknown`: Used for undefined or unknown spell types.

---

## IAiManager

### Properties

- `Pointer` (IntPtr): A pointer to the memory location or native object that this interface interacts with.
- `TargetPosition` (Vector3): The AI's target or destination position.
- `PathStart` (Vector3): The starting position of the AI's current path.
- `PathEnd` (Vector3): The ending position of the AI's current path.
- `CurrentPathSegment` (int): Index of the AI's current path segment.
- `PathSegments` (List<Vector3>): List of positions defining the AI's path.
- `PathSegmentsCount` (int): Total number of segments in the AI's path.
- `CurrentPosition` (Vector3): The AI's current position.
- `IsDashing` (bool): Indicates if the AI is currently dashing.
- `DashSpeed` (float): Speed at which the AI dashes.
- `IsMoving` (bool): Indicates if the AI is currently moving.
- `MovementSpeed` (float): Speed at which the AI moves.

### Methods

- `RemainingPath` (IEnumerable<Vector3>): Gets the remaining path that the AI will follow.
- `GetRemainingPath() -> IEnumerable<Vector3>`: Returns the segments of the remaining path.

---

## IBuff

### Properties

- `Pointer` (IntPtr): A pointer to the memory location or native object representing the buff.
- `Name` (string): The name or identifier of the buff.
- `StartTime` (float): The game timestamp when the buff started.
- `EndTime` (float): The game timestamp when the buff is scheduled to end.
- `Count` (int): Represents the number of stacks or intensity of the buff. Some buffs can accumulate in intensity as they're applied multiple times.
- `CountAlt` (int): An alternative counter or metric related to the buff. Its usage may vary based on the specific buff type.

### Methods

- `CloneFrom(IBuff buff)`: Creates a copy or clone of the provided `buff` and applies its properties to the current instance.

## IGameState

The `IGameState` interface represents the current state of the game, providing properties related to in-game time and game activity status.

### Properties

- `Time` (float): Represents the current in-game time, usually measured in seconds since the start of the game session.
- `IsGameActive` (bool): Returns whether the game is currently active. By default, the game is considered active if the `Time` property is greater than 1 second.

---

## IMissile
Extends: [IBaseObject](#ibaseobject)

### Properties

- `Name` (string): The descriptive name of the missile.
- `Speed` (float): The speed at which the missile travels.
- `Position` (Vector3): The current 3D position of the missile in the game world.
- `SourceIndex` (int): Index or identifier of the source from which the missile was fired.
- `DestinationIndex` (int): Index or identifier of the destination or target of the missile.
- `StartPosition` (Vector3): The initial 3D position from which the missile was fired.
- `EndPosition` (Vector3): The final 3D position the missile is aimed at.
- `SpellName` (string): The name of the spell or ability that produced or is associated with the missile.
- `MissileName` (string): A specific name tied to the missile type.
- `Width`(float): The missile collision radious.
- `MissileData` (MissileData?): Missile spell data
- `SpellData` (SpellData?): Missile data

---

## IGameCamera

### Properties

- `RequireFullUpdate` (bool): Specifies whether the camera requires a full update. This might be set to true if the camera's settings or position have changed significantly and a complete recalibration is required.
- `IsValid` (bool): Indicates if the current camera instance or its configuration is valid.
- `ViewProjMatrix` (Matrix4x4): Represents the camera's view-projection matrix. This matrix is crucial for 3D transformations, converting world coordinates to camera-relative coordinates and then into screen-space.
- `RendererWidth` (int): The width of the renderer or the viewport that the camera is rendering to.
- `RendererHeight` (int): The height of the renderer or the viewport that the camera is rendering to.

### Methods

- `WorldToScreen(Vector3 worldPosition) -> Vector2`: Converts a 3D world position to a 2D screen position. If the world position is not visible on the screen, this method may return a position outside the screen's bounds.
- `WorldToScreen(Vector3 worldPosition, out Vector2 screenPosition) -> bool`: Attempts to convert a 3D world position to a 2D screen position. Returns `true` if the operation is successful and assigns the result to the `screenPosition` out parameter. If the world position is not visible on the screen, this method may still succeed, but the resulting position might be outside the screen's bounds.

---

# Managers

---

## IGameManager Interface
Extends [IManager](#imanager)

### Properties

- `GameState` ([IGameState](#igamestate)): Provides information about the current state of the game.
- `LocalPlayer` ([ILocalPlayer](#ilocalplayer)): Represents the local player within the game.
- `GameInput` ([IGameInput](#igameinput)): Manages and processes user inputs during gameplay.
- `GameCamera` ([IGameCamera](#igamecamera)): Offers functionalities related to the game's camera system.
- `HeroManager` ([IHeroManager](#iheromanager)): Manages all hero-related functionalities.
- `ObjectManager` ([IObjectManager](#iobjectmanager)): Manages game objects present in the game environment.
- `TurretManager` ([ITurretManager](#iturretmanager)): Oversees the functionalities related to in-game turrets.
- `InhibitorManager` ([IInhibitorManager](#iinhibitormanager)): Manages in-game inhibitors.
- `MissileManager` ([IMissileManager](#imissilemanager)): Handles the behaviors and actions of missiles in the game.

### Events

- `GameLoaded`: An event triggered when the game finishes loading.

---

## IHeroManager

### Methods:

- `GetHeroes() -> IEnumerable<IHero>`:  
  Retrieves a collection of all heroes in the game, both allies and enemies.

- `GetAllyHeroes() -> IEnumerable<IHero>`:  
  Fetches all ally heroes on the map.

- `GetAllyHeroes(float range) -> IEnumerable<IHero>`:  
  Returns ally heroes within a specified range from the local player.

- `GetAllyHeroes(Vector3 position, float range) -> IEnumerable<IHero>`:  
  Retrieves ally heroes within a specified range from a given position.

- `GetEnemyHeroes() -> IEnumerable<IHero>`:  
  Fetches all enemy heroes on the map.

- `GetEnemyHeroes(float range) -> IEnumerable<IHero>`:  
  Returns enemy heroes within a specified range from the local player.

- `GetEnemyHeroes(Vector3 position, float range) -> IEnumerable<IHero>`:  
  Retrieves enemy heroes within a specified range from a given position.

- `GetHero(int networkId) -> IHero?`:  
  Fetches a hero by its network ID. Returns null if the hero with the given ID doesn't exist.

### Dependencies:

- **[IHero](#ihero)**

---

## IInhibitorManager

### Methods:

- `GetAllInhibitors() -> IEnumerable<IInhibitor>`:  
  Retrieves a collection of all inhibitors in the game.

- `GetAllInhibitors(float range) -> IEnumerable<IInhibitor>`:  
  Returns all inhibitors within a specified range from the local player.

- `GetAllInhibitors(Vector3 position, float range) -> IEnumerable<IInhibitor>`:  
  Retrieves all inhibitors within a specified range from a given position.

- `GetEnemyInhibitors() -> IEnumerable<IInhibitor>`:  
  Fetches all enemy inhibitors on the map.

- `GetEnemyInhibitors(float range) -> IEnumerable<IInhibitor>`:  
  Returns enemy inhibitors within a specified range from the local player.

- `GetEnemyInhibitors(Vector3 position, float range) -> IEnumerable<IInhibitor>`:  
  Retrieves enemy inhibitors within a specified range from a given position.

### Dependencies:

- **[IInhibitor](#iinhibitor)**

Sure, here's the formatted code for your README for the IMinionManager interface:

markdown

---

## IMinionManager

### Methods:

- `Create(IntPtr objectPointer, BatchReadContext batchReadContext) -> ObjectCreateResult`:  
  Creates a minion using the provided object pointer and batch read context.

- `Clear()`:  
  Clears or resets the manager's internal collection of minions.

- `Update(float deltaTime)`:  
  Updates the state of all managed minions based on the provided delta time.

- `GetAllyMinions() -> IEnumerable<IMinion>`:  
  Retrieves a collection of ally minions.

- `GetAllyMinions(float range) -> IEnumerable<IMinion>`:  
  Returns ally minions within a specified range from the local player.

- `GetAllyMinions(Vector3 position, float range) -> IEnumerable<IMinion>`:  
  Retrieves ally minions within a specified range from a given position.

- `GetEnemyMinions() -> IEnumerable<IMinion>`:  
  Fetches all enemy minions on the map.

- `GetEnemyMinions(float range) -> IEnumerable<IMinion>`:  
  Returns enemy minions within a specified range from the local player.

- `GetEnemyMinions(Vector3 position, float range) -> IEnumerable<IMinion>`:  
  Retrieves enemy minions within a specified range from a given position.

### Dependencies:

- **[IMinion](#iminion)**

---

## IMissileManager

### Methods:

- `GetMissiles() -> IEnumerable<IMissile>`:  
  Retrieves a collection of all active missiles on the map.

- `GetMissiles(int networkId) -> IEnumerable<IMissile>`:  
  Returns missiles associated with a particular network ID.

### Dependencies:

- **[IMissile](#imissile)**

---

## IMonsterManager

### Methods:

- `Create(IntPtr ptr, BatchReadContext batchReadContext) -> ObjectCreateResult`:  
  Creates a new monster instance based on the given pointer and context.

- `Clear()`:  
  Removes all monitored monster instances.

- `Update(float deltaTime)`:  
  Updates the manager and its monitored monsters based on the given time delta.

- `GetMonsters() -> IEnumerable<IMonster>`:  
  Retrieves a collection of all active monsters on the map.

- `GetMonsters(float range) -> IEnumerable<IMonster>`:  
  Returns monsters within the specified range.

- `GetMonsters(Vector3 position, float range) -> IEnumerable<IMonster>`:  
  Retrieves monsters within a specified range from a given position.

### Dependencies:

- **[IMonster](#imonster)**

---

## IPlantManager

### Methods:

- `Create(IntPtr ptr, BatchReadContext batchReadContext) -> ObjectCreateResult`:  
  Initiates a new plant instance based on the provided pointer and context.

- `Clear()`:  
  Removes all observed plant instances from the manager.

- `Update(float deltaTime)`:  
  Refreshes the manager and its monitored plants using the given time delta.

- `GetPlants() -> IEnumerable<IPlant>`:  
  Retrieves a collection of all active plants on the map.

- `GetPlants(float range) -> IEnumerable<IPlant>`:  
  Returns plants located within the specified range.

- `GetPlants(Vector3 position, float range) -> IEnumerable<IPlant>`:  
  Retrieves plants within a specific range from a given position.

### Dependencies:

- **[IPlant](#iplant)**

---

## ITrapManager

### Methods:

- `Create(IntPtr ptr, BatchReadContext batchReadContext) -> ObjectCreateResult`:  
  Initiates a new trap instance based on the provided pointer and context.

- `Clear()`:  
  Removes all observed trap instances from the manager.

- `Update(float deltaTime)`:  
  Refreshes the manager and its monitored traps using the given time delta.

- `GetAllyTraps() -> IEnumerable<ITrap>`:  
  Retrieves a collection of all active ally traps on the map.

- `GetAllyTraps(float range) -> IEnumerable<ITrap>`:  
  Returns ally traps located within the specified range.

- `GetAllyTraps(Vector3 position, float range) -> IEnumerable<ITrap>`:  
  Retrieves ally traps within a specific range from a given position.

- `GetEnemyTraps() -> IEnumerable<ITrap>`:  
  Retrieves a collection of all active enemy traps on the map.

- `GetEnemyTraps(float range) -> IEnumerable<ITrap>`:  
  Returns enemy traps located within the specified range.

- `GetEnemyTraps(Vector3 position, float range) -> IEnumerable<ITrap>`:  
  Retrieves enemy traps within a specific range from a given position.

### Dependencies:

- **[ITrap](#itrap)**

---

## ITurretManager

### Methods:

- `GetAllyTurrets() -> IEnumerable<ITurret>`:  
  Fetches a collection of all active ally turrets on the map.

- `GetAllyTurrets(float range) -> IEnumerable<ITurret>`:  
  Retrieves ally turrets located within the specified range.

- `GetAllyTurrets(Vector3 position, float range) -> IEnumerable<ITurret>`:  
  Obtains ally turrets within a certain range from the given position.

- `GetEnemyTurrets() -> IEnumerable<ITurret>`:  
  Fetches a collection of all active enemy turrets on the map.

- `GetEnemyTurrets(float range) -> IEnumerable<ITurret>`:  
  Retrieves enemy turrets located within the specified range.

- `GetEnemyTurrets(Vector3 position, float range) -> IEnumerable<ITurret>`:  
  Obtains enemy turrets within a specific range from a given position.

### Dependencies:

- **[ITurret](#iturret)**

---

## IWardManager

### Methods:

- `Create(IntPtr ptr, BatchReadContext batchReadContext) -> ObjectCreateResult`:  
  Creates a ward entity from the given pointer and context.

- `Clear()`:  
  Removes all tracked ward entities from the manager.

- `Update(float deltaTime)`:  
  Updates the state of all wards managed by this interface based on the elapsed time.

- `GetAllyWards() -> IEnumerable<IWard>`:  
  Fetches a collection of all active ally wards on the map.

- `GetAllyWards(float range) -> IEnumerable<IWard>`:  
  Retrieves ally wards located within the specified range.

- `GetAllyWards(Vector3 position, float range) -> IEnumerable<IWard>`:  
  Obtains ally wards within a certain range from the given position.

- `GetEnemyWards() -> IEnumerable<IWard>`:  
  Fetches a collection of all active enemy wards on the map.

- `GetEnemyWards(float range) -> IEnumerable<IWard>`:  
  Retrieves enemy wards located within the specified range.

- `GetEnemyWards(Vector3 position, float range) -> IEnumerable<IWard>`:  
  Obtains enemy wards within a specific range from a given position.

### Dependencies:

- **[IWard](#iward)**:

---

## IObjectManager

Centralized manager for different game objects and specific game entities like minions, monsters, plants, wards, and traps. Provides a unified way to access and manage these objects.

### Properties:

- `MinionManager -> ` [IMinionManager](#iminionmanager):  
  Gets the manager responsible for handling minions.

- `MonsterManager -> ` [IMonsterManager](#imonstermanager):  
  Gets the manager responsible for handling monsters.

- `PlantManager -> ` [IPlantManager](#iplantmanager):  
  Gets the manager responsible for handling plants.

- `WardManager -> ` [IWardManager](#iwardmanager):  
  Gets the manager responsible for handling wards.

- `TrapManager -> ` [ITrapManager](#itrapmanager):  
  Gets the manager responsible for handling traps.

### Methods:

- `GetByNetworkId(int handle) -> IGameObject?`:  
  Retrieves a game object by its network ID.

- `GetGameObjects() -> IEnumerable<IGameObject>`:  
  Fetches all active game objects present in the game.

- `GetGameObjects(float range) -> IEnumerable<IGameObject>`:  
  Retrieves all game objects located within the specified range.

- `GetGameObjects(Vector3 position, float range) -> IEnumerable<IGameObject>`:  
  Obtains all game objects within a certain range from the given position.

### Dependencies:

- **[IMinionManager](#iminionmanager)**
- **[IMonsterManager](#imonstermanager)**
- **[IPlantManager](#iplantmanager)**
- **[IWardManager](#iwardmanager)**
- **[ITrapManager](#itrapmanager)**
- **[IGameObject](#igameobject)**

---

# Inputs

---

## IGameInput

Interface for managing game inputs, such as mouse positioning, issuing commands, and casting spells.

### Properties:

- `MousePosition -> Vector2`:  
  Gets the current position of the mouse cursor on the screen.

### Methods:

- `IssueOrder(Vector2 position, IssueOrderType issueOrderType) -> bool`:  
  Sends a command to a game entity based on the specified position and order type.

- `IssueOrder(Vector3 position, IssueOrderType issueOrderType) -> bool`:  
  Sends a command to a game entity based on the 3D position and order type.

- `Attack(IGameObject target) -> bool`:  
  Commands the game entity to attack the specified target.

- `CastEmote(int emote)`:  
  Executes an emote for the game entity based on the provided emote ID.

- `Update()`:  
  Periodic method to update internal states related to inputs.

- `LevelUpSpell(SpellSlot spellSlot) -> bool`:  
  Attempts to level up a specific spell slot.

- `CastSpell(SpellSlot spellSlot) -> bool`:  
  Casts a specified spell.

- `SelfCastSpell(SpellSlot spellSlot) -> bool`:  
  Self-casts the specified spell.

- `CastSpell(SpellSlot spellSlot, Vector2 position) -> bool`:  
  Casts the specified spell towards the given 2D position.

- `CastSpell(SpellSlot spellSlot, Vector3 position) -> bool`:  
  Casts the specified spell towards the given 3D position.

- `CastSpell(SpellSlot spellSlot, IGameObject target) -> bool`:  
  Casts the specified spell on a particular game object target.

---

## IssueOrderType

Enumerates the types of commands or orders that can be issued.

- `Move`: Move to a location.
- `Attack`: Attack an entity.
- `MoveAttack`: Move to a location and attack any enemy entities encountered.
- `AttackHero`: Specifically target and attack a hero.

---

## SpellSlot

Enumerates the available spell slots.

- `Q`: First basic ability.
- `W`: Second basic ability.
- `E`: Third basic ability.
- `R`: Ultimate ability.
- `Summoner1`: First summoner spell.
- `Summoner2`: Second summoner spell.
- `AutoAttack`: Regular auto-attack.

### Dependencies:

- **[IGameObject](#igameobject)**

---

# Calculations

---

## DamageType

- `Physical`: Represents physical damage dealt by basic attacks and some abilities.
- `Magic`: Represents magical damage dealt primarily by abilities.
- `True`: Represents damage that ignores resistances and is dealt as is.

---

## IDamageCalculator

Interface for calculating damage dealt from one unit to another, considering different damage types and other factors like resistances and penetration.

### Methods:

- `GetDamage(DamageType damageType, IAiBaseUnit source, IAttackableUnit destination, float damage) -> float`:  
  Calculates the final damage that will be dealt based on the type of damage, source unit, destination unit, and the base damage value.

- `GetPhysicalDamage(IAiBaseUnit source, IAttackableUnit destination, float damage) -> float`:  
  Calculates the physical damage to be dealt between the source and destination units.

- `GetMagicDamage(IAiBaseUnit source, IAttackableUnit destination, float damage) -> float`:  
  Calculates the magical damage to be dealt between the source and destination units.

- `GetDamage(float damage, float resistance, float flatPenetration, float percentPenetration) -> float`:  
  Calculates the final damage after considering resistances and penetrations. This method is more generic and can be used for any damage type.

### Dependencies:

- **[IAiBaseUnit](#iaibaseunit)**
- **[IAttackableUnit](#iattackableunit)**
- **[DamageType](#damagetype)**

## IDamagePrediction

Interface for predicting the future health of a target unit after a specified duration.

### Methods:

- `PredictHealth(IAttackableUnit target, float time) -> float`:  
  Predicts and returns the health of the `target` unit after the duration specified by `time`.

### Dependencies:

- **[IAttackableUnit](#iattackableunit)**

## IPrediction

Interface for predicting the future position of a hero, based on various parameters related to a skillshot or projectile.

### Methods:

- `PredictPosition(IHero target, Vector3 sourcePosition, float delay, float speed, float radius) -> PredictionResult`:  
  Predicts and returns the future position of the `target` hero based on the given source position, delay, speed, and radius of a projectile or skillshot. The result is wrapped inside a `PredictionResult` object.

### Dependencies:

- **[IHero](#ihero)**
- **[PredictionResult](#predictionresult)**

---

## PredictionResult

A structure that represents the result of a prediction, containing both the predicted position and the hit chance for a target.

### Properties:

- `Position -> Vector3`: Represents the predicted position of the target.
- `HitChance -> float`: Represents the likelihood (as a percentage from 0 to 100) of hitting the target at the predicted position.

---

# Menu

## IMainMenu

### Events

- `MenuOpen`: An event triggered when the main menu is opened.
- `MenuClose`: An event triggered when the main menu is closed.

### Methods

- `Render()`: Renders the main menu.
- `AddMenu(IMenu menu)`: Adds a submenu to the main menu.
- `CreateMenu(string name, ScriptType scriptType) -> IMenu`: Creates and returns a new submenu with the specified name and script type.
- `LoadSettings()`: Loads the settings for the main menu.
- `SaveSettings()`: Saves the settings for the main menu.
- `RemoveMenu(IMenu menu)`: Removes a submenu from the main menu.

### Dependencies

- [IMenu](#imenu): Represents a submenu within the main menu.
- [ScriptType](#scripttype): An enumeration representing different types of scripts.
- 
---

## IMenu

### Properties

- `Name` (string): The name of the menu.
- `ScriptType` ([ScriptType](#scripttype)): The type of script associated with the menu.

### Methods

- `AddElement(IMenuElement menuElement)`: Adds a menu element to the menu.
- `Render()`: Renders the menu elements.
- `ProcessKey(VirtualKey virtualKey, KeyState keyState)`: Processes key inputs for the menu.
- `LoadSettings(ISettingsProvider settingsProvider)`: Loads settings for the menu from a settings provider.
- `SaveSettings(ISettingsProvider settingsProvider)`: Saves settings for the menu to a settings provider.

### Menu Element Methods

- `AddSubMenu(string name) -> ISubMenu`: Adds a sub-menu to the menu.
- `AddToggle(string name, bool toggled) -> IToggle`: Adds a toggle element to the menu.
- `AddFloatSlider(string title, float value, float minValue, float maxValue, float step, int precision) -> IValueSlider`: Adds a value slider to the menu.
- `AddHotkey(string title, VirtualKey hotkey, HotkeyType hotkeyType, bool toggled) -> IHotkey`: Adds a hotkey element to the menu.
- `AddComboBox(string title, string[] items, int selectedIndex) -> IComboBox`: Adds a combo box to the menu.
- `AddEnumComboBox<T>(string title, T selectedItem) -> IEnumComboBox<T>`: Adds an enum combo box to the menu, where `T` is an enum type representing the selected item.

### Dependencies

- [IMenu](#imenu): Represents a submenu within the main menu.
- [ScriptType](#scripttype): An enumeration representing different types of scripts.
- [IMenuElement](#imenuelement): Represents an element that can be added to a menu.
- [ISettingsProvider](#isettingsprovider): Provides access to settings and configuration data.
- [ISubMenu](#isubmenu): Represents a submenu within the main menu.
- [IToggle](#itoggle): Represents a toggle element that can be added to a menu.
- [IValueSlider](#ivalueslider): Represents a value slider element that can be added to a menu.
- [IHotkey](#ihotkey): Represents a hotkey element that can be added to a menu.
- [IComboBox](#icombobox): Represents a combo box element that can be added to a menu.
- [IEnumComboBox<T>](#ienumcomboboxt): Represents an enum combo box element that can be added to a menu, where `T` is an enum type representing the selected item.

---

## IMenuElement

### Properties

- `Id` (string): A unique identifier for the menu element.
- `ImGuiId` (string): An identifier for use with ImGui.
- `SaveId` (string): An identifier for saving/loading settings.
- `Name` (string): The name or label of the menu element.
- `Description` (string): A description or tooltip for the menu element.

### Methods

- `Render()`: Renders the menu element.
- `ProcessKey(VirtualKey virtualKey, KeyState keyState)`: Processes key inputs for the menu element.
- `LoadSettings(ISettingsProvider settingsProvider)`: Loads settings for the menu element from a settings provider.
- `SaveSettings(ISettingsProvider settingsProvider)`: Saves settings for the menu element to a settings provider.

### Dependencies

- [ISettingsProvider](#isettingsprovider): Provides access to settings and configuration data.
- [VirtualKey](#virtualkey): An enumeration representing virtual key codes.
- [KeyState](#keystate): An enumeration representing key states.

---

## IComboBox
**Extends:** [IMenuElement](#imenuelement)

**Properties:**

- `SelectedIndex` (int): The index of the currently selected item.
- `Items` (string[]): An array of string items to be displayed in the combo box.

**Events:**

- `SelectionChanged` (event Action<int>): An event triggered when the selected index changes.


**Dependencies:**

- [ISettingsProvider](#isettingsprovider): Provides access to settings and configuration data.
- [VirtualKey](#virtualkey): An enumeration representing virtual key codes.
- [KeyState](#keystate): An enumeration representing key states.

---

## IEnumComboBox<T>
**Extends:** [IMenuElement](#imenuelement)

**Type Parameter:**

- `T` (enum): The enum type representing the selectable items.

**Properties:**

- `SelectedItem` (T): The currently selected item of the specified enum type.

**Events:**

- `SelectionChanged` (event Action<T>): An event triggered when the selected item changes.

**Dependencies:**

- [ISettingsProvider](#isettingsprovider): Provides access to settings and configuration data.
- [VirtualKey](#virtualkey): An enumeration representing virtual key codes.
- [KeyState](#keystate): An enumeration representing key states.

---

## HotkeyType (Enum)

**Members:**

- `Toggle` (0): Represents a hotkey that toggles its state.
- `Press` (1): Represents a hotkey that activates on key press.

---

## IHotkey

**Extends:** [IMenuElement](#imenuelement)

**Properties:**

- `VirtualKey` ([VirtualKey](#virtualkey)): The virtual key associated with the hotkey.
- `HotkeyType` ([HotkeyType](#hotkeytype)): The type of the hotkey, whether it's a toggle or press hotkey.
- `Enabled` (bool): Indicates whether the hotkey is currently enabled.

**Methods:**

- `KeyDown(VirtualKey virtualKey)`: Handles key-down events for the hotkey.
- `KeyUp(VirtualKey virtualKey)`: Handles key-up events for the hotkey.

**Dependencies:**

- [IMenuElement](#imenuelement): Represents a menu element within a script.
- [VirtualKey](#virtualkey): An enumeration representing virtual key codes.

---

## IToggle

**Extends:** [IMenuElement](#imenuelement)

**Properties:**

- `Toggled` (bool): Indicates whether the toggle is currently in the "on" state.

**Dependencies:**

- [IMenuElement](#imenuelement): Represents a menu element within a script.


---

## IValueSlider

**Extends:** [IMenuElement](#imenuelement)

**Properties:**

- `Value` (float): The current value of the slider.

**Dependencies:**

- [IMenuElement](#imenuelement): Represents a menu element within a script.


---

# Scripts

Supports C# and Lua scripting

---

## ScriptType

---

## IScriptingState

### Properties

- `ComboKey` [VirtualKey](#virtualkey): The virtual key code assigned to the combo action.
- `HarasKey` [VirtualKey](#virtualkey): The virtual key code assigned to the harassment action.
- `FarmKey` [VirtualKey](#virtualkey): The virtual key code assigned to the farming action.
- `ClearKey` [VirtualKey](#virtualkey): The virtual key code assigned to the clearing action.
- `ActionType` [ActionType](#actiontype): The combined scripting action type based on the key bindings.
- `IsCombo` (bool): Indicates whether the combo action is active.
- `IsHaras` (bool): Indicates whether the harassment action is active.
- `IsFarm` (bool): Indicates whether the farming action is active.
- `IsClear` (bool): Indicates whether the clearing action is active.

---

## ActionType

### Enum Values

- `None`: No scripting action is active.
- `Combo`: The combo action is active.
- `Haras`: The harassment action is active.
- `Farm`: The farming action is active.
- `Clear`: The clearing action is active.

---

### Enum Values

- `Prediction`: A script related to prediction.
- `Evade`: A script related to evading enemy actions.
- `OrbWalker`: A script related to orb-walking or auto-attacking.
- `TargetSelector`: A script related to target selection.
- `Utility`: A utility script providing miscellaneous functionality.
- `Champion`: A script specifically designed for a champion.

---

## ScriptingState

A class that represents the scripting state, allowing control over various script actions through key inputs.

### Properties

- `VirtualKey ComboKey { get; set; }`: The virtual key associated with triggering the combo action (default: Spacebar).
- `VirtualKey HarasKey { get; set; }`: The virtual key associated with triggering the harass action (default: C).
- `VirtualKey FarmKey { get; set; }`: The virtual key associated with triggering the farm action (default: X).
- `VirtualKey ClearKey { get; set; }`: The virtual key associated with triggering the clear action (default: V).
- `ActionType ActionType { get; private set; }`: Represents the current scripting action type based on key inputs.

- `bool IsCombo`: Indicates if the current scripting action includes the combo action.
- `bool IsHaras`: Indicates if the current scripting action includes the harass action.
- `bool IsFarm`: Indicates if the current scripting action includes the farm action.
- `bool IsClear`: Indicates if the current scripting action includes the clear action.

---

## ITargetSelector

### Properties

- `TargetSelectorMode ` [TargetSelectorMode](#targetselectormode): Gets or sets the current mode used for target selection.
- `float HealthWeight`: Determines the weight given to health when selecting targets in `Weighted` mode.
- `float AbilityPowerWeight`: Determines the weight given to ability power when selecting targets in `Weighted` mode.
- `float DamageWeight`: Determines the weight given to damage when selecting targets in `Weighted` mode.

### Methods

- `GetTarget() -> IHero?`: Retrieves the best target based on the current `TargetSelectorMode`.
- `GetTarget(float range) -> IHero?`: Retrieves the best target within a specified range based on the current `TargetSelectorMode`.

---

## TargetSelectorMode

Enumerates the possible types of TargetSelectorMode.
  - `Weighted`: Uses the various weights to find the best target.
  - `MinHealth`: Targets the unit with the lowest health.
  - `MaxAp`: Targets the unit with the highest Ability Power.
  - `MaxAd`: Targets the unit with the highest Attack Damage.
  - `Closest`: Targets the nearest unit.
  - `ClosestToCursor`: Targets the unit closest to the cursor.

## `MinionPrediction` Structure

### Properties

- `IMinion? Minion`: Represents the predicted minion.
- `float PredictedHealth`: Represents the predicted health of the minion.
- `bool IsValid`: Checks if the predicted minion is valid (i.e., not null).

---

## IMinionSelector

### Methods

- `GetKillableMinions(float range) -> IEnumerable<MinionPrediction>`: Retrieves minions that are predicted to be killable within a specified range.

- `GetKillableMinions(IEnumerable<IMinion> minions) -> IEnumerable<MinionPrediction>`: Retrieves a list of predicted killable minions from a given list of minions.

- `GetKillableMinions(float range, float damage, DamageType damageType) -> IEnumerable<MinionPrediction>`: Retrieves minions that are predicted to be killable within a specified range, given a specific damage and damage type.

- `GetKillableMinions(IEnumerable<IMinion> minions, float damage, DamageType damageType) -> IEnumerable<MinionPrediction>`: Retrieves a list of predicted killable minions from a given list of minions, given a specific damage and damage type.

- `GetBestKillableMinion(float range) -> MinionPrediction`: Retrieves the best killable minion within a specified range.

- `GetBestKillableMinion(float range, float damage, DamageType damageType) -> MinionPrediction`: Retrieves the best killable minion within a specified range, given a specific damage and damage type.

- `GetHealthiestMinion(float range) -> IMinion?`: Retrieves the minion with the most health within a specified range.

---

## Timer

### Properties

- `bool IsReady`: Checks whether the target time has been reached with respect to the game's current time.

### Methods

- `SetTime(float time)`: Directly sets the `targetTime` to the provided time.
- `SetDelay(float delay)`: Sets a delay by adding the specified delay to the current game time, and assigns the resulting value to `targetTime`.

---

## Lua

Lua scripts should be located in `/Resources/LuaScripts/{Type}` <br/>
Types
- Prediction -> `/Resources/LuaScripts/Prediction`
- Evade -> `/Resources/LuaScripts/Evade`
- OrbWalker -> `/Resources/LuaScripts/OrbWalker`
- TargetSelector -> `/Resources/LuaScripts/TargetSelector`
- Utility -> `/Resources/LuaScripts/Utility`
- Champion -> `/Resources/LuaScripts/Champion`

### Objects and Managers

- `Renderer`: [IRenderer](#irenderer)
- `DamageCalculator`: [IDamageCalculator](#idamagecalculator)
- `DamagePrediction`: [IDamagePrediction](#idamageprediction)
- `GameCamera`: [IGameCamera](#igamecamera)
- `Hero`: [ILocalPlayer](#ikocalplayer)
- `GameState`: [IGameState](#igamestate)
- `MinionManager`: [IMinionManager](#iminionmanager)
- `MonsterManager`: [IMonsterManager](#imonstermanager)
- `PlantManager`: [IPlantManager](#iplantmanager)
- `WardManager`:  [IWardManager](#iwardmanager)
- `TrapManager`: [ITrapManager](#itrapmanager)
- `ObjectManager`: [IObjectManager](#iobjectmanager)
- `TurretManager`: [ITurretManager](#iturretmanager)
- `InhibitorManager`: [IInhibitorManager](#iinhibitormanager)
- `MissileManager`: [IMissileManager](#imissilemanager)
- `HeroManager`: [IHeroManager](#iheromanager)
- `GameInput`: [IGameInput](#igameinput)
- `MinionSelector`: [IMinionSelector](#iminionselector)
- `ScriptingState`: [IScriptingState](#iscriptingstate)
- `TargetSelector`: [ITargetSelector](#itargetselector)
- `Prediction`: [IPrediction](#iprediction)

### Constructors and Functions

- `Vector2`: A constructor to create a 2D vector. `Vector2(X, Y)` `float` example ussage: `local v = Vector2(1, 5)`
- `Color`: A constructor to create a color. `Color(r, g, b, a)` `float` example ussage: `local v = Color(0.1, 0.2, 1.0, 1.0)`
- `Vector3`: A constructor to create a 3D vector.`Vector3(X, Y)` `float` example ussage: `local v = Vector2(1, 5, 10)`
- `Math.Distance`: A function to calculate the distance between vectors. example ussage: `local d = Math.Distance(v1, v2)`

### Enums:
  - `MouseButton`: [MouseButton](#mousebutton) example ussage: `MouseButton.Left`
  - `VirtualKey`: [VirtualKey](#virtualkey) example ussage: `VirtualKey.X`
  - `KeyState`:[KeyState](#keystate) example ussage: `KeyState.KeyDown`
  - `IssueOrderType`: [IssueOrderType](#issueordertype) example ussage: `IssueOrderType.Attack`
  - `DamageType`: [DamageType](#damagetype) example ussage: `DamageType.Physical`
  - `ScriptType`: [ScriptType](#scripttype) example ussage: `ScriptType.OrbWalker`
  - `HotkeyType`: [HotkeyType](#hotkeytype) example ussage: `HotkeyType.Toggle`

### MainMenu Functions

- `CreateMenu`: Creates a new [IMenu](#imenu) in the main menu.
- `RemoveMenu(menu)`: Removes menu from main menu.

### Using Objects
- `Calling properties`: example `local pos = Hero.Position`
- `Calling methods`: example `local screenPosition = Camera:WorldToScreen(Hero.Position)`

### Iterating enumerable example
```
local missiles = MissileManager:GetMissiles()
for missile in luanet.each(missiles) do
  if missile ~= nil then
      Renderer:Circle3D(missile.Position, 100, Color(0.0, 0.0, 1.0, 1.0), 1, GameState.Time, 0.5, 1)
 end
end
```

### Creating menu example
```
local humanizer = nil
local extraWindup = nil
local ping = nil
local stoppingDistance = nil
local comboHotkey = nil
local farmHotkey = nil
local clearHotkey = nil

function OnLoad()
    local menu = CreateMenu("Test Lua menu", ScriptType.OrbWalker)
    humanizer = menu:AddValueSlider("Humanizer", "Delay between actions is ms", 25, 25, 300)

    extraWindup = menu:AddValueSlider("Extra windup", "", 10, 0, 80)
    ping = menu:AddValueSlider("Ping", "", 35, 0, 250)

    stoppingDistance = menu:AddValueSlider("Stopping distance", "No move action will be taken when mouse is in range", 80, 0, 250)

    local hotkeysSubmenu = menu:AddSubMenu("Hotkeys", "You can setup hotkeys for orbwalker")
    
    comboHotkey = hotkeysSubmenu:AddHotkey("Combo", "Combo mode. Will attack enemy heroes.", VirtualKey.Spacebar, HotkeyType.Press)

    farmHotkey = hotkeysSubmenu:AddHotkey("Farm", "Farm mode. Will farm minions.", VirtualKey.X, HotkeyType.Press)

    clearHotkey = hotkeysSubmenu:AddHotkey("Clear", "Clear mode. Will attack minions.", VirtualKey.V, HotkeyType.Press)
    
end
```

### Example Attack command
```
    if gameInput:IssueOrder(target.Position, IssueOrderType.Attack) then
        return true
    end
```

### Example Move command
```
    if gameInput:IssueOrder(position, IssueOrderType.Move) == true then
        humanizeTimer = humanizer.Value / 1000
    end
```

## C# Scripts
Your c# scripts should extend : IScript
register you c# script in `ScriptsServiceInstaller` in `Scripts` project
```
  collection.AddSingleton<IScript, Your script name>();
```