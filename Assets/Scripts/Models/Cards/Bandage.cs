using System.Collections.Generic;
using Fight;
using Fight.Engine;
using Fight.Events;
using Tooling.StaticData.Data;

namespace Models.Cards
{
    public class Bandage : CardLogic
    {
        public Bandage(Card model) : base(model)
        {
        }

        public override List<IBattleEvent> Play(Context fightContext, ICombatParticipant owner)
        {
            throw new System.NotImplementedException();
        }
    }
}