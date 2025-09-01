using Models.Map;
using UnityEngine.AddressableAssets;

namespace Tooling.StaticData.EditorUI
{
    /// <summary>
    /// Represents a vertex on that map that is interactable by the player.
    /// </summary>
    public class NodeEvent : StaticData
    {
        public AssetReferenceSprite Icon;
        public NodeEventLogic       Logic;
    }
}