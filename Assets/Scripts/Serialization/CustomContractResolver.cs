using System;
using System.Collections;
using System.Reflection;
using System.Runtime.Serialization;
using Newtonsoft.Json.Serialization;
using Tooling.Logging;
using Tooling.StaticData.Data;
using Zenject;
using Type = System.Type;

namespace Serialization
{
    /// <summary>
    /// This class is used to inject dependencies into classes which are deserialized from JSON.
    /// </summary>
    public class CustomContractResolver : DefaultContractResolver
    {
        private readonly DiContainer    diContainer;
        private readonly NamingStrategy namingStrategy = new DefaultNamingStrategy();

        /// <summary>
        /// Used during runtime to instantiate objects with our di container.
        /// </summary>
        public CustomContractResolver(DiContainer diContainer)
        {
            this.diContainer = diContainer;
            NamingStrategy   = namingStrategy;
        }

        public CustomContractResolver()
        {
            NamingStrategy = namingStrategy;
        }

        protected override JsonObjectContract CreateObjectContract(Type objectType)
        {
            var objectContract = base.CreateObjectContract(objectType);

            // Instantiate objects that are bound to our container with its dependencies
            if (diContainer?.HasBinding(objectType) ?? false)
            {
                objectContract.OverrideCreator = args => diContainer.Instantiate(objectType, args);
            }

            objectContract.OnDeserializedCallbacks.Add(FindStaticDataReferences);

            return objectContract;
        }

        private void FindStaticDataReferences(object obj, StreamingContext context)
        {
            RecursivelyResolveStaticDataReferences(obj);
        }

        private static void RecursivelyResolveStaticDataReferences(object obj, int arrayIndex = -1)
        {
            var objType = obj.GetType();
            foreach (var field in objType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (IsStaticDataField(obj, field, out var staticDataReference, arrayIndex)
                 && (staticDataReference?.IsReferenceValid() ?? false))
                {
                    StaticDatabase.Instance.QueueReferenceForInject(
                        staticDataReference.Type,
                        staticDataReference.InstanceName,
                        obj,
                        field.Name
                    );
                }
                else if (typeof(IList).IsAssignableFrom(field.FieldType))
                {
                    var fieldList = (IList)field.GetValue(obj);
                    if (fieldList == null)
                    {
                        continue;
                    }

                    for (int i = 0; i < fieldList.Count; i++)
                    {
                        if (IsStaticDataField(obj, field, out var staticDataRef, i)
                         && staticDataRef.IsReferenceValid())
                        {
                            StaticDatabase.Instance.QueueReferenceForInject(
                                staticDataRef.Type,
                                staticDataRef.InstanceName,
                                obj,
                                field.Name,
                                i
                            );
                        }
                        else
                        {
                            RecursivelyResolveStaticDataReferences(fieldList[i], i);
                        }
                    }
                }
            }

            return;

            bool IsStaticDataField(object objToCheck, FieldInfo field, out StaticDataReference staticDataReference, int index = -1)
            {
                var isListField = index > -1;

                StaticData staticData;
                staticDataReference = null;
                if (isListField && field.GetValue(objToCheck) is IList list)
                {
                    if (index > list.Count - 1)
                    {
                        MyLogger.Error($"Index passed into field is greater than the list size! fieldName={field.Name}, index={index}, count={list.Count}");
                        return false;
                    }

                    staticData = list[index] as StaticData;
                }
                else
                {
                    staticData = field.GetValue(objToCheck) as StaticData;
                }

                if (staticData?.Reference == null)
                {
                    return false;
                }

                staticDataReference = staticData.Reference;
                return true;
            }
        }
    }
}