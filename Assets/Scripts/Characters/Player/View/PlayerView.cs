using Data;
using UnityEngine;

namespace Characters.View
{
    public class PlayerView : MonoBehaviour, CharacterMVC.IView, IPopulateData<PlayerView.Data>
    {
        public class Data
        {
            private string spriteKey;
        }

        public void Populate(Data data)
        {
            throw new System.NotImplementedException();
        }
    }
}