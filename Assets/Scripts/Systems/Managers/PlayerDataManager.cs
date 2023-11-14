using Characters.Model;
using Cysharp.Threading.Tasks;
using Systems.Managers.Base;
using Tooling.Logging;

namespace Systems.Managers
{
    public class PlayerDataManager : IManager
    {
        public PlayerDefinition CurrentPlayerDefinition { get; private set; }

        private SaveManager saveManager;

        [Zenject.Inject]
        void Construct(SaveManager saveManager)
        {
            this.saveManager = saveManager;
        }

        public async UniTask AfterStart()
        {
            CurrentPlayerDefinition = await saveManager.TryLoadSavedData();

            if (CurrentPlayerDefinition == null)
            {
                await CreateNewPlayer();
            }
        }

        public async UniTask CreateNewPlayer()
        {
            // TODO: Implement steam id
            CurrentPlayerDefinition = new PlayerDefinition(1);
            MyLogger.Log($"Created new player with id {CurrentPlayerDefinition.ID}");
 
            await saveManager.Save(CurrentPlayerDefinition);
        }
    }
}