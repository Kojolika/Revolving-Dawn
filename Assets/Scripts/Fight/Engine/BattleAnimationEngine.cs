using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Fight.Engine;
using Fight.Events;
using Tooling.Logging;

namespace Fight.Animations
{
    public class BattleAnimationEngine : IEventSubscriber<IBattleEvent>
    {
        public bool IsRunning { get; private set; }

        private          Queue<BattleEventAnimation> battleAnimationQueue;
        private readonly BattleEngine                battleEngine;
        private readonly IBattleAnimation.Factory    animationFactory;

        public BattleAnimationEngine(BattleEngine battleEngine, IBattleAnimation.Factory animationFactory)
        {
            this.battleEngine     = battleEngine;
            this.animationFactory = animationFactory;

            battleEngine.SubscribeToEvent<IBattleEvent>(this);
        }

        ~BattleAnimationEngine()
        {
            battleEngine?.UnsubscribeFromEvent<IBattleEvent>(this);
        }

        public void Run()
        {
            if (IsRunning)
            {
                MyLogger.Warning($"Requested run on {this} but {this} is already running.");
                return;
            }

            battleAnimationQueue = new();
            IsRunning            = true;
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
                    var first = battleAnimationQueue.Dequeue();
                    if (first.Animation == null)
                    {
                        MyLogger.Warning($"No animation found for battle event {first.BattleEvent}");
                        continue;
                    }

                    if (first.Animation.ShouldWait)
                    {
                        await first.Animation.Play(first.BattleEvent);
                    }
                    else
                    {
                        _ = first.Animation.Play(first.BattleEvent);
                    }
                }
                else
                {
                    await UniTask.Delay(500);
                }
            }
        }

        public void Enqueue(BattleEventAnimation battleEventAnimation) => battleAnimationQueue?.Enqueue(battleEventAnimation);

        private void LoadAndEnqueueAnimation(IBattleEvent battleEvent)
        {
            if (battleEvent == null)
            {
                return;
            }

            Enqueue(new BattleEventAnimation(battleEvent, animationFactory.Create(battleEvent)));
        }

        public void OnEvent(IBattleEvent eventData)
        {
            LoadAndEnqueueAnimation(eventData);
        }
    }

    public class BattleEventAnimation
    {
        public readonly IBattleEvent     BattleEvent;
        public readonly IBattleAnimation Animation;

        public BattleEventAnimation(IBattleEvent battleEvent, IBattleAnimation animation)
        {
            BattleEvent = battleEvent;
            Animation   = animation;
        }
    }
}