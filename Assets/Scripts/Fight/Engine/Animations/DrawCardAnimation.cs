using Cysharp.Threading.Tasks;
using Fight.Events;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Views;

namespace Fight.Animations
{
    [CreateAssetMenu(menuName = "RevolvingDawn/Fight/Animations/" + nameof(DrawCardAnimation), fileName = "New " + nameof(DrawCardAnimation))]
    public class DrawCardAnimation : ScriptableObjectAnimation<DrawCardEvent>
    {
        private PlayerHandView playerHandView;

        [Zenject.Inject]
        private void Construct(PlayerHandView playerHandView)
        {
            this.playerHandView = playerHandView;
        }

        public override async UniTask Play(DrawCardEvent battleEvent)
        {
            await playerHandView.DrawCard(battleEvent.CardDrawn);
            IsFinished = true;
        }

        public override UniTask Undo(DrawCardEvent battleEvent)
        {
            throw new System.NotImplementedException();
        }
    }
}