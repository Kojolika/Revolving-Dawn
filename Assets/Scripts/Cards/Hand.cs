using UnityEngine;

namespace cards
{
    public class Hand : MonoBehaviour
    {
        //used to store all the cards in the hand under one GameObject
        public static Hand staticInstance;
        void Awake()
        {
            if (staticInstance == null)
                staticInstance = this;
            else
                Destroy(this);
        }
    }
}
