using UnityEngine;
using TMPro;
using System.Collections.Generic;
using characters;

namespace cards
{
    public class Card3D : MonoBehaviour
    {
        [SerializeReference]
        SpecificCardSO cardScriptableObject;

        bool isManaCharged = false;

        [SerializeField]
        new TextMeshPro name;

        [SerializeField]
        TextMeshPro description;

        [SerializeField]
        SpriteRenderer artwork;

        [SerializeField]
        SpriteRenderer border;

        [SerializeField]
        GameObject manaSockets;
        [SerializeField]
        GameObject socket;

        public static float CAMERA_DISTANCE => Camera.main.nearClipPlane + 7;
        public static Vector3 DEFAULT_SCALE => new Vector3(0.2f, 1f, 0.3f);

        public void Play(List<Character> targets)
        {
            if (isManaCharged)
            {
                cardScriptableObject.PlayManaCharged(targets);
            }
            else
            {
                cardScriptableObject.PlayUncharged(targets);
            }
        }
        public Targeting GetTarget()
        {
            if (isManaCharged)
            {
                return cardScriptableObject.targetManaCharged;
            }
            else
            {
                return cardScriptableObject.target;
            }
        }

        void Awake()
        {
            LoadValuesFromCardScriptableObject();
        }
        void LoadValuesFromCardScriptableObject()
        {
            artwork.sprite = cardScriptableObject.artwork;
            border.sprite = cardScriptableObject.GetBorder();

            name.text = cardScriptableObject.name;
            name.font = SpecificCardSO.DEFAULT_FONT;
            name.color = SpecificCardSO.DEFAULT_FONT_COLOR;
            name.fontSize = SpecificCardSO.DEFAULT_FONT_NAME_SIZE;

            description.text = cardScriptableObject.description;
            description.font = SpecificCardSO.DEFAULT_FONT;
            description.color = SpecificCardSO.DEFAULT_FONT_COLOR;
            description.fontSize = SpecificCardSO.DEFAULT_FONT_NAME_SIZE;
        }
    }
}
