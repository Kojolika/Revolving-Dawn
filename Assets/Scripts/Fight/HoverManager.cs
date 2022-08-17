using UnityEngine;
using cards;

namespace fight
{
    internal class HoverManager : MonoBehaviour
    {
        public Card currentCard;
        Card previousCard;
        CardHandManager _cardHandManager;

        static Vector3 CARD_SCALE;
        bool resetRequired = false;
        bool isEnabled = false;
        const float MOVE_SPEED = 8f;

        void Start()
        {
            CARD_SCALE = new Vector3(0.2f, 1, 0.3f);
            _cardHandManager = this.gameObject.GetComponent<CardHandManager>();
        }
        public void Enable(bool value)
        {
            isEnabled = value;
        }
        private void Update()
        {
            if (!isEnabled) return;

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
                        ResetHand();
                        HoverCardEffects(currentCard);
                        resetRequired = false;
                    }
                    previousCard = currentCard;
                }
                //if not mousing over a card, reset the hand
                else
                {
                    if (!resetRequired)
                    {
                        currentCard = previousCard = null;
                        ResetHand();
                        resetRequired = true;
                    }
                }
            }
        }
        private void HoverCardEffects(Card card)
        {
            int cardposition = _cardHandManager.hand.IndexOf(card);
            float moveAmount;
            int positionDifference;
            const float SCALE_AMOUNT = 1.5f;


            for (int i = 0; i < _cardHandManager.hand.Count; i++)
            {
                if (i == cardposition)
                {
                    //Selected card will be centered
                    card.transform.rotation  = Quaternion.Euler(new Vector3(90f, 90f, -90f));
                    Vector3 p = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0, Camera.main.nearClipPlane));
                    StartCoroutine(_cardHandManager.MoveCardCoroutine(card,
                        new Vector3(_cardHandManager.curve.GetPoint(CardHandUtils.ReturnCardPosition(_cardHandManager.hand.Count, cardposition + 1)).x, p.y - 2f, card.transform.position.z - .1f),
                        0,
                        50f));
                    card.transform.localScale = new Vector3(0.2f, 1, 0.3f) * SCALE_AMOUNT;
                    continue;
                }


                if (_cardHandManager.hand[i].GetComponent<CardMover>() != null)
                    Destroy(_cardHandManager.hand[i].GetComponent<CardMover>());

                //Move Cards relative to their position of the selected card
                //i.e. cards closer more farther away
                if (i - cardposition == 0) positionDifference = 0;
                else positionDifference = 2 / (i - cardposition);
                moveAmount = CardHandUtils.ReturnCardPosition(_cardHandManager.hand.Count, i + 1) + positionDifference * .03f;

                //turn curve point into vector space
                Vector3 NewPosition = _cardHandManager.curve.GetPoint(moveAmount);

                //Add a small z value so cards on the right area always slightly infront of the left card
                //Gives a sense of realism to the card hand
                NewPosition.z -= (float)i / 100f;

                StartCoroutine(_cardHandManager.MoveCardCoroutine(_cardHandManager.hand[i],
                    NewPosition,
                    CardHandUtils.ReturnCardRotation(_cardHandManager.hand.Count, i + 1),
                    MOVE_SPEED));
            }

        }
        private void ResetHand()
        {
            //if(previousCard) previousCard.transform.localScale = new Vector3(0.2f, 1, 0.3f);
            //if(currentCard) currentCard.transform.localScale = new Vector3(0.2f, 1, 0.3f);
            StartCoroutine(_cardHandManager.MoveCardsToHandCurve());
            for (int i = 0; i < _cardHandManager.hand.Count; i++)
            {
                
                //Reset Scale
                _cardHandManager.hand[i].transform.localScale = new Vector3(0.2f, 1, 0.3f);
                 /*
                Vector3 NewPosition = _cardHandManager.curve.GetPoint(_cardHandManager.ReturnCardPositions(_cardHandManager.hand.Count, i+1));
                NewPosition.z -= (float)i / 100f;
                _cardHandManager.MoveCard(_cardHandManager.hand[i], NewPosition, _cardHandManager.ReturnCardRotation(_cardHandManager.hand.Count, i+1), moveSpeed);
                 */
            }
        }
    }
}