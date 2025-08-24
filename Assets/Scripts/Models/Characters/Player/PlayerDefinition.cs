namespace Models.Characters.Player
{
    [System.Serializable]
    public class PlayerDefinition
    {
        public int           Id;
        public RunDefinition CurrentRun;

        public PlayerDefinition(int id)
        {
            Id = id;
        }
    }
}