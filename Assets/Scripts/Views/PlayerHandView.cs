using Mana;
using UnityEngine;
using Utils;

namespace Views
{
    public class PlayerHandView : MonoBehaviour
    {
        [SerializeField] ManaPoolView manaPoolView;
        [SerializeField] CardView.Factory cardViewFactory;
        [SerializeField] Camera handViewCamera;
        [SerializeField] Transform handParent;
        [SerializeField] Transform cardSpawnLocation;
        [SerializeField] Transform cardDiscardLocation;
        [SerializeField] BezierCurve handCurve;

        [Zenject.Inject]
        private void Construct(CardView.Factory cardViewFactory)
        {
            this.cardViewFactory = cardViewFactory;
        }
    }
}