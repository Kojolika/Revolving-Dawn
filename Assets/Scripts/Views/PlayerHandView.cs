using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PrimeTween;
using Mana;
using Models;
using Settings;
using UnityEngine;
using Utils;
using Fight.Animations;
using Fight;
using Tooling.Logging;
using System.Linq;
using Models.Cards;

namespace Views
{
    public class PlayerHandView : MonoBehaviour
    {
        [SerializeField] Camera      handViewCamera;
        [SerializeField] Transform   handParent;
        [SerializeField] Transform   cardSpawnLocation;
        [SerializeField] Transform   cardDiscardLocation;
        [SerializeField] Transform   manaPoolViewLocation;
        [SerializeField] BezierCurve handCurve;

        private          CardView.Factory       cardViewFactory;
        private          PlayerHandViewSettings playerHandViewSettings;
        private readonly List<Sequence>         currentMoveTweens = new();

        private const float ApproxDistanceEquals = 0.1f;

        public  Dictionary<Card, CardView> CardViewsLookup { get; private set; }
        private List<CardView>             orderedCardViews;
        public  BattleEngine               BattleEngine          { get; private set; }
        public  BattleAnimationEngine      BattleAnimationEngine { get; private set; }
        public  Camera                     Camera                => handViewCamera;

        [Zenject.Inject]
        private void Construct(
            CardView.Factory       cardViewFactory,
            PlayerHandViewSettings playerHandViewSettings,
            BattleEngine           battleEngine,
            BattleAnimationEngine  battleAnimationEngine)
        {
            this.cardViewFactory        = cardViewFactory;
            CardViewsLookup             = new();
            orderedCardViews            = new();
            this.playerHandViewSettings = playerHandViewSettings;
            BattleEngine                = battleEngine;
            BattleAnimationEngine       = battleAnimationEngine;
        }

        public async UniTask DrawCard(Card cardModel)
        {
            var newCardView = cardViewFactory.Create(cardModel);
            newCardView.transform.position = cardSpawnLocation.position;
            newCardView.transform.SetParent(handParent);
            CardViewsLookup.Add(cardModel, newCardView);
            orderedCardViews.Add(newCardView);

            await CreateHandCurveAnimation(playerHandViewSettings.CardDrawMoveSpeed,
                                           playerHandViewSettings.CardDrawRotateSpeed, playerHandViewSettings.CardDrawMoveFunction);
        }

        public async UniTask PlayCardAnimation(Card card)
        {
            if (!CardViewsLookup.TryGetValue(card, out var cardView))
            {
                return;
            }

            if (!cardView.Model.StaticData.IsLostOnPlay)
            {
                await DiscardCardAnimation(card);
            }
            else
            {
                await LoseCardAnimation(cardView);
            }
        }

        public void RemoveCardFromHand(Card cardModel)
        {
            CardViewsLookup[cardModel].Collider.enabled = false;

            if (!CardViewsLookup.Remove(cardModel))
            {
                MyLogger.LogError($"Trying to set a card that doesn't exist in the hand.");
            }

            orderedCardViews.Remove(
                orderedCardViews.First(cardView => cardView.Model == cardModel)
            );
        }

        public async UniTask DiscardCardAnimation(Card card)
        {
            if (!CardViewsLookup.TryGetValue(card, out var cardView))
            {
                return;
            }

            var discardCardSeq = Sequence.Create();

            _ = discardCardSeq.Insert(
                playerHandViewSettings.CardPlayAnimationDuration * .1f,
                Tween.Position(
                    cardView.transform,
                    cardDiscardLocation.position,
                    playerHandViewSettings.CardPlayAnimationDuration,
                    ease: playerHandViewSettings.CardPlayEaseFunction)
            );
            _ = discardCardSeq.Insert(
                0f,
                Tween.Rotation(
                    cardView.transform,
                    new Vector3(cardView.transform.rotation.x, cardView.transform.rotation.y,
                                cardView.transform.rotation.z - 90f),
                    playerHandViewSettings.CardPlayAnimationDuration,
                    ease: playerHandViewSettings.CardPlayEaseFunction)
            );

            _ = discardCardSeq.Insert(
                0f,
                Tween.Scale(
                    cardView.transform,
                    new Vector3(0.2f, 0.2f, 0.2f),
                    playerHandViewSettings.CardPlayAnimationDuration,
                    ease: playerHandViewSettings.CardPlayEaseFunction)
            );

            await UniTask.WaitWhile(() => discardCardSeq.isAlive);

            Destroy(cardView);
        }

        public async UniTask LoseCardAnimation(CardView cardView)
        {
            var loseCardSeq = Sequence.Create();

            _ = loseCardSeq.Insert(
                0f,
                Tween.Position(
                    cardView.transform,
                    new Vector3(cardView.transform.position.x, cardView.transform.position.y + 3,
                                cardView.transform.position.z),
                    0.1f,
                    ease: playerHandViewSettings.CardPlayEaseFunction)
            );

            await UniTask.WaitWhile(() => loseCardSeq.isAlive);

            Destroy(cardView);
        }

        public async UniTask CreateHandCurveAnimation(float cardSpeed, float cardRotateSpeed, Ease easeFunction)
        {
            ClearMoveTweens();

            var handSize  = orderedCardViews.Count;
            var moveTasks = new UniTask[handSize];
            for (int i = 0; i < handSize; i++)
            {
                var pointOnCurve = GetCardPosition(handSize, i + 1);

                var newPosition = handCurve.GetPoint(pointOnCurve);
                newPosition.z -= i * .5f;

                var newRotation = GetCardRotationForHandSizeAndPosition(handSize, i);

                moveTasks[i] = MoveCard(orderedCardViews[i], newPosition, newRotation, cardSpeed, cardRotateSpeed, easeFunction);
            }

            await UniTask.WhenAll(moveTasks);
        }

