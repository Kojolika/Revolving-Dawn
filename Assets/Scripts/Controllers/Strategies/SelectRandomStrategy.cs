using Controllers.Strategies;
using Tooling.StaticData.Data;

namespace Models.Characters.Enemies.Strategies
{
    public class SelectRandomStrategy : ISelectMoveStrategy
    {
        public EnemyMove SelectMove(Enemy enemy)
        {
            var moves         = enemy.PossibleMoves;
            var enemyMoveSize = moves.Count;
            var rng           = new System.Random();
            var randomMove    = moves[rng.Next(enemyMoveSize - 1)];

            return randomMove;
        }
    }
}