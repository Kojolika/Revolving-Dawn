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
        float rotateSpeed = 1f;
        float moveSpeed = 2f;
        float z = 2f;
        float[] currentAngleInRad;
        Vector3[] fixedPoints;
        float[] fixedAngleInRad;
        bool rotating = false;

        void Start()
        {
            manaCount = mana.Count;

            fixedPoints = new Vector3[manaCount];
            currentAngleInRad = new float[manaCount];
            fixedAngleInRad = new float[manaCount];
            for (int i = 0; i < manaCount; i++)
                currentAngleInRad[i] = fixedAngleInRad[i] = 1f;
                
            fixedPoints = GetStoppingPoints();

            circleCenter = new Vector2(center.transform.localPosition.x, center.transform.localPosition.y);

            for (int i = 0; i < manaCount; i++)
            {
                var mana = this.mana[i];
                mana.transform.localPosition = fixedPoints[i];
            }

        }
        Vector3[] GetStoppingPoints()
        {
            Vector3[] points = new Vector3[manaCount];
            
            for (int i = 0; i < manaCount; i++)
            {
                var mana = this.mana[i];
                float offset = i * (360 / manaCount);
                currentAngleInRad[i] = fixedAngleInRad[i] = offset;
                Debug.Log("Rad " + i + ": " + currentAngleInRad[i]);
                points[i] = new Vector3(
                    circleCenter.x + (radius * Mathf.Cos(currentAngleInRad[i] * Mathf.Deg2Rad)),
                    circleCenter.y + (radius * Mathf.Sin(currentAngleInRad[i] * Mathf.Deg2Rad)),
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
            StartCoroutine(StopRotatingCoroutine());
        }
        IEnumerator StopRotatingCoroutine()
        {
            StopCoroutine(CircularRotateCoroutine());

            for (int i = 0; i < manaCount; i++)
            {
                var mana = this.mana[i];

                float minDistance = Vector3.Distance(mana.transform.localPosition, fixedPoints[i]);
                Vector3 newPos = fixedPoints[i];

                /*
                for(int p = 0; p < manaCount; p++)
                {
                    var point = fixedPoints[p];
                    if(minDistance > Vector3.Distance(mana.transform.localPosition, point))
                    {
                        minDistance = Vector3.Distance(mana.transform.localPosition, point);
                        newPos = point;
                        currentAngleInRad[i] = fixedAngleInRad[p];
                    }
                }
                */
                StartCoroutine(MoveMana(mana, newPos));
            }

            yield return null;
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
            StopCoroutine(StopRotatingCoroutine());
            while (rotating)
            {
                for (int i = 0; i < manaCount; i++)
                {
                    var mana = this.mana[i];

                    currentAngleInRad[i] += (Time.deltaTime * rotateSpeed);
                    mana.transform.localPosition = new Vector3(
                        circleCenter.x + radius + Mathf.Cos(currentAngleInRad[i] * Mathf.Deg2Rad),
                        circleCenter.y + radius + Mathf.Sin(currentAngleInRad[i] * Mathf.Deg2Rad),
                        z);
                }
                yield return null;
            }

        }
    }
}
