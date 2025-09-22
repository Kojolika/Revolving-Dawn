using System.Collections.Generic;
using Tooling.StaticData.Validation;
using UnityEngine.AddressableAssets;

namespace Tooling.StaticData.Data
{
    public class PlayerClass : StaticData
    {
        public AssetReferenceSprite ClassArt;
        public AssetReferenceSprite CardBorderArt;

        [ListSizeValidator(11, 11)]
        public List<Card> StartingDeck;

        public LocKey Description;
    }
}