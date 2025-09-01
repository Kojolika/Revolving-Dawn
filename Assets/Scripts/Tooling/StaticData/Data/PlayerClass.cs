using System.Collections.Generic;
using Tooling.StaticData.EditorUI.Validation;
using UnityEngine.AddressableAssets;

namespace Tooling.StaticData.EditorUI
{
    public class PlayerClass : StaticData
    {
        public AssetReferenceSprite ClassArt;
        public AssetReferenceSprite CardBorderArt;
        public List<Card> StartingDeck;
        public LocKey Description;
    }
}