using UnityEngine;
using cards;

namespace fight
{
    internal class HoverManager : MonoBehaviour
    {
        public Card currentCard;
        Card previousCard;
        CardHandMovementManager _cardHandMovementManager;

        const float SCALE_AMOUNT = 1.5f;
        const float MOVE_SPEED = 8f;

        bool resetRequired = false;

        public void Initialize(CardHandMovementManager CHMM, Card card){
            currentCard = card;
            _cardHandMovementManager = CHMM;
        }
        private void Update()
        {
            /*
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100))
            {
                //if mousing over a card set that as the current card
                if (hit.transform.gameObject.GetComponent(typeof(Card)))
                {
                    currentCard = hit.transform.gameObject.GetComponent(typeof(Card)) as Card;

                    //if the previous card is not the one currently mousing over, reset the hand to its normal positions
                    if (currentCard != previousCard)
                    {
                        ResetScale();
                        HoverCardEffects(currentCard);
                        resetRequired = true;
                    }
                    previousCard = currentCard;
                }
                //if not mousing over a card, reset the hand
                else if (resetRequired)
                {
                    //resetRequired flag ensures the hand is only reset once when the mouse is not mousing over a card
                    //and not every update frame
                    currentCard = previousCard = null;
                    ResetHand();
                    resetRequired = false;
                }
            }
            */
        }

        public void HoverCardEffects()
        {
            var card = currentCard;
            var hand = _cardHandMovementManager.GetComponent<FightManager>().GetPlayer()._playerCardDecks.Hand;
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
                    Vector3 p = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0, Camera.main.nearClipPlane));
                    card.transform.position = new Vector3(_cardHandMovementManager.curve.GetPoint(CardHandUtils.ReturnCardPosition(hand.Count, cardposition + 1)).x, p.y - 2f, card.transform.position.z - .1f);
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
            var hand = this.GetComponent<FightManager>().GetPlayer()._playerCardDecks.Hand;

            StopAllCoroutines();
            _cardHandMovementManager.StopAllCoroutines();

            StartCoroutine(_cardHandMovementManager.CreateHandCurve());

            ResetScale();
        }

        void ResetScale(){

            var hand = this.GetComponent<FightManager>().GetPlayer()._playerCardDecks.Hand;

            for (int i = 0; i < hand.Count; i++)
            {
                hand[i].transform.localScale = CardInfo.DEFAULT_SCALE;
            }
        }

        void OnDestroy() {
            //ResetHand();
            //ResetScale();
        }
    }
}