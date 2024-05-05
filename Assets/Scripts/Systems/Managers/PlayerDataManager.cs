using Characters.Model;
using Cysharp.Threading.Tasks;
using Models.Map;
using Settings;
using Systems.Managers.Base;
using Systems.Map;
using Tooling.Logging;

namespace Systems.Managers
{
    public class PlayerDataManager : IManager
    {
        public PlayerDefinition CurrentPlayerDefinition { get; private set; }

        private SaveManager saveManager;
        private MapSettings mapSettings;

        [Zenject.Inject]
        void Construct(SaveManager saveManager, MapSettings mapSettings)
        {
            this.saveManager = saveManager;
            this.mapSettings = mapSettings;
        }

        public async UniTask AfterStart()
        {
            CurrentPlayerDefinition = await saveManager.TryLoadSavedData();

            MyLogger.Log($"Current run after loading: {CurrentPlayerDefinition?.CurrentRun}");

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

        public async UniTask StartNewRun()
        {
            var newMap = new MapFactory().Create(mapSettings);
            CurrentPlayerDefinition.CurrentRun = new Characters.Player2.Run.RunDefinition()
            {
                Name = "Test",
                Gold = 0,
                CurrentMap = newMap,
                CurrentMapNode = newMap.Nodes[0]
            };

            await saveManager.Save(CurrentPlayerDefinition);
        }

        public async UniTask AbandonRun()
        {
            CurrentPlayerDefinition.CurrentRun = null;
            await saveManager.Save(CurrentPlayerDefinition);
        }
    }
}