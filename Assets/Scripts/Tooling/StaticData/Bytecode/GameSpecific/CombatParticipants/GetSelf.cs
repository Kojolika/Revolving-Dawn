using Tooling.StaticData.EditorUI;

namespace Tooling.StaticData
{
    /// <summary>
    /// The <see cref="IRuntimeCombatParticipant"/> that plays this will return itself.
    /// </summary>
    [System.Serializable, DisplayName("Self")]
    public struct GetSelf : IRuntimeCombatParticipant
    {
        public LiteralExpression.Type RuntimeType => LiteralExpression.Type.Int;
    }
}