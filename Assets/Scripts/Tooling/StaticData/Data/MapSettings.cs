using System.Collections.Generic;
using Tooling.StaticData.EditorUI;
using UnityEngine;

namespace Tooling.StaticData.Data
{
    public class MapSettings : EditorUI.StaticData
    {
        [Tooltip("The number of nodes to generate for the map. Each node will be an enemy,event,shop,etc. that the player can travel to")]
        [SerializeField]
        public int NumberOfNodes;

        [Tooltip("The number of paths to create from the start node to the end node (boss)")]
        [SerializeField]
        public int NumberOfPaths;

        [Tooltip("The width of the map")]
        [SerializeField]
        public int XDimension;

        [Tooltip("The height of the map")]
        [SerializeField]
        public int YDimension;

        [Tooltip("The amount of padding between the edge of the map and where nodes can be generated")]
        [SerializeField]
        public int EdgePadding;

        [Tooltip(
            "The amount of padding inside each region of the map that a node can be generated. The greater the padding the more spread apart the nodes are")]
        [SerializeField]
        public int RegionPadding;

        [Tooltip("The event for which the first node on the map will be.")]
        [SerializeField]
        public NodeEvent FirstNodeEvent;

        [Tooltip("The event for which the last node on the map will be.")]
        [SerializeField]
        public NodeEvent FinalNodeEvent;

        [Tooltip("Event types, and their weights of appearing on the map.")]
        [SerializeField]
        public List<EventSettings> EventSettings;

        [Tooltip("Type of possible enemies.")]
        [SerializeField]
        public List<EnemySpawnSettings> EnemySpawnSettings;

        [Tooltip("Type of possible elites.")]
        [SerializeField]
        public List<EnemySpawnSettings> EliteSpawnSettings;

        [Tooltip("Type of possible possible bosses.")]
        [SerializeField]
        public List<EnemySpawnSettings> BossSpawnSettings;

        [Tooltip("The higher this value, the more enemies will spawn at higher levels.")]
        [SerializeField]
        [Range(.001f, 1)]
        public float EnemyDifficultyMultiplier;

        [Tooltip("The higher this value, the more health enemies will have at higher levels.")]
        [SerializeField]
        [Range(1, float.MaxValue)]
        public float EnemyHealthMultiplier;
    }

    public class EventSettings
    {
        public float     Weight;
        public NodeEvent NodeEvent;
    }

    public class EnemySpawnSettings
    {
        public Enemy Enemy;

        [Tooltip("This range is a percentage of the map in which they will spawn.")]
        [Range(0, 1)]
        public float MinSpawnRange;

        [Tooltip("This range is a percentage of the map in which they will spawn.")]
        [Range(0, 1)]
        public float MaxSpawnRange;

        [Range(1, int.MaxValue)]
        public int EnemyDifficultyRating;
    }
}