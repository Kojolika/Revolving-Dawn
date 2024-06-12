using Characters.Model;
using Cysharp.Threading.Tasks;
using Models.Fight;
using Models.Map;
using Models.Player;
using Settings;
using Systems.Managers.Base;
using Tooling.Logging;

namespace Systems.Managers
{
    public class PlayerDataManager : IManager
    {
        public PlayerDefinition CurrentPlayerDefinition { get; private set; }

        private SaveManager saveManager;
        private MapSettings mapSettings;
        private MapDefinition.Factory mapFactory;

        [Zenject.Inject]
        void Construct(SaveManager saveManager, MapSettings mapSettings, MapDefinition.Factory mapFactory)
        {
            this.saveManager = saveManager;
            this.mapSettings = mapSettings;
            this.mapFactory = mapFactory;
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

        public async UniTask UpdateMapNode(NodeDefinition currentNode)
        {
            CurrentPlayerDefinition.CurrentRun.CurrentMapNode = currentNode;

            await saveManager.Save(CurrentPlayerDefinition);
        }

        public async UniTask StartFight(FightDefinition fightDefinition)
        {
            CurrentPlayerDefinition.CurrentRun.CurrentFight = fightDefinition;

            await saveManager.Save(CurrentPlayerDefinition);
        }

        public async UniTask StartNewRun(PlayerClassSODefinition playerClass)
        {
            var newMap = mapFactory.Create(mapSettings);
            CurrentPlayerDefinition.CurrentRun = new Characters.Player2.Run.RunDefinition()
            {
                Name = "Test",
                Gold = 0,
                CurrentMap = newMap,
                CurrentMapNode = newMap.Nodes[0],
                PlayerCharacter = new(playerClass)
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