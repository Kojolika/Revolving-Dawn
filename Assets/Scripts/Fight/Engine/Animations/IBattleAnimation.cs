using Cysharp.Threading.Tasks;
using Fight.Events;

namespace Fight.Animations
{
    public interface IBattleAnimation
    {
        UniTask Play(IBattleEvent battleEvent);
    }
}