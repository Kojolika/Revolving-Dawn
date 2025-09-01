using System;
using System.Collections.Generic;
using Controllers.Strategies;
using Fight.Engine;
using Models.Fight;
using Tooling.StaticData.EditorUI;

namespace Models.Characters
{
    public class EnemyLogic : IMoveParticipant
    {
        public Enemy               Model              { get; private set; }
        public EnemyMove           NextMove           { get; private set; }
        public ISelectMoveStrategy SelectMoveStrategy { get; private set; }

        public EnemyLogic(Enemy enemy, ISelectMoveStrategy selectMoveStrategy)
        {
            Model              = enemy;
            SelectMoveStrategy = selectMoveStrategy;
        }

        public void SelectMove()
        {
            NextMove = SelectMoveStrategy.SelectMove(Model);
        }

        public string   Name { get; }
        public TeamType Team { get; }

        public bool HasStat(Stat stat)
        {
            throw new NotImplementedException();
        }

        public float GetStat(Stat stat)
        {
            throw new NotImplementedException();
        }

        public void SetStat(Stat stat, float value)
        {
            throw new NotImplementedException();
        }

        public int GetBuff(Buff buff)
        {
            throw new NotImplementedException();
        }

        public void SetBuff(Buff buff, int value)
        {
            throw new NotImplementedException();
        }

        public List<(int stackSize, Buff)> GetBuffs()
        {
            throw new NotImplementedException();
        }

        public List<(float amount, Stat)> GetStats()
        {
            throw new NotImplementedException();
        }
    }
}