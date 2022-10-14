using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using utils;

namespace fight
{
    public class TargetingArrow : MonoBehaviour
    {
        BezierCurve arrowCurve;

        const float Z_DISTANCE = 15f;

        GameObject arrowHead;
        List<GameObject> arrowLinePieces;
        GameObject arrowCurveParent;
        Vector3 curveStartingPosition;

        Camera CardTargetingCam;


        const int NUM_OF_ARROW_PIECES = 15;
        float t = 0f;
        float pieceSize;

        void Start()
        {
            //Create Empty Gameobject for curve
            arrowCurveParent = new GameObject();
            arrowCurveParent.gameObject.name = "ArrowCurveParent";
            arrowCurveParent.transform.position = new Vector3(0f, 0f, 0f);

            arrowCurve = arrowCurveParent.AddComponent<BezierCurve>();
            
            CardTargetingCam = Instantiate(Resources.Load<Camera>("CardsTargetingCamera"),new Vector3(80f,0f,0f),Quaternion.identity);
        
            curveStartingPosition = CardTargetingCam.ViewportToWorldPoint(new Vector3(0.5f, 0.3f, Z_DISTANCE));

            arrowCurve.Reset();
            arrowCurve.SetPoint(1, curveStartingPosition);

            pieceSize = (float)1 / NUM_OF_ARROW_PIECES;
            arrowLinePieces = new List<GameObject>();
            for (int i = 0; i < NUM_OF_ARROW_PIECES; i++)
            {  
                arrowLinePieces.Add(Instantiate(Resources.Load<GameObject>("ArrowPiece"),
                    arrowCurve.GetPoint(pieceSize),
                    Quaternion.identity, arrowCurveParent.transform));
            }


        }
        void Update()
        {
            Vector3 MousePositionInWorldSpace = CardTargetingCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Z_DISTANCE));
            //MousePositionInWorldSpace = new Vector3(MousePositionInWorldSpace.x / Camera.main.scaledPixelWidth, MousePositionInWorldSpace.y / Camera.main.scaledPixelHeight, MousePositionInWorldSpace.z);
            arrowCurve.SetPoint(2, new Vector3(curveStartingPosition.x, MousePositionInWorldSpace.y, MousePositionInWorldSpace.z));
            arrowCurve.SetPoint(3, new Vector3(curveStartingPosition.x, MousePositionInWorldSpace.y + 2f, MousePositionInWorldSpace.z));
            arrowCurve.SetPoint(4, MousePositionInWorldSpace);

            t = pieceSize;
            for (int i = 0; i < NUM_OF_ARROW_PIECES; i++)
            {
                arrowLinePieces[i].transform.position = arrowCurve.GetPoint(t);
                Vector3 ArrowRotation = arrowCurve.GetDirection(t);
                arrowLinePieces[i].transform.rotation = Quaternion.Euler(0f, 0f, -ArrowRotation.x * 100f);
                t += pieceSize;
            }
        }
        private void OnDestroy()
        {
            Destroy(arrowCurveParent);
        }
    }
}