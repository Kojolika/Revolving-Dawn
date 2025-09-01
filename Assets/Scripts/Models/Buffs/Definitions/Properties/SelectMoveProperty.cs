using Fight;
using Fight.Engine;
using Fight.Events;
using Tooling.StaticData.EditorUI;

namespace Models.Buffs
{
    /// TODO: Add this buff to enemies at the start of combat
    public class SelectMoveProperty : IAfterEventT<TurnStartedEvent>
    {
        public int OnAfterExecute(Context fightContext, TurnStartedEvent battleEvent, Buff buff, int currentStackSize)
        {
            if (battleEvent.Target is IMoveParticipant target)
            {
                target.SelectMove();
            }
            
            return currentStackSize;
        }
    }
}