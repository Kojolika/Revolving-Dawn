using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Tooling.Logging;

namespace Tooling.StaticData
{
    public class ReferenceResolver : IReferenceResolver
    {
        private readonly Assembly assembly = Assembly.Load("Assembly-CSharp");

        private Dictionary<string, StaticData> references = new();

        public object ResolveReference(object context, string reference)
        {
            MyLogger.Log(
                $"{nameof(ResolveReference)}: Context type :{context.GetType()}, ref: {reference}, is reader: {context is JsonReader}");

            return references.GetValueOrDefault(reference);
        }

        public string GetReference(object context, object value)
        {
            MyLogger.Log($"{nameof(GetReference)}: Context type :{context.GetType()}, value: {value}");

            if (value is not StaticData staticData)
            {
                return string.Empty;
            }

            return $"{value.GetType().FullName}:{staticData.Name}";
        }

        public bool IsReferenced(object context, object value)
        {
            MyLogger.Log($"{nameof(IsReferenced)}: Context type :{context.GetType()}, value: {value}");

            if (value is not StaticData staticData)
            {
                return false;
            }

            return references.ContainsKey(GetReference(context, staticData));
        }

        public void AddReference(object context, string reference, object value)
        {
            MyLogger.Log($"{nameof(AddReference)}: Context type :{context.GetType()}, value: {value}, ref: {reference}");

            if (value is not StaticData staticData)
            {
                return;
            }

            references[GetReference(context, value)] = staticData;
        }
    }
}