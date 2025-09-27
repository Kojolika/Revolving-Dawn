using System;
using System.Collections.Generic;
using Common.Util;
using Cysharp.Threading.Tasks;
using Fight.Engine;
using Fight.Events;
using Tooling.Logging;
using UnityEngine;
using Utils.Extensions;
using Zenject;

namespace Fight
{
    public class BattleEngine
    {
        public Stack<IBattleEvent> BattleEventHistory { get; private set; } = new();

        private          List<IBattleEvent>                       battleEventQueue;
        private readonly Dictionary<Type, List<IEventSubscriber>> battleEventSubscribers = new();

        private int eventIndex;

        [Inject] private readonly Context fightContext;

        public void Run()
        {
            battleEventQueue = new List<IBattleEvent>();
            eventIndex       = 0;

            _ = EngineLoop();
        }

        public void InsertBeforeEvent(IBattleEvent battleEventInQueue, IBattleEvent battleEventToInsert)
        {
            var indexOfEvent = battleEventQueue.IndexOf(battleEventInQueue);
            if (indexOfEvent < 0)
            {
                return;
            }

            battleEventQueue.Insert(indexOfEvent, battleEventToInsert);
        }

        public void InsertAfterEvent(IBattleEvent battleEventToInsert, IBattleEvent battleEventInQueue)
        {
            var indexOfEvent = battleEventQueue.IndexOf(battleEventInQueue);
            if (indexOfEvent < 0)
            {
                return;
            }

            if (indexOfEvent == battleEventQueue.Count - 1)
            {
                AddEvent(battleEventToInsert);
            }
            else
            {
                battleEventQueue.Insert(indexOfEvent + 1, battleEventToInsert);
            }
        }

        private async UniTask EngineLoop()
        {
            while (true)
            {
                if (eventIndex < battleEventQueue.Count)
                {
                    ResolveEvent(battleEventQueue[eventIndex]);
                    eventIndex++;
                }
                else
                {
                    await UniTask.Yield(PlayerLoopTiming.Update);
                }
            }
        }

        private void ResolveEvent(IBattleEvent battleEvent)
        {
            TriggerOnBeforeBuffEffects(battleEvent);
            battleEvent.Execute(fightContext);
            TriggerOnAfterBuffEffects(battleEvent);

            BattleEventHistory.Push(battleEvent);

            DispatchEvent(battleEvent);

            MyLogger.Info($"{battleEvent.GetType().Name}: {battleEvent.Log()}");
        }

        private void TriggerOnBeforeBuffEffects(IBattleEvent battleEvent)
        {
            foreach (var participant in fightContext.GetAllCombatParticipants())
            {
                foreach (var (stackSize, buff) in participant.GetBuffs().OrEmptyIfNull())
                {
                    foreach (var onBefore in buff.OnBefore.OrEmptyIfNull())
                    {
                        if (onBefore.EventType == battleEvent.GetType())
                        {
                            ResolveEvent(new BuffTriggeredEvent(participant, battleEvent, onBeforeEvent: onBefore, onAfterEvent: null, buff, stackSize));
                        }
                    }
                }
            }
        }

        private void TriggerOnAfterBuffEffects(IBattleEvent battleEvent)
        {
            foreach (var participant in fightContext.GetAllCombatParticipants())
            {
                foreach (var (stackSize, buff) in participant.GetBuffs().OrEmptyIfNull())
                {
                    foreach (var onAfter in buff.OnAfter.OrEmptyIfNull())
                    {
                        if (onAfter.EventType == battleEvent.GetType())
                        {
                            ResolveEvent(new BuffTriggeredEvent(participant, battleEvent, onBeforeEvent: null, onAfterEvent: onAfter, buff, stackSize));
                        }
                    }
                }
            }
        }

        public void AddEvent(IBattleEvent battleEvent)
        {
            battleEventQueue.Add(battleEvent);
        }

        public void AddEvents(params IBattleEvent[] battleEvents)
        {
            battleEventQueue.AddRange(battleEvents);
        }

        public void SubscribeToEvent<T>(IEventSubscriber subscriber) where T : IBattleEvent
        {
            var eventType = typeof(T);
            if (!battleEventSubscribers.ContainsKey(eventType))
            {
                battleEventSubscribers[eventType] = new();
            }

            battleEventSubscribers[eventType].Add(subscriber);
        }

        public void UnsubscribeFromEvent<T>(IEventSubscriber subscriber) where T : IBattleEvent
        {
            var eventType = typeof(T);
            if (!battleEventSubscribers.TryGetValue(eventType, out var eventSubscriber))
            {
                MyLogger.Error($"Trying to unsubscribe from an event type that has no subscribers!");
                return;
            }

            eventSubscriber.Remove(subscriber);
        }

        private void DispatchEvent<T>(T evt) where T : IBattleEvent
        {
            if (!battleEventSubscribers.ContainsKey(typeof(T)) || battleEventSubscribers[typeof(T)].IsNullOrEmpty())
            {
                return;
            }

            foreach (var subscriber in battleEventSubscribers[typeof(T)])
            {
                subscriber.OnEvent(evt);
            }
        }
    }
}