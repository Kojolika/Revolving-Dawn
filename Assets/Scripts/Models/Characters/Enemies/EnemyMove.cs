using System.Collections.Generic;
using Models.CardEffects;
using UnityEngine;
using Utils.Attributes;

namespace Models.Characters
{
    [System.Serializable]
    public class EnemyMove
    {
        [SerializeField] private Sprite moveIntentSprite;
        [SerializeReference, DisplayAbstract(typeof(ICombatEffect))] private List<ICombatEffect> enemyMoves;

        public Sprite MoveIntentSprite => moveIntentSprite;
        public List<ICombatEffect> EnemyMoves => enemyMoves;
    }
}