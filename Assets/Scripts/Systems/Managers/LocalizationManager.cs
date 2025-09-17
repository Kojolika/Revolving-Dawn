using Tooling.StaticData.Data;

namespace Systems.Managers
{
    public class LocalizationManager : IManager
    {
        // TODO: TRANSLATE
        public string Translate(LocKey key, params object[] args)
        {
            return string.Format(key?.EnglishValue ?? string.Empty, args);
        }
    }
}