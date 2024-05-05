using Cysharp.Threading.Tasks;
using Fight.Events;
using UnityEngine;

namespace Fight.Animations
{
    [CreateAssetMenu(menuName = "RevolvingDawn/Fight/Animations/" + nameof(DealDamageAnimation), fileName = "New " + nameof(DealDamageAnimation))]
    public class DealDamageAnimation : BattleAnimation<DealDamageEvent>
    {
        [SerializeField] Animation anim1;
        // [SerializeField] Animation anim2;
        // ...
        public override UniTask Play(DealDamageEvent dealDamageEvent)
        {
            // Play all animations
            return UniTask.CompletedTask;
        }
    }
}