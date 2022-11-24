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
        public override Vector3 movePosition 
        { 
            get => _movePosition; 
            set => _movePosition = value; 
        }

        public override void LoadMoves()
        {
            _moves = new List<Move>();

            Attack attack1 = new Attack();
            attack1.damageAmount = 99f;
            attack1.targeting = Move.Enemy_Targeting.Player;
            //moves.Add(attack1);

            Block block1 = new Block();
            block1.blockAmount = 10f;
            block1.targeting = Move.Enemy_Targeting.Self;
            //moves.Add(block1);

            Special specal1 = new Special();
            specal1.targeting = Move.Enemy_Targeting.AllEnemies;
            moves.Add(specal1);

        }
        public override void ExecuteMove(Move move,Character target = null,List<Character> targets = null)
        {
            if(target != null) move.execute(target);
            else if(targets != null) move.execute(null,targets);
            else move.execute();

            //play enemy animation here
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

