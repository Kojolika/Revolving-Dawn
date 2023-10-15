using UnityEngine;
using Cards;
using Characters;
using Fight;

namespace Systems.Managers
{
    public class HoverManager : MonoBehaviour
    {
        public Card3D currentCard;
        CardHandManager _cardHandMovementManager;
        public Camera cardCam;

        const float SCALE_AMOUNT = 1.5f;
        const float MOVE_SPEED_HOVER = 8f;
        public const float MOVE_SPEED_RESET = 35f;

        public void Initialize(CardHandManager cardHandManager, Card3D card){
            currentCard = card;
            _cardHandMovementManager = cardHandManager;
        }
        public void HoverCardEffects()
        {
            var card = currentCard;
            var hand = PlayerCardDecksManager.Hand;
            int cardposition = hand.IndexOf(card);
            float moveAmount;
            float positionDifference;
            float selectedCardHoverHeight = 2.25f;

            StopAllCoroutines();
            _cardHandMovementManager.StopAllCoroutines();
            for (int i = 0; i < hand.Count; i++)
            {
                if (i == cardposition)
                {
                    //Selected card will perfectly straight, moved so the text is in view of the screen clear,
                    //and scaled up for better visability
                    card.transform.rotation  = Quaternion.Euler(CardConfiguration.DEFAULT_CARD_ROTATION);
                    Vector3 p = cardCam.ViewportToWorldPoint(new Vector3(0.5f, 0, cardCam.nearClipPlane));
                    card.transform.position = new Vector3(_cardHandMovementManager.curve.GetPoint(CardHandUtils.ReturnCardPosition(hand.Count, cardposition + 1)).x, p.y + selectedCardHoverHeight, card.transform.position.z - 1f);
                    card.transform.localScale = CardConfiguration.DEFAULT_SCALE * SCALE_AMOUNT;
                    continue;
                }

                //Move Cards relative to their position of the selected card
                //i.e. cards closer more farther away
                positionDifference = (1.75f) / (i - cardposition);
                moveAmount = CardHandUtils.ReturnCardPosition(hand.Count, i + 1) + (positionDifference) * .05f;
 
                //turn curve point into vector space
                Vector3 NewPosition = _cardHandMovementManager.curve.GetPoint(moveAmount);

                //Add a small z value so cards on the right area always slightly infront of the left card
                //Gives a sense of realism to the card hand
                NewPosition.z -= (float)i * 0.5f;

                StartCoroutine(_cardHandMovementManager.MoveCardCoroutine(hand[i],
                    NewPosition,
                    CardHandUtils.ReturnCardRotation(hand.Count, i + 1),
                    MOVE_SPEED_HOVER
                ));
            }
        }
        public void ResetHand(float speed)
        {
            StopAllCoroutines();
            _cardHandMovementManager.StopAllCoroutines();

            StartCoroutine(_cardHandMovementManager.CreateHandCurve(speed));

            ResetScale();
        }

        public void ResetScale(){

            var hand = PlayerCardDecksManager.Hand;

            for (int i = 0; i < hand.Count; i++)
            {
                hand[i].transform.localScale = CardConfiguration.DEFAULT_SCALE;
            }
        }
    }
}