        private async UniTask MoveCard(
            CardView cardView,
            Vector3  position,
            Vector3  rotation,
            float    cardSpeed,
            float    cardRotateSpeed,
            Ease     easeFunction)
        {
            var moveCardSeq = Sequence.Create();

            if (Vector3.Distance(cardView.transform.position, position) > ApproxDistanceEquals)
            {
                _ = moveCardSeq.Insert(
                    0f,
                    Tween.Position(
                        cardView.transform,
                        position,
                        cardSpeed,
                        ease: easeFunction));
            }

            if (cardView.transform.rotation.eulerAngles != rotation)
            {
                _ = moveCardSeq.Insert(
                    0f,
                    Tween.Rotation(
                        cardView.transform,
                        Quaternion.Euler(rotation),
                        cardRotateSpeed,
                        ease: easeFunction));
            }

            if (cardView.transform.localScale != cardView.DefaultScale)
            {
                _ = moveCardSeq.Insert(
                    0f,
                    Tween.Scale(
                        cardView.transform,
                        cardView.DefaultScale,
                        playerHandViewSettings.ScaleAnimationDuration,
                        ease: easeFunction));
            }

            currentMoveTweens.Add(moveCardSeq);

            await UniTask.WaitWhile(() => moveCardSeq.isAlive);
        }

        private void ClearMoveTweens()
        {
            foreach (var moveTween in currentMoveTweens)
            {
                moveTween.Stop();
            }

            currentMoveTweens.Clear();
        }

        public async UniTask HoverCard(CardView cardView)
        {
            ClearMoveTweens();
            var cardIndex = orderedCardViews.IndexOf(cardView);
            var handSize  = orderedCardViews.Count;
            var moveTasks = new UniTask[handSize];
            for (int i = 0; i < orderedCardViews.Count; i++)
            {
                if (i == cardIndex)
                {
                    // Selected card will perfectly straight, moved so the text is in view of the screen clear,
                    // and scaled up for better visibility
                    SetSelectionEffects(cardView);

                    await UniTask.WaitForEndOfFrame(cardView);

                    cardView.transform.rotation = Quaternion.Euler(Vector3.zero);

                    Vector3 bottomOfScreen      = handViewCamera.ViewportToWorldPoint(new Vector3(0, 0));
                    var     currentCardPosition = cardView.transform.position;
                    cardView.transform.position = new Vector3(
                        handCurve.GetPoint(GetCardPosition(orderedCardViews.Count, i + 1)).x,
                        bottomOfScreen.y + orderedCardViews[i].CardBorderRenderer.bounds.extents.y,
                        currentCardPosition.z - 1f);

                    continue;
                }

                UnsetSelectionEffects(orderedCardViews[i]);

                // Move Cards relative to their position of the selected card
                // i.e. cards closer more farther away
                float positionDifference = 1.75f / (i - cardIndex);
                float moveAmount         = GetCardPosition(orderedCardViews.Count, i + 1) + positionDifference * .05f;

                // turn curve point into vector space
                Vector3 newPosition = handCurve.GetPoint(moveAmount);

                // Add a small z value so cards on the right area always slightly in front of the left card
                // Gives a sense of realism to the card hand
                newPosition.z -= i * 0.5f;

                moveTasks[i] = MoveCard(
                    orderedCardViews[i],
                    newPosition,
                    GetCardRotationForHandSizeAndPosition(handSize, i),
                    playerHandViewSettings.CardHoverMoveSpeedInHand,
                    playerHandViewSettings.CardHoverRotateSpeedInHand,
                    playerHandViewSettings.CardHoverMoveFunction
                );
            }

            await UniTask.WhenAll(moveTasks);
        }

        public void SetSelectionEffects(CardView cardView)
        {
            cardView.transform.localScale = cardView.DefaultScale * playerHandViewSettings.CardHoverScaleFactor;
        }

        public void UnsetSelectionEffects(CardView cardView)
        {
            cardView.transform.localScale = cardView.DefaultScale;
        }

        private Vector3 GetCardRotationForHandSizeAndPosition(int handSize, int cardPosition)
        {
            // We want to rotate the cards more when the hand size is smaller
            float sizeMultiplier = -1 / (float)handSize;

            // Edit this to change how much the cards should be rotated
            float rotationMultiplier = playerHandViewSettings.CardHandRotationModifier;

            // The rotation is determined based on the midpoint
            int midpoint = handSize / 2;

            // We need to mirror the rotation on the either side of the midpoint
            cardPosition -= midpoint;

            // Since cardHands with an even amount dont have a single card midpoint
            // we need to shift all rotations to center them    
            float evenModifier = 0f;
            if (handSize % 2 == 0)
            {
                evenModifier = rotationMultiplier / -2f;
            }

            float rotation = handSize * cardPosition * sizeMultiplier * rotationMultiplier + evenModifier;

            return new Vector3(0f, 0f, rotation);
        }

        /// <summary>
        /// Returns the position from 0,1 for a card given its position and hand size.
        /// </summary>
        /// <param name="handSize">size of the hand</param>
        /// <param name="cardPosition">1-indexed position of the card in the hand</param>
        /// <returns> Returns a position between 0 and 1 for the bezier curve.</returns>
        private float GetCardPosition(int handSize, int cardPosition)
        {
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