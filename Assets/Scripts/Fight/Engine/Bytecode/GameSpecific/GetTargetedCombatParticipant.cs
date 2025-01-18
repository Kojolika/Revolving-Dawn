namespace Fight.Engine.Bytecode
{
    /// <summary>
    /// Gets the <see cref="ICombatParticipant"/> that the player is targeting.
    /// </summary>
    public struct GetTargetedCombatParticipant : IReduceTo<ICombatParticipant>, IMockable
    {
        private ICombatParticipant combatParticipant;

        public ICombatParticipant Reduce()
        {
            // TODO: api to grab targeted unit

            return default;
        }

        public string Log()
        {
            return $"Targeted {combatParticipant.Log()}";
        }
    }
}