#if !PRODUCTION || ENABLE_DEBUG_MENU

using Systems.Managers;
using Zenject;

namespace Koj.Debug
{
    public class CurrentRunPage : Page
    {
        public const string            Address = "Assets/Prefabs/Debug/CurrentRunPage.prefab";
        private      PlayerDataManager playerDataManager;

        [Inject]
        private void Construct(PlayerDataManager playerDataManager)
        {
            this.playerDataManager = playerDataManager;
        }

        private void Start()
        {
            if (playerDataManager.CurrentRun == null)
            {
                AddLabel("No Active Run!");
                return;
            }

            AddLabelWithValue("Seed", () => playerDataManager.CurrentSeed.ToString());
            AddLabelWithValue("Run Name", () => playerDataManager.CurrentRun.Name);
            AddLabelWithValue("Player Name", () => playerDataManager.CurrentRun.PlayerCharacter?.Name);
            AddLabelWithValue("Player Class", () => playerDataManager.CurrentRun.PlayerCharacter?.Class.Name);

            AddLabelWithValue("Map Name", () => playerDataManager.CurrentRun.CurrentMap?.Name);
            AddLabelWithValue("Level", () => playerDataManager.CurrentRun.CurrentMap?.CurrentNode.Level.ToString());
            AddLabelWithValue("Current Node Type", () => playerDataManager.CurrentRun.CurrentMap?.CurrentNode.Event.Name);
        }
    }
}

#endif