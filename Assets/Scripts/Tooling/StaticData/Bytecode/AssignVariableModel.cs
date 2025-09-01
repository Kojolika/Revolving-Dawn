namespace Tooling.StaticData.EditorUI.Bytecode
{
    public class AssignVariableModel : InstructionModel
    {
        /// <summary>
        /// The variable name
        /// </summary>
        public string Name;

        /// <summary>
        /// The variable value
        /// </summary>
        public ValueModel Value;
    }
}