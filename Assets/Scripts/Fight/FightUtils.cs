using System;
using System.Collections.Generic;
using Fight.Engine;
using Fight.Events;
using Models.Cards;
using Tooling.Logging;
using Tooling.StaticData.Data;
using UnityEngine.Assertions;
using Utils.Extensions;

namespace Fight
{
    public static class FightUtils
    {
        // TODO: Add a common key static class, create tests to make sure these common keys are defined in the static data base
        public const string HealthKey = "Health";

        public static IBattleEvent DealDamage(ICombatParticipant source, ICombatParticipant target, float damage)
        {
            var healthStat = StaticDatabase.Instance.GetStaticDataInstance<Stat>(HealthKey);
            return new StatLostEvent(target, source, healthStat, damage);
        }

        public static void DiscardCard(this ICardDeckParticipant participant, CardLogic cardLogic)
        {
            if (!participant.Hand.Remove(cardLogic))
            {
                MyLogger.Error(
                    $"Cannot discard card {cardLogic.Model.Name} because its not in the participant {participant.Name}'s hand!");
                return;
            }

            participant.Discard.Add(cardLogic);
        }

        public static void LoseCard(this ICardDeckParticipant participant, CardLogic cardLogic)
        {
            if (participant.Hand.Contains(cardLogic))
            {
                participant.Hand.Remove(cardLogic);
            }
            else if (participant.Discard.Contains(cardLogic))
            {
                participant.Discard.Remove(cardLogic);
            }
            else if (participant.Draw.Contains(cardLogic))
            {
                participant.Draw.Remove(cardLogic);
            }

            participant.Lost.Add(cardLogic);
        }

        /// <summary>
        /// Performs the logic to draw a card from a participant draw pile and returns the card drawn.
        /// </summary>
        public static CardLogic DrawCard(this ICardDeckParticipant participant)
        {
            if (participant.Draw.Count == 0)
            {
                if (participant.Discard.Count == 0)
                {
                    MyLogger.Error($"Fatal: No cards in the draw pile or discard pile! participant={participant.Name}");
                    return null;
                }

                ShuffleDeck(participant.Discard);
                participant.Draw.Clear();
                participant.Draw.AddRange(participant.Discard);
                participant.Discard.Clear();
            }

            var cardDrawn = participant.Draw[^1];
            participant.Draw.Remove(cardDrawn);
            participant.Hand.Add(cardDrawn);

            return cardDrawn;
        }

        public static void PlayCard(this ICardDeckParticipant participant, Context fightContext, CardLogic cardLogic)
        {
            fightContext.BattleEngine.AddEvents(cardLogic.Play(fightContext, participant).ToArray());
            if (cardLogic.Model.IsLostOnPlay)
            {
                fightContext.BattleEngine.AddEvent(new LoseCardEvent(participant, cardLogic));
            }
            else
            {
                fightContext.BattleEngine.AddEvent(new DiscardCardEvent(participant, cardLogic));
            }
        }

        public static void UpgradeCard(ref CardLogic cardLogic)
        {
            if (cardLogic.Model.Upgrade == null)
            {
                MyLogger.Error($"Cannot upgrade a card without an upgrade! card={cardLogic.Model.Name}");
                return;
            }
            // TODO:
            // cardLogic = cardLogic.Model.Upgrade.CardLogic;
        }

        public static void DowngradeCard(ref CardLogic cardLogic)
        {
            if (cardLogic.Model.Downgrade == null)
            {
                if (cardLogic.Model.Manas.IsNullOrEmpty())
                {
                    MyLogger.Error($"Cannot downgrade card {cardLogic.Model.Name} with zero mana to upgrade it.");
                    return;
                }

                MyLogger.Error("ERROR: TODO:, ADD default downgrades");

                /*var cardModel = card;
                card = CardSettings.DowngradeBaseCardsForMana
                                   .Where(cardManaPair => cardManaPair.ManaSODefinition.Representation == cardModel.Manas[0])
                                   .Select(cardManaPair => cardManaPair.Card.Representation)
                                   .First();

                Debug.Assert(card != null, "Card shouldn't be null after downgrading." +
                                           "ManaDefinition may be a different instance than the scriptableObject one or card settings do not have ManaDefinition's specified! ");*/
            }
            else
            {
                // TODO: impl
                //cardLogic = cardLogic.Model.Downgrade.CardLogic;
            }
        }

        /// <summary>
        /// Shuffle the specified deck by using the Fisher-Yates shuffle algorithm.
        /// </summary>
        /// <param name="deck">Deck to shuffle.</param>
        private static void ShuffleDeck(List<CardLogic> deck)
        {
            if (deck.IsNullOrEmpty())
            {
                return;
            }

            var rng      = new Random();
            int deckSize = deck.Count - 1;
            while (deckSize > 1)
            {
                var randomNum = rng.Next(0, deckSize);
                (deck[randomNum], deck[deckSize]) = (deck[deckSize], deck[randomNum]);
                deckSize--;
            }
        }

        public static void AddBuff(ICombatParticipant target, Buff buff, int amount)
        {
            int currentAmount = target.GetBuff(buff);
            int newAmount     = (int)MathF.Max(currentAmount + amount, buff.MaxStackSize);

            Assert.IsTrue(newAmount >= currentAmount);

            target.SetBuff(buff, newAmount);
        }

        public static void RemoveBuff(ICombatParticipant target, Buff buff, int amount)
        {
            int currentAmount = target.GetBuff(buff);
            int newAmount     = (int)MathF.Max(currentAmount - amount, buff.MaxStackSize);

            Assert.IsTrue(newAmount <= currentAmount);

            target.SetBuff(buff, newAmount);
        }

        public static void AddStat(ICombatParticipant target, Stat stat, float amount)
        {
            float currentAmount = target.GetStat(stat) ?? 0f;

            // We have a naming convention where a stat name can be prefixed with Max to allow max stats on characters on an individual basis
            // This allows flexibility
            float max = GetMaxStat(stat.Name) is { } maxStat &&
                        target.GetStat(maxStat) is { } maxStatAmount
                ? maxStatAmount
                : float.NegativeInfinity;

            float newAmount = MathF.Max(currentAmount + amount, max);

            Assert.IsTrue(newAmount >= currentAmount);

            target.SetStat(stat, newAmount);
        }


        public static void SetHealth(ICombatParticipant target, float health)
        {
            var healthStat = StaticDatabase.Instance.GetStaticDataInstance<Stat>(HealthKey);
            target.SetStat(healthStat, health);
        }

        public static float? GetHealth(ICombatParticipant target)
        {
            var healthStat = StaticDatabase.Instance.GetStaticDataInstance<Stat>(HealthKey);
            return target.GetStat(healthStat);
        }

        public static void SetMaxHealth(ICombatParticipant target, float health)
        {
            var maxHealthStat = GetMaxStat(HealthKey);
            target.SetStat(maxHealthStat, health);
        }

        public static float? GetMaxHealth(ICombatParticipant target)
        {
            var maxHealthStat = GetMaxStat(HealthKey);
            return target.GetStat(maxHealthStat);
        }

        public static Stat GetMaxStat(string statKey)
        {
            return StaticDatabase.Instance.GetStaticDataInstance<Stat>($"Max{statKey}");
        }
    }
}