using UnityEngine;
using cards;
using System.Collections;

public class Dragger : MonoBehaviour 
{

    Vector3 mousePos;
    static float CAMERA_DISTANCE;
    void Start() {
        CAMERA_DISTANCE = Camera.main.nearClipPlane + 7;
    }
    public void StartDragging(Card card)
    {
        StartCoroutine(DraggingCoroutine(card));
    }
    
    public IEnumerator DraggingCoroutine(Card card)
    {
        while(true)
        {
            Vector3 mousePoisiton = new Vector3(Input.mousePosition.x, Input.mousePosition.y, CAMERA_DISTANCE);
            mousePos = Camera.main.ScreenToWorldPoint(mousePoisiton);
            card.transform.position = mousePos;

            yield return null;
        }
    }
}