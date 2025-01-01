namespace Fight.Engine.Bytecode
{
    /// <summary>
    /// Gets the <see cref="ICombatParticipant"/> that the player is targeting.
    /// </summary>
    public struct GetTargetedCombatParticipant : IPushByte<ICombatParticipant>
    {
        public ICombatParticipant Push()
        {
            // TODO: api to grab targeted unit

            return default;
        }
    }
}