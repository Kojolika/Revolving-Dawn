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
            while (IsRunning)
            {
                if (battleEventQueue.Count > 0)
                {
                    var latestEvent = battleEventQueue.First();
                    latestEvent.Execute(this);
                    MyLogger.Log(latestEvent.Log());
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