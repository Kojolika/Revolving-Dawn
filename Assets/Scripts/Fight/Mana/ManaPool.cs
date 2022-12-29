using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using fight;

namespace mana
{
    public class ManaPool : MonoBehaviour
    {
        public List<Mana> mana = new List<Mana>();
        int manaCount;

        public GameObject center;
        float radius = .5f;
        Vector2 circleCenter;
        float rotateSpeed = 50f;
        float moveSpeed = 2f;
        float z = 2f;
        float[] angle;
        bool rotating = false;

        void Start()
        {
            manaCount = mana.Count;

            angle = new float[manaCount];
                
            circleCenter = new Vector2(center.transform.localPosition.x, center.transform.localPosition.y);
            
            var points = GetStoppingPoints();
            for (int i = 0; i < manaCount; i++)
            {
                mana[i].transform.localPosition = points[i];
            }
        }

        Vector3[] GetStoppingPoints()
        {
            Vector3[] points = new Vector3[manaCount];
            
            for (int i = 0; i < manaCount; i++)
            {
                float offset = i * (360 / manaCount);
                angle[i] = offset;

                points[i] = new Vector3(
                    circleCenter.x + (radius * Mathf.Cos(offset * Mathf.Deg2Rad)),
                    circleCenter.y + (radius * Mathf.Sin(offset * Mathf.Deg2Rad)),
                    z);
            }
            return points;
        }

        public bool IsRotating()
        {
            return rotating;
        }
        public void StopRotating()
        {
            rotating = false;
        }

        IEnumerator MoveMana(Mana mana, Vector3 destination)
        {
            while (mana.gameObject.transform.localPosition != destination)
            {
                    mana.gameObject.transform.localPosition = Vector3.MoveTowards(mana.gameObject.transform.localPosition, destination, moveSpeed * Time.deltaTime);
                    yield return null;
            }
        }
        public void StartCircularRotate()
        {
            rotating = true;
            StartCoroutine(CircularRotateCoroutine());
        }
        IEnumerator CircularRotateCoroutine()
        {
            while (rotating)
            {
                for (int i = 0; i < manaCount; i++)
                {
                    var mana = this.mana[i];

                    angle[i] += (Time.deltaTime * rotateSpeed);
                    mana.transform.localPosition = new Vector3(
                        circleCenter.x + radius + Mathf.Cos(angle[i] * Mathf.Deg2Rad),
                        circleCenter.y + radius + Mathf.Sin(angle[i] * Mathf.Deg2Rad),
                        z);
                }

                yield return null;
            }

        }
    }
}
