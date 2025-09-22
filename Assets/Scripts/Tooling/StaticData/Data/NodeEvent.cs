using System;
using Models.Map;
using Tooling.StaticData.Validation;
using UnityEngine.AddressableAssets;

namespace Tooling.StaticData.Data
{
    /// <summary>
    /// Represents a vertex on that map that is interactable by the player.
    /// </summary>
    public class NodeEvent : StaticData
    {
        public AssetReferenceSprite Icon;

        [IsAssignableFrom(typeof(NodeEventLogic))]
        public Type Logic;
    }
}