using UnityEngine;
using System.Dynamic;
using Systems.Managers;

namespace UI
{
    public abstract class Menu : MonoBehaviour
    {

        public abstract ExpandoObject MenuInput { get; set; }
        public virtual void OnBackPressed()
        {
            Close();
        }
        public virtual void Close()
        {
            MenuManager.StaticInstance.CloseMenu();
        }
        public virtual void Open(dynamic input = null)
        {
            MenuManager.StaticInstance.OpenMenu(this.gameObject, input);
        }
        public virtual void HandleInput(dynamic input)
        {
            if (input == null) return;
        }
    }
}
