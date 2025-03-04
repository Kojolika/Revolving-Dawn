using System.Collections.Generic;

namespace Tooling.StaticData
{
    /// <summary>
    /// This class has helpful functions to determine information about a script that's been created
    /// in the <see cref="Tooling.StaticData.EditorUI.EditorWindow"/>.
    /// </summary>
    public class ScriptAnalyzer
    {
        public List<VariableMetaData> GetVariableNames(List<IInstruction> instructions)
        {
            var returnValue = new List<VariableMetaData>();
            for (int i = 0; i < instructions.Count; i++)
            {
                var instruction = instructions[i];
                if (instruction is AssignVariable assignVariable)
                {
                    var name = assignVariable.Name;
                    var lineNumber = i;
                }
            }

            return returnValue;
        }

        public LiteralComputeInfo TryResolveLiteral(List<IInstruction> instructions)
        {
            for (int i = 0; i < instructions.Count; i++)
            {
                var instruction = instructions[i];

            }

            return default;
        }

        public readonly struct LiteralComputeInfo
        {
            /// <inheritdoc cref="VariableMetaData.IsComputedAtRuntime"/>
            public readonly bool IsComputedAtRuntime;

            /// <inheritdoc cref="VariableMetaData.VariableType"/>
            public readonly LiteralExpression.Type VariableType;

            /// <inheritdoc cref="VariableMetaData.Variable"/>
            public readonly LiteralExpression Value;

            /// <summary>
            /// A list of instructions can only return a valid variable if there is only 1 value on the stack after execution
            /// and that value is a type that is compatible with <see cref="LiteralExpression"/>
            /// </summary>
            public readonly bool IsValid;

            public LiteralComputeInfo(
                bool isComputedAtRuntime,
                LiteralExpression.Type variableType,
                LiteralExpression value,
                bool isValid)
            {
                IsComputedAtRuntime = isComputedAtRuntime;
                VariableType = variableType;
                Value = value;
                IsValid = isValid;
            }
        }

        public readonly struct VariableMetaData
        {
            /// <summary>
            /// The name of the variable
            /// </summary>
            public readonly string Name;

            /// <summary>
            /// The line that this variable was initialized
            /// </summary>
            public readonly int LineNumber;

            /// <summary>
            /// If the variable uses a function at runtime, this is marked as true.
            /// The real value of the variable can't be determined until the program runs.
            /// <remarks> This may be because of function calls like <see cref="GetTargetedRuntimeCombatParticipant"/></remarks>
            /// </summary>
            public readonly bool IsComputedAtRuntime;

            /// <summary>
            /// The type of variable
            /// </summary>
            public readonly LiteralExpression.Type VariableType;

            /// <summary>
            /// The actual variable value defined, only valid if <see cref="IsComputedAtRuntime"/> is false.
            /// <remarks> We're only storing literal's as variables for our purposes </remarks>
            /// </summary>
            public readonly LiteralExpression Variable;

            public VariableMetaData(
                string name,
                int lineNumber,
                bool isComputedAtRuntime,
                LiteralExpression.Type variableType,
                LiteralExpression variable)
            {
                Name = name;
                LineNumber = lineNumber;
                Variable = variable;
                IsComputedAtRuntime = isComputedAtRuntime;
                VariableType = variableType;
            }
        }
    }
}