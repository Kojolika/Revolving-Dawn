using System.Collections.Generic;
using Fight.Engine.Bytecode;
using Tooling.StaticData.Validation;
using UnityEngine.AddressableAssets;

namespace Tooling.StaticData
{
    public class Card : StaticData
    {
        public AssetReferenceSprite Art;
        public PlayerClass PlayerClass;
        public Card Upgrade;
        public Card Downgrade;
        public bool IsLostOnPlay;
        public List<IInstruction> PlayEffect;
    }
}