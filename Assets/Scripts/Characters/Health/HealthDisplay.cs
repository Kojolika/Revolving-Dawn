using UnityEngine;
using TMPro;

namespace characters
{
    public class HealthDisplay : MonoBehaviour
    {
        [SerializeField] TextMeshPro healthText;
        [SerializeField] float healthPercent;
        [SerializeField] GameObject blockIcon;
        [SerializeField] TextMeshPro blockText;
        public HealthSystem health;
        public void UpdateHealth()
        {

            healthText.text = health.GetHealthValue() + "/" + health.GetMaxHealthValue();
            healthPercent = health.GetHealthValue() / health.GetMaxHealthValue();
            if (healthPercent < 0) healthPercent = 0;

            this.gameObject.GetComponentInChildren<HealthBarInside>().gameObject.transform.localScale = new Vector3(
                healthPercent,
                this.gameObject.GetComponentInChildren<HealthBarInside>().gameObject.transform.localScale.y,
                this.gameObject.GetComponentInChildren<HealthBarInside>().gameObject.transform.localScale.z
            );
        }

        public void UpdateBlock()
        {
            if (health.GetBlockValue() <= 0f)
            {
                if (blockIcon.activeInHierarchy)
                {
                    blockIcon.SetActive(false);
                }
            }
            else
            {
                if (!blockIcon.activeInHierarchy)
                {
                    blockIcon.SetActive(true);
                }

                blockText.text = "" + health.GetBlockValue();
            }
        }
        void OnDestroy() 
        {
            health.OnDestroy();
        }
    }
}
