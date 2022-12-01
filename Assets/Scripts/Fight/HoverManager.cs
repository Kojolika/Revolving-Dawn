using UnityEngine;
using cards;

namespace fight
{
    internal class HoverManager : MonoBehaviour
    {
        public Card currentCard;
        CardHandManager _cardHandMovementManager;
        public Camera cardCam;

        const float SCALE_AMOUNT = 2f;
        const float MOVE_SPEED = 8f;

        bool resetRequired = false;

        public void Initialize(CardHandManager CHMM, Card card){
            currentCard = card;
            _cardHandMovementManager = CHMM;
        }
        public void HoverCardEffects()
        {
            var card = currentCard;
            var hand = _cardHandMovementManager.GetComponent<FightManager>().GetPlayer().playerCardDecks.Hand;
            int cardposition = hand.IndexOf(card);
            float moveAmount;
            int positionDifference;

            StopAllCoroutines();
            _cardHandMovementManager.StopAllCoroutines();
            for (int i = 0; i < hand.Count; i++)
            {
                if (i == cardposition)
                {
                    //Selected card will perfectly straight, moved so the text is in view of the screen clear,
                    //and scaled up for better visability
                    card.transform.rotation  = Quaternion.Euler(CardInfo.DEFAULT_CARD_ROTATION);
                    Vector3 p = cardCam.ViewportToWorldPoint(new Vector3(0.5f, 0, Camera.main.nearClipPlane));
                    card.transform.position = new Vector3(_cardHandMovementManager.curve.GetPoint(CardHandUtils.ReturnCardPosition(hand.Count, cardposition + 1)).x, p.y - 2.5f, card.transform.position.z - .1f);
                    card.transform.localScale = CardInfo.DEFAULT_SCALE * SCALE_AMOUNT;
                    continue;
                }

                //Move Cards relative to their position of the selected card
                //i.e. cards closer more farther away
                if (i - cardposition == 0) positionDifference = 0;
                else positionDifference = 2 / (i - cardposition);
                moveAmount = CardHandUtils.ReturnCardPosition(hand.Count, i + 1) + positionDifference * .03f;

                //turn curve point into vector space
                Vector3 NewPosition = _cardHandMovementManager.curve.GetPoint(moveAmount);

                //Add a small z value so cards on the right area always slightly infront of the left card
                //Gives a sense of realism to the card hand
                NewPosition.z -= (float)i / 100f;

                StartCoroutine(_cardHandMovementManager.MoveCardCoroutine(hand[i],
                    NewPosition,
                    CardHandUtils.ReturnCardRotation(hand.Count, i + 1),
                    MOVE_SPEED
                ));
            }
        }
        public void ResetHand()
        {
            var hand = this.GetComponent<FightManager>().GetPlayer().playerCardDecks.Hand;

            StopAllCoroutines();
            _cardHandMovementManager.StopAllCoroutines();

            StartCoroutine(_cardHandMovementManager.CreateHandCurve());

            ResetScale();
        }

        public void ResetScale(){

            var hand = this.GetComponent<FightManager>().GetPlayer().playerCardDecks.Hand;

            for (int i = 0; i < hand.Count; i++)
            {
                hand[i].transform.localScale = CardInfo.DEFAULT_SCALE;
            }
        }
    }
}