using Models.Characters;
using Tooling.StaticData.EditorUI;

namespace Controllers.Strategies
{
    public interface ISelectMoveStrategy
    {
        EnemyMove SelectMove(Enemy enemy);
    }
}