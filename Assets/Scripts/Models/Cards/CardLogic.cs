using System.Collections.Generic;
using Common.Util;
using Fight.Engine;
using Fight.Events;
using Tooling.Logging;
using Tooling.StaticData.Data;
using Zenject;
using Context = Fight.Context;

namespace Models.Cards
{
    /// <summary>
    /// Class that represents the logic of what happens when this card is played.
    /// The rest of the data and assets related to a card are stored in <see cref="Tooling.StaticData.Data.Card"/> in the <see cref="Tooling.StaticData.Data.StaticDatabase"/>
    ///
    /// These types are reference in the static database and are dynamically instanced with the card view during run time.
    /// </summary>
    public abstract class CardLogic
    {
        private readonly Dictionary<int, List<ICombatParticipant>> targetingLookup = new();

        /// <summary>
        /// Contains the static data and assets for this card.
        /// </summary>
        public Card Model { get; }

        public CardLogic(Card model)
        {
            Model = model;
        }

        /// <summary>
        /// Gets a float value defined on the <see cref="Model"/>'s <see cref="Card.DynamicValues"/> list
        /// </summary>
        /// <param name="name"> The name of the value in this lsit</param>
        /// <returns> the float associated with the value </returns>
        protected float GetFloat(string name)
        {
            foreach (var dynamicValue in Model.DynamicValues.OrEmptyIfNull())
            {
                if (dynamicValue.Name == name && float.TryParse(dynamicValue.Value, out float result))
                {
                    return result;
                }
            }

            MyLogger.Error($"Could not find a float for {name} on {Model.Name}");
            return 0;
        }

        /// <summary>
        /// Called during the target selection phase of when a card is played.
        /// This is always called BEFORE <see cref="Play"/>
        /// </summary>
        public void SetTargetsForIndex(int index, List<ICombatParticipant> targets)
        {
            if (index >= Model.TargetingOptions.Count || index < 0)
            {
                MyLogger.Error(
                    $"Invalid index targeting index! Must be in range of the {Model.TargetingOptions}! index={index}");
            }

            targetingLookup[index] = targets;
        }

        /// <summary>
        /// Use this during overrides of the <see cref="Play"/> to select which targets match your card effects.
        /// </summary>
        /// <returns> The target(s) from the <see cref="Tooling.StaticData.Data.Card.TargetingOptions"/> at the index </returns>
        protected List<ICombatParticipant> GetTargetsForOptionIndex(int index)
        {
            if (index >= Model.TargetingOptions.Count || index < 0)
            {
                MyLogger.Error(
                    $"Invalid index targeting index! Must be in range of the {Model.TargetingOptions}! index={index}");
                return null;
            }

            if (!targetingLookup.TryGetValue(index, out var targets))
            {
                MyLogger.Error(
                    $"Fatal error! Targeting look up does not contain targets for the index requested! index={index}");
                return null;
            }

            return targets;
        }

        /// <summary>
        /// What the card does on play
        /// </summary>
        /// <param name="fightContext"> Provides access to current fight data </param>
        /// <param name="owner"> The combat participant playing the card </param>
        /// <returns> The list of battle events to add to the battle engine </returns>
        public abstract List<IBattleEvent> Play(Context fightContext, ICombatParticipant owner);

        public class Factory : PlaceholderFactory<Card, CardLogic>
        {
        }

        public class CustomFactory : IFactory<Card, CardLogic>
        {
            private readonly DiContainer diContainer;

            public CustomFactory(DiContainer diContainer)
            {
                this.diContainer = diContainer;
            }

            public CardLogic Create(Card card)
            {
                var cardLogicType = card.CardLogic;
                if (cardLogicType == null || cardLogicType.IsAbstract)
                {
                    MyLogger.Error($"Couldn't create card, {nameof(card.CardLogic)} is null or abstract! cardLogic={card.CardLogic}");
                    return null;
                }

                return diContainer.Instantiate(card.CardLogic, extraArgs: new object[] { card }) as CardLogic;
            }
        }
    }
}