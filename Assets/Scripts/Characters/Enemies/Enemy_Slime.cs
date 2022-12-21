using UnityEngine;
using TMPro;
using System.Collections.Generic;

namespace characters
{
    public class Enemy_Slime : Enemy
    {
        float maxHealth = 16f;
        public override float MaxHealth { get => maxHealth; set => maxHealth= value; }
        HealthDisplay _healthDisplay;
        public override HealthDisplay healthDisplay { get => _healthDisplay; set => _healthDisplay = value; }

        public List<Move> _moves;
        public override List<Move> moves
        {
            get => _moves;
            set => _moves = value;   
        }

        Vector3 _movePosition = new Vector3(0f, .35f, 0f);
        public override Vector3 moveIconPosition 
        { 
            get => _movePosition; 
            set => _movePosition = value; 
        }

        public Move _currentMove = null;
        public override Move currentMove 
        { 
            get => _currentMove; 
            set => _currentMove = value; 
        }

        Vector3 _targetingBorderPosition = new Vector3(0f, 0.115f, 0f);
        public override Vector3 targetingBorderPosition 
        { 
            get => _targetingBorderPosition; 
            set => _targetingBorderPosition = value; 
        }

        Vector3 _healthBarPosition = new Vector3 (0f, -0.05f, 0f);
        public override Vector3 healthbarPosition
        { 
            get => _healthBarPosition; 
            set => _healthBarPosition = value; 
        }

        public override void LoadMoves()
        {
            _moves = new List<Move>();

            Attack attack1 = new Attack();
            attack1.damageAmount = 7f;
            attack1.targeting = Move.Enemy_Targeting.Player;
            attack1.enemyUsingMove = this;
            moves.Add(attack1);

            Block block1 = new Block();
            block1.blockAmount = 9f;
            block1.targeting = Move.Enemy_Targeting.Self;
            block1.enemyUsingMove = this;
            moves.Add(block1);

            Special special1 = new Special();
            special1.targeting = Move.Enemy_Targeting.Player;
            special1.enemyUsingMove = this;
            moves.Add(special1);

        }
        void Start()
        {
            LoadMoves();
            
            //not ideal, figure out a better way to do this
            //possibly a way in character.cs?
            //this.gameObject.AddComponent<TurnOnShadows>();
            CastShadows();
        }
    }
}

