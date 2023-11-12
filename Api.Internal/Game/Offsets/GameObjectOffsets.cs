using System.ComponentModel;
using System.Numerics;
using Api.Game.Objects;
using Api.Game.Offsets;
using Api.Internal.Game.Types;
using Api.Internal.Utils.Converters;
using Microsoft.Extensions.Configuration;

namespace Api.Internal.Game.Offsets
{
    internal class GameObjectOffsets : IGameObjectOffsets
    {
        public OffsetData NetworkId { get; }
        public OffsetData Name { get; }
        public OffsetData Team { get; }
        public OffsetData IsVisible { get; }
        public OffsetData Position { get; }
        public OffsetData ObjectName { get; }

        public GameObjectOffsets(IConfiguration configuration)
        {
            var cs = configuration.GetSection(nameof(GameObjectOffsets));
            
            NetworkId = new OffsetData(nameof(Team), Convert.ToUInt32(cs[nameof(NetworkId)], 16), typeof(int));
            Name = new OffsetData(nameof(Name), Convert.ToUInt32(cs[nameof(Name)], 16), typeof(TString));
            Team = new OffsetData(nameof(Team), Convert.ToUInt32(cs[nameof(Team)], 16), typeof(int));
            IsVisible = new OffsetData(nameof(IsVisible), Convert.ToUInt32(cs[nameof(IsVisible)], 16), typeof(bool));
            Position = new OffsetData(nameof(Position), Convert.ToUInt32(cs[nameof(Position)], 16), typeof(Vector3));
            ObjectName = new OffsetData(nameof(ObjectName), Convert.ToUInt32(cs[nameof(ObjectName)], 16), typeof(TString));
        }
        
        public IEnumerable<OffsetData> GetOffsets()
        {
            yield return NetworkId;
            yield return Name;
            yield return Team;
            yield return IsVisible;
            yield return Position;
            yield return ObjectName;
        }
    }
}