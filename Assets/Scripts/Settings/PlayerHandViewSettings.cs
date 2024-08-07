using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Settings
{
    [CreateAssetMenu(fileName = "New " + nameof(PlayerHandViewSettings), menuName = "RevolvingDawn/Settings/" + nameof(PlayerHandViewSettings))]
    public class PlayerHandViewSettings : ScriptableObject
    {
        [Tooltip("Controls the rotation amount of the fan in the card hand.")]
        [SerializeField] float cardHandRotationModifier;

        [Tooltip("Y position as a percentage on viewport where the card is considered in it's targeting phase.")]
        [Range(0f, 1f), SerializeField] float positionOnScreenWhereTargetingStarts;

        [Tooltip("The amount of pieces of the arrow curve when targeting with cards.")]
        [SerializeField] int numberOfArrowPiecesForTargetingArrow;

        [Space(20)]
        [Tooltip("The type of move function cards use when being drawn.")]
        [SerializeField] private PrimeTween.Ease cardDrawMoveFunction;

        [Tooltip("The speed which cards will be moved around in the player hand when drawn or discarded.")]
        [SerializeField] private float cardDrawMoveSpeed;

        [Tooltip("The speed which cards will be rotated around in the player hand when drawn or discarded")]
        [SerializeField] private float cardDrawRotateSpeed;

        [Space(20)]
        [Tooltip("The speed which cards will be moved around in the player hand when hovering a card.")]
        [SerializeField] private float cardHoverMoveSpeedInHand;

        [Tooltip("The speed which cards will be moved around in the player hand when hovering a card.")]
        [SerializeField] private float cardHoverRotateSpeedInHand;

        [Tooltip("The type of move function cards use when a card is being hovered.")]
        [SerializeField] private PrimeTween.Ease cardHoverMoveFunction;

        [Tooltip("How much the card is scaled by when its being hovered over.")]
        [SerializeField] private float cardHoverScaleFactor;

        [Tooltip("Duration of scaling a card after it was the hovering focus")]
        [SerializeField] private float scaleAnimationDuration;

        [Space(20)]
        [Tooltip("Time in seconds for the play animation.")]
        [SerializeField] private float cardPlayAnimationDuration;

        [Tooltip("Play animation ease function.")]
        [SerializeField] private PrimeTween.Ease cardPlayEaseFunction;

        [Space(20)]
        [SerializeField] private AssetReferenceT<Material> enemyOutlineMaterial;
        [SerializeField] private AssetReferenceT<Material> friendlyOutlineMaterial;
        [SerializeField] private AssetReferenceT<Material> defaultSpriteMaterial;

        public float CardHandRotationModifier => cardHandRotationModifier;
        public float PositionOnScreenWhereTargetingStarts => positionOnScreenWhereTargetingStarts;
        public int NumberOfArrowPiecesForTargetingArrow => numberOfArrowPiecesForTargetingArrow;
        public float CardDrawMoveSpeed => cardDrawMoveSpeed;
        public float CardDrawRotateSpeed => cardDrawRotateSpeed;
        public PrimeTween.Ease CardDrawMoveFunction => cardDrawMoveFunction;
        public float CardHoverMoveSpeedInHand => cardHoverMoveSpeedInHand;
        public float CardHoverRotateSpeedInHand => cardHoverRotateSpeedInHand;
        public PrimeTween.Ease CardHoverMoveFunction => cardHoverMoveFunction;
        public float CardHoverScaleFactor => cardHoverScaleFactor;
        public float ScaleAnimationDuration => scaleAnimationDuration;
        public float CardPlayAnimationDuration => cardPlayAnimationDuration;
        public PrimeTween.Ease CardPlayEaseFunction => cardPlayEaseFunction;
        public AssetReferenceT<Material> EnemyOutlineMaterial => enemyOutlineMaterial;
        public AssetReferenceT<Material> FriendlyOutlineMaterial => friendlyOutlineMaterial;
        public AssetReferenceT<Material> DefaultSpriteMaterial => defaultSpriteMaterial;
    }
}