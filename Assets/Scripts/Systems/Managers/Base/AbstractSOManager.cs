using System.Diagnostics.CodeAnalysis;
using Cysharp.Threading.Tasks;
using UnityEngine;

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
        /// Method that's called after all startups have completed, used to ensure that all initializations
        /// are done before referencing something a manager may create on startup.
        /// </summary>
        /// <returns></returns>
        public virtual UniTask AfterStart()
        {
            return UniTask.CompletedTask;
        }
    }
}