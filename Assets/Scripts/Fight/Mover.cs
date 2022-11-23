using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cards;

public class Mover : MonoBehaviour
{
    public void Initialize(Vector3 destination, float speed)
    {
        StartCoroutine(MoveCardCoroutine(destination, speed));

        if(this.gameObject.transform.position == destination) 
            Destroy(this);
    }
    IEnumerator MoveCardCoroutine(Vector3 destination, float speed)
    {
        while(this.gameObject.transform.position != destination)
        {
            this.gameObject.transform.position = Vector3.MoveTowards(this.gameObject.transform.position, destination, speed * Time.deltaTime);
            yield return null;
        }
    }
}
