using System.Collections.Generic;
using Models.Cards;
using Models.Fight;
using Tooling.StaticData.Data;

namespace Fight.Engine
{
    /// <summary>
    /// Represents a single character/enemy/player/thing in combat.
    /// </summary>
    public interface ICombatParticipant
    {
        /// <summary>
        /// The name of the participant
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The team this participant is on
        /// </summary>
        TeamType Team { get; }

        /// <summary>
        /// Returns whether this participant has this stat
        /// </summary>
        bool HasStat(Stat stat);
        
        /// <summary>
        /// Gets the stat value of a stat type for this participant
        /// </summary>
        float GetStat(Stat stat);

        /// <summary>
        /// Sets the stat value of a stat type for this participant
        /// </summary>
        void SetStat(Stat stat, float value);

        /// <summary>
        /// Gets the buff stack size of a buff type for this participant
        /// </summary>
        int GetBuff(Buff buff);

        /// <summary>
        /// Sets the buff stack size of a buff type for this participant
        /// </summary>
        void SetBuff(Buff buff, int value);

        /// <summary>
        /// Lists all the buffs this participant has
        /// </summary>
        List<(int stackSize, Buff)> GetBuffs();

        /// <summary>
        /// Lists all the stats this participant has
        /// </summary>
        List<(float amount, Stat)> GetStats();
    }

    /// <summary>
    /// Similar to see <see cref="ICombatParticipant"/> but this participant has a card deck associated with them
    /// </summary>
    public interface ICardDeckParticipant : ICombatParticipant
    {
        /// <summary>
        /// Returns all the cards in the deck of this participant
        /// </summary>
        List<CardLogic> Deck { get; }

        /// <summary>
        /// Returns all  the cards in the draw pile of this participant
        /// </summary>
        List<CardLogic> Draw { get; }

        /// <summary>
        /// Returns all the cards in the hand of this participant
        /// </summary>
        List<CardLogic> Hand { get; }

        /// <summary>
        /// Returns all the cards in the discard pile of this participant
        /// </summary>
        List<CardLogic> Discard { get; }

        /// <summary>
        /// Returns all the cards in the lost pile of this participant
        /// </summary>
        List<CardLogic> Lost { get; }
    }

    /// <summary>
    /// Similar to see <see cref="ICombatParticipant"/> but this participant has a set a moves they can choose from
    /// </summary>
    public interface IMoveParticipant : ICombatParticipant
    {
        void SelectMove();
    }
}