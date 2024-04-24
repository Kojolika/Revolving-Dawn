using System.Collections.Generic;
using Cards;
using Models.Buffs;
using System.Linq;
using UnityEngine;
using System;
using Fight.Events;

namespace Models.Cards
{
    [Serializable]
    public class CardAffectContainer
    {
        [SerializeReference] private ICardAffect cardAffect;
        [SerializeField] private CardAffectDefinition cardAffectDefinition;
    }

    public interface ICardAffect
    {
        List<IBattleEvent> Execute(List<IBuffable> targets);
        List<IBattleEvent> Execute(List<IHealth> targets);
    }

    [Serializable]
    public abstract class CardAffect<T> : ICardAffect where T : CardAffectDefinition
    {
        [SerializeField] private Targeting targeting;

        public Targeting Targeting => targeting;

        public abstract List<IBattleEvent> Execute(List<IBuffable> targets);
        public abstract List<IBattleEvent> Execute(List<IHealth> targets);
    }

    public abstract class CardAffectDefinition : ScriptableObject
    {
        public abstract string Description { get; }
        public virtual List<IBattleEvent> Execute(List<IBuffable> targets)
            => Execute(targets.Select(target => target as IHealth).ToList());
        public abstract List<IBattleEvent> Execute(List<IHealth> targets);
    }
}