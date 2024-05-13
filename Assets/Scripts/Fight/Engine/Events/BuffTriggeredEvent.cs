namespace Fight.Events
{
    public class BuffTriggeredEvent<TBuff, TEvent> : BattleEvent<TBuff> 
        where TEvent : IBattleEvent
        where TBuff : ITriggerableBuff<TEvent>
    {
        public override void Execute(TBuff target)
        {

        }

        public void Execute() => Execute(Target);
        public override string Log() => $"Buff {Target} triggered off of {Target}"  
    }
}