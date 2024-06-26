using System.Collections.Generic;
using Cards;
using Models.Buffs;
using System.Linq;
using UnityEngine;
using System;
using Fight.Events;


namespace Models.CardEffects
{
    /// <summary>
    /// Using an interface allows us to create a list of classes in the inspector.
    /// </summary>
    public interface ICombatEffect
    {
        List<IBattleEvent> Execute(List<IBuffable> targets);
        List<IBattleEvent> Execute(List<IHealth> targets);
    }

    /// <summary>
    /// Stats and effects of each card event are defined here.
    /// </summary>
    [Serializable]
    public abstract class CombatEffect : ICombatEffect
    {
        [SerializeField] private Targeting targeting;
        public Targeting Targeting => targeting;

        public virtual List<IBattleEvent> Execute(List<IBuffable> targets)
            => Execute(targets.Select(target => target as IHealth).ToList());
        public abstract List<IBattleEvent> Execute(List<IHealth> targets);
    }
}