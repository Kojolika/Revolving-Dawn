using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using cards;
using utils;

namespace fight
{
    public class CardHandManager : MonoBehaviour
    {
        internal List<Card> hand;
        internal BezierCurve curve;
        GameObject cardSpawner;
        GameObject cardDiscarder;
        float cardMoveSpeed = 35f;
        bool isUpdating = false;

        void Start()
        {
            hand = new List<Card>();
        }
        public bool IsUpdating(){
            return isUpdating;
        }
        public void Initialize(BezierCurve curve, GameObject cardspawner, GameObject carddiscarder)
        {
            this.curve = curve;
            cardSpawner = cardspawner;
            cardDiscarder = carddiscarder;
        }
        public void Discard(Card card)
        {
            try {hand.Remove(card);}
            catch
            {
                Debug.Log("Error: Card not in hand.");
                throw;
            }
        }
        public void Draw(Card card)
        {
            Card CardDrawn = Instantiate(card,
                cardSpawner.transform.position,
                card.transform.rotation);
            hand.Add(CardDrawn);
            StartCoroutine(MoveCardsToHandCurve());
        }
        internal IEnumerator MoveCardsToHandCurve()
        {
            //failsale
            if (hand.Count < 1) yield return null;

            while(isUpdating) yield return null;

            for (int i = 1; i <= hand.Count; i++)
            {
                Vector3 NewPosition = curve.GetPoint(CardHandUtils.ReturnCardPosition(hand.Count, i));
                NewPosition.z -= (float)i/100f;
                StartCoroutine(MoveCardCoroutine(hand[i - 1],
                    NewPosition,
                    CardHandUtils.ReturnCardRotation(hand.Count, i),
                    cardMoveSpeed));
            }
        }
        public IEnumerator MoveCardCoroutine(Card card, Vector3 newPosition, float cardRotation, float speed)
        {
            card.transform.rotation = Quaternion.Euler(new Vector3(90f + cardRotation, 90f, -90f));
            while(card.transform.position != newPosition)
            {
                isUpdating = true;
                card.transform.position = Vector3.MoveTowards(card.transform.position, newPosition, speed * Time.deltaTime);
                yield return null;
            }
            isUpdating = false;
        }
    }
}