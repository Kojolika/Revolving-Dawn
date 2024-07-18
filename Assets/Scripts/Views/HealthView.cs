using Fight;
using Models;
using TMPro;
using Tooling.Logging;
using UnityEngine;
using UnityEngine.PlayerLoop;
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
        private float healthFillStartingPointX;
        private float healthFillSizeX;

        [Inject]
        private void Construct(ICharacterView characterView, BattleEngine battleEngine)
        {
            this.health = characterView.Character.Health;
            this.battleEngine = battleEngine;
            this.characterView = characterView;
            this.healthFillStartingPointX = transform.position.x - healthFillRenderer.bounds.extents.x;
            this.healthFillSizeX = healthFillRenderer.bounds.size.x;

            UpdateHealthDisplay();
            health.HealthUpdated += OnHealthUpdated;
        }

        public void PreviewCardEffects(CardModel cardModel)
        {

        }

        private void OnHealthUpdated(ulong amount) => UpdateHealthDisplay();
        private void UpdateHealthDisplay()
        {
            float healthPercent = (float)health.CurrentHealth / health.MaxHealth;
            healthAmountText.SetText($"{health.CurrentHealth}/{health.MaxHealth}");
            healthFillRenderer.transform.localScale = new Vector3(
                healthPercent,
                healthFillRenderer.transform.localScale.y,
                healthFillRenderer.transform.localScale.z
            );

            /*             var normalizedFillPosition = Computations.Normalize(healthPercent, 0, 1, healthFillStartingPointX, healthFillSizeX);
                        healthFillRenderer.transform.position = new Vector3(
                            normalizedFillPosition,
                            healthFillRenderer.transform.position.y,
                            healthFillRenderer.transform.position.z
                        ); */
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
                var characterViewTransform = characterView.transform;
                var newHealthView = Instantiate(healthViewPrefab, characterViewTransform);
                newHealthView.transform.position = characterView.HealthViewLocation.position;
                diContainer.Inject(newHealthView, new object[] { characterView });

                return newHealthView;
            }
        }
    }
}