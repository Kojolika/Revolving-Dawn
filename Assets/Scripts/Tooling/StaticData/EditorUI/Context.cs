using System.Collections.Generic;

namespace Tooling.StaticData.EditorUI
{
    /// <summary>
    /// Represents the editor context for a static data object.
    /// </summary>
    public class Context
    {
        public StaticData StaticData { get; set; }
        public HashSet<string> Variables { get; set; }

        public Context(StaticData staticData)
        {
            this.StaticData = staticData;
        }
    }
}