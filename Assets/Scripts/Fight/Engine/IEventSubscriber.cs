using Fight.Events;

namespace Fight.Engine
{
    public interface IEventSubscriber<in T> : IEventSubscriber where T : IBattleEvent
    {
        void OnEvent(T eventData);

        void IEventSubscriber.OnEvent(IBattleEvent eventData) => OnEvent(eventData);
    }

    public interface IEventSubscriber
    {
        void OnEvent(IBattleEvent eventData);
    }
}