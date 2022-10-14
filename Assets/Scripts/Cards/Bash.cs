using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

        public override void Play()
        {
            if (IsManaCharged())
            {
                //Play powerful effect
            }
            else
            {
                //Play regular effect
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
        }
        void Start() {
            LoadInfo(cardSO);
        }
    }
}
