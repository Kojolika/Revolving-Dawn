using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using cards;
using characters;

namespace fight
{
    internal class PlayCardManager : MonoBehaviour
    {

        [SerializeField] static float CAMERA_DISTANCE;
        [SerializeField] static float SCALE_CARD_SIZE_BY = 0.8f;

        HoverManager _hoverManager;
        CardHandMovementManager _cardHandMovementManager;
        TargetingArrow _targetingArrow;
        FightManager fm;
        List<SpriteRenderer> targets = new List<SpriteRenderer>();

        int numEnemies;
        bool isEnabled = false;
        bool dragging = false;
        bool cardArrowTargeting = false;
        bool centeringCard = false;
        bool isHandUpdating = false;
        bool hasPreviouslyEnteredPlayArea = false;

        Card currentCard;
        Vector3 currentCardOriginalRotation;
        Vector3 currentCardOriginalPosition;
        Camera CardArrowCam;

        void Start()
        {
            _hoverManager = this.gameObject.GetComponent<HoverManager>();
            _cardHandMovementManager = this.gameObject.GetComponent<CardHandMovementManager>();

            fm = this.GetComponent<FightManager>();
            numEnemies = fm.enemies.Capacity;

            CAMERA_DISTANCE = Camera.main.nearClipPlane + 7;

            _cardHandMovementManager.TriggerIsHandUpdating += HandUpdateEventHandler;
        }

        public void Enable(bool value)
        {
            isEnabled = value;
        }

        public void HandUpdateEventHandler(bool isUpdating)
        {
            isHandUpdating = isUpdating;
        }
        bool CardInPlayArea()
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
        bool CardInHandArea()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits;
            hits = Physics.RaycastAll(ray, 100.0F);
            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit hit = hits[i];
                if (hit.transform.gameObject.name == "CardHandArea")
                {
                    return true;
                }
            }
            return false;
        }
        bool HasLeftPlayArea()
        {
            if (CardInHandArea() && hasPreviouslyEnteredPlayArea)
            {
                hasPreviouslyEnteredPlayArea = false;
                return true;
            }
            else
                return false;
        }
        bool HasEnteredPlayArea()
        {
            if (CardInPlayArea() && !hasPreviouslyEnteredPlayArea)
            {
                hasPreviouslyEnteredPlayArea = true;
                return true;
            }
            else
                return false;
        }
        GameObject MouseOverCharacter()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits;
            hits = Physics.RaycastAll(ray, 100.0F);
            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit hit = hits[i];
                if (hit.transform.gameObject.GetComponent<Character>())
                {
                    return hit.transform.gameObject;
                }
            }
            return null;
        }
        bool IsEnemy()
        {
            var go = MouseOverCharacter();
            if (go != null && go.GetComponent<Enemy>())
                return true;
            else
                return false;
        }

        public void ActivateCardTargeting()
        {
            //Play small effect for entering playable area here
            //Then check if card requires targeting arrow
            if (currentCard.GetTarget() == (int)Targeting.Enemy)
            {
                //For this case, lets the player automatically target the only enemy
                //Quality of life
                if (numEnemies == 1)
                {
                    var target = fm.enemies[0].GetComponent<Targeting_Border>().border.GetComponent<SpriteRenderer>();
                    target.enabled = true;
                    targets.Add(target);
                }
                //Otherwise, if there is more thna one enemy the player needs to be able to choose the enemy
                //Therefore, the targeting arrow is drawn when the player moves the card into the play area 
                else
                {

                    cardArrowTargeting = true;
                    //move card to center and scale down to targeting...
                    Vector3 cardCenterPosition = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.16f, CAMERA_DISTANCE));

                    if (!centeringCard)
                    {
                        CardMover _CardMover = currentCard.gameObject.AddComponent<CardMover>();
                        _CardMover.Initialize(currentCard, cardCenterPosition, 0f, 40f);
                        centeringCard = true;
                    }

                    //draw arrow for targeting
                    if (!this.gameObject.GetComponent<TargetingArrow>())
                    {
                        _targetingArrow = this.gameObject.AddComponent<TargetingArrow>();
                    }
                }
            }
        }
        void DeActivateCardTargeting()
        {
            StartCoroutine(ResetHand());
        }

        void FixedUpdate()
        {
            if (!isEnabled) return;

            //if current card is selected
            if (dragging)
            {
                //if card is selected, allows player to cancel selection with right click and move card back to its position
                if (Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKey(KeyCode.Mouse1))
                {
                    StartCoroutine(ResetHand());
                    return;
                }

                //if Cards has no precise target, follow mouse position
                if (!cardArrowTargeting)
                {
                    Vector3 cardTrackingMousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, CAMERA_DISTANCE);
                    currentCard.transform.position = Camera.main.ScreenToWorldPoint(cardTrackingMousePosition);
                }

                if (HasEnteredPlayArea())
                {
                    ActivateCardTargeting();
                }
                else if (HasLeftPlayArea())
                {
                    DeActivateCardTargeting();
                }

            }
            //if there is no selected card, allows the player the ability to select one with left click
            else if (_hoverManager.currentCard && (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKey(KeyCode.Mouse0)))
            {
                _hoverManager.Enable(false);
                dragging = true;
                currentCard = _hoverManager.currentCard;

                //save card position in hand
                currentCardOriginalPosition = currentCard.transform.position;
                currentCardOriginalRotation = currentCard.transform.rotation.eulerAngles;
                currentCard.transform.rotation = Quaternion.Euler(CardInfo.DEFAULT_CARD_ROTATION);
                currentCard.transform.localScale = CardInfo.DEFAULT_SCALE * SCALE_CARD_SIZE_BY;
                Debug.Log("Card OG position when selected: " + currentCardOriginalPosition);
                return;
            }
        }

        IEnumerator ResetHand()
        {
            ResetCardPosition();
            ResetCardRotation();
            ResetFlags();
            DestroyTargetingArrow();
            ClearTargets();

            yield return new WaitForSeconds(0.05f);
            _hoverManager.Enable(true);
        }
        void ResetCardPosition() => currentCard.transform.position = currentCardOriginalPosition;
        void ResetCardRotation() => currentCard.transform.rotation = Quaternion.Euler(currentCardOriginalRotation);
        void ResetFlags() => dragging = cardArrowTargeting = centeringCard = false;
        //void ResetCardScale() => currentCard.transform.localScale = CardInfo.DEFAULT_SCALE;
        void ClearTargets()
        {
            foreach (var target in targets)
            {
                target.enabled = false;
            }
        }
        void DestroyTargetingArrow()
        {
            if (this.gameObject.GetComponent<TargetingArrow>())
            {
                Destroy(this.gameObject.GetComponent<TargetingArrow>());
            }
        }
    }
}