using System.Collections.Generic;
using Tooling.StaticData.Data.Validation;
using UnityEngine;

namespace Tooling.StaticData.Data
{
    // TODO: Create default cards and a default static data instance
    public class DowngradeCardSettings : StaticData
    {
        [Tooltip("The Card and mana defined here will determine the final downgrade a card can have for which mana type it is."
               + "That is, if a card has a null downgrade, it will be transformed to the card here depending on its mana type.")]
        [ListSizeValidator(minCount: 4, maxCount: 4)]
        public List<DefaultDowngrade> DefaultDowngrades;
    }

    public struct DefaultDowngrade
    {
        [Required]
        public Mana Mana;

        [Required]
        public Card Card;
    }
}