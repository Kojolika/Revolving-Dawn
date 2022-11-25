using UnityEngine;
using TMPro;
using System.Collections.Generic;

namespace characters
{
    public class Enemy_Slime : Enemy
    {
        public HealthSystem _health;
        public List<Move> _moves;
        public Vector3 _movePosition;
        public Move _currentMove = null;
        [SerializeField] HealthDisplay healthDisplay;

        public override HealthSystem health 
        {
            get => _health;
            set => _health = value;
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

        public override void LoadMoves()
        {
            _moves = new List<Move>();

            Attack attack1 = new Attack();
            attack1.damageAmount = 7f;
            attack1.targeting = Move.Enemy_Targeting.Player;
            moves.Add(attack1);

            Block block1 = new Block();
            block1.blockAmount = 10f;
            block1.targeting = Move.Enemy_Targeting.Self;
            moves.Add(block1);

            Special specal1 = new Special();
            specal1.targeting = Move.Enemy_Targeting.AllEnemies;
            moves.Add(specal1);

        }
        void Start()
        {
            _health = new HealthSystem();
            health.SetMaxHealth(50f);
            health.SetHealth(50f);

            healthDisplay.health = health;
            healthDisplay.UpdateHealth();

            _movePosition = new Vector3(0f, .4f, 0f);

            LoadMoves();
            
            //not ideal, figure out a better way to do this
            //possibly a way in character.cs?
            //this.gameObject.AddComponent<TurnOnShadows>();
            CastShadows();
        }
    }
}

