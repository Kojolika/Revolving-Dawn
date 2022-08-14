using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cards;

public class CardMover : MonoBehaviour
{
    Card cardToMove;
    Vector3 destination;
    float speed;
    float rotation;


    public void Initialize(Card card, Vector3 destination, float rotation, float speed)
    {
        if (this.gameObject.GetComponent<CardMover>())
        {
            foreach(CardMover cd in this.gameObject.GetComponents<CardMover>())
            {
                if(cd != this) Destroy(cd);
            }
        }
        cardToMove = card;
        this.destination = destination;
        this.rotation = rotation;
        this.speed = speed;

        cardToMove.transform.rotation = Quaternion.Euler(new Vector3(90f + this.rotation, 90f, -90f));

    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (cardToMove.transform.position == destination)
        {
            Destroy(this);
        }

        cardToMove.transform.position = Vector3.MoveTowards(cardToMove.transform.position, destination, speed * Time.deltaTime);

    }
}
