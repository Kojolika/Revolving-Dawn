using Cysharp.Threading.Tasks;
using Fight.Events;

namespace Fight.Animations
{
    public interface IBattleAnimation
    {
        UniTask PlayAnimation(IBattleEvent battleEvent);
    }
}