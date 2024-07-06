using System;
using Cysharp.Threading.Tasks;
using Fight.Events;
using Models.Characters;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Fight.Animations
{
    [CreateAssetMenu(menuName = "RevolvingDawn/Fight/Animations/" + nameof(TurnStartedEventAnimation), fileName = "New " + nameof(TurnStartedEventAnimation))]
    public class TurnStartedEventAnimation : ScriptableObjectAnimation<TurnStartedEvent>
    {
        private Canvas canvas;

        [Zenject.Inject]
        void Construct(Canvas canvas)
        {
            this.canvas = canvas;
        }

        public async override UniTask Play(TurnStartedEvent battleEvent)
        {
            if (battleEvent.Target is PlayerCharacter playerCharacter)
            {
                var text = animatorPrefab.GetComponentInChildren<TextMeshProUGUI>();
                text.SetText($"{playerCharacter.Name}'s turn started!");

                await LoadAndPlayAnimator();
            }
        }

        public override UniTask Undo(TurnStartedEvent battleEvent)
        {
            throw new NotImplementedException();
        }
    }
}
