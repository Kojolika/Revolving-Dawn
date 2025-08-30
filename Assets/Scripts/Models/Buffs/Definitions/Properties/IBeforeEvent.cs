using Fight;
using Fight.Events;
using Tooling.StaticData;

namespace Models.Buffs
{
    public interface IBeforeEvent
    {
        /// <summary>
        /// Performs any logic that occurs RIGHT before the battleEvent.
        /// </summary>
        /// <returns> the stack size of the buff after this trigger </returns>
        int OnBeforeExecute(Context fightContext, IBattleEvent battleEvent, Buff buff, int currentStackSize);
    }

    public interface IBeforeEventT<in TEvent> : IBeforeEvent where TEvent : IBattleEvent
    {
        /// <summary>
        /// Casts the event if it's the correct type, otherwise does nothing.
        /// </summary>
        int IBeforeEvent.OnBeforeExecute(Context fightContext, IBattleEvent battleEvent, Buff buff, int currentStackSize)
        {
            if (battleEvent is TEvent eventT)
            {
                return OnBeforeExecute(fightContext, eventT, buff, currentStackSize);
            }

            return currentStackSize;
        }


        /// <inheritdoc cref="IBeforeEvent.OnBeforeExecute"/>
        int OnBeforeExecute(Context fightContext, TEvent battleEvent, Buff buff, int currentStackSize);
    }

    public interface IAfterEvent
    {
        /// <summary>
        /// Performs any logic that occurs RIGHT after the battleEvent.
        /// </summary>
        /// <returns> the stack size of the buff after this trigger </returns>
        int OnAfterExecute(Context fightContext, IBattleEvent battleEvent, Buff buff, int currentStackSize);
    }


    public interface IAfterEventT<in TEvent> : IAfterEvent where TEvent : IBattleEvent
    {
        /// <summary>
        /// Casts the event if it's the correct type, otherwise does nothing.
        /// </summary>
        int IAfterEvent.OnAfterExecute(Context fightContext, IBattleEvent battleEvent, Buff buff, int currentStackSize)
        {
            if (battleEvent is TEvent eventT)
            {
                return OnAfterExecute(fightContext, eventT, buff, currentStackSize);
            }

            return currentStackSize;
        }

        /// <inheritdoc cref="IAfterEvent.OnAfterExecute"/>
        int OnAfterExecute(Context fightContext, TEvent battleEvent, Buff buff, int currentStackSize);
    }
}