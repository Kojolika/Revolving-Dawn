using System.Collections.Generic;
using Fight;
using Fight.Engine;
using Fight.Events;
using Tooling.StaticData.Data;

namespace Models.Cards
{
    public class Rock : CardLogic
    {
        public Rock(Card model) : base(model)
        {
        }

        public override List<IBattleEvent> Play(Context fightContext, ICombatParticipant owner)
        {
            // TODO: block 1?
            throw new System.NotImplementedException();
        }
    }
}