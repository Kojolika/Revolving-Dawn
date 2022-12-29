using System.Collections;
using UnityEngine;

namespace fight
{
    public class Mover : MonoBehaviour
    {
        bool local;
        Vector3 destination;
        float speed;
        public void Initialize(Vector3 destination, float speed, bool local = false)
        {
            this.local = local;
            this.destination = destination;
            this.speed = speed;
        }       
        public IEnumerator MoveGameObjectCoroutine()
        {
            if (local)
            {
                while (this.gameObject.transform.localPosition != destination)
                {
                    this.gameObject.transform.localPosition = Vector3.MoveTowards(this.gameObject.transform.localPosition, destination, speed * Time.deltaTime);
                    yield return null;
                }
            }
            else
            {
                while (this.gameObject.transform.position != destination)
                {
                    this.gameObject.transform.position = Vector3.MoveTowards(this.gameObject.transform.position, destination, speed * Time.deltaTime);
                    yield return null;
                }
            }

        }
    }
}
