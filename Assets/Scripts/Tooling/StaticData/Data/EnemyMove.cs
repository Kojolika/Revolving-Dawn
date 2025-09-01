using System.Collections.Generic;
using Models.Cards;
using UnityEngine.AddressableAssets;

namespace Tooling.StaticData.EditorUI
{
    public class EnemyMove : StaticData
    {
        public AssetReferenceSprite    MoveIntentSprite;
        public List<Targeting.Options> TargetingOptions;
    }
}