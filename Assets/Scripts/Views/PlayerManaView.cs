using Tooling.StaticData.Data;
using UI.Common;
using UnityEngine;
using UnityEngine.UI;
using Views.Common;

namespace Views
{
    public class PlayerManaView : MonoBehaviour, IView<(Mana mana, long amount)>
    {
        [SerializeField] private Image image;
        [SerializeField] private Label amountLabel;

        public void Populate((Mana mana, long amount) data)
        {
            image.color = data.mana.Color;
            amountLabel.SetText(data.amount.ToString());
        }
    }
}