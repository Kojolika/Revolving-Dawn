using System;

namespace Tooling.StaticData.EditorUI.Bytecode
{
    /// <summary>
    /// Marks a class as having functions with <see cref="ByteFunction"/> attributes that will be used at runtime.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class ByteCodeContainer : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field)]
    public class ByteFunction : Attribute
    {
        public readonly Type Output;
        public readonly Type[] Inputs;

        public ByteFunction(Type output, params Type[] inputs)
        {
            Inputs = inputs;
            Output = output;
        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ByteProperty : Attribute
    {
        public readonly Type Type;

        public ByteProperty(Type type)
        {
            Type = type;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface)]
    public class ByteObject : Attribute
    {
    }
}