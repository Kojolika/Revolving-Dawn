#if !PRODUCTION || ENABLE_DEBUG_MENU
namespace Koj.Debug
{
    public class PlayerPage : DebugMenu.Page
    {
        public const string Address = "Assets/Prefabs/Debug/PlayerDebugMenu.prefab";
    }
}
#endif