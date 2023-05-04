using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using fight;

namespace mana
{
    public class ManaPool : MonoBehaviour
    {
        public List<Mana3D> pool = new List<Mana3D>();
        [SerializeField] Mana3D manaPrefab;
        int manaCount;

        public GameObject center;
        float radius = .5f;
        Vector2 circleCenter;
        float rotateSpeed = 50f;
        float z = 2f;
        float[] angle;
        bool rotating = false;

        void Start()
        {
            circleCenter = new Vector2(center.transform.localPosition.x, center.transform.localPosition.y);
            ResetPool();
        }

        Vector3[] GetStoppingPoints()
        {
            Vector3[] points = new Vector3[manaCount];
            angle = new float[manaCount];

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
            StopCoroutine(CircularRotateCoroutine());
        }

        public void StartCircularRotate()
        {
            rotating = true;
            StartCoroutine(CircularRotateCoroutine());
        }
        IEnumerator CircularRotateCoroutine()
        {
            manaCount = pool.Count;
            while (rotating)
            {
                for (int i = 0; i < manaCount; i++)
                {
                    var mana = this.pool[i];

                    angle[i] += (Time.deltaTime * rotateSpeed);
                    mana.transform.localPosition = new Vector3(
                        circleCenter.x + radius + Mathf.Cos(angle[i] * Mathf.Deg2Rad),
                        circleCenter.y + radius + Mathf.Sin(angle[i] * Mathf.Deg2Rad),
                        z);
                }

                yield return null;
            }

        }

        public void AddMana(ManaType type)
        {    
            Mana3D mana = Instantiate(manaPrefab, this.transform, true);

            pool.Add(mana);
            ResetPool();
        }
        public void RemoveMana(Mana3D mana)
        {
            StopAllCoroutines();
            pool.Remove(mana);
            ResetPool();
        }

        void ResetPool()
        {
            manaCount = this.pool.Count;
            var points = GetStoppingPoints();
            for (int i = 0; i < manaCount; i++)
            {
                pool[i].transform.localPosition = points[i];
            }
        }
    }
}
