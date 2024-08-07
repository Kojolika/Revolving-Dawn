using Cysharp.Threading.Tasks;
using Fight.Events;
using UnityEngine;
using Views;

namespace Fight.Animations
{
    [CreateAssetMenu(menuName = "RevolvingDawn/Fight/Animations/" + nameof(DiscardCardAnimation), fileName = "New " + nameof(DiscardCardAnimation))]
    public class DiscardCardAnimation : ScriptableObjectAnimation<DiscardCardEvent>
    {
        private PlayerHandView playerHandView;

        [Zenject.Inject]
        void Construct(PlayerHandView playerHandView)
        {
            this.playerHandView = playerHandView;
        }

        public async override UniTask Play(DiscardCardEvent battleEvent)
        {
            await playerHandView.DiscardCardAnimation(battleEvent.Target);
            IsFinished = true;
        }

        public override UniTask Undo(DiscardCardEvent battleEvent)
        {
            throw new System.NotImplementedException();
        }
    }
}