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
        public IEnumerator ExecuteMove(List<Character> targets = null)
        {
            if(targets != null) currentMove.execute(targets:targets);
            else currentMove.execute();

            yield return null;
            //Animation here?
        }
    }
}

