using System.Collections.Generic;
using UnityEngine;

namespace characters
{
    public class Affects : MonoBehaviour
    {
        //This component will be added to each character when an affect is added
        public List<Affect> list = new List<Affect>();
        List<Affect> startOfTurnList = new List<Affect>();
        List<Affect> endOfTurnList = new List<Affect>();
        List<Affect> passiveList = new List<Affect>();

        GameObject IconHolder = null;

        public void AddAffect(Affect affect)
        {
            list.Add(affect);
            
            if(affect.WhenApplied == TurnTime.StartOfTurn)
            {
                startOfTurnList.Add(affect);
            }
            if(affect.WhenApplied == TurnTime.EndOfTurn)
            {
                endOfTurnList.Add(affect);
            }

            if(!IconHolder)
            {
                
            }
        }

        public void ApplyStartOfTurnAffects()
        {
            if(this.gameObject.TryGetComponent<Player>(out Player player))
            {
                foreach(Affect a in startOfTurnList)
                {
                    a.Apply(player);
                }
            }
            else if(this.gameObject.TryGetComponent<Enemy>(out Enemy enemy))
            {
                foreach(Affect a in endOfTurnList)
                {
                    a.Apply(enemy);
                }
            }
        }
        public void ApplyEndOfTurnAffects()
        {
            if(this.gameObject.TryGetComponent<Player>(out Player player))
            {
                foreach(Affect a in endOfTurnList)
                {
                    a.Apply(player);
                }
            }
            else if(this.gameObject.TryGetComponent<Enemy>(out Enemy enemy))
            {
                foreach(Affect a in endOfTurnList)
                {
                    a.Apply(enemy);
                }
            }
        }


    }
}
