using UnityEngine;

namespace UI
{
    public class UIInput : MonoBehaviour
    {
        public static UIInput input;
        [SerializeField] GameObject escapeMenu;
        private void Awake()
        {
            if (input == null)
                input = this;
            else
                Destroy(this);
        }

        void Update() 
        {
             if(Input.GetKeyUp(KeyCode.Escape))
             {
                escapeMenu.SetActive(!escapeMenu.activeInHierarchy);
             }
        }
    }
}
