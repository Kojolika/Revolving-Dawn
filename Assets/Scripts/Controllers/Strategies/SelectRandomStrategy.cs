using Controllers.Strategies;

namespace Models.Characters.Enemies.Strategies
{
    [System.Serializable]
    public class SelectRandomStrategy : ISelectMoveStrategy
    {
        public EnemyMove SelectMove(EnemyModel enemyModel)
        {
            var moves = enemyModel.Moves;
            var enemyMoveSize = moves.Count;
            var rng = new System.Random();
            var randomMove = moves[rng.Next(enemyMoveSize - 1)];

            return randomMove;
        }
    }
}