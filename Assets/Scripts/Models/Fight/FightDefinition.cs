using System.Collections.Generic;
using Models.Characters;

namespace Models.Fight
{
    [System.Serializable]
    public class FightDefinition
    {
        public List<Enemy> Enemies;
        public PlayerHero PlayerHero;
    }
}