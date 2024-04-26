using System;
using System.Collections.Generic;
using Fight.Events;
using Models.Buffs;
using UnityEngine;
using Utils.Attributes;

namespace Models.CardEffects
{
    [Serializable]
    public class ApplyBuffEffect : CardEffect
    {
        [SerializeReference, DisplayInterface(typeof(IBuff))] IBuff buff;
        public override List<IBattleEvent> Execute(List<IHealth> targets)
        {
            throw new System.NotImplementedException();
        }
    }
}