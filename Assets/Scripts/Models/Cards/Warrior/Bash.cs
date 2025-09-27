using System.Collections.Generic;
using Fight;
using Fight.Engine;
using Fight.Events;

namespace Models.Cards.Warrior
{
    public class Bash : CardLogic
    {
        public Bash(Tooling.StaticData.Data.Card model) : base(model)
        {
        }

        public override List<IBattleEvent> Play(Context fightContext, ICombatParticipant owner)
        {
            var target = GetTargetsForOptionIndex(0)[0];

            return new List<IBattleEvent> { FightUtils.DealDamage(owner, target, GetFloat("damage")) };
        }
    }
}