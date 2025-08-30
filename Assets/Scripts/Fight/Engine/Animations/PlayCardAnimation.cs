using Cysharp.Threading.Tasks;
using Fight.Events;
using UnityEngine;
using Views;

namespace Fight.Animations
{
    [CreateAssetMenu(menuName = "RevolvingDawn/Fight/Animations/" + nameof(PlayCardAnimation), fileName = "New " + nameof(PlayCardAnimation))]
    public class PlayCardAnimation : ScriptableObjectAnimation<PlayCardEvent>
    {
        private PlayerHandView playerHandView;

        [Zenject.Inject]
        void Construct(PlayerHandView playerHandView)
        {
            this.playerHandView = playerHandView;
        }

        public async override UniTask Play(PlayCardEvent battleEvent)
        {
            await playerHandView.PlayCardAnimation(battleEvent.Card);
            IsFinished = true;
        }

        public override UniTask Undo(PlayCardEvent battleEvent)
        {
            throw new System.NotImplementedException();
        }
    }
}