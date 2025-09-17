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
        private SaveManager       saveManager;

        [Inject]
        private void Construct(PlayerDataManager playerDataManager, SaveManager saveManager)
        {
            this.playerDataManager = playerDataManager;
            AddLabelWithValue("Id", () => playerDataManager.CurrentPlayerDefinition.Id.ToString());
            AddButton(() => SaveManager.IsSavingEnabled ? "Saving Enabled" : "Saving Disabled",
                      () => SaveManager.IsSavingEnabled = !SaveManager.IsSavingEnabled);
        }

        public void ResetSave()
        {
            _ = playerDataManager.AbandonRun();
        }
    }
}
#endif