using System.Collections.Generic;
using Tooling.StaticData.Data;
using UnityEngine;
using Utils;

namespace Views
{
    public class TargetingArrowView : MonoBehaviour
    {
        [SerializeField] BezierCurve bezierCurve;
        [SerializeField] GameObject  arrowPiecePrefab;
        [SerializeField] Transform   arrowPieceParent;

        private Camera                 playerHandViewCamera;
        private float                  arrowPieceSize;
        private List<GameObject>       arrowPieces;
        private PlayerHandViewSettings playerHandViewSettings;

        [Zenject.Inject]
        private void Construct(PlayerHandView playerHandView, PlayerHandViewSettings playerHandViewSettings)
        {
            playerHandViewCamera        = playerHandView.Camera;
            this.playerHandViewSettings = playerHandViewSettings;

            bezierCurve.Reset();

            arrowPieceSize = (float)1 / playerHandViewSettings.NumberOfArrowPiecesForTargetingArrow;
            arrowPieces    = new List<GameObject>();
            for (int i = 0; i < playerHandViewSettings.NumberOfArrowPiecesForTargetingArrow; i++)
            {
                arrowPieces.Add(
                    Instantiate(arrowPiecePrefab, bezierCurve.GetPoint(arrowPieceSize), Quaternion.identity, arrowPieceParent)
                );
            }

            SetActive(false);
        }

        public void SetActive(bool isActive) => gameObject.SetActive(isActive);

        public void DrawCurveForCardAndScreenPoint(Vector3 cardPosition, Vector2 screenPoint)
        {
            //Set curve based on mouse position
            float   curveZValue             = cardPosition.z + 1f;
            Vector3 screenPointInWorldSpace = playerHandViewCamera.ScreenToWorldPoint(new Vector3(screenPoint.x, screenPoint.y, curveZValue));
            bezierCurve.SetPoint(1, new Vector3(cardPosition.x, cardPosition.y, curveZValue));
            bezierCurve.SetPoint(2, new Vector3(cardPosition.x, screenPointInWorldSpace.y, curveZValue));
            bezierCurve.SetPoint(3, new Vector3(cardPosition.x, screenPointInWorldSpace.y + 2f, curveZValue));
            bezierCurve.SetPoint(4, screenPointInWorldSpace);

            float positionOnCurve = arrowPieceSize;
            for (int i = 0; i < playerHandViewSettings.NumberOfArrowPiecesForTargetingArrow; i++)
            {
                //set position of pieces on curve
                arrowPieces[i].transform.position = bezierCurve.GetPoint(positionOnCurve);
                //arrowPieces[i].transform.position = new Vector3(arrowPieces[i].transform.position.x, arrowPieces[i].transform.position.y, arrowPieces[i].transform.position.z + (i * 0.1f));

                Vector3 arrowRotation = bezierCurve.GetDirection(positionOnCurve);
                //change rotation of pieces on curve so they follow the curve
                arrowPieces[i].transform.rotation =
                    Quaternion.Euler(
                        0f, 0f, -arrowRotation.x * Computations.Normalize(i, 0, playerHandViewSettings.NumberOfArrowPiecesForTargetingArrow, 1, 175f));
                positionOnCurve += arrowPieceSize;
            }
        }
    }
}