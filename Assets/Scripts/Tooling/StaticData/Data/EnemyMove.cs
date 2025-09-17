using System.Collections.Generic;
using Models.Cards;
using UnityEngine.AddressableAssets;

namespace Tooling.StaticData.Data
{
    public class EnemyMove : StaticData
    {
        public AssetReferenceSprite    MoveIntentSprite;
        public List<Targeting.Options> TargetingOptions;
    }
}