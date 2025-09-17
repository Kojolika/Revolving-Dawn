using Models.Characters;
using Tooling.StaticData.Data;

namespace Controllers.Strategies
{
    public interface ISelectMoveStrategy
    {
        EnemyMove SelectMove(Enemy enemy);
    }
}