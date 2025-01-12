using System;
using System.Collections;
using System.Reflection;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Tooling.Logging;
using Tooling.StaticData;
using Zenject;

namespace Serialization
{
    /// <summary>
    /// This class is used to inject dependencies into classes which are deserialized form JSON.
    /// </summary>
    public class CustomContractResolver : DefaultContractResolver
    {
        private readonly DiContainer diContainer;
        private readonly NamingStrategy namingStrategy = new DefaultNamingStrategy();

        /// <summary>
        /// Used during runtime to instantiate objects with our di container.
        /// </summary>
        public CustomContractResolver(DiContainer diContainer)
        {
            this.diContainer = diContainer;
            NamingStrategy = namingStrategy;
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

        public static void RecursivelyResolveStaticDataReferences(object obj, int arrayIndex = -1)
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
                        field.Name,
                        arrayIndex
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
                var staticData = isListField
                    ? (field.GetValue(objToCheck) as IList)?[index] as StaticData
                    : field.GetValue(objToCheck) as StaticData;

                if (staticData == null)
                {
                    staticDataReference = null;
                    return false;
                }

                staticDataReference = staticData.SerializedReference;

                return true;
            }
        }
    }
}