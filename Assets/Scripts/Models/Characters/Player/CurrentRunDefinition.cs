using Models.Map;
using Models.Fight;

namespace Models.Characters.Player
{
    public class RunDefinition
    {
        public string          Name;
        public PlayerCharacter PlayerCharacter;
        public MapDefinition   CurrentMap;
        public FightDefinition CurrentFight;

        /// <summary>
        /// The seed used to generate random effects.
        /// </summary>
        public int Seed;
    }
}