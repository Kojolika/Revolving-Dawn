using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Fight;
using System;
using Systems.Managers;

namespace Characters
{
    public abstract class Enemy : Character
    {
        public abstract List<Move> Moves { get; set; }
        public abstract Vector3 MoveIconPosition { get; set; }
        protected GameObject moveSprite;
        public abstract Move CurrentMove { get; set; }
        public abstract void LoadMoves();

        static string pathToMoveObjects = "Assets/Prefabs/Characters/Enemies/Moves/";
        static GameObject attackMove = null;
        static GameObject blockMove = null;
        static GameObject specialMove = null;
        static GameObject AttackMove
        {
            get
            {
                //get and cache move icon gameobject
                if (attackMove == null)
                {
                    attackMove = AssetDatabase.LoadAssetAtPath<GameObject>(pathToMoveObjects + "AttackMove.prefab");
                }
                return attackMove;
            }
        }
        static GameObject BlockMove
        {
            get
            {
                //get and cache move icon gameobject
                if (blockMove == null)
                {

                    blockMove = AssetDatabase.LoadAssetAtPath<GameObject>(pathToMoveObjects + "BlockMove.prefab");
                }
                return blockMove;
            }
        }
        static GameObject SpecialMove
        {
            get
            {
                //get and cache move icon gameobject
                if (specialMove == null)
                {
                    specialMove = AssetDatabase.LoadAssetAtPath<GameObject>(pathToMoveObjects + "SpecialMove.prefab");
                }
                return specialMove;
            }
        }

        public IEnumerator ExecuteMove(List<Character> targets = null)
        {
            CurrentMove.execute(targets);

            yield return null;
            //Animation here?
        }

        public override void Awake()
        {
            base.Awake();
            FightEvents.OnCharacterTurnAction += PerformTurn;
            FightEvents.OnCharacterTurnStarted += ChooseMove;
        }
        private void OnDestroy()
        {
            FightEvents.OnCharacterTurnAction -= PerformTurn;
            FightEvents.OnCharacterTurnStarted -= ChooseMove;
        }
        //Need to use the same System.Random object to get different random values, hence static keyword
        static System.Random random = new System.Random();
        public void ChooseMove(Character character)
        {
            //Choose the current move during the start of the players turn
            if (character != FightManager_Obsolete.CurrentPlayer) return;

            int randomInt = random.Next(0, Moves.Count);
            CurrentMove = Moves[randomInt];

            Type type = CurrentMove.GetType();
            if (type == typeof(Attack))
            {
                //Set gameboject at icon position
                moveSprite = Instantiate(AttackMove,this.gameObject.transform, false);
                moveSprite.transform.localPosition = MoveIconPosition;
                //set attack value of icon
                Attack move = CurrentMove as Attack;
                moveSprite.transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text = "" + move.damageAmount;
            }
            else if (type == typeof(Block))
            {
                //Set gameboject at icon position
                moveSprite = Instantiate(BlockMove, this.gameObject.transform, false);
                moveSprite.transform.localPosition = MoveIconPosition;
                //set block value of icon
                Block move = CurrentMove as Block;
                moveSprite.transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text = "" + move.blockAmount;

            }
            else if (type == typeof(Special))
            {
                //Set gameboject at icon position
                moveSprite = Instantiate(SpecialMove, this.gameObject.transform, false);
                moveSprite.transform.localPosition = MoveIconPosition;
            }
        }
        public void PerformTurn(Character character)
        {
            if (character != this.GetComponent<Character>()) return;

            List<Character> targets = new List<Character>();
            switch (CurrentMove.Targeting)
            {
                case Move.Enemy_Targeting.All:
                    targets.Add(FightManager_Obsolete.CurrentPlayer);
                    targets.AddRange(FightManager_Obsolete.CurrentEnemies);
                    break;
                case Move.Enemy_Targeting.AllEnemies:
                    targets.AddRange(FightManager_Obsolete.CurrentEnemies);
                    break;
                case Move.Enemy_Targeting.None:
                    break;
                case Move.Enemy_Targeting.Player:
                    targets.Add(FightManager_Obsolete.CurrentPlayer);
                    break;
                case Move.Enemy_Targeting.Self:
                    targets.Add((Character)this);
                    break;
            }

            StartCoroutine(ExecuteMove(targets));

            //set current move to nothing
            CurrentMove = null;
            Destroy(moveSprite);

        }
    }
}

