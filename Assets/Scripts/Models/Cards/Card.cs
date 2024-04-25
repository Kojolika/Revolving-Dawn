using UnityEngine;
using System.Collections.Generic;
using Models.Player;
using Models.CardEffects;

namespace Models
{
    [CreateAssetMenu(fileName = "New Card", menuName = "Cards/New Card")]
    public class Card : ScriptableObject
    {
        #region Public Accessors

        public string Name => name;
        public List<Models.Mana> Manas => manas;
        public Sprite Artwork => artwork;
        public PlayerClass Class => @class;
        public Card NextCard => nextCard;
        public Card PreviousCard => previousCard;
        public List<CardEffectContainer> PlayEffects => playEffects;

        #endregion

        [SerializeField] private new string name;
        [SerializeField] private List<Models.Mana> manas;
        [SerializeField] private Sprite artwork;
        [SerializeField] private PlayerClass @class;
        [SerializeField] private Card nextCard;
        [SerializeField] private Card previousCard;
        [SerializeField] private List<CardEffectContainer> playEffects;
    }
}