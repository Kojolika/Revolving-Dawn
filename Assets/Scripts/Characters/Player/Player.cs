using System.Collections.Generic;
using cards;
using UnityEngine;

namespace characters
{
    public class Player : Character
    {
        public int DrawAmount = 3;
        public Health _health;
        public PlayerCardDecks _playerCardDecks;

        void Start() {
            _playerCardDecks = new PlayerCardDecks();
            
            _playerCardDecks.Deck = this.GetComponent<TestDeck>().deck;
            _playerCardDecks.DrawPile = _playerCardDecks.Deck;
            
            _playerCardDecks.Hand = new List<Card>();
            _playerCardDecks.Discard = new List<Card>();
            _playerCardDecks.Lost = new List<Card>();
        }
        public override float GetHealth()
        {
            throw new System.NotImplementedException();
        }
    }
}
