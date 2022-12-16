using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace characters
{
    public class Affects : MonoBehaviour
    {
        //This component will be added to each character when an affect is added
        public List<Affect> list = new List<Affect>();
        AffectIcons iconHolder = null;

        public void AddAffect(Affect affect)
        {
            if (!iconHolder)
            {
                iconHolder = this.gameObject.GetComponentInChildren<HealthDisplay>().gameObject.GetComponentInChildren<AffectIcons>();
                iconHolder.Initialize(this);
            }

            Affect affectInList = list.FirstOrDefault(affectToCheck => affectToCheck.GetType() == affect.GetType());

            if (affectInList != null)
            {
                if (affect.IsStackable)
                    affectInList.Count += affect.Count;

                iconHolder.UpdateAffectIcon(affectInList);
            }
            else
            {
                list.Add(affect);
                iconHolder.AddAffectIcon(affect);
            }
        }

        public void ApplyStartOfTurnAffects()
        {
            foreach (Affect a in list)
            {
                if (a.WhenAffectTriggers == TurnTime.StartOfTurn)
                    a.Apply(this.GetComponent<Character>());
            }
        }
        public void ApplyEndOfTurnAffects()
        {
            foreach (Affect a in list)
            {
                if (a.WhenAffectTriggers == TurnTime.EndOfTurn)
                    a.Apply(this.GetComponent<Character>());
            }
        }


    }
}
