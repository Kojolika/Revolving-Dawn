using UnityEngine;
using cards;

namespace fight
{
    internal class PlayCardManager : MonoBehaviour
    {

        [SerializeField] static float CAMERA_DISTANCE;
        [SerializeField] static float SCALE_CARD_SIZE_BY = 0.6f;

        HoverManager _hoverManager;
        CardHandManager _cardHandManager;
        TargetingArrow _targetingArrow;

        bool isEnabled = false;
        bool dragging = false;
        bool cardTargeting = false;
        bool centering = false;

        Card currentCard;
        Vector3 currentCardOriginalPosition;
        Vector3 currentCardOriginalScale;


        void Start()
        {
            _hoverManager = this.gameObject.GetComponent<HoverManager>();
            _cardHandManager = this.gameObject.GetComponent<CardHandManager>();

            CAMERA_DISTANCE = Camera.main.nearClipPlane + 7;
        }

        public void Enable(bool value)
        {
            isEnabled = value;
        }

        public bool CardInPlayArea()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits;
            hits = Physics.RaycastAll(ray, 100.0F);
            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit hit = hits[i];
                if (hit.transform.gameObject.name == "CardPlayingArea")
                {
                    return true;
                }
            }
            return false;
        }

        public void ActivateCardTargeting()
        {
            //Play small effect for entering playable area

            //Then check if card requires targeting arrow
            if (currentCard.GetTarget() == (int)Targeting.Enemy)
            {
                cardTargeting = true;

                //move card to center and scale down to targeting...
                Vector3 cardCenterPosition = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.16f, CAMERA_DISTANCE));

                if (!centering)
                {
                    CardMover _CardMover = currentCard.gameObject.AddComponent<CardMover>();
                    _CardMover.Initialize(currentCard, cardCenterPosition, 0f, 40f);
                    centering = true;
                }
                currentCard.transform.localScale = currentCardOriginalScale * SCALE_CARD_SIZE_BY;

                //draw arrow for targeting
                if (!this.gameObject.GetComponent<TargetingArrow>())
                {
                    _targetingArrow = this.gameObject.AddComponent<TargetingArrow>();
                    _targetingArrow.Initialize(cardCenterPosition);
                }
            }
        }

        void FixedUpdate()
        {
            if (!isEnabled) return;
            
            //if current card is selected
            if (dragging)
            {
                //if Cards has no target, follow mouse position
                if (!cardTargeting)
                {
                    Vector3 cardTrackingMousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, CAMERA_DISTANCE);
                    currentCard.transform.position = Camera.main.ScreenToWorldPoint(cardTrackingMousePosition);
                    if (CardInPlayArea()) ActivateCardTargeting();
                }

                //if card is selected, allows player to cancel selection with right click and move card back to its position
                if (Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKey(KeyCode.Mouse1))
                {
                    MoveCardToCorrectHandPosition();
                    _cardHandManager.CreateHandCurve();
                    _hoverManager.Enable(true);
                    ResetFlags();
                    ResetCardScale();
                    DestroyTargetingArrow();
                }
            }
            //if there is no selected card, allows the player the ability to select one with left click
            else if (_hoverManager.currentCard && (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKey(KeyCode.Mouse0)))
            {

                _hoverManager.Enable(false);
                dragging = true;
                currentCard = _hoverManager.currentCard;

                currentCardOriginalPosition = currentCard.transform.position;
                currentCardOriginalScale = currentCard.transform.localScale;
            }
        }

        void MoveCardToCorrectHandPosition()
        {
            CardMover _CardMover = currentCard.gameObject.AddComponent<CardMover>();
            _CardMover.Initialize(currentCard, currentCardOriginalPosition, 0f, 50f);
        }
        void ResetFlags() => dragging = cardTargeting = centering = false;
        void ResetCardScale() => currentCard.transform.localScale = currentCardOriginalScale;

        void DestroyTargetingArrow()
        {
            if (this.gameObject.GetComponent<TargetingArrow>())
            {
                Destroy(this.gameObject.GetComponent<TargetingArrow>());
            }
        }
    }
}