using System;
using Cysharp.Threading.Tasks;
using Fight.Events;
using TMPro;
using UI;
using UnityEngine;


namespace Fight.Animations
{
    [CreateAssetMenu(menuName = "RevolvingDawn/Fight/Animations/" + nameof(TurnStartedEventAnimation), fileName = "New " + nameof(TurnStartedEventAnimation))]
    public class TurnStartedEventAnimation : ScriptableObjectAnimation<TurnStartedEvent>
    {
        private FightOverlay fightOverlay;

        [Zenject.Inject]
        void Construct(FightOverlay fightOverlay)
        {
            this.fightOverlay = fightOverlay;
        }

        public async override UniTask Play(TurnStartedEvent battleEvent)
        {
            var text = animatorPrefab.GetComponentInChildren<TextMeshProUGUI>();
            text.SetText(battleEvent.Log());

            await PlayAndReleaseAnimator(Instantiate(animatorPrefab, fightOverlay.Canvas.transform));
            IsFinished = true;
        }

        public override UniTask Undo(TurnStartedEvent battleEvent)
        {
            throw new NotImplementedException();
        }
    }
}
