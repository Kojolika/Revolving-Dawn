using System.Collections.Generic;
using UnityEngine.AddressableAssets;

namespace Tooling.StaticData.EditorUI
{
    public class Enemy : StaticData
    {
        public AssetReferenceSprite Image;
        public List<EnemyMove>      PossibleMoves;
        public List<StatAmount>     Stats;
    }

    public class StatAmount
    {
        public Stat  Stat;
        public float Amount;
    }
}