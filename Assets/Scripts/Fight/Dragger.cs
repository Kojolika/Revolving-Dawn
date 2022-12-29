using UnityEngine;
using cards;
using System.Collections;

public class Dragger : MonoBehaviour 
{
    public Camera cardCam;
    Vector3 mousePos;
    public void StartDragging()
    {
        StartCoroutine(DraggingCoroutine(this.gameObject));
    }
    
    public void StopDragging()
    {
        StopCoroutine(DraggingCoroutine(this.gameObject));
    }
    IEnumerator DraggingCoroutine(GameObject go)
    {
        while(true)
        {
            Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, cardCam.nearClipPlane + 7f);
            mousePos = cardCam.ScreenToWorldPoint(mousePosition);
            go.transform.position = mousePos;

            yield return null;
        }
    }
}