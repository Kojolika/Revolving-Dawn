using UnityEngine;
using TMPro;
using System.Collections.Generic;
using Characters;
using Mana;

namespace Cards
{

    public static class CardConfiguration
    {
        public static Vector3 DEFAULT_CARD_ROTATION = new Vector3(90f, 90f, -90f);
        public static Vector3 DEFAULT_SCALE = new Vector3(0.2f, 1f, 0.3f);
        public static float CAMERA_DISTANCE = Camera.main.nearClipPlane + 7;
        public static TMP_FontAsset DEFAULT_FONT = Resources.Load<TMP_FontAsset>("DeterminationSansWebRegular-369X SDF");
        public static Color DEFAULT_FONT_COLOR = Color.white;
        public const float DEFAULT_FONT_NAME_SIZE = 10f;
        public const float DEFAULT_FONT_NAME_SIZE_UI = 23f;
        public const float DEFAULT_FONT_DESCRIPTION_SIZE = 9f;
        public const float DEFAULT_FONT_DESCRIPTION_SIZE_UI = 21f;

        static Sprite BORDER_WARRIOR = null;
        static Sprite BORDER_ROGUE = null;
        static Sprite BORDER_MAGE = null;
        static Sprite BORDER_PRIEST = null;
        static Sprite BORDER_NEUTRAL = null;

        public static Sprite GetClassBorder(PlayerClass playerClass)
        {
            switch (playerClass)
            {
                case (PlayerClass.Warrior):
                    if (BORDER_WARRIOR == null)
                    {
                        BORDER_WARRIOR = Resources.Load<Sprite>("Warrior_Border");
                        return BORDER_WARRIOR;
                    }
                    else return BORDER_WARRIOR;
                case (PlayerClass.Rogue):
                    if (BORDER_ROGUE == null)
                    {
                        BORDER_ROGUE = Resources.Load<Sprite>("Rogue_Border");
                        return BORDER_ROGUE;
                    }
                    else return BORDER_ROGUE;
                case (PlayerClass.Mage):
                    if(BORDER_MAGE == null)
                    {
                        BORDER_MAGE = Resources.Load<Sprite>("Mage_Border");
                        return BORDER_MAGE;
                    }
                    else return BORDER_MAGE;
                case (PlayerClass.Priest):
                    if(BORDER_PRIEST == null){
                        BORDER_PRIEST = Resources.Load<Sprite>("Priest_Border");
                        return BORDER_PRIEST;
                    }
                    else return BORDER_PRIEST;
                case (PlayerClass.Classless):
                    if(BORDER_NEUTRAL == null){
                        BORDER_NEUTRAL = Resources.Load<Sprite>("Neutral_Border");
                        return BORDER_NEUTRAL;
                    }
                    else return BORDER_NEUTRAL;
            }
            return Resources.Load<Sprite>("Neutral_Border");
        }
    }
    
    public enum Targeting
    {
        Friendly,
        Enemy,
        RandomEnemy,
        AllEnemies,
        All,
        None
    }


}

