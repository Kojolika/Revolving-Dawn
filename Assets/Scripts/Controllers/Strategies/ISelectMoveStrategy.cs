using Models.Characters;

namespace Controllers.Strategies
{
    public interface ISelectMoveStrategy
    {
        EnemyMove SelectMove(EnemyModel enemyModel);
    }
}