using System.Collections.Generic;
using Cards;
using Models.Buffs;
using System.Linq;
using UnityEngine;
using System;

namespace Models.Cards
{
    [Serializable]
    public class CardAffect
    {
        [SerializeField] private CardAffectDefinition cardAffect;
        [SerializeField] private ulong[] args;
        [SerializeField] private Targeting targeting;
        
        public Targeting Targeting => targeting;

        public void Execute(List<IBuffable> targets) => cardAffect.Execute(targets, args);
        public void Execute(List<IHealth> targets) => cardAffect.Execute(targets, args);
    }

    public abstract class CardAffectDefinition : ScriptableObject
    {
        public abstract string Description(params ulong[] args);
        public virtual void Execute(List<IBuffable> targets, params ulong[] args)
            => Execute(targets.Select(target => target as IHealth).ToList());
        public abstract void Execute(List<IHealth> targets, params ulong[] args);
    }
}