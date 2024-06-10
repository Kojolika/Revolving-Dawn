using System;
using System.Collections.Generic;
using Models.Characters;
using Models.Map;
using UnityEngine;
using Utils.Attributes;

namespace Settings
{
    [CreateAssetMenu(fileName = "New " + nameof(MapSettings), menuName = "RevolvingDawn/Settings/" + nameof(MapSettings))]
    public class MapSettings : ScriptableObject
    {
        [Tooltip("The number of nodes to generate for the map. Each node will be an enemy,event,shop,etc. that the player can travel to")]
        [SerializeField]
        private int numberOfNodes;

        [Tooltip("The number of paths to create from the start node to the end node (boss)")]
        [SerializeField]
        private int numberOfPaths;

        [Tooltip("The width of the map")]
        [SerializeField]
        private int xDimension;

        [Tooltip("The height of the map")]
        [SerializeField]
        private int yDimension;

        [Tooltip("The amount of padding between the edge of the map and where nodes can be generated")]
        [SerializeField]
        private int edgePadding;

        [Tooltip("The amount of padding inside each region of the map that a node can be generated. The greater the padding the more spread apart the nodes are")]
        [SerializeField]
        private int regionPadding;

        [Tooltip("The event for which the first node on the map will be.")]
        [SerializeField]
        private NodeEventSODefinition firstNodeEvent;

        [Tooltip("The event for which the last node on the map will be.")]
        [SerializeField]
        private NodeEventSODefinition finalNodeEvent;

        [Tooltip("Event types, and their weights of appearing on the map.")]
        [SerializeField]
        private List<EventSettings> eventSettings;

        [Tooltip("Type of possible enemies.")]
        [SerializeField]
        private List<EnemySpawnSettings> enemySpawnSettings;

        [Tooltip("Type of possible elites.")]
        [SerializeField]
        private List<EnemySpawnSettings> eliteSpawnSettings;

        [Tooltip("Type of possible possible bosses.")]
        [SerializeField]
        private List<EnemySpawnSettings> bossSpawnSettings;

        [Tooltip("The higher this value, the more enemies will spawn at higher levels.")]
        [SerializeField]
        [Range(.001f, 1)]
        private float enemyDifficultyMultiplier;

        [Tooltip("The higher this value, the more health enemies will have at higher levels.")]
        [SerializeField]
        [Range(1, float.MaxValue)]
        private float enemyHealthMultiplier;

        public int NumberOfNodes => numberOfNodes;
        public int NumberOfPaths => numberOfPaths;
        public int XDimension => xDimension;
        public int YDimension => yDimension;
        public int EdgePadding => edgePadding;
        public int RegionPadding => regionPadding;
        public NodeEventSODefinition FirstNodeEvent => firstNodeEvent;
        public NodeEventSODefinition FinalNodeEvent => finalNodeEvent;
        public List<EventSettings> EventSettings => eventSettings;
        public List<EnemySpawnSettings> EnemySpawnSettings => enemySpawnSettings;
        public List<EnemySpawnSettings> EliteSpawnSettings => eliteSpawnSettings;
        public List<EnemySpawnSettings> BossSpawnSettings => bossSpawnSettings;
        public float EnemyDifficultyMultiplier => enemyDifficultyMultiplier;
        public float EnemyHealthMultiplier => enemyHealthMultiplier;
    }

    [Serializable]
    public class EventSettings
    {
        [SerializeField]
        private float weight;

        [SerializeField]
        private NodeEventSODefinition nodeEventDefinition;

        public float Weight => weight;
        public NodeEventSODefinition NodeEventDefinition => nodeEventDefinition;
    }

    [Serializable]
    public class EnemySpawnSettings
    {
        [SerializeField]
        private EnemySODefinition enemy;

        [Tooltip("This range is a percentage of the map in which they will spawn.")]
        [SerializeField]
        [Range(0, 1)]
        private float minSpawnRange;

        [Tooltip("This range is a percentage of the map in which they will spawn.")]
        [SerializeField]
        [Range(0, 1)]
        private float maxSpawnRange;

        [SerializeField]
        [Range(1, int.MaxValue)]
        private int enemyDifficultyRating;

        public EnemySODefinition Enemy => enemy;
        public float MinSpawnRange => minSpawnRange;
        public float MaxSpawnRange => maxSpawnRange;
        public int EnemyDifficultyRating => enemyDifficultyRating;
    }

    [Serializable]
    public struct NumberSettings<T> where T : struct
    {
        public enum TypeOfNumberSetting
        {
            Number,
            Min,
            Max,
        }

        [SerializeField]
        private TypeOfNumberSetting settingsType;

        [SerializeField]
        private T value;

        public readonly TypeOfNumberSetting SettingsType => settingsType;
        public readonly T Value => value;
    }


}