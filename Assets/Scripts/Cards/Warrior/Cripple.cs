using System.Collections.Generic;
using UnityEngine;
using TMPro;
using characters;
using fightDamageCalc;


namespace cards{
    public class Cripple : Card
    {
        [SerializeField] CardScriptableObject cardSO;
        [SerializeField] TextMeshPro nameText;
        [SerializeField] TextMeshPro description;
        [SerializeField] GameObject artwork;
        [SerializeField] GameObject border;


        [SerializeField] Targeting target = Targeting.Enemy;
        bool manaCharged = false;

        float damage = 4;
        int weakenAmount = 2;

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
                    //Deal damage
                    currentPlayer.PerformNumberAction(new Number(damage, FightInfo.NumberType.Attack),target);

                    //Add affect
                    currentPlayer.PerformAffectAction(new Weaken(2), target);
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
        void Awake() {
            LoadInfo(cardSO);

            //Use this to update text damage later
            description.text = description.text.Replace("NEWLINE","\n");
        }
    }
}
