using UnityEngine;
using System.Collections.Generic;
using characters;
using mana;
using utils;
using fightDamageCalc;

namespace cards
{
    public abstract class Card : ScriptableObject
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
                foreach (Number number in descriptionReplacementsInterface)
                {
                    this.owner.PerformNumberAction(number, character);
                }
            }
        } 
        [Space(20)]
        [SerializeField] protected List<Number> descriptionReplacementsInterface = new List<Number>();
        //key is the text to be replaced in the description, value is the replacement
        [SerializeField] protected SerializableDictionary<string, Number> descriptionReplacements = new SerializableDictionary<string, Number>();

        [SerializeField] protected Card nextCardUpgrade;
        [SerializeField] protected Card previousCard = null;

        //true means transform to next card, false is transform to previous card
        public Card Transform(bool direction)
        {
            if (direction)
            {
                nextCardUpgrade.previousCard = this;
                return nextCardUpgrade;
            }
            else
            {
                previousCard.nextCardUpgrade = this;
                return previousCard;
            }
        }
        Chain chain = new Chain();
        public void UpdateDescription(Character target)
        {
            foreach (KeyValuePair<string, Number> entry in descriptionReplacements)
            {
                Number copy = entry.Value;
                description = descriptionWithReplaceables.Replace("" + entry.Key, "" + chain.process(copy, owner, target).Amount);
            }
        }
        void OnEnable()
        {
            descriptionReplacements.Clear();
            for (int index = 0; index < descriptionReplacementsInterface.Count; index++)
            {
                descriptionReplacements.Add(descriptionReplacementsInterface[index].getType().ToString().ToUpper() + "" + (index + 1), descriptionReplacementsInterface[index]);
            }
            UpdateDescription(null);
        }
    }
}
