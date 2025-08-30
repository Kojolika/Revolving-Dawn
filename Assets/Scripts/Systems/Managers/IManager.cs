using Cysharp.Threading.Tasks;

namespace Systems.Managers
{
    /// <summary>
    /// Interface for managers to inherit from, defines functions that are used across all of them.
    /// </summary>
    public interface IManager
    {
        /// <summary>
        /// Used for initialization and dependencies when starting a manager.
        /// Think of this as Awake.
        /// </summary>
        /// <returns>Completed task.</returns>
        UniTask Startup()
        {
            return UniTask.CompletedTask;
        }

        /// <summary>
        /// Used for any tasks after initialization, such as managers requiring dependencies between other managers.
        /// Think of this as Start.
        /// </summary>
        /// <returns>Completed task.</returns>
        UniTask AfterStart()
        {
            return UniTask.CompletedTask;
        }
    }
}