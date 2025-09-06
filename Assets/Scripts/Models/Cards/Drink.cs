using System.Collections.Generic;
using Fight;
using Fight.Engine;
using Fight.Events;
using Tooling.StaticData.Data;

namespace Models.Cards
{
    public class Drink : CardLogic
    {
        public Drink(Card model) : base(model)
        {
        }

        public override List<IBattleEvent> Play(Context fightContext, ICombatParticipant owner)
        {
            // TODO: heal one
            throw new System.NotImplementedException();
        }
    }
}