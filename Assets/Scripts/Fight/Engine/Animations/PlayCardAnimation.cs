using Cysharp.Threading.Tasks;
using Fight.Events;

namespace Fight.Animations
{
    public class PlayCardAnimation : BattleAnimation<PlayCardEvent>
    {
        public override UniTask Play(PlayCardEvent battleEvent)
        {
            throw new System.NotImplementedException();
        }
    }
}