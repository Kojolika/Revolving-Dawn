using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Fight.Engine;
using Fight.Events;
using Tooling.Logging;
using UnityEngine;
using Utils.Extensions;

namespace Fight
{
    public class BattleEngine
    {
        public Stack<IBattleEvent> BattleEventHistory { get; private set; } = new();
        
        private List<IBattleEvent> battleEventQueue;
        private readonly Dictionary<Type, List<IEventSubscriber>> battleEventSubscribers = new();
        
        public void Run()
        {
            battleEventQueue = new List<IBattleEvent>();
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
            int eventIndex = 0;
            while (Application.isPlaying)
            {
                if (eventIndex < battleEventQueue.Count)
                {
                    battleEventQueue[eventIndex].OnBeforeExecute(this);

                    battleEventQueue[eventIndex].Execute(this);
                    BattleEventHistory.Push(battleEventQueue[eventIndex]);
                    
                    DispatchEvent(battleEventQueue[eventIndex]);

                    MyLogger.Log($"{battleEventQueue[eventIndex].GetType().Name}: {battleEventQueue[eventIndex].Log()}");

                    battleEventQueue[eventIndex].OnAfterExecute(this);
                    eventIndex++;
                }
                else
                {
                    await UniTask.Yield(PlayerLoopTiming.Update);
                }
            }
        }

        public void AddEvent(IBattleEvent battleEvent) => battleEventQueue.Add(battleEvent);

        public void SubscribeToEvent<T>(IEventSubscriber subscriber) where T : IBattleEvent
        {
            var eventType = typeof(T);
            if (!battleEventSubscribers.ContainsKey(eventType))
            {
                battleEventSubscribers[eventType] = new();
            }

            battleEventSubscribers[eventType].Add(subscriber);
        }

        public void UnsubscribeToEvent<T>(IEventSubscriber subscriber) where T : IBattleEvent
        {
            var eventType = typeof(T);
            if (!battleEventSubscribers.TryGetValue(eventType, out var eventSubscriber))
            {
                MyLogger.LogError($"Trying to unsubscribe from an event type that has no subscribers!");
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