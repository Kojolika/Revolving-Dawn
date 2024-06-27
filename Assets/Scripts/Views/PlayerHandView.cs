using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Mana;
using Models;
using Settings;
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
        private CardSettings cardSettings;

        [Zenject.Inject]
        private void Construct(CardView.Factory cardViewFactory, ManaPoolView manaPoolView, CardSettings cardSettings)
        {
            this.cardViewFactory = cardViewFactory;
            this.manaPoolView = manaPoolView;
            manaPoolView.transform.position = manaPoolViewLocation.position;
            hand = new List<CardView>();
            this.cardSettings = cardSettings;
        }


        public void DrawCards(List<Card> cards)
        {
            foreach (var card in cards)
            {
                var newCardView = cardViewFactory.Create(card);
                newCardView.transform.position = cardSpawnLocation.position;
                newCardView.transform.SetParent(handParent);
                hand.Add(newCardView);
            }
            _ = CreateHandCurve();
        }

        private async UniTask CreateHandCurve()
        {
            var handSize = hand.Count;
            UniTask[] tasks = new UniTask[handSize];
            for (int i = 0; i < handSize; i++)
            {
                Vector3 newPosition = handCurve.GetPoint((float)i / handSize);
                newPosition.z -= (float)i * .5f;

                Vector3 newRotation = default;

                tasks[i] = MoveCard(hand[i], newPosition, newRotation);
            }

            foreach (var task in tasks)
            {
                await task;
            }
        }

        private async UniTask MoveCard(CardView cardView, Vector3 position, Vector3 rotation)
        {

            while (Mathf.Abs(cardView.transform.position.x - position.x) > .01f
                && Mathf.Abs(cardView.transform.position.y - position.y) > .01f
                && Mathf.Abs(cardView.transform.rotation.x - rotation.x) > .01f
                && Mathf.Abs(cardView.transform.rotation.y - rotation.y) > .01f)
            {
                cardView.transform.position = Vector3.MoveTowards(cardView.transform.position, position, cardSettings.CardMoveSeedInHand * Time.deltaTime);

                //cardView.transform.Rotate();
                await UniTask.WaitForEndOfFrame(this);
            }
        }

        public void DiscardCard(Card card)
        {

        }
    }
}