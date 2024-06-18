using UnityEngine;
using Zenject;

namespace Views
{
    public class CardView : MonoBehaviour
    {
        public readonly Models.Card cardModel;
        public CardView(Models.Card cardModel)
        {
            this.cardModel = cardModel;
        }

        public class Factory : PlaceholderFactory<Models.Card, CardView>
        {

        }
    }
}