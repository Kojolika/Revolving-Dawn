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

        // TODO: Named float numbers in static data
        // We can specify them in json instead of hardcoded
        private const float Damage = 6f;

        public override List<IBattleEvent> Play(Context fightContext, ICombatParticipant owner)
        {
            var target = GetTargetsForOptionIndex(0)[0];

            return new List<IBattleEvent> { FightUtils.DealDamage(owner, target, Damage) };
        }
    }
}