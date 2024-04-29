using UnityEngine;
using Utils;

namespace Settings
{
    [CreateAssetMenu(fileName = "New Map Settings", menuName = "RevolvingDawn/Settings/Map")]
    public class MapSettings : ScriptableObject
    {
        public ReadOnly<int> NumberOfLevels;
        public ReadOnly<int> NumberOfNodes;
        public ReadOnly<int> NumberOfPaths;
    }
}