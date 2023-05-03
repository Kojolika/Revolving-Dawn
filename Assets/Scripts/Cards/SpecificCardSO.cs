using UnityEngine;
using System.Collections.Generic;
using characters;
using mana;
using TMPro;
using utils;
using fightDamageCalc;

namespace cards
{
    public abstract class SpecificCardSO : ScriptableObject
    {

        [SerializeField] public Character owner; //character that plays this card
        [SerializeField] ManaType[] _mana; //what mana this card uses to charge
        public new string name; //name of the card
        [SerializeField] string descriptionWithReplaceables; // description of what the card does, contains words that will be replaced by number values, edit this one
        [HideInInspector] public string description; //final description that is shown on the card after word replacements have been replaced, should not be edited by itself
        public Sprite artwork; //card art
        public PlayerClass @class = PlayerClass.Classless; //class of the card
        public Targeting target;   //what the card targets
        public abstract void PlayUncharged(List<Character> targets); //effect of the card when played

        public Targeting targetManaCharged;
        public abstract void PlayManaCharged(List<Character> targets);


        [SerializeField] protected List<Number> descriptionReplacementsInterface = new List<Number>();

        //key is the text to be replaced in the description, value is the replacement
        [SerializeField] protected SerializableDictionary<string, float> descriptionReplacements = new SerializableDictionary<string, float>();


        void UpdateDescription()
        {
            foreach (KeyValuePair<string, float> entry in descriptionReplacements)
            {
                description = descriptionWithReplaceables.Replace("" + entry.Key, "" + entry.Value);
            }
        }
        public void UpdateDescriptionForCurrentTargets(List<Character> targets)
        {

        }
        void OnEnable()
        {
            descriptionReplacements.Clear();
            for (int index = 0; index < descriptionReplacementsInterface.Count; index++)
            {
                descriptionReplacements.Add(descriptionReplacementsInterface[index].getType().ToString().ToUpper() + "" + (index + 1), descriptionReplacementsInterface[index].Amount);
            }
            UpdateDescription();
        }
        public Sprite GetBorder()
        {
            switch (@class)
            {
                case (PlayerClass.Warrior):
                    return Resources.Load<Sprite>("Warrior_Border");
                case (PlayerClass.Rogue):
                    return Resources.Load<Sprite>("Rogue_Border");
                case (PlayerClass.Mage):
                    return Resources.Load<Sprite>("Mage_Border");
                case (PlayerClass.Priest):
                    return Resources.Load<Sprite>("Priest_Border");
                case (PlayerClass.Classless):
                    return Resources.Load<Sprite>("Neutral_Border");
            }
            return Resources.Load<Sprite>("Neutral_Border");
        }

        //Configuration Variables
        public static Vector3 DEFAULT_ROTATION => new Vector3(90f, 90f, -90f);
        public static TMP_FontAsset DEFAULT_FONT => Resources.Load<TMP_FontAsset>("DeterminationSansWebRegular-369X SDF");
        public static Color DEFAULT_FONT_COLOR => Color.white;
        [SerializeReference]
        public const float DEFAULT_FONT_NAME_SIZE = 10f;
        [SerializeReference]
        public const float DEFAULT_FONT_NAME_SIZE_UI = 23f;
        [SerializeReference]
        public const float DEFAULT_FONT_DESCRIPTION_SIZE = 9f;
        [SerializeReference]
        public const float DEFAULT_FONT_DESCRIPTION_SIZE_UI = 21f;
    }
}
