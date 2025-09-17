using System.Collections.Generic;
using Fight;
using Fight.Engine;
using Fight.Events;
using Tooling.StaticData.Data;

namespace Models.Cards
{
    public class Refresh : CardLogic
    {
        public Refresh(Card model) : base(model)
        {
        }

        public override List<IBattleEvent> Play(Context fightContext, ICombatParticipant owner)
        {
            //TODO: Choose a mana and reroll it
            return null;
        }
    }
}