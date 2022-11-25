using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace characters{
    public abstract class Enemy : Character
    {
        public abstract List<Move> moves {get; set;}
        public abstract Vector3 moveIconPosition {get;set;}
        public abstract void LoadMoves();
        public abstract Move currentMove{get; set;}
        public void ExecuteMove(Character target = null,List<Character> targets = null)
        {
            if(target != null) currentMove.execute(target);
            else if(targets != null) currentMove.execute(targets:targets);
            else currentMove.execute();

            //Animation here?
        }
    }
}

