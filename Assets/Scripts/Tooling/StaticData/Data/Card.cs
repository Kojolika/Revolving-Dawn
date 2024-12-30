using Tooling.StaticData.Validation;

namespace Tooling.StaticData
{
    public class Card : StaticData
    {
        [AddressableAssetKey] public string Art;
        public PlayerClass PlayerClass;
        public Card Upgrade;
        public Card Downgrade;
        public bool IsLostOnPlay;
    }
}