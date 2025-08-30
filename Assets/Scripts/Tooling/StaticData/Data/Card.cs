using System.Collections.Generic;
using UnityEngine.AddressableAssets;

namespace Tooling.StaticData
{
    public class Card : StaticData
    {
        public AssetReferenceSprite                 Art;
        public List<Mana>                           Manas;
        public PlayerClass                          PlayerClass;
        public Card                                 Upgrade;
        public Card                                 Downgrade;
        public LocKey                               Description;
        public bool                                 IsLostOnPlay;
        public List<Models.Cards.Targeting.Options> TargetingOptions;
        public Models.Cards.Card                    CardLogic;
    }
}