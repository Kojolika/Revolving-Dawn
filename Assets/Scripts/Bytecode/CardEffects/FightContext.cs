using Models.Characters;
using Models.Fight;
using Models.Map;
using System.Collections.Generic;

namespace Bytecode
{
    public class FightContext
    {
        public readonly List<Character> Characters;
        public readonly MapDefinition CurrentMap;
        public readonly NodeDefinition CurrentNode;
        public readonly FightDefinition CurrentFight;
    }
}