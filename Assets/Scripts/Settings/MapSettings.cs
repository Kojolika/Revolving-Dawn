using UnityEngine;
using Utils;

namespace Settings
{
    [CreateAssetMenu(fileName = "New Map Settings", menuName = "RevolvingDawn/Settings/Map")]
    public class MapSettings : ScriptableObject
    {
        [SerializeField] private int numberOfLevels;
        [SerializeField] private int numberOfNodes;
        [SerializeField] private int numberOfPaths;
        [SerializeField] private int xDimension;
        [SerializeField] private int yDimension;
        [SerializeField] private int edgePadding;


        public int NumberOfLevels => numberOfLevels;
        public int NumberOfNodes => numberOfNodes;
        public int NumberOfPaths => numberOfPaths;
        public int XDimension => xDimension;
        public int YDimension => yDimension;
        public int EdgePadding => edgePadding;
    }
}