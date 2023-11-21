using Cysharp.Threading.Tasks;

namespace Systems.Managers.Base
{
    /// <summary>
    /// Interface for managers that only have a limited lifetime, example: fight manager that is only alive
    /// during fights.
    /// </summary>
    public interface IPartTimeManager : IManager
    {
        public UniTask ShutDown()
        {
            return UniTask.CompletedTask;
        }
    }
}