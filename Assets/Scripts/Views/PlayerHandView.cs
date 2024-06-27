using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Mana;
using Models;
using UnityEngine;
using UnityEngine.XR;
using Utils;

namespace Views
{
    public class PlayerHandView : MonoBehaviour
    {
        [SerializeField] Camera handViewCamera;
        [SerializeField] Transform handParent;
        [SerializeField] Transform cardSpawnLocation;
        [SerializeField] Transform cardDiscardLocation;
        [SerializeField] Transform manaPoolViewLocation;
        [SerializeField] BezierCurve handCurve;

        private CardView.Factory cardViewFactory;
        private ManaPoolView manaPoolView;
        private List<CardView> hand;

        [Zenject.Inject]
        private void Construct(CardView.Factory cardViewFactory, ManaPoolView manaPoolView)
        {
            this.cardViewFactory = cardViewFactory;
            this.manaPoolView = manaPoolView;
            manaPoolView.transform.position = manaPoolViewLocation.position;
            hand = new List<CardView>();
        }


        public void DrawCard(Card card)
        {
            var newCardView = cardViewFactory.Create(card);
            newCardView.transform.position = cardSpawnLocation.position;
            newCardView.transform.SetParent(handParent);
            hand.Add(newCardView);
        }


        private UniTask CreateHandCurve()
        {
            var handSize = hand.Count;
            UniTask[] tasks = new UniTask[handSize];
            for (int i = 0; i < handSize; i++)
            {
                Vector3 newPosition = handCurve.GetPoint((float)i / handSize);

                newPosition.z -= (float)i * .5f;
                tasks[i - 1] = StartCoroutine(MoveCardCoroutine(hand[i - 1],
                    newPosition,
                    CardHandUtils.ReturnCardRotation(hand.Count, i),
                    cardMoveSpeed));
            }

            foreach (Coroutine task in tasks)
                yield return task;
        }

        private UniTask MoveCard(CardView cardView)
        {

        }
        public void DiscardCard(Card card)
        {

        }
    }
}