using System.Collections.Generic;
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

        void Start()
        {
            hand = new List<Card>();
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
            UpdateHand();
        }
        internal void UpdateHand()
        {
            //failsale
            if (hand.Count < 1) return;

            for (int i = 1; i <= hand.Count; i++)
            {
                Vector3 NewPosition = curve.GetPoint(ReturnCardPositions(hand.Count, i));
                NewPosition.z -= (float)i/100f;
                MoveCard(hand[i - 1], NewPosition , ReturnCardRotation(hand.Count, i), cardMoveSpeed);
            }
        }
        internal void MoveCard(Card card, Vector3 newPosition, float rotationAmount, float speed)
        {
            CardMover _CardMover = card.gameObject.AddComponent<CardMover>();
            _CardMover.Initialize(card, newPosition, rotationAmount, speed);
        }
        internal float ReturnCardRotation(int HandSize, int CardPosition)
        {
            //Returns the amount of rotation on the x axis on which the card will rotate
            float MaxAngle = 2.5f * HandSize;
            float MinAngle = 1f;

            float Rotation;
            int NumToNormalize = CardPosition;
            
            float Midpoint = (HandSize / 2) + 1;
            Computations _comp = new Computations();

            if (CardPosition == (int)Midpoint && ((HandSize%2) != 0)) Rotation = 0f;
            else if (CardPosition > Midpoint -1)
            {
                NumToNormalize = CardPosition - (int)Midpoint;
                Rotation = _comp.Normalize((float)NumToNormalize, 0f, (float)Midpoint, MinAngle, MaxAngle);
            }
            else
            {
                Rotation = _comp.Normalize((float)NumToNormalize, 0f, (float)Midpoint, MinAngle, MaxAngle);
                Rotation = MaxAngle - Rotation;
                Rotation *= -1f;
            }
            return Rotation;
        }
        internal float ReturnCardPositions(int HandSize, int CardPosition)
        {
           //returns a position between 0 and 1 for the bezier curve
            float result = 0.5f;

            switch (HandSize)
            {
                case 1:
                    result = 0.5f;
                    break;
                case 2:
                    switch (CardPosition)
                    {
                        case 1:
                            result = 0.42f;
                            break;
                        case 2:
                            result = 0.58f;
                            break;
                    }
                    break;
                case 3:
                    switch (CardPosition)
                    {
                        case 1:
                            result = 0.35f;
                            break;
                        case 2:
                            result = 0.50f;
                            break;
                        case 3:
                            result = 0.65f;
                            break;
                    }
                    break;
                case 4:
                    switch (CardPosition)
                    {
                        case 1:
                            result = 0.26f;
                            break;
                        case 2:
                            result = 0.42f;
                            break;
                        case 3:
                            result = 0.58f;
                            break;
                        case 4:
                            result = 0.74f;
                            break;
                    }
                    break;
                case 5:
                    switch (CardPosition)
                    {
                        case 1:
                            result = 0.20f;
                            break;
                        case 2:
                            result = 0.35f;
                            break;
                        case 3:
                            result = 0.50f;
                            break;
                        case 4:
                            result = 0.65f;
                            break;
                        case 5:
                            result = 0.80f;
                            break;
                    }
                    break;
                case 6:
                    switch (CardPosition)
                    {
                        case 1:
                            result = 0.20f;
                            break;
                        case 2:
                            result = 0.32f;
                            break;
                        case 3:
                            result = 0.44f;
                            break;
                        case 4:
                            result = 0.56f;
                            break;
                        case 5:
                            result = 0.68f;
                            break;
                        case 6:
                            result = 0.80f;
                            break;
                    }
                    break;
                case 7:
                    switch (CardPosition)
                    {
                        case 1:
                            result = 0.20f;
                            break;
                        case 2:
                            result = 0.30f;
                            break;
                        case 3:
                            result = 0.40f;
                            break;
                        case 4:
                            result = 0.50f;
                            break;
                        case 5:
                            result = 0.60f;
                            break;
                        case 6:
                            result = 0.70f;
                            break;
                        case 7:
                            result = 0.80f;
                            break;
                    }
                    break;
                case 8:
                    switch (CardPosition)
                    {
                        case 1:
                            result = 0.15f;
                            break;
                        case 2:
                            result = 0.25f;
                            break;
                        case 3:
                            result = 0.35f;
                            break;
                        case 4:
                            result = 0.45f;
                            break;
                        case 5:
                            result = 0.55f;
                            break;
                        case 6:
                            result = 0.65f;
                            break;
                        case 7:
                            result = 0.75f;
                            break;
                        case 8:
                            result = 0.85f;
                            break;
                    }
                    break;
                case 9:
                    switch (CardPosition)
                    {
                        case 1:
                            result = 0.10f;
                            break;
                        case 2:
                            result = 0.20f;
                            break;
                        case 3:
                            result = 0.30f;
                            break;
                        case 4:
                            result = 0.40f;
                            break;
                        case 5:
                            result = 0.50f;
                            break;
                        case 6:
                            result = 0.60f;
                            break;
                        case 7:
                            result = 0.70f;
                            break;
                        case 8:
                            result = 0.80f;
                            break;
                        case 9:
                            result = 0.90f;
                            break;
                    }
                    break;
                case 10:
                    switch (CardPosition)
                    {
                        case 1:
                            result = 0.05f;
                            break;
                        case 2:
                            result = 0.15f;
                            break;
                        case 3:
                            result = 0.25f;
                            break;
                        case 4:
                            result = 0.35f;
                            break;
                        case 5:
                            result = 0.45f;
                            break;
                        case 6:
                            result = 0.55f;
                            break;
                        case 7:
                            result = 0.65f;
                            break;
                        case 8:
                            result = 0.75f;
                            break;
                        case 9:
                            result = 0.85f;
                            break;
                        case 10:
                            result = 0.95f;
                            break;
                    }
                    break;
            }
            return result;
        }
    }
}