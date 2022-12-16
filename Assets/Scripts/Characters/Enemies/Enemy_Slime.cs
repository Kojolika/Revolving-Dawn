using UnityEngine;
using TMPro;
using System.Collections.Generic;

namespace characters
{
    public class Enemy_Slime : Enemy
    {
        public HealthSystem _health;
        public List<Move> _moves;
        public Move _currentMove = null;
        HealthDisplay _healthDisplay;

        Vector3 _targetingBorderPosition = new Vector3(0f, 0.115f, 0f);
        Vector3 _movePosition = new Vector3(0f, .35f, 0f);
        Vector3 _healthBarPosition = new Vector3 (0f, -0.05f, 0f);

        public override HealthDisplay healthDisplay 
        {
            get => _healthDisplay;
            set => _healthDisplay = value;
        }
        public override List<Move> moves
        {
            get => _moves;
            set => _moves = value;   
        }
        public override Vector3 moveIconPosition 
        { 
            get => _movePosition; 
            set => _movePosition = value; 
        }
        public override Move currentMove 
        { 
            get => _currentMove; 
            set => _currentMove = value; 
        }
        public override Vector3 targetingBorderPosition 
        { 
            get => _targetingBorderPosition; 
            set => _targetingBorderPosition = value; 
        }
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
            attack1.enemyUsingMove = this.gameObject;
            moves.Add(attack1);

            Block block1 = new Block();
            block1.blockAmount = 10f;
            block1.targeting = Move.Enemy_Targeting.Self;
            block1.enemyUsingMove = this.gameObject;
            moves.Add(block1);

            Special special1 = new Special();
            special1.targeting = Move.Enemy_Targeting.AllEnemies;
            special1.enemyUsingMove = this.gameObject;
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
        public override void InitializeHealth()
        {
            healthDisplay.health = new HealthSystem();
            healthDisplay.health.SetHealth(16);
            healthDisplay.health.SetMaxHealth(16);
            healthDisplay.UpdateHealth();

            healthDisplay.transform.localPosition = healthbarPosition;
        }
    }
}

