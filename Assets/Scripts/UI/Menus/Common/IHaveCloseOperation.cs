using Cysharp.Threading.Tasks;

namespace UI.Menus.Common
{
    public interface IHaveCloseOperation
    {
        /// <summary>
        /// Handle operations before closing here.
        /// </summary>
        void OnClose();

        /// <summary>
        /// Set default so objects can obt into async method if needed.
        /// </summary>
        UniTask OnCloseAsync() => UniTask.CompletedTask;
    }
}