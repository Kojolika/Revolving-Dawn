namespace Models.Characters.Enemies.Strategies
{
    public interface ISelectMoveStrategy
    {
        EnemyMove SelectMove(EnemyModel enemyModel);
    }
}