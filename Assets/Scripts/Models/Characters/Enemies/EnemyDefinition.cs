using System.Collections.Generic;
using Models.Health;
using UnityEngine;

namespace Models.Characters
{
    [CreateAssetMenu(fileName = nameof(EnemyDefinition), menuName = "RevolvingDawn/Enemies/" + nameof(EnemyDefinition))]
    public class EnemyDefinition : ScriptableObject
    {
        [SerializeField] private new string name;
        [SerializeField] private HealthDefinition healthDefinition;
        [SerializeField] private Sprite enemySprite;
        [SerializeField] private List<EnemyMove> enemyMoves;

        public string Name => name;
        public Sprite EnemySprite => enemySprite;
        public HealthDefinition HealthDefinition => healthDefinition;
        public List<EnemyMove> EnemyMoves => enemyMoves;
    }
}