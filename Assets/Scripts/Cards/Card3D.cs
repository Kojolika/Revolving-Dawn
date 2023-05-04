using UnityEngine;
using TMPro;
using System.Collections.Generic;
using characters;

namespace cards
{
    public class Card3D : MonoBehaviour
    {
        [SerializeReference] SpecificCardSO cardScriptableObject;

        [SerializeField] new TextMeshPro name;

        [SerializeField] TextMeshPro description;

        [SerializeField] SpriteRenderer artwork;

        [SerializeField] SpriteRenderer border;

        [SerializeField] GameObject manaSockets;
        [SerializeField] GameObject socket;

        public static float CAMERA_DISTANCE => Camera.main.nearClipPlane + 7;
        public static Vector3 DEFAULT_SCALE => new Vector3(0.2f, 1f, 0.3f);

        public void Play(List<Character> targets) => cardScriptableObject.Play(targets);
        public Targeting GetTarget() => cardScriptableObject.target;

        void Awake()
        {
            PopulateFromData();
        }
        //Summary:
        //Transform card to next or previous in chain
        //
        //Args:
        //direction: true is forward transform (typically an upgrade to the card)
        //           false is backwards transform
        public void Transform(bool direction)
        {
            cardScriptableObject = cardScriptableObject.Transform(direction) == null ? cardScriptableObject : cardScriptableObject.Transform(direction);
            PopulateFromData();
        }
        void PopulateFromData()
        {
            artwork.sprite = cardScriptableObject.artwork;
            border.sprite = CardConfiguration.GetClassBorder(cardScriptableObject.@class);

            name.text = cardScriptableObject.name;
            name.font = CardConfiguration.DEFAULT_FONT;
            name.color = CardConfiguration.DEFAULT_FONT_COLOR;
            name.fontSize = CardConfiguration.DEFAULT_FONT_NAME_SIZE;

            description.text = cardScriptableObject.description;
            description.font = CardConfiguration.DEFAULT_FONT;
            description.color = CardConfiguration.DEFAULT_FONT_COLOR;
            description.fontSize = CardConfiguration.DEFAULT_FONT_NAME_SIZE;
        }
    }
}
