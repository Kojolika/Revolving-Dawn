using UnityEngine;
using cards;
using System.Collections;

namespace fight
{
    //Finite State Machine for player inputs and events
    public class PlayerInputState
    {
        public static PlayerTurnInputManager _input;

        public virtual PlayerInputState Transition()
        {
            return new DefaultState();
        }

        public virtual void Exit() => Debug.Log("testing exit");

        public void Initialize(PlayerTurnInputManager inputManager)
        {
            _input = inputManager;
        }
    }
    public class DefaultState : PlayerInputState
    {
        bool change = false;
        Card currentCard = null;

        public DefaultState(){
            _input.TriggerCardMouseOver += CardMouseOver;
        }

        public override PlayerInputState Transition()
        {
            if(change)
            {
                Exit();
                return new HoveringState(currentCard);
            }

            Exit();
            return new DefaultState();
        }
        void CardMouseOver(Card card)
        {
            change = true;
            currentCard = card;  
        } 
        public override void Exit()
        {
            _input.TriggerCardMouseOver -= CardMouseOver;
        }

    }

    public class HoveringState : PlayerInputState
    {
        bool changeDefault = false;
        bool changeDragging = false;
        Card currentCard;
        HoverManager hoverManager = null;

        public HoveringState(Card card)
        {
            _input.TriggerLeftClicked += LeftClicked;
            _input.TriggerNoCardMouseOver += NoCardMouseOver;
            _input.TriggerCardMouseOver += CardMouseOver;

            NewCardForHoverEffects(card);
        }

        public override PlayerInputState Transition()
        {
            if(changeDefault)
            {
                Exit();
                return new DefaultState();
            }

            if(changeDragging)
            {
                Exit();
                return new DraggingState(currentCard); 
            } 
            
            return this;
        }

        void LeftClicked() => changeDragging = true;
        void NoCardMouseOver() => changeDefault = true;
        void CardMouseOver(Card card)
        {
            NewCardForHoverEffects(card);
        }
        void NewCardForHoverEffects(Card card)
        {
            if(currentCard == card) return;
            Debug.Log("not same card");

            if(hoverManager != null) GameObject.Destroy(hoverManager);

            currentCard = card;
            hoverManager = _input.gameObject.AddComponent<HoverManager>();
            hoverManager.Initialize(_input.gameObject.GetComponent<CardHandMovementManager>(),currentCard);
            hoverManager.ResetHand();
            hoverManager.HoverCardEffects();
        } 
        public override void Exit()
        {
            hoverManager.ResetHand();
            _input.TriggerLeftClicked -= LeftClicked;
            _input.TriggerNoCardMouseOver -= NoCardMouseOver;
            _input.TriggerCardMouseOver -= CardMouseOver;

            GameObject.Destroy(hoverManager);
        }
    }

    public class DraggingState : PlayerInputState
    {
        bool changeDefault = false;
        bool changeTargeting = false;
        Card currentCard = null;
        Vector3 originalRotation;
        Dragger newDragger;

        public DraggingState(Card card)
        {
            if(currentCard == card) return;

            originalRotation = card.transform.rotation.eulerAngles;

            currentCard = card;
            _input.TriggerRightClicked += RightClicked;
            _input.TriggerMouseEnterPlayArea += OnEnterPlayArea;

            currentCard.transform.rotation = Quaternion.Euler(CardInfo.DEFAULT_CARD_ROTATION);
            newDragger = currentCard.gameObject.AddComponent<Dragger>();
            newDragger.StartDragging(currentCard);
        }

        public override PlayerInputState Transition()
        {
            if(changeDefault)
            {
                Exit();
                return new DefaultState();
            }

            if(changeTargeting)
            {
                Exit();
                return new TargetingState(currentCard);
            }
            
            return this;
        }
    
        void RightClicked() => changeDefault = true;
        void OnEnterPlayArea() => changeTargeting = true;
        public override void Exit()
        {
            _input.TriggerRightClicked -= RightClicked;
            _input.TriggerMouseEnterPlayArea -= OnEnterPlayArea;

            if(currentCard.GetTarget() != 1)
            {
                currentCard.transform.rotation = Quaternion.Euler(originalRotation);

                newDragger.StopCoroutine(newDragger.DraggingCoroutine(currentCard));
                GameObject.Destroy(newDragger);
            }

        }
    }

    public class TargetingState : PlayerInputState
    {
        bool change = false;
        Card currentCard;
        Dragger dragger;

        public TargetingState(Card card)
        {
            _input.TriggerRightClicked += RightClicked;
            _input.TriggerMouseEnterCardArea += OnEnterCardArea;
            dragger = card.GetComponent<Dragger>();
        }

        public override PlayerInputState Transition()
        {
            if(change) 
            {
                Exit();
                return new DefaultState();
            }

            return this;
        }
        void RightClicked() => change = true;
        void OnEnterCardArea() => change = true;
        public override void Exit()
        {
            _input.TriggerRightClicked -= RightClicked;
            _input.TriggerMouseEnterCardArea -= OnEnterCardArea;

            //currentCard.transform.rotation = Quaternion.Euler(originalRotation);

            dragger.StopCoroutine(dragger.DraggingCoroutine(currentCard));
            GameObject.Destroy(dragger);
        }
    }
}