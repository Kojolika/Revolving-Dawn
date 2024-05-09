using Models.Characters;
using UnityEngine;
using Utils.Attributes;

namespace Models.Map
{
    [System.Serializable]
    public class EnemyQuantity
    {
        [SerializeReference, DisplayInterface(typeof(IEnemy))]
        public IEnemy Enemy;

        [Header("Below range specifies the number of enemies that can spawn.")]
        [Header(" If the amount varies select a range. With min <= max")]
        [Header("Otherwise if you always want to spawn x number of enemies. Set both to x")]

        [Tooltip("The min number of enemies that can spawn of this type")]
        [Range(1, 10)]
        public int MinRange;

        [Tooltip("The max number of enemies that can spawn of this type")]
        [Range(1, 10)]
        public int MaxRange;
    }
}