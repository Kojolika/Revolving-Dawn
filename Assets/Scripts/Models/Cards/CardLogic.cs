using System.Collections.Generic;
using Fight;
using Fight.Engine;
using Fight.Events;
using Tooling.Logging;
using Tooling.StaticData.EditorUI;

namespace Models.Cards
{
    /// <summary>
    /// Class that represents the logic of what happens when this card is played.
    /// The rest of the data and assets related to a card are stored in <see cref="Tooling.StaticData.EditorUI.Card"/> in the <see cref="Tooling.StaticData.EditorUI.StaticDatabase"/>
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
        /// Called during the target selection phase of when a card is played.
        /// This is always called BEFORE <see cref="Play"/>
        /// </summary>
        public void SetTargetsForIndex(int index, List<ICombatParticipant> targets)
        {
            if (index >= Model.TargetingOptions.Count || index < 0)
            {
                MyLogger.Error($"Invalid index targeting index! Must be in range of the {Model.TargetingOptions}! index={index}");
            }

            targetingLookup[index] = targets;
        }

        /// <summary>
        /// Use this during overrides of the <see cref="Play"/> to select which targets match your card effects.
        /// </summary>
        /// <returns> The target(s) from the <see cref="Tooling.StaticData.EditorUI.Card.TargetingOptions"/> at the index </returns>
        protected List<ICombatParticipant> GetTargetsForOptionIndex(int index)
        {
            if (index >= Model.TargetingOptions.Count || index < 0)
            {
                MyLogger.Error($"Invalid index targeting index! Must be in range of the {Model.TargetingOptions}! index={index}");
                return null;
            }

            if (!targetingLookup.TryGetValue(index, out var targets))
            {
                MyLogger.Error($"Fatal error! Targeting look up does not contain targets for the index requested! index={index}");
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
    }
}