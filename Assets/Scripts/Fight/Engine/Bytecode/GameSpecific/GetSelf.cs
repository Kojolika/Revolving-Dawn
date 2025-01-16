namespace Fight.Engine.Bytecode
{
    /// <summary>
    /// The <see cref="ICombatParticipant"/> that plays this will return itself.
    /// </summary>
    public struct GetSelf : IReduceTo<ICombatParticipant>
    {
        private ICombatParticipant combatParticipant;

        public ICombatParticipant Reduce()
        {
            //TODO: API to get self
            return default;
        }

        public string Log()
        {
            return $"Get self: {combatParticipant.Log()}";
        }
    }
}