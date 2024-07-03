using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using PrimeTween;
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
        private List<Sequence> moveTweens = new();

        [Zenject.Inject]
        private void Construct(CardView.Factory cardViewFactory, ManaPoolView manaPoolView, CardSettings cardSettings)
        {
            this.cardViewFactory = cardViewFactory;
            this.manaPoolView = manaPoolView;
            manaPoolView.transform.position = manaPoolViewLocation.position;
            hand = new List<CardView>();
            this.cardSettings = cardSettings;
        }


        public void DrawCard(CardModel cardModel)
        {
            var newCardView = cardViewFactory.Create(cardModel);
            newCardView.transform.position = cardSpawnLocation.position;
            newCardView.transform.SetParent(handParent);
            hand.Add(newCardView);

            CreateHandCurve();
        }

        public void DiscardCard(CardModel card)
        {

        }


        private void CreateHandCurve()
        {
            CleanMoveTweens();

            var handSize = hand.Count;
            for (int i = 0; i < handSize; i++)
            {
                var pointOnCurve = GetCardPosition(handSize, i + 1);

                var newPosition = handCurve.GetPoint(pointOnCurve);
                newPosition.z -= i * .5f;

                var newRotation = new Vector3(0f, 0f, GetZRotationForCard(handSize, i + 1));

                moveTweens.Add(MoveCard(hand[i], newPosition, newRotation));
            }
        }

        private Sequence MoveCard(CardView cardView, Vector3 position, Vector3 rotation)
        {
            return Sequence.Create()
                .Group(Tween.PositionAtSpeed(cardView.transform, position, cardSettings.CardMoveSpeedInHand, ease: cardSettings.CardMoveFunction))
                .Group(Tween.RotationAtSpeed(cardView.transform, Quaternion.Euler(rotation), cardSettings.CardMoveSpeedInHand, ease: cardSettings.CardMoveFunction));
        }

        private void CleanMoveTweens()
        {
            foreach (var moveTween in moveTweens)
            {
                moveTween.Stop();
            }

            moveTweens.Clear();
        }

        private void HoverCard(CardView cardView)
        {
            CleanMoveTweens();
            var cardIndex = hand.IndexOf(cardView);
            for (int i = 0; i < hand.Count; i++)
            {
                if (i == cardIndex)
                {
                    // Selected card will perfectly straight, moved so the text is in view of the screen clear,
                    // and scaled up for better visibility
                    cardView.transform.rotation = Quaternion.Euler(Vector3.zero);
                    Vector3 p = handViewCamera.ViewportToWorldPoint(new Vector3(0.5f, 0, handViewCamera.nearClipPlane));
                    cardView.transform.position = new Vector3(
                        handCurve.GetPoint(GetCardPosition(hand.Count, i + 1)).x,
                        p.y,
                        cardView.transform.position.z - 1f);
                    cardView.Focus();
                    continue;
                }

                cardView.UnFocus();
                // Move Cards relative to their position of the selected card
                // i.e. cards closer more farther away
                float positionDifference = 1.75f / (i - cardIndex);
                float moveAmount = GetCardPosition(hand.Count, i + 1) + (positionDifference) * .05f;

                //turn curve point into vector space
                Vector3 newPosition = handCurve.GetPoint(moveAmount);

                // Add a small z value so cards on the right area always slightly in front of the left card
                // Gives a sense of realism to the card hand
                newPosition.z -= i * 0.5f;

                MoveCard(hand[i],
                    newPosition,
                    new Vector3(0f, 0f, GetZRotationForCard(cardIndex, i + 1))
                );
            }
        }

        /// <summary>
        /// Returns the angle of rotation of the card given its position in the hand.
        /// </summary>
        /// <param name="handSize">size of the card hand</param>
        /// <param name="cardPosition">1-indexed position of the card in the hand</param>
        private float GetZRotationForCard(int handSize, int cardPosition)
        {
            //Returns the amount of rotation on the x axis on which the card will rotate
            float maxAngle = 2.5f * handSize;
            float minAngle = 1f;

            float rotation;
            int numToNormalize = cardPosition;

            float midpoint = (handSize / 2) + 1;

            if (cardPosition == (int)midpoint && ((handSize % 2) != 0)) rotation = 0f;
            else if (cardPosition > midpoint - 1)
            {
                numToNormalize = cardPosition - (int)midpoint;
                rotation = Computations.Normalize((float)numToNormalize, 0f, (float)midpoint, minAngle, maxAngle);
            }
            else
            {
                rotation = Computations.Normalize((float)numToNormalize, 0f, (float)midpoint, minAngle, maxAngle);
                rotation = maxAngle - rotation;
                rotation *= -1f;
            }
            return -rotation;
        }

        /// <summary>
        /// Returns the x position from 0,1 for a card given its position and hand size.
        /// </summary>
        /// <param name="handSize">size of the hand</param>
        /// <param name="cardPosition">1-indexed position of the card in the hand</param>
        /// <returns></returns>
        private float GetCardPosition(int handSize, int cardPosition)
        {
            //returns a position between 0 and 1 for the bezier curve
            float result = 0.5f;

            switch (handSize)
            {
                case 1:
                    result = 0.5f;
                    break;
                case 2:
                    switch (cardPosition)
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
                    switch (cardPosition)
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
                    switch (cardPosition)
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
                    switch (cardPosition)
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
                    switch (cardPosition)
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
                    switch (cardPosition)
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
                    switch (cardPosition)
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
                    switch (cardPosition)
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
                    switch (cardPosition)
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