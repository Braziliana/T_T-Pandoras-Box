using Api.Game.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Api.Game.GameInputs
{
    public interface IGameInput
    {
        Vector2 MousePosition { get; }
        bool IssueOrder(Vector2 position, IssueOrderType issueOrderType);
        bool IssueOrder(Vector3 position, IssueOrderType issueOrderType);
        bool Attack(IGameObject target);
        void CastEmote(int emote);

        void Update(float deltaTime);
        
        bool LevelUpSpell(SpellSlot spellSlot);
        bool CastSpell(SpellSlot spellSlot);
        bool SelfCastSpell(SpellSlot spellSlot);
        bool CastSpell(SpellSlot spellSlot, Vector2 position);
        bool CastSpell(SpellSlot spellSlot, Vector3 position);
        bool CastSpell(SpellSlot spellSlot, IGameObject target);
    }
}
