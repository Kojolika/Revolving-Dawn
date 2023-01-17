using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public abstract class Menu : MonoBehaviour
    {
        public virtual void OnBackPressed()
        {
            Close();
        }
        public virtual void Close()
        {
            MenuManager.staticInstance.CloseMenu();
        }
        public virtual void Open()
        {
            MenuManager.staticInstance.OpenMenu(this.gameObject);
        }
        public virtual void Open(DeckType deckType){}
    }
}
