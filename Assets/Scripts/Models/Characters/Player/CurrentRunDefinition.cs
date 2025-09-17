using Models.Map;
using Models.Fight;

namespace Models.Characters.Player
{
    [System.Serializable]
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