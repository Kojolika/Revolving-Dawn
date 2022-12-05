using UnityEngine;
using cards;
using System.Collections;

public class Dragger : MonoBehaviour 
{
    public Camera cardCam;
    Vector3 mousePos;
    public void StartDragging(Card card)
    {
        StartCoroutine(DraggingCoroutine(card));
    }
    
    public IEnumerator DraggingCoroutine(Card card)
    {
        while(true)
        {
            Vector3 mousePoisiton = new Vector3(Input.mousePosition.x, Input.mousePosition.y, cardCam.nearClipPlane + 7f);
            mousePos = cardCam.ScreenToWorldPoint(mousePoisiton);
            card.transform.position = mousePos;

            yield return null;
        }
    }
}