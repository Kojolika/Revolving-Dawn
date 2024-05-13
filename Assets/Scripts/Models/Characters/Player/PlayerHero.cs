using System.Collections.Generic;
using Models.Player;

namespace Models.Characters
{
    public class PlayerHero : Character
    {
        public PlayerClassDefinition Class { get; private set; }
        public Player.Decks Decks { get; private set;}
    }
}