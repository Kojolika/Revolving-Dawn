using Cysharp.Threading.Tasks;
using Systems.Managers.Base;
using Models.Fight;
using Fight;

namespace Systems.Managers
{
    public class FightManager2 : IPartTimeManager
    {
        private PlayerDataManager playerDataManager;
        private BattleEngine battleEngine;

        [Zenject.Inject]
        void Construct(PlayerDataManager playerDataManager)
        {
            this.playerDataManager = playerDataManager;
        }

        UniTask IManager.Startup()
        {
            battleEngine = new BattleEngine();
            return UniTask.CompletedTask;
        }
    }
}