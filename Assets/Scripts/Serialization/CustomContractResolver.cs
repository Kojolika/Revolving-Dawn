using System.Collections;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using Common.Util;
using Models.Characters;
using Models.Map;
using Newtonsoft.Json.Serialization;
using Tooling.Logging;
using Tooling.StaticData.Data;
using Tooling.StaticData.Data.Bytecode;
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
            if (obj == null)
            {
                return;
            }

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
                else if (typeof(IEnumerable).IsAssignableFrom(field.FieldType)
                      && field.FieldType != typeof(string)) // Optimization; a static data reference will never be inside a string
                {
                    var enumeration = (IEnumerable)field.GetValue(obj);
                    if (enumeration == null)
                    {
                        continue;
                    }

                    int i = 0;
                    foreach (var item in enumeration)
                    {
                        if (IsStaticDataField(item, out var staticDataRef))
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
                            RecursivelyResolveStaticDataReferences(item);
                        }

                        i++;
                    }
                }
            }

            foreach (var prop in objType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                // Prevents TargetParameterCountException, we don't care to check indexed properties
                if (prop.GetIndexParameters().Length > 0)
                {
                    return;
                }

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
                else if (typeof(IEnumerable).IsAssignableFrom(prop.PropertyType) && prop.PropertyType != typeof(string))
                {
                    var enumeration = (IEnumerable)prop.GetValue(obj);
                    if (enumeration == null)
                    {
                        continue;
                    }

                    int i = 0;
                    foreach (var item in enumeration)
                    {
                        if (IsStaticDataField(item, out var staticDataRef))
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
                            RecursivelyResolveStaticDataReferences(item);
                        }

                        i++;
                    }
                }
            }
        }

        private static bool IsStaticDataField(object obj, out StaticDataReference staticDataReference)
        {
            if (obj is not StaticData staticData)
            {
                staticDataReference = default;
                return false;
            }

            staticDataReference = staticData.Reference;
            return staticDataReference.IsReferenceValid();
        }
    }
}