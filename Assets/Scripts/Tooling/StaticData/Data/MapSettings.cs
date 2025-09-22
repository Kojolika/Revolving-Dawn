using System.Collections.Generic;
using Tooling.StaticData.Validation;
using UnityEngine;

// ReSharper disable UnassignedField.Global

namespace Tooling.StaticData.Data
{
    public class MapSettings : StaticData
    {
        [Tooltip("The number of levels to generate on a map. Each level can have multiple nodes.")]
        public int NumberOfLevels;

        [Tooltip("Range of nodes that can be on a level")]
        [Required]
        public Range NodesPerLevel;

        [Tooltip("The number of paths to create from the start node to the end node (boss)")]
        public int NumberOfPaths;

        [Tooltip("The event for which the first node on the map will be.")]
        [Required]
        public NodeEvent FirstNodeEvent;

        [Tooltip("The event for which the last node on the map will be.")]
        [Required]
        public NodeEvent FinalNodeEvent;

        [Tooltip("Event types, and their weights of appearing on the map.")]
        [Required]
        public List<EventSettings> EventSettings;

        [Tooltip("Type of possible enemies.")] public List<EnemySpawnSettings> EnemySpawnSettings;

        [Tooltip("Type of possible elites.")] public List<EnemySpawnSettings> EliteSpawnSettings;

        [Tooltip("Type of possible possible bosses.")]
        public List<EnemySpawnSettings> BossSpawnSettings;

        [Tooltip("The higher this value, the more enemies will spawn at higher levels.")] [Range(.001f, 1)]
        public float EnemyDifficultyMultiplier;

        [Tooltip("The higher this value, the more health enemies will have at higher levels.")] [Range(1, float.MaxValue)]
        public float EnemyHealthMultiplier;
    }

    public class EventSettings
    {
        public float     Weight;
        public NodeEvent NodeEvent;
    }

    public class Range
    {
        public int Min;
        public int Max;
    }

    public class EnemySpawnSettings
    {
        public Enemy Enemy;

        [Tooltip("This range is a percentage of the map in which they will spawn.")] [Range(0, 1)]
        public float MinSpawnRange;

        [Tooltip("This range is a percentage of the map in which they will spawn.")] [Range(0, 1)]
        public float MaxSpawnRange;

        [Range(1, int.MaxValue)] public int EnemyDifficultyRating;
    }
}