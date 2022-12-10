using System.Collections.Generic;
using UnityEngine;
using characters;
using TMPro;
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
                    //Deal damage
                    float finalDamage = chain.process(new Number(damage, FightInfo.NumberType.Attack), target).Amount;
                    target.healthDisplay.health.DealDamage(finalDamage);

                    //Add affect
                    if(target.gameObject.TryGetComponent<Affects>(out Affects affects))
                    {
                        affects.AddAffect(new Weaken(weakenAmount));
                    }
                    else
                    {
                        var newAffects = target.gameObject.AddComponent<Affects>();
                        newAffects.AddAffect(new Weaken(weakenAmount));
                    }
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
        }
    }
}
