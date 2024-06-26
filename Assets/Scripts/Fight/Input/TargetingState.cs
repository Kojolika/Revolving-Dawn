using UnityEngine;
using Cards;
using System.Collections.Generic;
using Characters;
using Fight;
using Systems.Managers;


namespace FightInput
{
        public class TargetingState : PlayerInputState
    {
        ChangeStateTo changeStateTo = ChangeStateTo.Targeting;
        Card3D currentCard;
        Enemy previousEnemy = null;
        Dragger dragger;
        TargetingArrow targetingArrow;
        int cardtarget;
        Mover mover;
        List<Character> targets = new List<Character>();
        CardHandManager cardMovementManager = _input.GetComponent<CardHandManager>();

        public TargetingState(Card3D card)
        {
            currentCard = card;

            _input.LeftClicked += LeftClicked;
            _input.RightClicked += RightClicked;
            _input.MouseEnterCardArea += OnEnterCardArea;


            cardtarget = (int)card.GetTarget();
            switch (cardtarget)
            {
                case 0: //Friendly
                    dragger = currentCard.GetComponent<Dragger>();

                    var player = FightManager_Obsolete.CurrentPlayer;
                    targets.Add(player);
                    player.GetComponent<Targeting_Border>().border.GetComponent<SpriteRenderer>().enabled = true;
                    break;

                ///////////////////////////////////////////////

                case 1: //Enemy
                    //Hand is still reseting
                    _input.GetComponent<CardHandManager>().StopAllCoroutines();

                    _input.EnemyMouseOver += OnEnemyMouseOver;

                    Vector3 cardCenterPosition = _input.cardCam.ViewportToWorldPoint(new Vector3(0.5f, 0.25f, CardConfiguration.CAMERA_DISTANCE - 3f));

                    mover = currentCard.gameObject.AddComponent<Mover>();
                    mover.Initialize(cardCenterPosition, 40f);
                    mover.StartCoroutine(mover.MoveGameObjectCoroutine());

                    targetingArrow = currentCard.gameObject.AddComponent<TargetingArrow>();
                    targetingArrow.cardCam = _input.cardCam;

                    break;

                ///////////////////////////////////////////////

                case 2: //RandomEnemy
                    dragger = currentCard.GetComponent<Dragger>();

                    foreach (Enemy e in FightManager_Obsolete.CurrentEnemies)
                    {
                        targets.Add(e);
                        e.GetComponent<Targeting_Border>().border.GetComponent<SpriteRenderer>().enabled = true;
                    }
                    break;

                ///////////////////////////////////////////////

                case 3: //AllEnemies
                    dragger = currentCard.GetComponent<Dragger>();

                    foreach (Enemy e in FightManager_Obsolete.CurrentEnemies)
                    {
                        targets.Add(e);
                        e.GetComponent<Targeting_Border>().border.GetComponent<SpriteRenderer>().enabled = true;
                    }
                    break;

                ///////////////////////////////////////////////

                case 4: //All
                    dragger = currentCard.GetComponent<Dragger>();


                    foreach (Enemy e in FightManager_Obsolete.CurrentEnemies)
                    {
                        targets.Add(e);

                        e.GetComponent<Targeting_Border>().border.GetComponent<SpriteRenderer>().enabled = true;
                    }

                    targets.Add(FightManager_Obsolete.CurrentPlayer);
                    FightManager_Obsolete.CurrentPlayer.GetComponent<Targeting_Border>().border.GetComponent<SpriteRenderer>().enabled = true;

                    break;

                ///////////////////////////////////////////////

                case 5: //None
                    dragger = currentCard.GetComponent<Dragger>();
                    break;
            }
        }

        public override PlayerInputState Transition()
        {
            switch(changeStateTo)
            {
                case ChangeStateTo.Default:
                    Exit();
                    return new DefaultState();
                case ChangeStateTo.Targeting:
                    return this;
            }

            return this;
        }

        void LeftClicked() 
        {
            if (targets.Count > 0)
            {
                //Trigger playing the card event
                cardMovementManager.TriggerPlayCard(currentCard, targets);
                changeStateTo = ChangeStateTo.Default;
            }
        }
        void RightClicked() => changeStateTo = ChangeStateTo.Default;
        void OnEnterCardArea() => changeStateTo = ChangeStateTo.Default;
        void OnEnemyMouseOver(Enemy enemy)
        {
            if (enemy != null)
            {
                
                if (enemy != previousEnemy)
                {
                    RemoveTargets();
                    enemy.GetComponent<Targeting_Border>().border.GetComponent<SpriteRenderer>().enabled = true;
                    targets.Add(enemy);
                    previousEnemy = enemy;

                    currentCard.UpdateDescription(enemy);

                    //targetingArrow.ChangeColor(Resources.Load<Material>("Arrow_Red"));
                }
            }
            else
            {
                RemoveTargets();
                previousEnemy = null;
                currentCard.UpdateDescription(null);

                //targetingArrow.ChangeColor(Resources.Load<Material>("Arrow"));
            }
        }
        void RemoveTargets()
        {
            for (int i = 0; i < targets.Count; i++)
            {
                var target = targets[i];
                target.GetComponent<Targeting_Border>().border.GetComponent<SpriteRenderer>().enabled = false;
                targets.Remove(target);
            }
        }
        public override void Exit()
        {
            _input.LeftClicked -= LeftClicked;
            _input.RightClicked -= RightClicked;
            _input.MouseEnterCardArea -= OnEnterCardArea;

            currentCard.UpdateDescription(null);

            //reset the hand
            _input.GetComponent<CardHandManager>().CreateHand();

            foreach (Character character in targets)
            {
                if(character == null) continue;
                character.GetComponent<Targeting_Border>().border.GetComponent<SpriteRenderer>().enabled = false;
            }

            switch (cardtarget)
            {
                case 0: //Friendly
                    StopDragging();
                    break;

                case 1: //Enemy
                    if (mover) GameObject.Destroy(mover);

                    _input.EnemyMouseOver -= OnEnemyMouseOver;

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
            dragger.StopDragging();
            GameObject.Destroy(dragger);
        }
    }
}