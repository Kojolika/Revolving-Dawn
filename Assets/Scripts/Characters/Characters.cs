using UnityEngine;

namespace Characters
{
    public class Characters : MonoBehaviour
    {
        //Used to group characters in the scene under one GameObject
        public static Characters staticInstance;

        void Awake()
        {
            if (staticInstance == null)
            {
                staticInstance = this;
            }
            else
                Destroy(this);
        }
    }
}