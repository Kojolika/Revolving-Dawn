using System.Runtime.InteropServices;
using Fight;
using Fight.Engine;
using Models;
using Models.Cards;
using Models.Characters;
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

        [Inject]
        private void Construct(ICombatParticipant combatParticipant, BattleEngine battleEngine)
        {
            // TODO: Health stat
            //this.health = combatParticipant.Health;
            this.battleEngine = battleEngine;

            UpdateHealthDisplay();
            health.HealthUpdated += OnHealthUpdated;
        }

        public void PreviewCardEffects(CardLogic cardLogic)
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
    }
}