using System.Collections.Generic;
using Cards;
using Models.Buffs;
using System.Linq;
using UnityEngine;
using System;
using Fight.Events;
using Utils.Attributes;


namespace Models.CardEffects
{
    /// <summary>
    /// This has been designed in a specfic way which allows us add any type of <see cref="ICardEffect"/> to
    /// a <see cref="Models.Card.Card"/> in the inspector.
    /// This way we can assemble cards like building blocks by adding any effect we want to them. 
    /// </summary>

    /// <summary>
    /// This class is a wrapper around <see cref="ICardEffect"/> so we can create lists of them
    /// in the inspector. (see <see cref="DisplayInterfaceAttribute"/> for more details)
    /// </summary>
    [Serializable]
    public class CardEffectWrapper
    {
        [SerializeReference, DisplayInterface(typeof(ICardEffect))] private ICardEffect cardEffect;
        public ICardEffect CardEffect => cardEffect;
    }

    /// <summary>
    /// Using an interface allows us to create a list of classes in the inspector.
    /// </summary>
    public interface ICardEffect
    {
        List<IBattleEvent> Execute(List<IBuffable> targets);
        List<IBattleEvent> Execute(List<IHealth> targets);
    }

    /// <summary>
    /// Stats and effects of each card event are defined here.
    /// </summary>
    [Serializable]
    public abstract class CardEffect : ICardEffect
    {
        [SerializeField] private Targeting targeting;
        public Targeting Targeting => targeting;

        public virtual List<IBattleEvent> Execute(List<IBuffable> targets)
            => Execute(targets.Select(target => target as IHealth).ToList());
        public abstract List<IBattleEvent> Execute(List<IHealth> targets);
    }
}