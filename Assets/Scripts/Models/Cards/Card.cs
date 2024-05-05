using UnityEngine;
using System.Collections.Generic;
using Models.Player;
using Models.CardEffects;
using Utils.Attributes;
using Models.Buffs;

namespace Models
{
    [CreateAssetMenu(fileName = "New Card", menuName = "RevolvingDawn/Cards/New Card")]
    public class Card : ScriptableObject
    {
        #region Public Accessors

        public string Name => name;
        public List<Models.Mana> Manas => manas;
        public Sprite Artwork => artwork;
        public PlayerClassDefinition Class => @class;
        public Card NextCard => nextCard;
        public Card PreviousCard => previousCard;
        public List<CardEffectWrapper> PlayEffects => playEffects;

        #endregion

        [SerializeField] private new string name;
        [SerializeField] private List<Models.Mana> manas;
        [SerializeField] private Sprite artwork;
        [SerializeField] private PlayerClassDefinition @class;
        [SerializeField] private Card nextCard;
        [SerializeField] private Card previousCard;
        [SerializeField] private List<CardEffectWrapper> playEffects;
    }
}