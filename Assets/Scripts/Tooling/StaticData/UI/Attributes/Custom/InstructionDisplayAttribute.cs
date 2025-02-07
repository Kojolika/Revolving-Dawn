using System;
using Fight.Engine.Bytecode;

namespace Tooling.StaticData.Attributes.Custom
{
    /// <summary>
    /// Exposes the inputs and outputs of an <see cref="IInstruction"/> when drawn by the <see cref="GeneralField"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
    public class InstructionDisplayAttribute : Attribute
    {
        public readonly DisplayType Display;

        /// <summary>
        /// The types of inputs/outputs that this instruction has.
        /// </summary>
        public readonly Type[] Types;

        public InstructionDisplayAttribute(DisplayType display, params Type[] types)
        {
            Display = display;
            Types = types;
        }
    }

    public enum DisplayType
    {
        /// <summary>
        /// Specifies this will draw the inputs for an <see cref="IInstruction"/>
        /// </summary>
        Input,

        /// <summary>
        /// Specifies this will draw the outputs for an <see cref="IInstruction"/>
        /// </summary>
        Output
    }
}