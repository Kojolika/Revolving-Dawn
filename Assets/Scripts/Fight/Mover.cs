using System.Collections;
using UnityEngine;

namespace fight
{
    public class Mover : MonoBehaviour
    {
        bool local;
        public void Initialize(Vector3 destination, float speed, bool local = false)
        {
            this.local = local;
            StartCoroutine(MoveCardCoroutine(destination, speed));

            if (this.gameObject.transform.position == destination && !local)
                Destroy(this);
            else if(this.gameObject.transform.localPosition == destination)
                Destroy(this);
        }       
        IEnumerator MoveCardCoroutine(Vector3 destination, float speed)
        {
            if (local)
            {
                while (this.gameObject.transform.localPosition != destination)
                {
                    Debug.Log("Moving local...");
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
