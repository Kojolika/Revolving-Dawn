using UnityEngine;

namespace Settings
{
    [CreateAssetMenu(fileName = "New " + nameof(MapSettings), menuName = "RevolvingDawn/Settings/" + nameof(MapSettings))]
    public class MapSettings : ScriptableObject
    {
        [Tooltip("The number of nodes to generate for the map. Each node will be an enemy,event,shop,etc. that the player can travel to")]
        [SerializeField] private int numberOfNodes;
        [Tooltip("The number of paths to create from the start node to the end node (boss)")]
        [SerializeField] private int numberOfPaths;
        [Tooltip("The width of the map")]
        [SerializeField] private int xDimension;
        [Tooltip("The height of the map")]
        [SerializeField] private int yDimension;
        [Tooltip("The amount of padding between the edge of the map and where nodes can be generated")]
        [SerializeField] private int edgePadding;
        [Tooltip("The amount of padding inside each region of the map that a node can be generated. The greater the padding the more spread apart the nodes are")]
        [SerializeField] private int regionPadding;

        public int NumberOfNodes => numberOfNodes;
        public int NumberOfPaths => numberOfPaths;
        public int XDimension => xDimension;
        public int YDimension => yDimension;
        public int EdgePadding => edgePadding;
        public int RegionPadding => regionPadding;
    }
}