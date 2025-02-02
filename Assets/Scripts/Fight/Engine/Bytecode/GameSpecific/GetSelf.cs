namespace Fight.Engine.Bytecode
{
    /// <summary>
    /// The <see cref="ICombatParticipant"/> that plays this will return itself.
    /// </summary>
    [System.Serializable]
    public struct GetSelf : IInstruction
    {
        public void Execute(Context context)
        {
            var targetedCombatParticipant = context.Fight.GetTargetedCombatant();
            context.Memory.Push(targetedCombatParticipant);

            context.Logger.Log(LogLevel.Info, $"Pushed {targetedCombatParticipant.Name}");
        }
    }
}