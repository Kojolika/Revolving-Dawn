using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace characters{
    public abstract class Enemy : Character
    {
        public abstract List<Move> moves {get; set;}
        public abstract Vector3 movePosition {get;set;}
        public abstract void LoadMoves();
        public abstract void ExecuteMove(Move move,Character target = null,List<Character> targets = null);
    }
}

