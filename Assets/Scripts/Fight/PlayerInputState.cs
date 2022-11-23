using UnityEngine;
using cards;
using System.Collections.Generic;
using characters;

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
        void CardMouseOver(Card card) => NewCardForHoverEffects(card);
            
        void NewCardForHoverEffects(Card card)
        {
            if(currentCard == card) return;
            if(hoverManager != null) GameObject.Destroy(hoverManager);

            currentCard = card;
            hoverManager = _input.gameObject.AddComponent<HoverManager>();
            hoverManager.Initialize(_input.gameObject.GetComponent<CardHandManager>(),currentCard);
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
        Dragger newDragger;

        public DraggingState(Card card)
        {
            if(currentCard == card) return;
            
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

            if(currentCard.GetTarget() == 1 || changeDefault)
            {
                newDragger.StopCoroutine(newDragger.DraggingCoroutine(currentCard));
                GameObject.Destroy(newDragger);
            }
        }
    }

    public class TargetingState : PlayerInputState
    {
        bool change = false;
        bool leftClicked = false;

        Card currentCard;
        Dragger dragger;
        TargetingArrow targetingArrow;
        int cardtarget;
        Mover mover;
        List<Character> targets = new List<Character>();
        FightManager fightManager = _input.GetComponent<FightManager>();
        CardHandManager cardHandMovementManager = _input.GetComponent<CardHandManager>();

        public TargetingState(Card card)
        {
            currentCard = card;

            _input.TriggerLeftClicked += LeftClicked;
            _input.TriggerRightClicked += RightClicked;
            _input.TriggerMouseEnterCardArea += OnEnterCardArea;
            

            cardtarget = card.GetTarget();
            switch (cardtarget)
            {
                case 0: //Friendly
                dragger = currentCard.GetComponent<Dragger>();

                var player = fightManager.GetPlayer();
                Debug.Log("Player: " + player);
                targets.Add(player);
                player.GetComponent<Targeting_Border>().border.GetComponent<SpriteRenderer>().enabled = true;
                break;
                
                ///////////////////////////////////////////////

                case 1: //Enemy
                //Hand is still reseting
                _input.GetComponent<CardHandManager>().StopAllCoroutines();

                _input.TriggerEnemyMouseOver += OnEnemyMouseOver;

                Vector3 cardCenterPosition = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.16f, CardInfo.CAMERA_DISTANCE));

                mover = currentCard.gameObject.AddComponent<Mover>();
                mover.Initialize(cardCenterPosition, 40f);

                targetingArrow = currentCard.gameObject.AddComponent<TargetingArrow>();

                break;

                ///////////////////////////////////////////////

                case 2: //RandomEnemy
                dragger = currentCard.GetComponent<Dragger>();

                foreach(Enemy e in fightManager.enemies)
                {
                    targets.Add(e);
                    e.GetComponent<Targeting_Border>().border.GetComponent<SpriteRenderer>().enabled = true;
                }
                break;

                ///////////////////////////////////////////////

                case 3: //AllEnemies
                dragger = currentCard.GetComponent<Dragger>();

                foreach(Enemy e in fightManager.enemies)
                {
                    targets.Add(e);
                    e.GetComponent<Targeting_Border>().border.GetComponent<SpriteRenderer>().enabled = true;   
                }
                break;

                ///////////////////////////////////////////////

                case 4: //All
                dragger = currentCard.GetComponent<Dragger>();

                
                foreach(Enemy e in fightManager.enemies)
                {
                    targets.Add(e);
                    
                    e.GetComponent<Targeting_Border>().border.GetComponent<SpriteRenderer>().enabled = true;
                }

                targets.Add(fightManager.GetPlayer());
                fightManager.GetPlayer().GetComponent<Targeting_Border>().border.GetComponent<SpriteRenderer>().enabled = true;

                break;

                ///////////////////////////////////////////////

                case 5: //None
                dragger = currentCard.GetComponent<Dragger>();
                break;
            }
        }

        public override PlayerInputState Transition()
        {
            if(change) 
            {
                Exit();
                return new DefaultState();
            }

            if(targets.Count > 0 && leftClicked)
            {
                //Trigger playing the card event
                cardHandMovementManager.OnCardPlayed(currentCard, targets);
                
                Exit();
                return new DefaultState();
            }

            leftClicked = false;
            
            return this;
        }

        void LeftClicked() => leftClicked = true;
        void RightClicked() => change = true;
        void OnEnterCardArea() => change = true;
        void OnEnemyMouseOver(Enemy enemy)
        {
            if(enemy != null)
            {
                enemy.GetComponent<Targeting_Border>().border.GetComponent<SpriteRenderer>().enabled = true;
                targets.Add(enemy);
            } 
            else
            {
                for(int i=0; i<targets.Count; i++)
                {
                    var target = targets[i];
                    target.GetComponent<Targeting_Border>().border.GetComponent<SpriteRenderer>().enabled = false;
                    targets.Remove(target);
                }
            }
        } 
        
        

        public override void Exit()
        {
            _input.TriggerLeftClicked -= LeftClicked;
            _input.TriggerRightClicked -= RightClicked;
            _input.TriggerMouseEnterCardArea -= OnEnterCardArea;
            

            //reset the hand
            _input.GetComponent<CardHandManager>().CreateHand();

            foreach(Character c in targets)
            {
                c.GetComponent<Targeting_Border>().border.GetComponent<SpriteRenderer>().enabled = false;
            }

            switch (cardtarget)
            {
                case 0: //Friendly
                StopDragging();           
                break;

                case 1: //Enemy
                if(mover) GameObject.Destroy(mover);

                _input.TriggerEnemyMouseOver -= OnEnemyMouseOver;

                GameObject.Destroy(targetingArrow);
                break;

                case 2: //RandomEnemy
                StopDragging();            
                break;

                case 3: //AllEnemies
                StopDragging();            
                break;

                case 4: //All
                StopDragging();           
                break;

                case 5: //None
                StopDragging();          
                break;
            }
        }

        void StopDragging()
        {
            dragger.StopCoroutine(dragger.DraggingCoroutine(currentCard));
            GameObject.Destroy(dragger);            
        }
    }
}