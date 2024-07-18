
namespace Cards
{
    public static class Targeting
    {
        public enum Options
        {
            Friendly,
            Enemy,
            RandomEnemy,
            AllEnemies,
            All,
            None
        }

        public static string GetSuffixDescription(Options options)
            => options switch
            {
                Options.Friendly => "to a friendly character.",
                Options.Enemy => "to an enemy.",
                Options.RandomEnemy => "to a random enemy.",
                Options.AllEnemies => "to all enemies.",
                Options.All => "to all characters.",
                Options.None => "",
                _ => ""
            };
    }
}

