using System.Collections.Generic;
using Controllers.Strategies;
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
        [SerializeReference, Utils.Attributes.DisplayAbstract(typeof(ISelectMoveStrategy))] ISelectMoveStrategy selectMoveStrategy;
        [SerializeField] private List<EnemyMove> moves;

        public AssetReferenceSprite AvatarReference => avatarReference;
        public HealthDefinition HealthDefinition => healthDefinition;
        public ISelectMoveStrategy SelectMoveStrategy => selectMoveStrategy;
        public List<EnemyMove> Moves => moves;
        public EnemyModel Representation => new(this);
    }

    public class EnemyModel
    {
        [JsonProperty("name")]
        public readonly string Name;

        [JsonProperty("health_definition")]
        public readonly HealthDefinition HealthDefinition;

        [JsonProperty("avatar_reference")]
        public readonly AssetReferenceSprite AvatarReference;

        [JsonProperty("select_move_strategy")]
        public readonly ISelectMoveStrategy SelectMoveStrategy;

        [JsonProperty("moves")]
        public readonly List<EnemyMove> Moves;

        [JsonConstructor]
        public EnemyModel()
        {

        }

        public EnemyModel(EnemySODefinition enemySODefinition)
        {
            Name = enemySODefinition.name;
            HealthDefinition = enemySODefinition.HealthDefinition;
            AvatarReference = enemySODefinition.AvatarReference;
            SelectMoveStrategy = enemySODefinition.SelectMoveStrategy;
            Moves = enemySODefinition.Moves;
        }
    }
}