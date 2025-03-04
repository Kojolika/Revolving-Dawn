using Tooling.StaticData.EditorUI;

namespace Tooling.StaticData
{
    /// <summary>
    /// Gets the <see cref="IRuntimeCombatParticipant"/> that the player is targeting.
    /// </summary>
    [System.Serializable, DisplayName("Targeted")]
    public struct GetTargetedRuntimeCombatParticipant : IRuntimeCombatParticipant
    {
        public LiteralExpression.Type RuntimeType => LiteralExpression.Type.Int;
    }
}