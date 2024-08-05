using System.Collections.Generic;
using Models.CardEffects;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Utils.Attributes;

namespace Models.Characters
{
    [System.Serializable]
    public class EnemyMove
    {
        [SerializeField] private AssetReferenceSprite moveIntentSprite;
        [SerializeReference, DisplayAbstract(typeof(ICombatEffect))] private List<ICombatEffect> moveEffects;

        public AssetReferenceSprite MoveIntentSprite => moveIntentSprite;
        public List<ICombatEffect> MoveEffects => moveEffects;
    }
}