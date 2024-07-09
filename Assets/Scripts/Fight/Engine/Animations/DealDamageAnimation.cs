using Cysharp.Threading.Tasks;
using Fight.Events;
using UnityEngine;

namespace Fight.Animations
{
    [CreateAssetMenu(menuName = "RevolvingDawn/Fight/Animations/" + nameof(DealDamageAnimation), fileName = "New " + nameof(DealDamageAnimation))]
    public class DealDamageAnimation : ScriptableObjectAnimation<DealDamageEvent>
    {
        [SerializeField] Animation anim1;

        // [SerializeField] Animation anim2;
        // ...

        #region IBattleAnimation<DealDamageEvent>



        public override UniTask Play(DealDamageEvent dealDamageEvent)
        {
            // Play all animations
            return UniTask.CompletedTask;
        }

        public override UniTask Undo(DealDamageEvent dealDamageEvent)
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}