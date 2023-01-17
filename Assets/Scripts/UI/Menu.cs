using UnityEngine;
using System.Dynamic;

namespace UI
{
    public abstract class Menu : MonoBehaviour
    {

        public abstract ExpandoObject MenuInput {get; set;}
        public virtual void OnBackPressed()
        {
            Close();
        }
        public virtual void Close()
        {
            MenuManager.staticInstance.CloseMenu();
        }
        public virtual void Open(dynamic input = null)
        {
            MenuManager.staticInstance.OpenMenu(this.gameObject, input);
        }
        public virtual void HandleInput(dynamic input)
        {
            if(input == null) return;
        }
    }
}
