using System;
using System.Collections.Generic;
using Characters;
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

        [Tooltip("The event for which the first node on the map will be.")]
        [SerializeReference, DisplayInterface(typeof(INodeEvent))]
        private INodeEvent firstNodeEvent;

        [Tooltip("The event for which the last node on the map will be.")]
        [SerializeReference, DisplayInterface(typeof(INodeEvent))]
        private INodeEvent finalNodeEvent;

        [Tooltip("Event types, and their weights of appearing on the map.")]
        [SerializeField]
        private List<EventWeight> eventWeights;

        [Tooltip("Type of possible enemies.")]
        [SerializeField]
        private List<EnemySpawnSettings> enemys;

        [Tooltip("Type of possible elites.")]
        [SerializeField]
        private List<EnemySpawnSettings> elites;

        [Tooltip("Type of possible possible bosses.")]
        [SerializeField]
        private List<EnemySpawnSettings> bosses;

        public int NumberOfNodes => numberOfNodes;
        public int NumberOfPaths => numberOfPaths;
        public int XDimension => xDimension;
        public int YDimension => yDimension;
        public int EdgePadding => edgePadding;
        public int RegionPadding => regionPadding;
        public INodeEvent FirstNodeEvent => firstNodeEvent;
        public INodeEvent FinalNodeEvent => finalNodeEvent;
        public List<EventWeight> EventWeights => eventWeights;
        public List<EnemySpawnSettings> Enemys => enemys;
        public List<EnemySpawnSettings> Elites => elites;
        public List<EnemySpawnSettings> Bosses => bosses;
    }

    [Serializable]
    public class EventWeight
    {
        [SerializeField] private float weight;
        [SerializeReference, DisplayInterface(typeof(INodeEvent))] private NodeEvent nodeEvent;

        public float Weight => weight;
        public NodeEvent NodeEvent => nodeEvent;
    }

    [Serializable]
    public class EnemySpawnSettings
    {
        [SerializeField]
        private EnemySODefinition enemy;

        [SerializeField]
        private NumberSettings<int> minSpawnRange;

        [SerializeField]
        private NumberSettings<int> maxSpawnRange;

        [SerializeField]
        [Range(1, 10)]
        private int enemyDifficultyRating;


        public EnemySODefinition Enemy => enemy;
        public NumberSettings<int> MinSpawnRange => minSpawnRange;
        public NumberSettings<int> MaxSpawnRange => maxSpawnRange;
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