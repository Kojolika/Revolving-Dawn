using System.Collections.Generic;
using Cards;
using Models.Buffs;
using System.Linq;
using UnityEngine;
using System;
using Fight.Events;

/// <summary>
/// It may seem a little strange how we have the card effect classes set up here.
/// But there is a particular reason for it.
/// This has been designed in a specfic way which allows us add any type of <see cref="CardEffect"/> to
/// a <see cref="Models.Card.Card"/> in the inspector.
/// This way we can assemble cards as blocks by adding any effect we want to them. 
/// </summary>
namespace Models.CardEffects
{
    /// <summary>
    /// This class is the one we see have attached to <see cref="Models.Card"/>,
    /// using a custom propety drawers (<see cref="CardEffectDrawer"/>)
    /// </summary>
    [Serializable]
    public class CardEffectContainer
    {
        [SerializeReference] private ICardEffect cardEffect;
        [SerializeField] private CardEffectDefinition cardEffectDefinition;

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
    /// This is where the actual static data of the effect is defined.
    /// Since the data is static, we use a <see cref="ScriptableObject"/> as
    /// we can reference the same effect on every card that uses it. 
    /// </summary>
    public abstract class CardEffectDefinition : ScriptableObject
    {
        public abstract string Description { get; }
        public virtual List<IBattleEvent> Execute(List<IBuffable> targets)
            => Execute(targets.Select(target => target as IHealth).ToList());
        public abstract List<IBattleEvent> Execute(List<IHealth> targets);
    }

    /// <summary>
    /// This is where the variable effects of the card effect are defined. That way different cards
    /// can have different stats for each card effect.
    /// Note: we only allow one derived type for each T.
    /// </summary>
    /// <typeparam name="T">The static definition for this card effect.</typeparam>
    [Serializable]
    public abstract class CardEffect<T> : ICardEffect where T : CardEffectDefinition
    {
        [SerializeField] private Targeting targeting;

        public Targeting Targeting => targeting;

        public abstract List<IBattleEvent> Execute(List<IBuffable> targets);
        public abstract List<IBattleEvent> Execute(List<IHealth> targets);
    }
}