using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using fight;

namespace mana
{
    public class ManaPool : MonoBehaviour
    {
        public List<Mana> manaPool = new List<Mana>();
        int manaCount;

        public GameObject center;
        float radius = .5f;
        Vector2 circleCenter;
        float rotateSpeed = 50f;
        float moveSpeed = .001f;
        float z = 2f;
        float[] angle;

        void Start()
        {
            manaCount = manaPool.Count;
            angle = new float[manaCount];
            for (int i = 0; i < manaCount; i++)
                angle[i] = 1f;

            circleCenter = new Vector2(center.transform.localPosition.x, center.transform.localPosition.y);

            for (int i = 0; i < manaCount; i++)
            {
                var mana = manaPool[i];
                float offset = i * (360 / manaCount);
                angle[i] = offset;
                mana.transform.localPosition = new Vector3(
                    circleCenter.x + radius + Mathf.Cos(angle[i] * Mathf.Deg2Rad),
                    circleCenter.y + radius + Mathf.Sin(angle[i] * Mathf.Deg2Rad),
                    z);
            }

        }
        public void StopRotating()
        {
            StartCoroutine(StopRotatingCoroutine());
        }
        IEnumerator StopRotatingCoroutine()
        {
            StopCoroutine(CircularRotateCoroutine());

            for (int i = 0; i < manaCount; i++)
            {
                var mana = manaPool[i];
                float offset = i * (360 / manaCount);
                angle[i] = offset;

                Vector3 newPos = new Vector3(
                    circleCenter.x + radius + Mathf.Cos(angle[i] * Mathf.Deg2Rad),
                    circleCenter.y + radius + Mathf.Sin(angle[i] * Mathf.Deg2Rad),
                    z);

                Mover mover = mana.gameObject.AddComponent<Mover>();
                mover.Initialize(newPos, moveSpeed, true);
            }

            yield return null;
        }
        public void StartCircularRotate()
        {
            StartCoroutine(CircularRotateCoroutine());
        }
        IEnumerator CircularRotateCoroutine()
        {
            while (true)
            {
                for (int i = 0; i < manaCount; i++)
                {
                    var mana = manaPool[i];
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
