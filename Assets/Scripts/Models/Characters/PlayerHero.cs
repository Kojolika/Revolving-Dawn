using Models.Player;

namespace Models.Characters
{
    public class PlayerHero : Character
    {
        public override string Name => Class.Name;
        public PlayerClassDefinition Class { get; private set; }
    }
}