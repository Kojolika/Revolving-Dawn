using Models.Characters.Player;
using Cysharp.Threading.Tasks;
using Models.Characters;
using Models.Fight;
using Models.Map;
using Tooling.Logging;
using Tooling.StaticData.Data;

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
            MyLogger.Info($"Created new player with id {CurrentPlayerDefinition.Id}");

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
        
        /// <summary>
        /// Adds the default internal buffs a player has at the start of a run
        /// </summary>
        private static void AddInitialBuffs(PlayerCharacter playerCharacter)
        {
            var drawHandSizeBuff = StaticDatabase.Instance.GetStaticDataInstance<Buff>("DrawHandSize");
            playerCharacter.SetBuff(drawHandSizeBuff, 1);
        }

        public async UniTask AbandonRun()
        {
            MyLogger.Info("Deleting run...");
            if (CurrentPlayerDefinition != null)
            {
                CurrentPlayerDefinition.CurrentRun = null;
            }

            await saveManager.Save(CurrentPlayerDefinition);
            MyLogger.Info("Run deleted");
        }
    }
}