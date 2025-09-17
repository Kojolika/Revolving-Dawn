using UnityEngine.UIElements;

namespace Utils.Extensions
{
    public static class VisualElementExtensions
    {
        /// <summary>
        /// Removes an element from a VisualElement's container if it is a child.
        /// </summary>
        public static void RemoveIfChild(this VisualElement visualElement, VisualElement child)
        {
            if (child == null)
            {
                return;
            }

            if (visualElement.IndexOf(child) is var childIndex and > -1)
            {
                visualElement.RemoveAt(childIndex);
            }
        }
    }
}