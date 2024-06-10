using Characters.Model;
using Models.Map;

namespace Models.Fight
{
    public static class FightDefinitionFactory
    {
        public static FightDefinition GenerateFightForLevel(PlayerDefinition playerDefinition)
        {
            if (playerDefinition.CurrentRun.PlayerCharacter == null)
            {
                throw new System.Exception("Fight started with the playing have a class!");
            }


            return new FightDefinition()
            {
                PlayerCharacter = playerDefinition.CurrentRun.PlayerCharacter,
                //Enemies = enemies
            };
        }
    }
}