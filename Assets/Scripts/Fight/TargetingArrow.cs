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

        public Camera cardCam;
        public Camera arrowCam;


        const int NUM_OF_ARROW_PIECES = 20;
        float t = 0f;
        float pieceSize;

        void Start()
        {
            //Create Empty Gameobject for curve
            arrowCurveParent = new GameObject();
            arrowCurveParent.gameObject.name = "ArrowCurveParent";
            arrowCurveParent.transform.position = new Vector3(0f, 0f, 0f);

            arrowCurve = arrowCurveParent.AddComponent<BezierCurve>();
            
            arrowCam = GameObject.Find("CardsTargetingCamera(Clone)").GetComponent<Camera>();
        
            curveStartingPosition = arrowCam.ViewportToWorldPoint(new Vector3(0.5f, 0.3f, Z_DISTANCE));

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

        public void ChangeColor(Material m)
        {
           foreach(var piece in arrowLinePieces)
           {
                piece.GetComponentInChildren<MeshRenderer>().materials[0] = m;
           } 

        }
        void Update()
        {
            //Set curve based on mouse position
            Vector3 MousePositionInWorldSpace = arrowCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Z_DISTANCE));
            arrowCurve.SetPoint(2, new Vector3(curveStartingPosition.x, MousePositionInWorldSpace.y, MousePositionInWorldSpace.z));
            arrowCurve.SetPoint(3, new Vector3(curveStartingPosition.x, MousePositionInWorldSpace.y + 2f, MousePositionInWorldSpace.z));
            arrowCurve.SetPoint(4, MousePositionInWorldSpace);

            t = pieceSize;
            for (int i = 0; i < NUM_OF_ARROW_PIECES; i++)
            {
                //set position of pieces on curve
                arrowLinePieces[i].transform.position = arrowCurve.GetPoint(t);
                arrowLinePieces[i].transform.position = new Vector3(arrowLinePieces[i].transform.position.x, arrowLinePieces[i].transform.position.y,arrowLinePieces[i].transform.position.z + (i * 0.1f));

                Vector3 ArrowRotation = arrowCurve.GetDirection(t);
                //change rotation of pieces on curve so they follow the curve
                arrowLinePieces[i].transform.rotation = Quaternion.Euler(0f, 0f, -ArrowRotation.x * Computations.Normalize(i,0,NUM_OF_ARROW_PIECES,1,175f));
                t += pieceSize;
            }
        }
        private void OnDestroy()
        {
            Destroy(arrowCurveParent);
        }
    }
}