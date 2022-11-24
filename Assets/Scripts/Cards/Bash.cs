using UnityEngine;
using UnityEngine.UI;
using characters;
using TMPro;
using System.Collections.Generic;

namespace cards{
    public class Bash : Card
    {
        [SerializeField] CardScriptableObject cardSO;
        [SerializeField] TextMeshPro nameText;
        [SerializeField] TextMeshPro description;
        [SerializeField] GameObject artwork;
        [SerializeField] GameObject border;


        [SerializeField] Targeting target = Targeting.Enemy;
        bool manaCharged = false;

        float damage = 6;

        public override void Play(List<Character> targets)
        {
            if (IsManaCharged())
            {
                //Play powerful effect
            }
            else
            {
                //Play regular effect
                foreach(var target in targets)
                {
                    target.health.DealDamage(damage);
                }
            }
        }
        public override int GetTarget()
        {
            if (IsManaCharged())
            {
                //Potentially have powerful cards change targetting
                //if so do target = ...;
                return (int)target;
            }
            return (int)target;
        }

        public override bool IsManaCharged()
        {
            return manaCharged;
        }


        public override void LoadInfo(CardScriptableObject cardSO){
            artwork.GetComponent<MeshRenderer>().material = cardSO.artwork;
            border.GetComponent<MeshRenderer>().material = cardSO.border;

            nameText.text = cardSO.name;
            description.text = cardSO.description;
            nameText.font = CardInfo.DEFAULT_FONT;
            description.font = CardInfo.DEFAULT_FONT;
        }
        void Start() {
            LoadInfo(cardSO);
        }
    }
}
