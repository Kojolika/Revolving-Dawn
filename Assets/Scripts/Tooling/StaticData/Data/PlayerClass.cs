using System.Collections.Generic;
using UnityEngine.AddressableAssets;

namespace Tooling.StaticData
{
    public class PlayerClass : StaticData
    {
        public AssetReferenceSprite ClassArt;
        public AssetReferenceSprite CardBorderArt;
        public List<Card> StartingDeck;
        public LocKey Description;
    }
}