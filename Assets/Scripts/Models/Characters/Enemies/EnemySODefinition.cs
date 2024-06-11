using System.Collections.Generic;
using Newtonsoft.Json;
using Serialization;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Models.Characters
{
    [CreateAssetMenu(fileName = nameof(EnemySODefinition), menuName = "RevolvingDawn/Enemies/" + nameof(EnemySODefinition))]
    public class EnemySODefinition : ScriptableObject, IHaveSerializableRepresentation<EnemyModel>
    {
        [SerializeField] private HealthDefinition healthDefinition;
        [SerializeField] private AssetReferenceSprite avatarReference;
        [SerializeField] private List<EnemyMove> enemyMoves;

        public AssetReferenceSprite AvatarReference => avatarReference;
        public HealthDefinition HealthDefinition => healthDefinition;
        public List<EnemyMove> EnemyMoves => enemyMoves;


        private EnemyModel representation;
        public EnemyModel Representation
        {
            get
            {
                representation ??= new EnemyModel(this);
                return representation;
            }
            private set => representation = value;
        }
    }

    public class EnemyModel
    {
        [JsonProperty("name")]
        public readonly string Name;

        [JsonProperty("health_definition")]
        public readonly HealthDefinition HealthDefinition;

        [JsonProperty("avatar_reference")]
        public readonly AssetReferenceSprite AvatarReference;

        [JsonProperty("enemy_moves")]
        public readonly List<EnemyMove> EnemyMoves;

        [JsonConstructor]
        public EnemyModel()
        {

        }

        public EnemyModel(EnemySODefinition enemySODefinition)
        {
            Name = enemySODefinition.name;
            HealthDefinition = enemySODefinition.HealthDefinition;
            AvatarReference = enemySODefinition.AvatarReference;
            EnemyMoves = enemySODefinition.EnemyMoves;
        }
    }
}