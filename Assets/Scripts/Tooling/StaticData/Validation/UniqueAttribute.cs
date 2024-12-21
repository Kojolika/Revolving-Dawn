using System;
using System.Collections.Generic;
using System.Reflection;
using Tooling.Logging;

namespace Tooling.StaticData.Validation
{
    [AttributeUsage(AttributeTargets.Field)]
    public class UniqueAttribute : Attribute, IValidationAttribute
    {
        public List<string> errorMessages { get; private set; }

        public bool Validate(Type type, StaticData obj, FieldInfo fieldInfo, List<StaticData> allObjects)
        {
            var fieldDict = new Dictionary<Type, HashSet<object>>();
            var thisFieldValue = fieldInfo.GetValue(obj);

            errorMessages = new List<string>();
            
            MyLogger.LogError($"Validating: {fieldInfo.Name}, with value, ");

            foreach (var staticData in allObjects)
            {
                if (staticData == obj)
                {
                    continue;
                }

                if (fieldDict.TryGetValue(staticData.GetType(), out var fieldValueHashSet))
                {
                    fieldValueHashSet.Add(fieldInfo.GetValue(staticData));
                }
                else
                {
                    fieldDict.Add(staticData.GetType(), new HashSet<object> { fieldInfo.GetValue(staticData) });
                }
            }

            if (fieldDict.TryGetValue(obj.GetType(), out var fieldValHashSet)
                && fieldValHashSet.Contains(thisFieldValue))
            {
                errorMessages.Add($"StaticData {obj.Name}'s field {fieldInfo.Name} is not unqiue!");
            }

            return errorMessages.Count == 0;
        }
    }
}