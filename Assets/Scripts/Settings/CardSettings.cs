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

            public ManaSODefinition ManaSODefinition => manaDefinition;
            public CardSODefinition Card => card;
        }

        [Tooltip("The Card and mana defined here will determine the final downgrade a card can have for which mana type it is."
            + "That is, if a card has a null downgrade, it will be transformed to the card here depending on its mana type.")]
        [SerializeField] private List<CardManaPair> downgradeBaseCardsForMana;

        [Tooltip("The speed which cards will be moved around in the player hand.")]
        [SerializeField] private float cardMoveSpeedInHand;

        [Tooltip("The speed which cards will be rotated around in the player hand.")]
        [SerializeField] private float  cardRotateSpeedInHand;
            
        [Tooltip("The type of move function cards use when being drawn.")]
        [SerializeField] PrimeTween.Ease cardMoveFunction;


        public List<CardManaPair> DowngradeBaseCardsForMana => downgradeBaseCardsForMana;
        public float CardMoveSpeedInHand => cardMoveSpeedInHand;
        public float CardRotateSpeedInHand => cardRotateSpeedInHand;
        public PrimeTween.Ease CardMoveFunction => cardMoveFunction;
    }
}