using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Fight.Events;
using Tooling.Logging;

namespace Fight
{
    public class BattleEngine
    {
        public bool IsRunning { get; private set; }
        private List<IBattleEvent> battleEventQueue;
        public Stack<IBattleEvent> BattleEventHistory { get; private set; } = new Stack<IBattleEvent>();

        public event Action<IBattleEvent> EventOccurred;

        public void Run()
        {
            battleEventQueue = new List<IBattleEvent>();
            IsRunning = true;
            _ = EngineLoop();
        }

        public void Stop()
        {
            battleEventQueue.Clear();
            IsRunning = false;
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
            if (indexOfEvent == battleEventQueue.Count() - 1)
            {
                AddEvent(battleEventToInsert);
            }
            else
            {
                battleEventQueue.Insert(indexOfEvent + 1, battleEventToInsert);
            }
        }

        async UniTask EngineLoop()
        {
            int eventIndex = 0;
            while (IsRunning)
            {
                if (eventIndex < battleEventQueue.Count)
                {
                    battleEventQueue[eventIndex].OnBeforeExecute(this);

                    battleEventQueue[eventIndex].Execute(this);
                    BattleEventHistory.Push(battleEventQueue[eventIndex]);
                    EventOccurred?.Invoke(battleEventQueue[eventIndex]);
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
    }
}