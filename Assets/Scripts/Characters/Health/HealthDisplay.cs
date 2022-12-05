using UnityEngine;
using TMPro;

namespace characters
{
    public class HealthDisplay : MonoBehaviour
    {
        [SerializeField] TextMeshPro healthText;
        [SerializeField] float healthPercent;
        public HealthSystem health;

        public void UpdateHealth()
        {
            healthText.text = health.GetHealthValue() + "/" + health.GetMaxHealthValue();
            healthPercent = health.GetHealthValue() /health.GetMaxHealthValue();
            if(healthPercent < 0) healthPercent = 0;
            
            this.gameObject.GetComponentInChildren<HealthBarInside>().gameObject.transform.localScale = new Vector3(
                healthPercent,
                this.gameObject.GetComponentInChildren<HealthBarInside>().gameObject.transform.localScale.y,
                this.gameObject.GetComponentInChildren<HealthBarInside>().gameObject.transform.localScale.z
            );
        }
    }
}
