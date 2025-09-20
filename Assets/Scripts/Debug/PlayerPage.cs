using System.Globalization;
using Common.Util;
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
            if (this.playerDataManager.CurrentPlayerDefinition == null)
            {
                return;
            }

            AddLabelWithValue("Id", () => playerDataManager.CurrentPlayerDefinition.Id.ToString());
            AddButton(() => SaveManager.IsSavingEnabled ? "Saving Enabled" : "Saving Disabled",
                      () => SaveManager.IsSavingEnabled = !SaveManager.IsSavingEnabled);

            if (this.playerDataManager.CurrentPlayerDefinition.CurrentRun?.PlayerCharacter == null)
            {
                return;
            }

            AddLabel("Stats:");
            foreach (var (amount, stat) in playerDataManager.CurrentPlayerDefinition.CurrentRun.PlayerCharacter.GetStats().OrEmptyIfNull())
            {
                AddLabelWithValue($"Stat {stat.Name}", () => amount.ToString(CultureInfo.InvariantCulture));
            }
        }

        public void ResetSave()
        {
            _ = playerDataManager.AbandonRun();
        }
    }
}
#endif