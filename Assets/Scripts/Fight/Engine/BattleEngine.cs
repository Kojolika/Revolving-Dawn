using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fight.Events;
using Tooling.Logging;

namespace Fight
{
    public class BattleEngine
    {
        public bool IsRunning { get; private set; }
        private List<IBattleEvent> battleEventQueue;
        public Stack<IBattleEvent> BattleEventHistory { get; private set; } = new Stack<IBattleEvent>();

        public void Run()
        {
            battleEventQueue = new List<IBattleEvent>();
            IsRunning = true;
            EngineLoop();
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

        public void InsertAfterEvent(IBattleEvent battleEventInQueue, IBattleEvent battleEventToInsert)
        {
            var indexOfEvent = battleEventQueue.IndexOf(battleEventInQueue);
            if (indexOfEvent < 0)
            {
                return;
            }
            if (indexOfEvent == battleEventQueue.Count() - 1)
            {
                battleEventQueue.Add(battleEventToInsert);
            }
            else
            {
                battleEventQueue.Insert(indexOfEvent + 1, battleEventToInsert);
            }
        }

        async void EngineLoop()
        {
            int eventIndex = 0;
            while (IsRunning)
            {
                if (eventIndex < battleEventQueue.Count)
                {
                    battleEventQueue[eventIndex].OnBeforeExecute(this);

                    battleEventQueue[eventIndex].Execute(this);
                    BattleEventHistory.Push(battleEventQueue[eventIndex]);
                    MyLogger.Log($"{battleEventQueue[eventIndex].GetType().Name}: {battleEventQueue[eventIndex].Log()}");

                    battleEventQueue[eventIndex].OnAfterExecute(this);
                    eventIndex++;
                }
                else
                {
                    await Task.Delay(500);
                }
            }
        }

        public void AddEvent(IBattleEvent battleEvent) => battleEventQueue.Add(battleEvent);
    }
}