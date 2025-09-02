namespace Tooling.StaticData.Data.Bytecode
{
    /// <summary>
    /// If the Boolean evaluates to true, execute the next combat byte instruction.
    /// </summary>
    public class If : InstructionModel
    {
        public Statement IfTrueStatement;
        public Statement ElseStatement;
    }
}