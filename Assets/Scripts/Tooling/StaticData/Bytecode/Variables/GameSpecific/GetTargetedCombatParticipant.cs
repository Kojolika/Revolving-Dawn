using Tooling.StaticData.EditorUI;

namespace Tooling.StaticData.Bytecode
{
    [DisplayName("Game Functions/Get Targeted CombatParticipant"), System.Serializable]
    public class GetTargetedCombatParticipant : Variable<int>
    {
        public override Type Type => Type.Int;
        public override bool IsComputedAtRuntime => true;
        public override int Value { get; set; }
    }
}