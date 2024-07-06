using System;
using Cysharp.Threading.Tasks;
using Fight.Events;
using Tooling.Logging;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;

namespace Fight.Animations
{
    public interface IBattleAnimation
    {
        bool ShouldWait { get; }
        AsyncOperationHandle AsyncOperationHandle { get; set; }
        UniTask Play(IBattleEvent battleEvent);
        UniTask Undo(IBattleEvent battleEvent);

        public class Factory : PlaceholderFactory<IBattleEvent, IBattleAnimation> { }
        public class CustomFactory : IFactory<IBattleEvent, IBattleAnimation>
        {
            private readonly DiContainer diContainer;
            public CustomFactory(DiContainer diContainer)
            {
                this.diContainer = diContainer;
            }

            public IBattleAnimation Create(IBattleEvent battleEvent)
            {
                IBattleAnimation battleAnimation = null;
                try
                {
                    var opHandle = Addressables.LoadAssetAsync<IBattleAnimation>($"{battleEvent.GetType().Name}Animation");
                    battleAnimation = opHandle.WaitForCompletion();
                    
                    if (battleAnimation != null)
                    {
                        battleAnimation.AsyncOperationHandle = opHandle;
                        diContainer.Inject(battleAnimation);
                    }
                }
                catch (InvalidKeyException)
                {
                    MyLogger.LogWarning($"No addressable animation for event of type {battleEvent.GetType()}!");
                }

                return battleAnimation;
            }
        }
    }

    public interface IBattleAnimation<T> : IBattleAnimation where T : IBattleEvent
    {
        async UniTask IBattleAnimation.Play(IBattleEvent battleEvent) => await Play((T)battleEvent);
        async UniTask IBattleAnimation.Undo(IBattleEvent battleEvent) => await Undo((T)battleEvent);
        UniTask Play(T battleEvent);
        UniTask Undo(T battleEvent);
    }
}