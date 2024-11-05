using Models.Mana;
using UI.Common;
using UnityEngine;
using UnityEngine.UI;
using Views.Common;

namespace Views
{
    public class PlayerManaView : MonoBehaviour, IView<(ManaModel mana, long amount)>
    {
        [SerializeField] private Image image;
        [SerializeField] private Label amountLabel;
        public void Populate((ManaModel mana, long amount) data)
        {
            image.color = data.mana.Color;
            amountLabel.SetText(data.amount.ToString());
        }
    }
}