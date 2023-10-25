using System.Diagnostics.CodeAnalysis;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using Logger = Tooling.Logging.Logger;

namespace Systems.Managers.Base
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public abstract class AbstractSOManager : ScriptableObject, IManager
    {
        /// <summary>
        /// Method that's called on creation of the manager.
        /// </summary>
        /// <returns>UniTask so the operation can be async.</returns>
        public virtual UniTask Startup()
        {
            return UniTask.CompletedTask;
        }

        /// <summary>
        /// Used to bind manager to a type for Zenject dependency injection.
        /// </summary>
        /// <param name="diContainer">The container it this manager binds to.</param>
        public virtual UniTask Bind(DiContainer diContainer)
        {
            return UniTask.CompletedTask;
        }
    }
}