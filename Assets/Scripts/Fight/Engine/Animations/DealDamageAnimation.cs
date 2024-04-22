using Cysharp.Threading.Tasks;
using Fight.Events;

namespace Fight.Animations
{
    public class DealDamageAnimation : BattleAnimation<DealDamageEvent>
    {
        // [SerializeField] Animation anim1;
        // [SerializeField] Animation anim2;
        // ...
        public override UniTask PlayAnimation(DealDamageEvent dealDamageEvent)
        {
            // Play all animations
            return UniTask.CompletedTask;
        }
    }
}