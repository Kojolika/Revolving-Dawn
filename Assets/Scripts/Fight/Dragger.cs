using UnityEngine;
using cards;
using System.Collections;

public class Dragger : MonoBehaviour 
{

    Vector3 mousePos;
    public void StartDragging(Card card)
    {
        StartCoroutine(DraggingCoroutine(card));
    }
    
    public IEnumerator DraggingCoroutine(Card card)
    {
        while(true)
        {
            Vector3 mousePoisiton = new Vector3(Input.mousePosition.x, Input.mousePosition.y, CardInfo.CAMERA_DISTANCE);
            mousePos = Camera.main.ScreenToWorldPoint(mousePoisiton);
            card.transform.position = mousePos;

            yield return null;
        }
    }
}