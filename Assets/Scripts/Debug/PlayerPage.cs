using Systems.Managers;
using Zenject;

#if !PRODUCTION || ENABLE_DEBUG_MENU
namespace Koj.Debug
{
    public class PlayerPage : DebugMenu.Page
    {
        public const string Address = "Assets/Prefabs/Debug/PlayerDebugMenu.prefab";

        private SaveManager saveManager;

        [Inject]
        private void Construct(SaveManager saveManager)
        {
            this.saveManager = saveManager;
        }

        public void ResetSave()
        {
        }
    }
}
#endif