using System.Collections.Generic;
using Fight;
using Fight.Engine;
using Fight.Events;

namespace Models.Cards
{
    /// <summary>
    /// Weak card, default card for red mana on downgrades
    /// </summary>
    public class Ignite : CardLogic
    {
        public Ignite(Tooling.StaticData.Data.Card model) : base(model)
        {
        }

        // TODO: Named float numbers in static data
        // We can specify them in json instead of hardcoded
        private const float Damage = 1f;

        public override List<IBattleEvent> Play(Context fightContext, ICombatParticipant owner)
        {
            var target = GetTargetsForOptionIndex(0)[0];
            return new List<IBattleEvent> { FightUtils.DealDamage(owner, target, Damage) };
        }
    }
}