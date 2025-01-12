namespace Fight.Engine.Bytecode
{
    /// <summary>
    /// The <see cref="ICombatParticipant"/> that plays this will return itself.
    /// </summary>
    public struct GetSelf : IPush<ICombatParticipant>
    {
        public ICombatParticipant Push()
        {
            //TODO: API to get self
            return default;
        }
    }
}