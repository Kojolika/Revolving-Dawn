using System.Collections.Generic;
using Tooling.StaticData.Validation;

namespace Tooling.StaticData
{
    public class PlayerClass : StaticData
    {
        [AddressableAssetKey] public string ClassArt;

        [AddressableAssetKey] public string CardBorderArt;
        public List<Card> StartingDeck;
        public LocKey Description;
    }
}