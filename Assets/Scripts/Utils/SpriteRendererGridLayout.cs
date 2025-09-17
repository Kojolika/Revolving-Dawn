using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Utils
{
    /// <summary>
    /// This component acts similar to the <see cref="UnityEngine.UI.GridLayoutGroup"/> for rect transforms.
    /// Note: this component works under the assumption that each child sprite renderer is of the same size sprite
    /// </summary>
    [ExecuteAlways]
    public class SpriteRendererGridLayout : MonoBehaviour
    {
        /// <summary>
        /// The axis in which the grid elements will populate on.
        /// </summary>
        [Tooltip("The axis in which the grid elements will populate on.")]
        [SerializeField] Axis expandDirection;

        /// <summary>
        /// The direction on the axis in which the grid elements will populate on.
        /// </summary>
        [Tooltip("The direction on the axis in which the grid elements will populate on.")]
        [SerializeField] Direction direction;

        /// <summary>
        /// The spacing between the grid elements.
        /// </summary>
        [Tooltip("The spacing between the grid elements.")]
        [SerializeField] Vector2 padding;

        /// <summary>
        /// The number of rows in the grid.
        /// </summary>
        [Tooltip("The number of rows in the grid.")]
        [SerializeField] int elementsPerAxis = 1;

        /// <summary>
        /// Normalized position between 0 and 1 where, 0,0 is the top left border and 1,1 is the bottom right.
        /// The grid elements will be positioned according to this pivot.
        /// (Example: for centering, set the pivot to be 0.5, 0.5)
        /// </summary>
        [Tooltip("Normalized position between 0 and 1 where, 0,0 is the top left border and 1,1 is the bottom right.")]
        [SerializeField]
        Vector2 pivotPosition = new(0.5f, 0.5f);

        /// <summary>
        /// List of the elements in this grid. 
        /// The elements are determined if they are a child of this component with a <see cref="SpriteRenderer"/> attached to them.
        /// </summary>
        private readonly List<SpriteRenderer> childSpriteRenderersList = new();
        private Vector3 localStartPoint = Vector3.zero;

        private void OnValidate()
        {
            if (padding.x < 0)
            {
                throw new System.Exception("Must have a non-negative value for the x padding of the grid cell.");
            }
            if (padding.y < 0)
            {
                throw new System.Exception("Must have a non-negative value for the y padding of the grid cell.");
            }

            if (elementsPerAxis <= 0)
            {
                throw new System.Exception("Must have a positive value for the number of rows.");
            }

            pivotPosition = new Vector2(
                Mathf.Clamp(pivotPosition.x, 0, 1),
                Mathf.Clamp(pivotPosition.y, 0, 1)
            );

            _ = SetPositionsInGrid();
        }

        private void ValidateRendererList()
        {
            // handle renderers that are disabled or have changed parents
            var removedChildren = new HashSet<SpriteRenderer>();
            foreach (var spriteRenderer in childSpriteRenderersList)
            {
                if (spriteRenderer == null || !spriteRenderer.transform.IsChildOf(transform) || !spriteRenderer.gameObject.activeInHierarchy)
                {
                    removedChildren.Add(spriteRenderer);
                }
            }
            foreach (var spriteRenderer in removedChildren)
            {
                childSpriteRenderersList.Remove(spriteRenderer);
            }

            // handle new renderers that have been added
            foreach (var spriteRenderer in GetComponentsInChildren<SpriteRenderer>())
            {
                if (childSpriteRenderersList.Contains(spriteRenderer))
                {
                    continue;
                }

                childSpriteRenderersList.Add(spriteRenderer);
            }

            // remove duplicate references
            var visitedRenderers = new HashSet<SpriteRenderer>();
            for (int i = 0; i < childSpriteRenderersList.Count; i++)
            {
                var spriteRenderer = childSpriteRenderersList[i];
                if (visitedRenderers.Contains(spriteRenderer))
                {
                    childSpriteRenderersList.RemoveAt(i);
                }

                visitedRenderers.Add(spriteRenderer);
            }
        }

        private void OnTransformChildrenChanged()
        {

            _ = SetPositionsInGrid();
        }

        [ContextMenu("Reset Grid Positions")]
        private async UniTask SetPositionsInGrid()
        {
            ValidateRendererList();

            // We wait a for the update event as spriteRenderers that were added in this frame
            // will not have their position value updated until the next frame
            await UniTask.Yield(PlayerLoopTiming.Update);

            int currentElementsPerAxisCount = 0;
            float maxXDistance = 0;
            float maxYDistance = 0;
            for (int i = 0; i < childSpriteRenderersList.Count; i++)
            {
                var spriteRenderer = childSpriteRenderersList[i];
                var spriteRendererTransform = spriteRenderer.transform;
                if (i == 0)
                {
                    spriteRendererTransform.localPosition = localStartPoint;
                }
                else
                {
                    var previousElementLocalPosition = childSpriteRenderersList[i - 1].transform.localPosition;
                    if (expandDirection == Axis.Horizontal)
                    {
                        if (currentElementsPerAxisCount >= elementsPerAxis)
                        {
                            spriteRendererTransform.localPosition = new Vector3(
                                localStartPoint.x,
                                previousElementLocalPosition.y
                                    + (transform.InverseTransformVector(childSpriteRenderersList[i - 1].bounds.size).y
                                    + padding.y) * (direction == Direction.Positive ? 1 : -1),
                                previousElementLocalPosition.z
                            );
                            currentElementsPerAxisCount = 0;
                        }
                        else
                        {
                            spriteRendererTransform.localPosition = new Vector3(
                                previousElementLocalPosition.x
                                    + (transform.InverseTransformVector(childSpriteRenderersList[i - 1].bounds.size).x
                                    + padding.x) * (direction == Direction.Positive ? 1 : -1),
                                previousElementLocalPosition.y,
                                previousElementLocalPosition.z
                            );
                        }
                    }
                    else
                    {
                        if (currentElementsPerAxisCount >= elementsPerAxis)
                        {
                            spriteRendererTransform.localPosition = new Vector3(
                                previousElementLocalPosition.x
                                    + (transform.InverseTransformVector(childSpriteRenderersList[i - 1].bounds.size).x
                                    + padding.x) * (direction == Direction.Positive ? 1 : -1),
                                localStartPoint.y,
                                previousElementLocalPosition.z
                            );
                            currentElementsPerAxisCount = 0;
                        }
                        else
                        {
                            spriteRendererTransform.localPosition = new Vector3(
                                previousElementLocalPosition.x,
                                previousElementLocalPosition.y
                                    + (transform.InverseTransformVector(childSpriteRenderersList[i - 1].bounds.size).y
                                    + padding.y) * (direction == Direction.Positive ? 1 : -1),
                                previousElementLocalPosition.z
                            );
                        }
                    }
                }

                var absXDistanceFromStart = Mathf.Abs(spriteRendererTransform.localPosition.x - localStartPoint.x);
                if (absXDistanceFromStart > maxXDistance)
                {
                    maxXDistance = absXDistanceFromStart;
                }
                var absYDistanceFromStart = Mathf.Abs(spriteRendererTransform.localPosition.y - localStartPoint.y);
                if (absYDistanceFromStart > maxYDistance)
                {
                    maxYDistance = absYDistanceFromStart;
                }
                currentElementsPerAxisCount++;
            }

            foreach (var element in childSpriteRenderersList)
            {
                var elementTransform = element.transform;
                elementTransform.localPosition = new Vector3(
                    elementTransform.localPosition.x - (pivotPosition.x * maxXDistance * (direction == Direction.Positive ? 1 : -1)),
                    elementTransform.localPosition.y - (pivotPosition.y * maxYDistance * (direction == Direction.Positive ? 1 : -1)),
                    elementTransform.localPosition.z
                );
            }
        }

        private enum Axis
        {
            Horizontal,
            Vertical,
        }
        private enum Direction
        {
            Positive,
            Negative
        }
    }
}