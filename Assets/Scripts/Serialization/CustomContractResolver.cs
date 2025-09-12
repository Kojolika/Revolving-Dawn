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

        private static void FindStaticDataReferences(object obj, StreamingContext context)
        {
            RecursivelyResolveStaticDataReferences(obj);
        }

        private static void RecursivelyResolveStaticDataReferences(object obj)
        {
            var objType = obj.GetType();
            foreach (var field in objType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (IsStaticDataField(field.GetValue(obj), out var staticDataReference))
                {
                    StaticDatabase.Instance.QueueReferenceForInject(
                        staticDataReference.Type,
                        staticDataReference.InstanceName,
                        obj,
                        field.Name,
                        StaticDatabase.MemberType.Field
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
                        if (IsStaticDataField(fieldList[i], out var staticDataRef))
                        {
                            StaticDatabase.Instance.QueueReferenceForInject(
                                staticDataRef.Type,
                                staticDataRef.InstanceName,
                                obj,
                                field.Name,
                                StaticDatabase.MemberType.Field,
                                i
                            );
                        }
                        else
                        {
                            RecursivelyResolveStaticDataReferences(fieldList[i]);
                        }
                    }
                }
            }

            foreach (var prop in objType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (IsStaticDataField(prop.GetValue(obj), out var staticDataReference))
                {
                    StaticDatabase.Instance.QueueReferenceForInject(
                        staticDataReference.Type,
                        staticDataReference.InstanceName,
                        obj,
                        prop.Name,
                        StaticDatabase.MemberType.Property
                    );
                }
                else if (typeof(IList).IsAssignableFrom(prop.PropertyType))
                {
                    var propList = (IList)prop.GetValue(obj);
                    if (propList == null)
                    {
                        continue;
                    }

                    for (int i = 0; i < propList.Count; i++)
                    {
                        if (IsStaticDataField(propList[i], out var staticDataRef))
                        {
                            StaticDatabase.Instance.QueueReferenceForInject(
                                staticDataRef.Type,
                                staticDataRef.InstanceName,
                                obj,
                                prop.Name,
                                StaticDatabase.MemberType.Property,
                                i
                            );
                        }
                        else
                        {
                            RecursivelyResolveStaticDataReferences(propList[i]);
                        }
                    }
                }
            }
        }

        private static bool IsStaticDataField(object obj, out StaticDataReference staticDataReference)
        {
            if (obj is not StaticData staticData)
            {
                staticDataReference = null;
                return false;
            }

            staticDataReference = staticData.Reference;
            return staticDataReference.IsReferenceValid();
        }
    }
}