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
        //Must only be size 3 or less
        [SerializeField]
        ManaType[] _mana;
        public new string name;
        [SerializeField]
        string descriptionWithReplaceables;
        [HideInInspector]
        public string description;
        public Sprite artwork;
        public PlayerClass @class = PlayerClass.Classless;
        public Targeting target;

        public Targeting manaChargedTarget;

        public abstract void PlayUncharged(List<Character> targets);
        public abstract void PlayManaCharged(List<Character> targets);

        [SerializeField]//maybe use fight damage calc number instead
        protected List<Number> descriptionReplacementsInterface = new List<Number>();
        
        [SerializeField] //key is the text to be replaced in the description, value is the replacement
        protected SerializableDictionary<string, float> descriptionReplacements = new SerializableDictionary<string, float>();


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
