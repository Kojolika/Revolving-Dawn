using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fight.Events;

namespace Fight
{
    public class BattleEngine
    {
        public bool IsRunning { get; private set; }
        private Queue<IBattleEvent> battleEventQueue;

        public void Run()
        {
            battleEventQueue = new Queue<IBattleEvent>();
            IsRunning = true;
            EngineLoop();
        }

        public void Stop()
        {
            battleEventQueue.Clear();
            IsRunning = false;
        }

        async void EngineLoop()
        {
            while (IsRunning)
            {
                if (battleEventQueue.Count > 0)
                {
                    var latestEvent = battleEventQueue.Dequeue();
                    latestEvent.Execute();
                    Console.Write(latestEvent.Log());
                }
                else
                {
                    await Task.Delay(500);
                }
            }
        }

        public void AddEvent(IBattleEvent battleEvent) => battleEventQueue.Enqueue(battleEvent);
    }
}