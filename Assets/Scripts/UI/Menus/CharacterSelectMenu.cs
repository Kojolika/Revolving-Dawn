using System.Collections.Generic;
using Data.Definitions.Player;
using UI.Menus.Common;

namespace UI.Menus
{
    public class CharacterSelectMenu : Menu<CharacterSelectMenu.Data>
    {
        public class Data
        {
            private List<ClassDefinition> classDefinitions;
        }

        public override void Populate(Data data)
        {
            throw new System.NotImplementedException();
        }
    }
}