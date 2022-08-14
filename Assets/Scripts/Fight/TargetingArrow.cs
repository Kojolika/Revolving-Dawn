using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using utils;

namespace fight
{
    public class TargetingArrow : MonoBehaviour
    {
        BezierCurve arrowCurve;

        float zDistance;

        GameObject arrowHead;
        List<GameObject> arrowLinePieces;
        GameObject arrowCurveParent;
        Vector3 currentCardPosition;
        Vector3 curveStartingPosition;


        static int NUM_OF_ARROW_PIECES = 20;
        float t = 0f;
        float pieceSize;

        public void Initialize(Vector3 position)
        {
            currentCardPosition = position;
        }
        void Start()
        {
            //Create Empty Gameobject for curve
            arrowCurveParent = new GameObject();
            arrowCurveParent.gameObject.name = "ArrowCurveParent";
            arrowCurveParent.transform.position = new Vector3(0f, 0f, 0f);

            arrowCurve = arrowCurveParent.AddComponent<BezierCurve>();
            arrowCurve.transform.position = currentCardPosition;

            zDistance = 15f;
            curveStartingPosition = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.3f, zDistance));
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
            Vector3 MousePositionInWorldSpace = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, zDistance));
            //MousePositionInWorldSpace = new Vector3(MousePositionInWorldSpace.x / Camera.main.scaledPixelWidth, MousePositionInWorldSpace.y / Camera.main.scaledPixelHeight, MousePositionInWorldSpace.z);
            arrowCurve.SetPoint(2, new Vector3(curveStartingPosition.x, MousePositionInWorldSpace.y, MousePositionInWorldSpace.z));
            arrowCurve.SetPoint(3, new Vector3(curveStartingPosition.x, MousePositionInWorldSpace.y, MousePositionInWorldSpace.z));
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