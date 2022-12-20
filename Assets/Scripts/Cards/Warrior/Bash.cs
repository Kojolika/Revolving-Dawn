using System.Collections.Generic;
using UnityEngine;
using characters;
using TMPro;
using fightDamageCalc;


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
            Chain chain = new Chain();
            if (IsManaCharged())
            {
                //Play powerful effect
            } 
            else
            {
                //Play regular effect
                foreach(var target in targets)
                {
                    currentPlayer.PerformDamageNumberAction(new Number(damage, FightInfo.NumberType.Attack),target);
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


        public override void LoadInfo(CardScriptableObject cardSO)
        {
            artwork.GetComponent<MeshRenderer>().material = cardSO.artwork;
            border.GetComponent<MeshRenderer>().material = cardSO.border;

            nameText.text = cardSO.name;
            description.text = cardSO.description;
            nameText.font = CardInfo.DEFAULT_FONT;
            description.font = CardInfo.DEFAULT_FONT;

            nameText.color = Color.black;
            description.color = Color.black;
        }
        
        void Awake() {
            LoadInfo(cardSO);
        }
    }
}
