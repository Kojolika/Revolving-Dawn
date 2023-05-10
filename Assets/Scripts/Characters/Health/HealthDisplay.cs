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
        [SerializeField] internal Character owner;

        void Awake()
        {
            owner = this.transform.parent.GetComponent<Character>();
        }
        public void UpdateHealth()
        {

            healthText.text = health.GetHealthValue() + "/" + health.GetMaxHealthValue();
            healthPercent = health.GetHealthValue() / health.GetMaxHealthValue();
            if (healthPercent < 0) healthPercent = 0;

            HealthBarInside healthBarInside = this.gameObject.GetComponentInChildren<HealthBarInside>();
            healthBarInside.gameObject.transform.localScale = new Vector3(
                healthPercent,
                healthBarInside.gameObject.transform.localScale.y,
                healthBarInside.gameObject.transform.localScale.z
            );

            if (health.GetHealthValue() <= 0)
            {
                //Enemies inherit from Enemy class, which inherits from Character
                if (owner.GetType().IsSubclassOf(typeof(Enemy)))
                { 
                    fight.FightEvents.TriggerEnemyDied((Enemy)owner);
                }
                //Player inherits from Character
                else if(owner.GetType() == typeof(Player))
                {
                    fight.FightEvents.TriggerPlayerDied((Player)owner);
                }
            }
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
