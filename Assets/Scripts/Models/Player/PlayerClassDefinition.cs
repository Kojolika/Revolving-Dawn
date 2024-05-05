using System.Collections.Generic;
using UnityEngine;

namespace Models.Player
{
    [CreateAssetMenu(fileName = "New " + nameof(PlayerClassDefinition), menuName = "RevolvingDawn/Player/Classes")]
    public class PlayerClassDefinition : ScriptableObject
    {
        [SerializeField] private new string name;
        [SerializeField] private List<Card> startingDeck;
        [SerializeField] private string description;
        [SerializeField] private Sprite characterAvatar;
        [SerializeField] private Sprite cardBorder;
        
        public string Name => name;
        public List<Card> StartingDeck => startingDeck;
        public string Description => description;
        public Sprite CharacterAvatar => characterAvatar;
        public Sprite CardBorder => cardBorder;
    }
}