using System.Globalization;
using Fight;
using Fight.Engine;
using TMPro;
using UnityEngine;
using Zenject;

namespace Views
{
    public class HealthView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer borderRenderer;
        [SerializeField] private SpriteRenderer healthFillRenderer;
        [SerializeField] private TextMeshPro    healthAmountText;
        [SerializeField] private Transform      viewParent;

        private BattleEngine battleEngine;

        [Inject]
        private void Construct(ICombatParticipant combatParticipant, BattleEngine battleEngine)
        {
            // TODO: Health stat
            //this.health = combatParticipant.Health;
            this.battleEngine = battleEngine;

            UpdateHealthDisplay(FightUtils.GetHealth(combatParticipant), FightUtils.GetMaxHealth(combatParticipant));
        }

        /// <summary>
        /// This changes the scale of the healthBar with the assumption the initial scale is 1.
        /// </summary>
        private void UpdateHealthDisplay(float? health, float? maxHealth)
        {
            bool hasHealth     = health.HasValue;
            bool hasMaxHealth  = maxHealth.HasValue;
            bool showHealthBar = hasHealth || hasMaxHealth;
            
            viewParent.gameObject.SetActive(showHealthBar);
            if (!showHealthBar)
            {
                return;
            }

            float  healthPercent;
            string healthBarText;
            if (hasHealth && !hasMaxHealth)
            {
                // Participant with no max hp should show full health bar
                healthPercent = 1f;
                healthBarText = health.ToString();
            }
            else if (!hasHealth && hasMaxHealth)
            {
                // Participant with max health but no health should be dead...
                healthPercent = 0;
                healthBarText = 0f.ToString(CultureInfo.CurrentCulture);
            }
            else
            {
                // Otherwise show normal percentage
                healthPercent = health.Value / maxHealth.Value;
                healthBarText = $"{health.Value}/{maxHealth.Value}";
            }

            healthAmountText.SetText(healthBarText);

            // We need to convert the bounds size from world to local space since 
            // we are moving the fill of the healthBar in local space
            var boundsSizeBefore = transform.InverseTransformVector(healthFillRenderer.bounds.size).x;
            var scaleBefore      = healthFillRenderer.transform.localScale;
            healthFillRenderer.transform.localScale = new Vector3(
                healthPercent,
                scaleBefore.y,
                scaleBefore.z
            );

            var boundsAfter    = transform.InverseTransformVector(healthFillRenderer.bounds.size).x;
            var positionBefore = healthFillRenderer.transform.localPosition;
            healthFillRenderer.transform.localPosition = new Vector3(
                positionBefore.x - (boundsSizeBefore - boundsAfter) / 2f,
                positionBefore.y,
                positionBefore.z
            );
        }
    }
}