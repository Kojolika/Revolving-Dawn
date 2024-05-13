using UnityEngine;
using System.Collections.Generic;
using Models.Player;
using Models.CardEffects;
using Models.Mana;
using Utils.Attributes;

namespace Models
{
    [CreateAssetMenu(fileName = "New Card", menuName = "RevolvingDawn/Cards/New Card")]
    public class Card : ScriptableObject
    {
        #region Public Accessors

        public string Name => name;
        public List<ManaDefinition> Manas => manas;
        public Sprite Artwork => artwork;
        public PlayerClassDefinition Class => @class;
        public Card NextCard => nextCard;
        public Card PreviousCard => previousCard;
        public List<ICombatEffect> PlayEffects => playEffects;

        #endregion

        [SerializeField] private new string name;
        [SerializeField] private List<ManaDefinition> manas;
        [SerializeField] private Sprite artwork;
        [SerializeField] private PlayerClassDefinition @class;
        [SerializeField] private Card nextCard;
        [SerializeField] private Card previousCard;
        [SerializeReference, DisplayInterface(typeof(ICombatEffect))] private List<ICombatEffect> playEffects;
    }
}