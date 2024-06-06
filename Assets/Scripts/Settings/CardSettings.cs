using System.Collections.Generic;
using Models;
using Models.Mana;
using UnityEngine;

namespace Settings
{
    [CreateAssetMenu(fileName = "New " + nameof(CardSettings), menuName = "RevolvingDawn/Settings/" + nameof(CardSettings))]
    public class CardSettings : ScriptableObject
    {
        [System.Serializable]
        public class CardManaPair
        {
            [SerializeField] private ManaSODefinition manaDefinition;
            [SerializeField] private CardSODefinition card;
        }

        [Tooltip("The Card and mana defined here will determine the final downgrade a card can have for which mana type it is."
            + "That is, if a card has a null downgrade, it will be transformed to the card here depending on its mana type.")]
        [SerializeField] private List<CardManaPair> downgradeBaseCardsForMana;
    }
}