using Fight.Events;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Fight.Animations
{
    public abstract class BattleAnimation<E> : ScriptableObject, IBattleAnimation where E : IBattleEvent
    {
        public async UniTask Play(IBattleEvent battleEvent) => await Play(battleEvent);
        public abstract UniTask Play(E battleEvent);
    }
}