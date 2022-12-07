using System.Collections.Generic;
using UnityEngine;

namespace characters
{
    public class Affects : MonoBehaviour
    {
        //This component will be added to each character when an affect is added
        public List<Affect> list;
        List<Affect> startOfTurnList;
        List<Affect> endOfTurnList;

        void Start() 
        {
            list = new List<Affect>();
            startOfTurnList = new List<Affect>();
            endOfTurnList = new List<Affect>();
        }

        public void AddAffect(Affect affect)
        {
            list.Add(affect);
            
            if(affect.IsStartOfTurn)
            {
                startOfTurnList.Add(affect);
            }
            if(affect.IsEndOfTurn)
            {
                endOfTurnList.Add(affect);
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
