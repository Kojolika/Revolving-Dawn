using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Fight;
using Mana;
using Models;
using Settings;
using Tooling.Logging;
using UnityEngine;
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


        public void DrawCards(List<CardModel> cards)
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
                MyLogger.Log($"Moving card...");
                await task;
                MyLogger.Log($"Done moving...");
            }
        }

        private async UniTask MoveCard(CardView cardView, Vector3 position, Vector3 rotation)
        {
            while (Vector3.Distance(cardView.transform.position, position) >= 0.01f)
            {
                cardView.transform.position = Vector3.MoveTowards(cardView.transform.position, position, cardSettings.CardMoveSeedInHand * Time.deltaTime);

                var current = cardView.transform.eulerAngles;
                cardView.transform.eulerAngles = new Vector3(
                    Mathf.MoveTowardsAngle(current.x, rotation.x, cardSettings.CardMoveSeedInHand * Time.deltaTime), 
                    Mathf.MoveTowardsAngle(current.y, rotation.y, cardSettings.CardMoveSeedInHand * Time.deltaTime), 
                    Mathf.MoveTowardsAngle(current.z, rotation.z, cardSettings.CardMoveSeedInHand * Time.deltaTime)
                );

                await UniTask.WaitForEndOfFrame(this);
            }
        }

        public void DiscardCard(CardModel card)
        {

        }
    }
}