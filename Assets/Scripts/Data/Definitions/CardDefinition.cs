using UnityEngine;
using System.Collections.Generic;
using Characters;
using Mana;
using FightDamageCalc;
using Cards;

namespace Data.Definitions
{
    [System.Serializable]
    //[CreateAssetMenu(fileName = "New Card", menuName = "Cards/New Card")]
    public class CardDefinition : ScriptableObject
    {
        public Character owner; //character that plays this card
        public ManaType[] mana; //what mana this card uses to charge
        public new string name; //name of the card
        [SerializeField] public string descriptionWithReplaceables; // description of what the card does, contains words that will be replaced by number values, edit this one
        public Sprite artwork; //card art
        public PlayerClass playerClass = PlayerClass.Classless; //class of the card

        public Targeting.Options target; //who the card targets

        // Add list of Keywords in future
        [Space(20)]
        [SerializeField]
        public List<Number> numberValues = new List<Number>();

        [SerializeField]
        protected List<Affect> affectValues = new List<Affect>();

        //TODO: convert to foreign key
        [SerializeField]
        protected CardDefinition nextCardUpgrade = null;
        //TODO: convert to foreign key
        [SerializeField]
        protected CardDefinition previousCard = null;

        //true means transform to next card, false is transform to previous card
        public CardDefinition Transform(bool direction)
        {
            if (direction)
            {
                if (nextCardUpgrade != null)
                {
                    nextCardUpgrade.previousCard = this;
                    nextCardUpgrade.owner = this.owner;
                }

                return nextCardUpgrade;
            }
            else
            {
                if (previousCard != null)
                {
                    previousCard.nextCardUpgrade = this;
                    previousCard.owner = this.owner;
                }

                return previousCard;
            }
        }

        void OnEnable()
        {
            //If there is no next upgrade, there must not be any mana for an upgrade
            if (nextCardUpgrade == null)
            {
                mana = new ManaType[] { };
                Debug.LogWarning("Cannot add mana to a card that has no upgrade");
            }
        }

        public virtual void Play(List<Character> targets) //effect of the card when played
        {
            foreach (Character character in targets)
            {
                foreach (Number number in numberValues)
                {
                    this.owner.PerformNumberAction(number, character);
                }

                foreach (Affect affect in affectValues)
                {
                    this.owner.PerformAffectAction(affect, character);
                }
            }
        }
    }
}