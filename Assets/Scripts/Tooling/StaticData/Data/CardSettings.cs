using System.Collections.Generic;
using UnityEngine;

namespace Tooling.StaticData.Data
{
    public class CardSettings : StaticData
    {
        [Tooltip("The Card and mana defined here will determine the final downgrade a card can have for which mana type it is."
               + "That is, if a card has a null downgrade, it will be transformed to the card here depending on its mana type.")]
        public List<CardManaPair> DefaultDowngrades;

        public class CardManaPair
        {
            public Mana Mana;
            public Card Card;
        }
    }
}