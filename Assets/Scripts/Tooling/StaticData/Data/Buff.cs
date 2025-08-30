using System.Collections.Generic;
using Models.Buffs;
using UnityEngine.AddressableAssets;

namespace Tooling.StaticData
{
    public class Buff : StaticData
    {
        /// <summary>
        /// Can we have multiple stacks of this buff?
        /// </summary>
        public bool IsStackable;

        /// <summary>
        /// The max number of stacks possible.
        /// Only used if <see cref="IsStackable"/> is true.
        /// </summary>
        public long MaxStackSize;

        /// <summary>
        /// Is this an internal logic buff?
        ///
        /// <example> Player characters have a buff that draws cards on turn start but is not viewable by the player</example>
        /// </summary>
        public bool IsInternal;

        public AssetReferenceSprite Icon;

        public List<IBeforeEvent> OnBefore;
        public List<IAfterEvent>  OnAfter;
    }
}