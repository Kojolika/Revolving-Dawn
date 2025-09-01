using Tooling.StaticData.EditorUI;

namespace Systems.Managers
{
    public class LocalizationManager : IManager
    {
        // TODO: TRANSLATE
        public string Translate(LocKey key, params object[] args)
        {
            return string.Format(key.EnglishValue, args);
        }
    }
}