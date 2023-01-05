using UnityEngine;
using fightInput;

namespace UI
{
    public class UIInput : MonoBehaviour
    {
        public static UIInput staticInstance;
        [SerializeField] GameObject escapeMenu;

        private void Awake()
        {
            if (staticInstance == null)
                staticInstance = this;
            else
                Destroy(this);

            escapeMenu.SetActive(false);
        }

        void Update() 
        {
            if(Input.GetKeyUp(KeyCode.Escape))
            {
                bool toggle = !escapeMenu.activeInHierarchy;
                escapeMenu.SetActive(toggle);
                PlayerTurnInputManager.staticInstance.isEnabled = toggle;  
            }
        }
    }
}
