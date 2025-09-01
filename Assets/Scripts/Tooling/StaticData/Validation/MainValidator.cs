using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tooling.Logging;
using UnityEditor;
using Utils.Extensions;

namespace Tooling.StaticData.EditorUI.Validation
{
    public class MainValidator
    {
        private readonly List<IValidator> validators;

        public MainValidator(List<IValidator> validators)
        {
            this.validators = validators;
        }

        public Dictionary<System.Type, Dictionary<StaticData, List<string>>> ValidateObjects(List<StaticData> objects)
        {
            return ValidateObjects(objects, validators);
        }

        /// <summary>
        /// Validates a list of objects and returns a dictionary that maps their type to the errors found
        /// </summary>
        /// <param name="objects">List of objects to validate</param>
        /// <param name="validators">List of custom validators</param>
        /// <returns>A dictionary mapping the type to the list of errors for that type.</returns>
        private static Dictionary<System.Type, Dictionary<StaticData, List<string>>> ValidateObjects(
            List<StaticData> objects,
            List<IValidator> validators)
        {
            var errorDict = new Dictionary<System.Type, Dictionary<StaticData, List<string>>>();
            var objectCount = objects.Count;
            for (int i = 0; i < objectCount; i++)
            {
                var obj = objects[i];
                var objType = obj.GetType();

                EditorUtility.DisplayProgressBar("Validating", $"{objType.Name}", (float)i / objectCount);

                if (IsValid(objType, objects[i], objects, out var errors, validators))
                {
                    continue;
                }

                if (errorDict.TryGetValue(objType, out var objToErrorDict))
                {
                    objToErrorDict.Add(obj, errors);
                }
                else
                {
                    errorDict.Add(objType, new Dictionary<StaticData, List<string>>());
                    errorDict[objType].Add(obj, errors);
                }
            }

            EditorUtility.ClearProgressBar();

            return errorDict;
        }

        private static bool IsValid(System.Type type,
            StaticData obj,
            List<StaticData> objects,
            out List<string> errorMessages,
            List<IValidator> validators = null)
        {
            var fieldAttributesTuple = Utils.GetFields(type)
                .Select(field => (field, attributes: field.GetCustomAttributes(true)
                    .Where(attribute => attribute is IValidator)))
                .ToList();

            // use to display the progress on the progress bar
            var attributeCount = fieldAttributesTuple
                .SelectMany(tuple => tuple.attributes)
                .Count();

            errorMessages = new();
            int attributeCounter = 0;
            foreach (var tuple in fieldAttributesTuple)
            {
                var field = tuple.field;

                // TODO: add progress bar for validators
                if (!validators.IsNullOrEmpty())
                {
                    var fieldValidators = validators!.Where(validator => validator.CanValidate(field.FieldType));
                    foreach (var validator in fieldValidators)
                    {
                        if (!validator.Validate(type, obj, field, objects))
                        {
                            errorMessages.AddRange(validator.errorMessages
                                .Select(errorMessage => $"{validator.GetType().Name}: [{type.Name}.{field.Name}] error: {errorMessage}"));
                        }
                    }
                }

                foreach (var attribute in tuple.attributes)
                {
                    EditorUtility.DisplayProgressBar(
                        "Validating", $"{attribute.GetType()}: {obj.GetType().Name}.{tuple.field.Name}",
                        (float)attributeCounter / attributeCount
                    );

                    var validationAttribute = (IValidator)attribute;
                    if (!validationAttribute.CanValidate(field.FieldType))
                    {
                        MyLogger.LogError($"Validation attribute :{attribute.GetType()} is applied to field type {field.FieldType}" +
                                          $"but the {nameof(IValidator.CanValidate)} returns false for the field type.");
                        attributeCounter++;
                        continue;
                    }

                    if (!validationAttribute.Validate(type, obj, tuple.field, objects))
                    {
                        var tuple1 = tuple;
                        errorMessages.AddRange(validationAttribute.errorMessages
                            .Select(errorMessage =>
                                $"{attribute.GetType().Name}: [{type.Name}.{tuple1.field.Name}] error: {errorMessage}"));
                    }

                    attributeCounter++;
                }
            }

            EditorUtility.ClearProgressBar();

            return errorMessages.Count == 0;
        }
    }
}