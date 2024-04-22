using Fight.Events;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Fight.Animations
{
    public abstract class BattleAnimation<E> : ScriptableObject, IBattleAnimation where E : IBattleEvent
    {
        public async UniTask PlayAnimation(IBattleEvent battleEvent) => await PlayAnimation(battleEvent);
        public abstract UniTask PlayAnimation(E battleEvent);
    }
}