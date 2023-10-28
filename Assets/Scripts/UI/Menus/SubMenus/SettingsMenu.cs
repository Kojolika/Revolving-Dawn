using Data;
using UI.Common;
using UnityEngine;

namespace UI.Menus.SubMenus
{
    public class SettingsMenu : MonoBehaviour, IPopulateData<Data.Null>
    {
        [SerializeField] private TabGroup tabGroup;
        
        public void Populate(Null data)
        {
        }
    }
}
