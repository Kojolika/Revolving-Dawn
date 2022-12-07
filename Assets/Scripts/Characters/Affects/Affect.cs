using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace characters
{
    public abstract class Affect
    {
        public abstract void Apply(Character target);
        public abstract bool IsStartOfTurn{get; set;}
        public abstract bool IsEndOfTurn{get; set;}
        public abstract int Count {get;set;}
    }    
}

