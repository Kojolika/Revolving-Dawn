using UnityEngine;
using System.Collections.Generic;
using characters;
using mana;
using utils;
using fightDamageCalc;

namespace cards
{
    [CreateAssetMenu(fileName = "New Card", menuName = "Cards/New Card")]
    public class Card : ScriptableObject
    {
        public Character owner; //character that plays this card
        public ManaType[] mana; //what mana this card uses to charge
        public new string name; //name of the card
        [SerializeField] string descriptionWithReplaceables; // description of what the card does, contains words that will be replaced by number values, edit this one
        [HideInInspector] public string description; //final description that is shown on the card after word replacements have been replaced, should not be edited by itself
        public Sprite artwork; //card art
        public PlayerClass @class = PlayerClass.Classless; //class of the card
        public Targeting target;   //who the card targets
        // Add list of Keywords in future
        public virtual void Play(List<Character> targets) //effect of the card when played
        {
            foreach (Character character in targets)
            {
                foreach (Number number in numberValues)
                {
                    this.owner.PerformNumberAction(number, character);
                }
            }
        }
        [Space(20)]
        [SerializeField] protected List<Number> numberValues = new List<Number>();
        [SerializeField] protected List<Affect> affectValues = new List<Affect>();
        [SerializeField] protected Card nextCardUpgrade = null;
        [SerializeField] protected Card previousCard = null;

        //true means transform to next card, false is transform to previous card
        public Card Transform(bool direction)
        {
            if (direction)
            {
                if (nextCardUpgrade != null)
                {
                    nextCardUpgrade.previousCard = this;
                    nextCardUpgrade.owner = this.owner;
                    nextCardUpgrade.UpdateDescription(null);
                }
                return nextCardUpgrade;
            }
            else
            {
                if (previousCard != null)
                {
                    previousCard.nextCardUpgrade = this;
                    previousCard.owner = this.owner;
                    previousCard.UpdateDescription(null);
                }
                return previousCard;
            }
        }
        Chain chain = new Chain();
        public void UpdateDescription(Character target) // need to move this to Card3D, CardUI
        {
            for (int index = 0; index < numberValues.Count; index++)
            {
                Number copy = numberValues[index];
                description = descriptionWithReplaceables.Replace(copy.getType().ToString().ToUpper() + (index + 1), "" + chain.process(copy, owner, target).Amount);
            }
        }
        void OnEnable()
        {
            UpdateDescription(null);

            //If there is no next upgrade, there can not be any mana for an upgrade
            if (nextCardUpgrade == null)
            {
                mana = new ManaType[] { };
                Debug.LogWarning("Cannot add mana to a card that has no upgrade");
            }
        }
    }
}
