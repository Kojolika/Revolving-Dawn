using System.Collections.Generic;
using Fight.Engine.Bytecode;
using ModestTree;
using Tooling.Logging;
using Utils.Extensions;

namespace Tooling.StaticData.Validation
{
    public class InstructionValidator : TypeValidator<List<IInstruction>>
    {
        public override List<string> errorMessages { get; } = new();


        private readonly Interpreter interpreter = new();

        public InstructionValidator()
        {
            //interpreter.RegisterMockable<GetTargetedCombatParticipant, MockGetTargetedCombatParticipant>();
        }

        protected override bool Validate(List<IInstruction> value, List<StaticData> allObjects)
        {
            if (value.IsNullOrEmpty())
            {
                return true;
            }

            var byteStack = new Stack<IInstruction>();
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