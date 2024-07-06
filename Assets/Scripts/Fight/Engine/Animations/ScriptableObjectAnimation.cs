using Cysharp.Threading.Tasks;
using Fight.Events;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Fight.Animations
{
    [CreateAssetMenu(menuName = "RevolvingDawn/Fight/Animations/" + nameof(ScriptableObjectAnimation), fileName = "New " + nameof(ScriptableObjectAnimation))]
    public class ScriptableObjectAnimation : ScriptableObject, IBattleAnimation
    {
        [SerializeField] protected bool shouldWait;
        [SerializeField] protected Animator animatorPrefab;

        public bool ShouldWait => shouldWait;
        public AsyncOperationHandle AsyncOperationHandle { get; set; }

        public virtual async UniTask Play(IBattleEvent battleEvent) => await LoadAndPlayAnimator();
        public virtual UniTask Undo(IBattleEvent battleEvent)
        {
            return default;
        }

        protected async UniTask LoadAndPlayAnimator()
        {
            var animator = Instantiate(animatorPrefab);
            animator.Play(animator.GetNextAnimatorStateInfo(0).fullPathHash);

            // Wait for anim to start
            await UniTask.WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1);

            // Wait for anim to finish
            await UniTask.WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1);

            var animatorGO = animator.gameObject;
            animatorGO.SetActive(false);
            Destroy(animatorGO);
            Addressables.Release(AsyncOperationHandle);
        }
    }

    public abstract class ScriptableObjectAnimation<T> : ScriptableObjectAnimation, IBattleAnimation<T>
        where T : IBattleEvent
    {
        public async override UniTask Play(IBattleEvent battleEvent) => await Play((T)battleEvent);
        public async override UniTask Undo(IBattleEvent battleEvent) => await Undo((T)battleEvent);
        public abstract UniTask Play(T battleEvent);
        public abstract UniTask Undo(T battleEvent);
    }
}