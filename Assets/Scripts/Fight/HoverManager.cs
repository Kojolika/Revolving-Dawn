using UnityEngine;
using cards;

namespace fight
{
    internal class HoverManager : MonoBehaviour
    {
        public Card currentCard;
        Card previousCard;
        CardHandMovementManager _cardHandMovementManager;

        static float SCALE_AMOUNT = 1.5f;
        static Vector3 CARD_SCALE;
        const float MOVE_SPEED = 8f;
        bool resetRequired = false;
        bool isEnabled = false;
        bool IsHandUpdating;

        void Start()
        {
            CARD_SCALE = new Vector3(0.2f, 1, 0.3f);
            _cardHandMovementManager = this.gameObject.GetComponent<CardHandMovementManager>();

            _cardHandMovementManager.TriggerIsHandUpdating += HandUpdateEventHandler;
        }

        public void Enable(bool value)
        {
            isEnabled = value;
        }

        public void HandUpdateEventHandler(bool isUpdating){
            IsHandUpdating = isUpdating;
        }

        private void Update()
        {
            if (!isEnabled) return;
            if(IsHandUpdating) return;
            
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
                        //ResetHand();
                        ResetScale();
                        HoverCardEffects(currentCard);
                        resetRequired = true;
                    }
                    previousCard = currentCard;
                }
                //if not mousing over a card, reset the hand
                else
                {
                    if (resetRequired)
                    {
                        currentCard = previousCard = null;
                        ResetHand();
                        resetRequired = false;
                    }
                }
            }
        }

        private void HoverCardEffects(Card card)
        {
            var hand = this.GetComponent<FightManager>().GetPlayer()._playerCardDecks.Hand;
            int cardposition = hand.IndexOf(card);
            float moveAmount;
            int positionDifference;

            StopAllCoroutines();
            _cardHandMovementManager.StopAllCoroutines();
            for (int i = 0; i < hand.Count; i++)
            {
                if (i == cardposition)
                {
                    //Selected card will be centered
                    card.transform.rotation  = Quaternion.Euler(new Vector3(90f, 90f, -90f));
                    Vector3 p = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0, Camera.main.nearClipPlane));
                    StartCoroutine(_cardHandMovementManager.MoveCardCoroutine(card,
                        new Vector3(_cardHandMovementManager.curve.GetPoint(CardHandUtils.ReturnCardPosition(hand.Count, cardposition + 1)).x, p.y - 2f, card.transform.position.z - .1f),
                        0,
                        50f));
                    card.transform.localScale = new Vector3(0.2f, 1, 0.3f) * SCALE_AMOUNT;
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
                    MOVE_SPEED));
            }

        }
        void ResetHand()
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
                hand[i].transform.localScale = CARD_SCALE; 
            }
        }
    }
}