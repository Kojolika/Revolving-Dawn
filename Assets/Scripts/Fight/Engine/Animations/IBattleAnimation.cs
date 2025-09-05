using Cysharp.Threading.Tasks;
using Fight.Events;
using Systems.Managers;
using Tooling.Logging;
using UnityEngine.AddressableAssets;
using Zenject;

namespace Fight.Animations
{
    public interface  IBattleAnimation
    {
        bool ShouldWait { get; }
        bool IsFinished { get; }
        UniTask Play(IBattleEvent battleEvent);
        UniTask Undo(IBattleEvent battleEvent);

        public class Factory : PlaceholderFactory<IBattleEvent, IBattleAnimation> { }
        public class CustomFactory : IFactory<IBattleEvent, IBattleAnimation>
        {
            private readonly DiContainer diContainer;
            private readonly AddressablesManager addressablesManager;
            public CustomFactory(DiContainer diContainer, AddressablesManager addressablesManager)
            {
                this.diContainer = diContainer;
                this.addressablesManager = addressablesManager;
            }

            public IBattleAnimation Create(IBattleEvent battleEvent)
            {
                IBattleAnimation battleAnimation = null;

                var animationConventionKey = $"{battleEvent.GetType().Name}Animation";

                battleAnimation = addressablesManager.LoadGenericAssetSync<ScriptableObjectAnimation>(animationConventionKey, () => battleAnimation.IsFinished);

                if (battleAnimation != null)
                {
                    diContainer.Inject(battleAnimation);
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