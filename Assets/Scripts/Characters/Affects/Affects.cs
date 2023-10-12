using System.Collections.Generic;
using System.Linq;
using Fight;
using UnityEngine;

namespace Characters
{
    public class Affects : MonoBehaviour
    {
        //This component will be added to each character when an affect is added
        public List<Affect> list = new List<Affect>();
        AffectIcons iconHolder = null;

        void Start()
        {
            FightEvents.OnCharacterTurnStarted += ApplyStartOfTurnAffects;
            FightEvents.OnCharacterTurnEnded += ApplyEndOfTurnAffects;
        }
         void OnDestroy() 
        {
            FightEvents.OnCharacterTurnStarted -= ApplyStartOfTurnAffects;
            FightEvents.OnCharacterTurnEnded -= ApplyEndOfTurnAffects;
        }

        public void AddAffect(Affect affect)
        {
            if (!iconHolder)
            {
                iconHolder = this.gameObject.GetComponentInChildren<HealthDisplay>().gameObject.GetComponentInChildren<AffectIcons>();
            }

            Affect affectInList = list.FirstOrDefault(affectToCheck => affectToCheck.GetType() == affect.GetType());

            if (affectInList != null)
            {
                if (affect.IsStackable)
                    affectInList.StackSize += affect.StackSize;

                iconHolder.UpdateAffectIcon(affectInList);
            }
            else
            {
                list.Add(affect);
                iconHolder.AddAffectIcon(affect);
            }
        }

        public void ApplyStartOfTurnAffects(Character character)
        {
            if (character != this.GetComponent<Character>()) return;

            //List of affects that need to be deleted
            List<Affect> temp = new List<Affect>();

            foreach (Affect a in list)
            {
                if (a.WhenAffectTriggers == TurnTime.StartOfTurn)
                    a.Apply(this.GetComponent<Character>());

                if (a.WhenStackLoss == TurnTime.StartOfTurn)
                {
                    a.StackSize -= a.StackLostAmount;
                    if(a.StackSize <= 0) temp.Add(a);
                    iconHolder.UpdateAffectIcon(a);
                }
            }

            foreach(Affect a in temp) list.Remove(a);
        }
        public void ApplyEndOfTurnAffects(Character character)
        {
            if (character != this.GetComponent<Character>()) return;
            
            //List of affects that need to be deleted
            List<Affect> temp = new List<Affect>();

            foreach (Affect a in list)
            {
                if (a.WhenAffectTriggers == TurnTime.EndOfTurn)
                    a.Apply(this.GetComponent<Character>());

                if (a.WhenStackLoss == TurnTime.EndOfTurn)
                {
                    a.StackSize -= a.StackLostAmount;
                    if(a.StackSize <= 0) temp.Add(a);
                    iconHolder.UpdateAffectIcon(a);
                }

            }

            foreach(Affect a in temp) list.Remove(a);
        }
    }
}
