using System.Collections.Generic;
using Models.CardEffects;
using Models.Health;
using UnityEngine;
using Utils.Attributes;

namespace Models.Characters
{
    [CreateAssetMenu(fileName = nameof(EnemyDefinition), menuName = "RevolvingDawn/Enemies/" + nameof(EnemyDefinition))]
    public class EnemyDefinition : ScriptableObject
    {
        [SerializeField] private HealthDefinition healthDefinition;
        [SerializeField] private List<EnemyMove> enemyMoves;

        [SerializeReference, DisplayInterface(typeof(ICombatEffect))] private List<ICombatEffect> test;
        

        public List<EnemyMove> EnemyMoves => enemyMoves;
        public HealthDefinition HealthDefinition => healthDefinition;
    }
}