using System.Collections.Generic;
using UnityEngine;
using Fight.Events;
using Cysharp.Threading.Tasks;

namespace Fight.Animations
{
    public class BattleAnimationEngine : MonoBehaviour
    {
        public bool IsRunning { get; private set; }
        private Queue<IBattleAnimation> battleAnimationQueue;
        private Queue<IBattleEvent> battleEventQueue;

        public void Run(Queue<IBattleEvent> battleEvents = null)
        {
            battleAnimationQueue = new Queue<IBattleAnimation>();
            IsRunning = true;
            EngineLoop();
        }

        public void Stop()
        {
            battleAnimationQueue.Clear();
            IsRunning = false;
        }

        async void EngineLoop()
        {
            while (IsRunning)
            {
                if (battleAnimationQueue.Count > 0)
                {
                    var latestEvent = battleEventQueue.Dequeue();
                    await battleAnimationQueue.Dequeue().PlayAnimation(latestEvent);
                }
                else
                {
                    await UniTask.Delay(500);
                }
            }
        }

        public void AddEvent(IBattleEvent battleEvent) => battleEventQueue.Enqueue(battleEvent);
    }
}