using System.Collections.Generic;
using Controllers.Strategies;
using Newtonsoft.Json;
using Serialization;
using Tooling.StaticData.EditorUI;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Models.Characters
{
    [CreateAssetMenu(fileName = nameof(EnemySODefinition), menuName = "RevolvingDawn/Enemies/" + nameof(EnemySODefinition))]
    public class EnemySODefinition : ScriptableObject, IHaveSerializableRepresentation<EnemyModel>
    {
        [SerializeField] private HealthDefinition     healthDefinition;
        [SerializeField] private AssetReferenceSprite avatarReference;

        [SerializeReference, Utils.Attributes.DisplayAbstract(typeof(ISelectMoveStrategy))]
        ISelectMoveStrategy selectMoveStrategy;

        [SerializeField] private List<EnemyMove> moves;

        public AssetReferenceSprite AvatarReference    => avatarReference;
        public HealthDefinition     HealthDefinition   => healthDefinition;
        public ISelectMoveStrategy  SelectMoveStrategy => selectMoveStrategy;
        public List<EnemyMove>      Moves              => moves;
        public EnemyModel           Representation     => new(this);
    }

    public class EnemyModel
    {
        public readonly string Name;

        public readonly HealthDefinition HealthDefinition;

        public readonly AssetReferenceSprite AvatarReference;

        public readonly ISelectMoveStrategy SelectMoveStrategy;
        public readonly List<EnemyMove>     Moves;

        [JsonConstructor]
        public EnemyModel()
        {
        }

        public EnemyModel(EnemySODefinition enemySODefinition)
        {
            Name               = enemySODefinition.name;
            HealthDefinition   = enemySODefinition.HealthDefinition;
            AvatarReference    = enemySODefinition.AvatarReference;
            SelectMoveStrategy = enemySODefinition.SelectMoveStrategy;
            Moves              = enemySODefinition.Moves;
        }
    }
}