using UnityEngine;
using cards;

namespace fight
{
    internal class HoverManager : MonoBehaviour
    {
        public Card currentCard;
        Card previousCard;
        CardHandManager _cardHandManager;

        bool updating = false;

        bool resetRequired = false;
        bool isEnabled = false;
        const float moveSpeed = 8f;

        void Start()
        {
            _cardHandManager = this.gameObject.GetComponent<CardHandManager>();
        }
        public void Enable(bool value)
        {
            isEnabled = value;
        }
        private void Update()
        {
            if (!isEnabled) return;
            if (updating) return;

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
            float MoveAmount;
            int PositionDifference;
            const float ScaleAmount = 1.5f;


            for (int i = 0; i < _cardHandManager.hand.Count; i++)
            {
                if (i == cardposition)
                {
                    //Selected card will be centered
                    card.transform.rotation  = Quaternion.Euler(new Vector3(90f, 90f, -90f));
                    Vector3 p = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0, Camera.main.nearClipPlane));
                    _cardHandManager.MoveCard(card, new Vector3(_cardHandManager.curve.GetPoint(_cardHandManager.ReturnCardPositions(_cardHandManager.hand.Count, cardposition + 1)).x, p.y - 2f, card.transform.position.z - .1f), 0, 50f);
                    card.transform.localScale = new Vector3(0.2f, 1, 0.3f) * ScaleAmount;
                    continue;
                }


                if (_cardHandManager.hand[i].GetComponent<CardMover>() != null)
                    Destroy(_cardHandManager.hand[i].GetComponent<CardMover>());

                //Move Cards relative to their position of the selected card
                //i.e. cards closer more farther away
                if (i - cardposition == 0) PositionDifference = 0;
                else PositionDifference = 2 / (i - cardposition);
                MoveAmount = _cardHandManager.ReturnCardPositions(_cardHandManager.hand.Count, i + 1) + PositionDifference * .03f;

                //turn curve point into vector space
                Vector3 NewPosition = _cardHandManager.curve.GetPoint(MoveAmount);

                //Add a small z value so cards on the right area always slightly infront of the left card
                //Gives a sense of realism to the card hand
                NewPosition.z -= (float)i / 100f;

                _cardHandManager.MoveCard(_cardHandManager.hand[i], NewPosition, _cardHandManager.ReturnCardRotation(_cardHandManager.hand.Count, i + 1), moveSpeed);
            }

        }
        private void ResetHand()
        {
            for (int i = 0; i < _cardHandManager.hand.Count; i++)
            {
                //Reset Scale
                _cardHandManager.hand[i].transform.localScale = new Vector3(0.2f, 1, 0.3f);

                Vector3 NewPosition = _cardHandManager.curve.GetPoint(_cardHandManager.ReturnCardPositions(_cardHandManager.hand.Count, i+1));
                NewPosition.z -= (float)i / 100f;
                _cardHandManager.MoveCard(_cardHandManager.hand[i], NewPosition, _cardHandManager.ReturnCardRotation(_cardHandManager.hand.Count, i+1), moveSpeed);
            }
        }
    }
}