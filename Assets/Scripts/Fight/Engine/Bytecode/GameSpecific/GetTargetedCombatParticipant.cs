using System.Text;

namespace Fight.Engine.Bytecode
{
    /// <summary>
    /// Gets the <see cref="ICombatParticipant"/> that the player is targeting.
    /// </summary>
    [System.Serializable]
    public struct GetTargetedCombatParticipant : IPush<ICombatParticipant>
    {
        public void Execute(Context context)
        {
            var combatParticipant = context.Fight.GetTargetedCombatant();
            context.Memory.Push(combatParticipant);

            context.Logger.Log(LogLevel.Info, $"Pushed {combatParticipant.Name}");
        }
    }

    [System.Serializable]
    public struct GetAllCombatParticipants : IPush<StoreableList<ICombatParticipant>>
    {
        public void Execute(Context context)
        {
            var combatParticipants = context.Fight.GetAllCombatants().ToStoreableList();
            context.Memory.Push(combatParticipants);

            var stringBuilder = new StringBuilder();
            for (int i = 0; i < combatParticipants.Count; i++)
            {
                // Print the name and a comma, unless we're at the last element then print no comma
                stringBuilder.Append($"{combatParticipants[i].Name}{(i == combatParticipants.Count - 1 ? string.Empty : ", ")}");
            }

            var combatParticipantNames = stringBuilder.ToString();
            context.Logger.Log(LogLevel.Info, $"Pushed {combatParticipantNames}");
        }
    }
}