using System;
using System.Collections.Generic;
using Models.Cards;
using Tooling.StaticData.Data.Validation;
using UnityEngine.AddressableAssets;

namespace Tooling.StaticData.Data
{
    public class Card : StaticData
    {
        public AssetReferenceSprite Art;
        public List<Mana>           Manas;
        public PlayerClass          PlayerClass;
        public Card                 Upgrade;
        public Card                 Downgrade;
        public LocKey               Description;
        public bool                 IsLostOnPlay;

        [ListSizeValidator(minCount: 1)]
        public List<Targeting.Options> TargetingOptions;

        [IsAssignableFrom(typeof(CardLogic))]
        public Type CardLogic;
    }
}