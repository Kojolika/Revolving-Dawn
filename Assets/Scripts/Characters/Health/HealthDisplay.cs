using UnityEngine;
using TMPro;

namespace characters
{
    public class HealthDisplay : MonoBehaviour
    {

        [SerializeField] TextMeshPro healthText;
        public HealthSystem health;

        [SerializeField] float hp;
        [SerializeField] float maxHP;

        public void UpdateHealth()
        {
            Debug.Log("Updated health");
            healthText.text = health.GetHealthValue() + "/" + health.GetMaxHealthValue();
            hp = health.GetHealthValue();
            maxHP = health.GetMaxHealthValue();
        }
    }
}
