using Cysharp.Threading.Tasks;
using Systems.Managers.Base;
using Models.Fight;
using Fight;

namespace Systems.Managers
{
    public class FightManager2 : IPartTimeManager
    {
        private readonly PlayerDataManager playerDataManager;
        private readonly BattleEngine battleEngine;

        public FightManager2(PlayerDataManager playerDataManager, BattleEngine battleEngine)
        {
            this.battleEngine = new BattleEngine();
            this.playerDataManager = playerDataManager;
        }
    }
}