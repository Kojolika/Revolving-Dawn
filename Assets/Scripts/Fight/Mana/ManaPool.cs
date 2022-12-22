using UnityEngine;
using System.Collections.Generic;

namespace mana
{
    public class ManaPool : MonoBehaviour 
    {
        public List<Mana> manaPool = new List<Mana>();
        int manaCount;

        public GameObject center;
        float radius = .5f;
        Vector2 circleCenter;
        float speed = 20f;
        float z = 0f;
        float[] angle;

        void Start() 
        {
            manaCount = manaPool.Count;
            angle = new float[manaCount];
            for(int i=0;i<manaCount; i++)
                angle[i] = 1f;

            circleCenter = new Vector2(center.transform.localPosition.x, center.transform.localPosition.y);

            for(int i=0; i<manaCount; i++)
            {
                var mana = manaPool[i];
                float offset = i * (360/manaCount);
                angle[i] = offset;
                mana.transform.localPosition = new Vector3(
                    circleCenter.x + radius + Mathf.Cos(angle[i] * Mathf.Deg2Rad),
                    circleCenter.y + radius + Mathf.Sin(angle[i] * Mathf.Deg2Rad),
                    z);
            }
        }

        void LateUpdate() 
        {
            for(int i=0; i<manaCount; i++)
            {
                var mana = manaPool[i];
                angle[i]  +=  (Time.deltaTime * speed);
                mana.transform.localPosition = new Vector3(
                    circleCenter.x + radius + Mathf.Cos(angle[i] * Mathf.Deg2Rad),
                    circleCenter.y + radius + Mathf.Sin(angle[i] * Mathf.Deg2Rad),
                    z);
            }
        }
    }
}
