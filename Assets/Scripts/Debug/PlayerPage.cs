using Systems.Managers;
using Tooling.Logging;
using Zenject;

#if !PRODUCTION || ENABLE_DEBUG_MENU
namespace Koj.Debug
{
    public class PlayerPage : Page
    {
        public const string Address = "Assets/Prefabs/Debug/PlayerDebugMenu.prefab";

        private PlayerDataManager playerDataManager;

        [Inject]
        private void Construct(PlayerDataManager playerDataManager)
        {
            this.playerDataManager = playerDataManager;
            AddLabelWithValue("Id", () => playerDataManager.CurrentPlayerDefinition.Id.ToString());
        }

        public void ResetSave()
        {
            _ = playerDataManager.AbandonRun();
        }
    }
}
#endif