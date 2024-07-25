using System.Runtime.InteropServices;
using Fight;
using Models;
using Systems;
using TMPro;
using Tooling.Logging;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using Utils;
using Zenject;

namespace Views
{
    public class HealthView : MonoBehaviour
    {
        [SerializeField] SpriteRenderer borderRenderer;
        [SerializeField] SpriteRenderer healthFillRenderer;
        [SerializeField] TextMeshPro healthAmountText;

        private Health health;
        private BattleEngine battleEngine;
        private ICharacterView characterView;

        [Inject]
        private void Construct(ICharacterView characterView, BattleEngine battleEngine)
        {
            this.health = characterView.CharacterModel.Health;
            this.battleEngine = battleEngine;
            this.characterView = characterView;

            transform.SetParent(characterView.transform);
            var localBoundsSizeY = characterView.Renderer.localBounds.size.y;
            var buffer = localBoundsSizeY * .1f;
            transform.localPosition = new Vector3(0, localBoundsSizeY + buffer, 0);

            UpdateHealthDisplay();
            health.HealthUpdated += OnHealthUpdated;
        }

        public void PreviewCardEffects(CardModel cardModel)
        {

        }

        private void OnHealthUpdated(ulong amount) => UpdateHealthDisplay();

        /// <summary>
        /// This changes the scale of the healthBar with the assumption the initial scale is 1.
        /// </summary>
        private void UpdateHealthDisplay()
        {
            float healthPercent = (float)health.CurrentHealth / health.MaxHealth;
            healthAmountText.SetText($"{health.CurrentHealth}/{health.MaxHealth}");

            // We need to convert the bounds size from world to local space since 
            // we are moving the fill of the healthBar in local space
            var boundsSizeBefore = transform.InverseTransformVector(healthFillRenderer.bounds.size).x;
            var scaleBefore = healthFillRenderer.transform.localScale;
            healthFillRenderer.transform.localScale = new Vector3(
                healthPercent,
                scaleBefore.y,
                scaleBefore.z
            );

            var boundsAfter = transform.InverseTransformVector(healthFillRenderer.bounds.size).x;
            var positionBefore = healthFillRenderer.transform.localPosition;
            healthFillRenderer.transform.localPosition = new Vector3(
                positionBefore.x - (boundsSizeBefore - boundsAfter) / 2f,
                positionBefore.y,
                positionBefore.z
            );
        }

        private void OnDestroy()
        {
            health.HealthUpdated -= OnHealthUpdated;
        }

        public class Factory : PlaceholderFactory<ICharacterView, HealthView> { }

        public class CustomFactory : IFactory<ICharacterView, HealthView>
        {
            private readonly HealthView healthViewPrefab;
            private readonly DiContainer diContainer;
            public CustomFactory(HealthView healthViewPrefab, DiContainer diContainer)
            {
                this.healthViewPrefab = healthViewPrefab;
                this.diContainer = diContainer;
            }

            public HealthView Create(ICharacterView characterView)
            {
                var newHealthView = Instantiate(healthViewPrefab, characterView.transform);
                diContainer.Inject(newHealthView, new ICharacterView[] { characterView });

                return newHealthView;
            }
        }
    }
}