using System.Collections.Generic;

namespace Fight.Engine.Bytecode
{
    /// <summary>
    /// This class provides APIs to grab information from the current fight that some instructions may need.
    /// </summary>
    public interface IFightContext
    {
        ICombatParticipant GetTargetedCombatant();
        List<ICombatParticipant> GetAllCombatants();
        ICombatParticipant GetSelf();
        
        // TODO: What else? Level number? Map number?
    }
}