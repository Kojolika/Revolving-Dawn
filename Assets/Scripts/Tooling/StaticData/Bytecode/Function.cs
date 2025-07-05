using System;

namespace Tooling.StaticData.Bytecode
{
    public class Function : Attribute
    {
        public readonly Type Output;
        public readonly Type[] Inputs;

        public Function(Type output, params Type[] inputs)
        {
            Inputs = inputs;
            Output = output;
        }
    }

    public class Property : Attribute
    {
        public readonly Type Type;

        public Property(Type type)
        {
            Type = type;
        }
    }

    public class Object : Attribute
    {
    }
}