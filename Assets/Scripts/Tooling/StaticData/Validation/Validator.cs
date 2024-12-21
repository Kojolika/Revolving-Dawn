using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace Tooling.StaticData.Validation
{
    public class Validator
    {
        /// <summary>
        /// Validates a list of objects and returns a dictionary that maps their type to the errors found
        /// </summary>
        /// <param name="objects">List of objects to validate</param>
        /// <param name="bindingFlags">The flags to search the fields of the objects to validate</param>
        /// <returns>A dictionary mapping the type to the list of errors for that type.</returns>
        public static Dictionary<Type, Dictionary<StaticData, List<string>>> ValidateObjects(List<StaticData> objects,
            BindingFlags bindingFlags)
        {
            var errorDict = new Dictionary<Type, Dictionary<StaticData, List<string>>>();
            

            var objectCount = objects.Count;
            for (int i = 0; i < objectCount; i++)
            {
                var obj = objects[i];
                var objType = obj.GetType();

                EditorUtility.DisplayProgressBar("Validating", $"{objType.Name}", (float)i / objectCount);

                if (IsValid(objType, objects[i], objects, out var errors, bindingFlags))
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

        public static bool IsValid(Type type,
            StaticData obj,
            List<StaticData> objects,
            out List<string> errorMessages,
            BindingFlags bindingFlags)
        {
            var fieldAttributesTuple = type.GetFields(bindingFlags)
                .Select(field => (field, attributes: field.GetCustomAttributes(true)
                    .Where(attribute => attribute is IValidationAttribute)))
                .ToList();

            // use to display the progress on the progress bar
            var attributeCount = fieldAttributesTuple
                .SelectMany(tuple => tuple.attributes)
                .Count();

            errorMessages = new();
            int attributeCounter = 0;
            foreach (var tuple in fieldAttributesTuple)
            {
                foreach (var attribute in tuple.attributes)
                {
                    EditorUtility.DisplayProgressBar(
                        "Validating", $"{attribute.GetType()}: {obj.GetType().Name}.{tuple.field.Name}",
                        (float)attributeCounter / attributeCount
                    );

                    var validationAttribute = (IValidationAttribute)attribute;
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