using System.Collections.Generic;
using Models.CardEffects;
using UnityEngine;

namespace Models.Characters
{
    [System.Serializable]
    public class EnemyMove
    {
        [SerializeField] private Sprite moveIntentSprite;
        [SerializeField] private List<CombatEffectWrapper> enemyMoves;

        public Sprite MoveIntentSprite => moveIntentSprite;
        public List<CombatEffectWrapper> EnemyMoves => enemyMoves;
    }
}