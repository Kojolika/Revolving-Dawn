using System.Collections.Generic;
using UnityEngine;
using Fight.Events;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Linq;

namespace Fight.Animations
{
    public class BattleAnimationEngine : MonoBehaviour
    {
        public bool IsRunning { get; private set; }
        private Queue<(AsyncOperationHandle<IBattleAnimation> animationAssetHandle, IBattleEvent battleEvent)> battleAnimationQueue;
        public void Run(Queue<IBattleEvent> battleEvents = null)
        {
            battleAnimationQueue = new Queue<(AsyncOperationHandle<IBattleAnimation>, IBattleEvent)>();
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
                    var first = battleAnimationQueue.Dequeue();
                    var animation = await first.animationAssetHandle;
                    await animation.Play(first.battleEvent);
                }
                else
                {
                    await UniTask.Delay(500);
                }
            }
        }
    
        public void EnqueueAnimationFromEvent(IBattleEvent battleEvent)
            => battleAnimationQueue.Enqueue((BattleAnimationFactory.CreateAnimationFromEvent(battleEvent), battleEvent));

        public void EnqueueAnimationsFromEvent(IEnumerable<IBattleEvent> battleEvents)
        {   
            foreach(var battleEvent in battleEvents)
            {
                battleAnimationQueue.Enqueue((BattleAnimationFactory.CreateAnimationFromEvent(battleEvent), battleEvent));
            }
        }
    }

    /// <summary>
    /// For animations we are using a notation where the name of the event followed by 'Animation'
    /// will be the addressable asset key for the corresponding animation.
    /// This class loads the animation using addresables
    /// </summary>
    public static class BattleAnimationFactory
    {
        public static AsyncOperationHandle<IBattleAnimation> CreateAnimationFromEvent(IBattleEvent battleEvent)
            => Addressables.LoadAssetAsync<IBattleAnimation>($"{nameof(battleEvent)}Animation");

        public static Queue<AsyncOperationHandle<IBattleAnimation>> CreateAnimationsFromEvent(IEnumerable<IBattleEvent> battleEvents)
        {
            var animationHandles = new Queue<AsyncOperationHandle<IBattleAnimation>>();
            foreach (var battleEvent in battleEvents)
            {
                animationHandles.Enqueue(CreateAnimationFromEvent(battleEvent));
            }
            return animationHandles;
        }
    }
}