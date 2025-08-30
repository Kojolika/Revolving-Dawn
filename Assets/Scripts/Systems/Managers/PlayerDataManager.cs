using Models.Characters.Player;
using Cysharp.Threading.Tasks;
using Models.Fight;
using Models.Map;
using Settings;
using Tooling.Logging;
using Tooling.StaticData;

namespace Systems.Managers
{
    public class PlayerDataManager : IManager
    {
        public PlayerDefinition CurrentPlayerDefinition { get; private set; }

        private SaveManager           saveManager;
        private MapSettings           mapSettings;
        private CharacterSettings     characterSettings;
        private MapDefinition.Factory mapFactory;

        [Zenject.Inject]
        async void Construct(SaveManager saveManager, MapSettings mapSettings, CharacterSettings characterSettings, MapDefinition.Factory mapFactory)
        {
            this.saveManager       = saveManager;
            this.mapSettings       = mapSettings;
            this.characterSettings = characterSettings;
            this.mapFactory        = mapFactory;

            CurrentPlayerDefinition = await saveManager.TryLoadSavedData();

            if (CurrentPlayerDefinition == null)
            {
                _ = CreateNewPlayer();
            }
        }

        public async UniTask CreateNewPlayer()
        {
            // TODO: Implement steam id
            CurrentPlayerDefinition = new PlayerDefinition(1);
            MyLogger.Log($"Created new player with id {CurrentPlayerDefinition.Id}");

            await saveManager.Save(CurrentPlayerDefinition);
        }

        public async UniTask UpdateMapNode(NodeDefinition currentNode)
        {
            CurrentPlayerDefinition.CurrentRun.CurrentMap.CurrentNode = currentNode;

            await saveManager.Save(CurrentPlayerDefinition);
        }

        public async UniTask SaveFight(FightDefinition fightDefinition)
        {
            CurrentPlayerDefinition.CurrentRun.CurrentFight = fightDefinition;

            await saveManager.Save(CurrentPlayerDefinition);
        }

        public async UniTask StartNewRun(PlayerClass playerClass)
        {
            var newMap = mapFactory.Create(mapSettings);
            CurrentPlayerDefinition.CurrentRun = new RunDefinition()
            {
                Name            = "Test",
                CurrentMap      = newMap,
                PlayerCharacter = new(playerClass, characterSettings)
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