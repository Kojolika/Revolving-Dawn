using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tooling.StaticData.Data
{
    public class TypeView : VisualElement
    {
        /// <summary>
        /// Displays the name of the type.
        /// </summary>
        private readonly Label typeLabel;

        /// <summary>
        /// Displays the number of validation errors for the instances of this type.
        /// </summary>
        private readonly Label validationErrorLabel;

        public TypeView()
        {
            typeLabel = new Label();
            validationErrorLabel = new Label
            {
                style = { color = Color.red }
            };

            Add(typeLabel);
            Add(validationErrorLabel);

            style.flexDirection = FlexDirection.Row;
        }

        /// <param name="staticDataType">The type of static data</param>
        /// <param name="numValidationErrors">Number of instances of the <see cref="staticDataType"/> that have validation errors</param>
        public void BindItem(System.Type staticDataType, int numValidationErrors)
        {
            typeLabel.text = staticDataType.Name;
            validationErrorLabel.text = numValidationErrors > 0
                ? numValidationErrors.ToString()
                : string.Empty;
        }

        public void UnBindItem()
        {
            typeLabel.text = string.Empty;
            validationErrorLabel.text = string.Empty;
        }
    }
}