using Models.Characters;
using Tooling.StaticData;

namespace Controllers.Strategies
{
    public interface ISelectMoveStrategy
    {
        EnemyMove SelectMove(EnemyModel enemyModel);
    }
}