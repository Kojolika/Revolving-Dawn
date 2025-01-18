using System.Collections.Generic;
using Fight.Engine.Bytecode;
using Tooling.Logging;

namespace Tooling.StaticData.Validation
{
    public class ByteValidator : TypeValidator<List<ICombatByte>>
    {
        public override List<string> errorMessages { get; } = new();


        private readonly Interpreter interpreter = new();

        public ByteValidator()
        {
            interpreter.RegisterMockable<GetTargetedCombatParticipant, MockGetTargetedCombatParticipant>();
        }

        protected override bool Validate(List<ICombatByte> value, List<StaticData> allObjects)
        {
            var byteStack = new Stack<ICombatByte>();
            foreach (var combatByte in value)
            {
                byteStack.Push(combatByte);
            }

            var validationResult = interpreter.ValidateWithMocks(byteStack);
            errorMessages.AddRange(validationResult.Errors);

            return validationResult.IsSuccess;
        }
    }
}