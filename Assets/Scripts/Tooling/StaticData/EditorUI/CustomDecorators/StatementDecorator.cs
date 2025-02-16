using JetBrains.Annotations;

namespace Tooling.StaticData.EditorUI
{
    [UsedImplicitly]
    public class StatementDecorator : IDecorator<Statement>
    {
        private InstructionListDecorator instructionListDecorator;

        public void Dispose(GeneralField generalField)
        {
            instructionListDecorator?.Dispose(generalField);
        }

        public void DecorateElement(GeneralField generalField, Statement element)
        {
            instructionListDecorator ??= new();
            instructionListDecorator.DecorateElement(generalField, element?.ByteStatement?.Instructions);
        }
    }
}