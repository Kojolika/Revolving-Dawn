using System;
using Cysharp.Threading.Tasks;
using Fight.Events;
using UnityEngine;

namespace Fight.Animations
{
    [CreateAssetMenu(menuName = "RevolvingDawn/Fight/Animations/" + nameof(BattleStartedEventAnimation), fileName = "New " + nameof(BattleStartedEventAnimation))]
    public class BattleStartedEventAnimation : ScriptableObjectAnimation
    {

        public override async UniTask Play(IBattleEvent battleEvent)
        {
            var mainCamera = Camera.main;
            mainCamera.enabled = true;

            await base.Play(battleEvent);
            
            mainCamera.enabled = true;
        }

        public override UniTask Undo(IBattleEvent battleEvent)
        {
            throw new NotImplementedException();
        }
    }
}